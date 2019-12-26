using GymProject.Application.Command;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.MediatorBehavior;
using GymProject.Application.Test.FakeCommands;
using GymProject.Application.Test.Utils;
using GymProject.Application.Validator.TrainingDomain;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.Test.Util;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain;
using GymProject.Infrastructure.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace GymProject.Application.Test.UnitTest.CQRS.TrainingDomain
{
    public class TrainingDomainCommandsTest
    {




        [Fact]
        public async Task PlanDraftWorkoutCommandSuccess()
        {
            GymContext context;
            ILogger<PlanDraftWorkoutCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<PlanDraftWorkoutCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);


            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);
            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);


            // Test
            uint planId = 1;
            uint weekId = 1;
            uint weekPnum = context.TrainingWeeks.Find(weekId).ProgressiveNumber;
            int workoutsNumberBefore = context.WorkoutTemplates.Count();
            int trainingPlansNumberBefore = context.TrainingPlans.Count();
            IEnumerable<uint?> planWorkouts = planRepository.Find(planId).WorkoutIds;

            PlanDraftWorkoutCommand command = new PlanDraftWorkoutCommand(planId, weekId, weekPnum);
            PlanDraftWorkoutCommandHandler handler = new PlanDraftWorkoutCommandHandler(
                workoutRepository, planRepository, logger);

            Assert.True(await handler.Handle(command, default));

            // Check new workout
            WorkoutTemplateRoot added = context.WorkoutTemplates.Last();

            Assert.Equal(trainingPlansNumberBefore, context.TrainingPlans.Count());
            Assert.Equal(workoutsNumberBefore + 1, context.WorkoutTemplates.Count());
            Assert.Equal(planWorkouts.Count() + 1, planRepository.Find(planId).WorkoutIds.Count);
            Assert.Contains(context.TrainingWeeks.Find(weekId).WorkoutIds.Last().Value, planRepository.Find(planId).WorkoutIds);

            // Check link with plan
            TrainingWeekEntity week = context.TrainingWeeks.Find(weekId);
            Assert.Equal(added.Id, week.WorkoutIds.Last());
        }


        [Fact]
        public async Task PlanDraftWorkoutCommandFail()
        {
            GymContext context;
            ILogger<PlanDraftWorkoutCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<PlanDraftWorkoutCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);
            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);

            // Test 1: Cannot create workout
            uint fakePlanId = (uint)context.TrainingPlans.Count() + 1;
            uint weekId = 1;
            uint weekPnum = context.TrainingWeeks.Find(weekId).ProgressiveNumber;

            int workoutsNumberBefore = context.WorkoutTemplates.Count();
            int trainingPlansNumberBefore = context.TrainingPlans.Count();

            PlanDraftWorkoutCommand command = new PlanDraftWorkoutCommand(fakePlanId, weekId, weekPnum);
            PlanDraftWorkoutCommandHandler handler = new PlanDraftWorkoutCommandHandler(
                workoutRepository, planRepository, logger);

            Assert.False(await handler.Handle(command, default));

            // Check no changes
            Assert.Equal(trainingPlansNumberBefore, context.TrainingPlans.Count());
            Assert.Equal(workoutsNumberBefore, context.WorkoutTemplates.Count());

            // Test 2: Cannot link to plan
            uint planId = 1;
            weekId = 1;
            uint fakeWeekPnum = 100;

            command = new PlanDraftWorkoutCommand(planId, weekId, fakeWeekPnum);
            handler = new PlanDraftWorkoutCommandHandler(
                workoutRepository, planRepository, logger);

            Assert.False(await handler.Handle(command, default));

            // Check no changes
            Assert.Equal(workoutsNumberBefore, context.WorkoutTemplates.Count());
        }


        [Fact]
        public async Task DraftTrainingPlanCommandSuccess()
        {
            GymContext context;
            ILogger<CreateDraftTrainingPlanCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<CreateDraftTrainingPlanCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            IAthleteRepository athleteRepo = new SQLAthleteRepository(context);
            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);
            int workoutsNumberBefore = context.TrainingPlans.Count();

            // Test
            uint userId = 1;

            CreateDraftTrainingPlanCommand command = new CreateDraftTrainingPlanCommand(userId);
            CreateDraftTrainingPlanCommandHandler handler = new CreateDraftTrainingPlanCommandHandler(planRepository, athleteRepo, logger);

            Assert.True(await handler.Handle(command, default));

            // Check new training plan
            TrainingPlanRoot added = context.TrainingPlans.Last();
            int workoutsNumberAfter = context.TrainingPlans.Count();

            Assert.Equal(workoutsNumberBefore + 1, workoutsNumberAfter);
            Assert.Equal(userId, added.OwnerId);
            Assert.Equal(1, added.TrainingWeeks.Count);
            Assert.Empty(added.TrainingWeeks.SelectMany(x => x.WorkoutIds));

            // Check training plan in user library
            AthleteRoot athlete = context.Athletes.Find(userId);
            Assert.Contains(added.Id.Value, athlete.TrainingPlans.Select(x => x.TrainingPlanId));
        }


        [Fact]
        public async Task AddWorkingSetIntensityTechniqueCommandSuccess()
        {
            GymContext context;
            ILogger<AddWorkingSetIntensityTechniqueCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<AddWorkingSetIntensityTechniqueCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);

            // Test
            uint workoutId = 1;
            uint workUnitPnum = 1;
            uint wsetPnum = 1;
            uint intensityTechniqueId = 100;
            WorkoutTemplateRoot srcWorkout = workoutRepository.Find(workoutId);

            IReadOnlyCollection<uint?> srcTechniques = srcWorkout.CloneWorkingSet(workUnitPnum, wsetPnum).IntensityTechniqueIds;

            AddWorkingSetIntensityTechniqueCommand command = new AddWorkingSetIntensityTechniqueCommand(workoutId, workUnitPnum, wsetPnum, intensityTechniqueId);
            AddWorkingSetIntensityTechniqueCommandHandler handler = new AddWorkingSetIntensityTechniqueCommandHandler(workoutRepository, logger);

            Assert.True(await handler.Handle(command, default));

            // Check modifications
            WorkoutTemplateRoot newWorkout = workoutRepository.Find(workoutId);
            IReadOnlyCollection<uint?> destTechniques = newWorkout.CloneWorkingSet(workUnitPnum, wsetPnum).IntensityTechniqueIds;

            Assert.Equal(srcTechniques.Count() + 1, destTechniques.Count);
            Assert.Equal(intensityTechniqueId, destTechniques.Last().Value);
            Assert.True(srcTechniques.SequenceEqual(destTechniques.Take(destTechniques.Count - 1)));
        }


        [Fact]
        public async Task AttachTrainingPlanNoteCommandSuccess()
        {
            GymContext context;
            ILogger<AttachTrainingPlanNoteCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<AttachTrainingPlanNoteCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            IAthleteRepository athletes = new SQLAthleteRepository(context);

            // Test
            uint planId = 1;
            uint userId = 1;
            uint noteId = 100;
            var src = athletes.Find(planId).CloneTrainingPlanOrDefault(planId);

            AttachTrainingPlanNoteCommand command = new AttachTrainingPlanNoteCommand(userId, planId, noteId);
            AttachTrainingPlanNoteCommandHandler handler = new AttachTrainingPlanNoteCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check modifications
            var newPlan = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

            Assert.Equal(noteId, newPlan.TrainingPlanNoteId);


            // Test - Clean note
            src = newPlan;
            uint finalNoteId = 456;

            command = new AttachTrainingPlanNoteCommand(userId, planId, finalNoteId);
            handler = new AttachTrainingPlanNoteCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check modifications
            var finalPlan = athletes.Find(planId).CloneTrainingPlanOrDefault(planId);

            Assert.Equal(finalNoteId, finalPlan.TrainingPlanNoteId);
        }


        [Fact]
        public async Task AttachWorkUnitTemplateNoteCommandSuccess()
        {
            GymContext context;
            ILogger<AttachWorkUnitTemplateNoteCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<AttachWorkUnitTemplateNoteCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);

            // Test
            uint workoutId = 2;
            uint workUnitPnum = 0;
            uint noteId = 1231;
            WorkoutTemplateRoot srcWorkout = workoutRepository.Find(workoutId);

            AttachWorkUnitTemplateNoteCommand command = new AttachWorkUnitTemplateNoteCommand(workoutId, workUnitPnum, noteId);
            AttachWorkUnitTemplateNoteCommandHandler handler = new AttachWorkUnitTemplateNoteCommandHandler(workoutRepository, logger);

            Assert.True(await handler.Handle(command, default));

            // Check modifications
            WorkoutTemplateRoot newWorkout = workoutRepository.Find(workoutId);

            Assert.Equal(noteId, newWorkout.CloneWorkUnit(workUnitPnum).WorkUnitNoteId);
        }



        [Fact]
        public async Task CreateDraftTrainingPlanCommandSuccess()
        {
            GymContext context;
            ILogger<CreateDraftTrainingPlanCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<CreateDraftTrainingPlanCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            IAthleteRepository athleteRepo = new SQLAthleteRepository(context);
            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);

            // Test
            uint ownerId = 3;

            CreateDraftTrainingPlanCommand command = new CreateDraftTrainingPlanCommand(ownerId);
            CreateDraftTrainingPlanCommandHandler handler = new CreateDraftTrainingPlanCommandHandler(planRepository, athleteRepo, logger);

            Assert.True(await handler.Handle(command, default));

            // Check Training Plan Created
            TrainingPlanRoot dest = context.TrainingPlans.Last();

            Assert.NotNull(dest);
            Assert.Empty(dest.WorkoutIds);
            Assert.Single(dest.TrainingWeeks);
            // Not testing everything ...

            // Check Trainin Plan in User Library
            AthleteRoot destAthlete = context.Athletes.Find(ownerId);

            Assert.Contains(dest.Id.Value, destAthlete.TrainingPlans.Select(x => x.TrainingPlanId));
        }


        [Fact]
        public async Task DeleteExcerciseFromWorkoutCommandSuccess()
        {
            GymContext context;
            ILogger<DeleteExcerciseFromWorkoutCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<DeleteExcerciseFromWorkoutCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);

            // Test
            uint id = 1;
            uint workUnitPnum = 1;
            WorkoutTemplateRoot src = workoutRepository.Find(id).Clone() as WorkoutTemplateRoot;

            DeleteExcerciseFromWorkoutCommand command = new DeleteExcerciseFromWorkoutCommand(id, workUnitPnum);
            DeleteExcerciseFromWorkoutCommandHandler handler = new DeleteExcerciseFromWorkoutCommandHandler(
                workoutRepository, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            WorkoutTemplateRoot dest = workoutRepository.Find(id);
            Assert.Equal(src.WorkUnits.Count, dest.WorkUnits.Count + 1);
            Assert.DoesNotContain(src.CloneWorkUnit(workUnitPnum), dest.WorkUnits);
        }


        [Fact]
        public async Task DeleteExcerciseFromWorkoutCommandFail()
        {
            GymContext context;
            ILogger<DeleteExcerciseFromWorkoutCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<DeleteExcerciseFromWorkoutCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);

            // Test
            uint id = 1;
            uint workUnitPnum = 12;
            WorkoutTemplateRoot src = workoutRepository.Find(id).Clone() as WorkoutTemplateRoot;

            DeleteExcerciseFromWorkoutCommand command = new DeleteExcerciseFromWorkoutCommand(id, workUnitPnum);
            DeleteExcerciseFromWorkoutCommandHandler handler = new DeleteExcerciseFromWorkoutCommandHandler(
                workoutRepository, logger);

            Assert.False(await handler.Handle(command, default));

            // Check
            WorkoutTemplateRoot dest = workoutRepository.Find(id);
            Assert.Equal(src.WorkUnits.Count, dest.WorkUnits.Count);
        }


        [Fact]
        public async Task DeleteTrainingPlanCommandSuccess()
        {
            GymContext context;
            ILogger<DeleteTrainingPlanCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<DeleteTrainingPlanCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);
            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);

            // Test
            uint id = 1;
            TrainingPlanRoot src = planRepository.Find(id).Clone() as TrainingPlanRoot;

            DeleteTrainingPlanCommand command = new DeleteTrainingPlanCommand(id);
            DeleteTrainingPlanCommandHandler handler = new DeleteTrainingPlanCommandHandler(
                workoutRepository, planRepository, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            foreach (uint workoutId in src.WorkoutIds.Select(x => x.Value))
                Assert.Null(workoutRepository.Find(workoutId));

            Assert.Null(planRepository.Find(id));
            Assert.DoesNotContain(id, context.TrainingPlans.Select(x => x.Id.Value));
        }

        [Fact]
        public async Task DeleteTrainingPlanCommandFail()
        {
            GymContext context;
            ILogger<DeleteTrainingPlanCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<DeleteTrainingPlanCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);
            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);

            // Test
            uint id = uint.MaxValue;
            int planNum = context.TrainingPlans.Count();
            int woNum = context.WorkoutTemplates.Count();

            DeleteTrainingPlanCommand command = new DeleteTrainingPlanCommand(id);
            DeleteTrainingPlanCommandHandler handler = new DeleteTrainingPlanCommandHandler(
                workoutRepository, planRepository, logger);

            Assert.False(await handler.Handle(command, default));

            // Check
            Assert.Equal(planNum, context.TrainingPlans.Count());
            Assert.Equal(woNum, context.WorkoutTemplates.Count());
        }


        [Fact]
        public async Task DeleteWorkingSetTemplateCommandSuccess()
        {
            GymContext context;
            ILogger<DeleteWorkingSetTemplateCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<DeleteWorkingSetTemplateCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);
            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);

            // Test
            uint id = 1;
            uint workUnitPnum = 0;
            uint wsPnum = 1;
            WorkoutTemplateRoot src = workoutRepository.Find(id).Clone() as WorkoutTemplateRoot;

            DeleteWorkingSetTemplateCommand command = new DeleteWorkingSetTemplateCommand(id, workUnitPnum, wsPnum);
            DeleteWorkingSetTemplateCommandHandler handler = new DeleteWorkingSetTemplateCommandHandler(
                workoutRepository, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            WorkoutTemplateRoot dest = workoutRepository.Find(id);
            Assert.Equal(src.CloneAllWorkingSets().Count(), dest.CloneAllWorkingSets().Count() + 1);
        }


        [Fact]
        public async Task DetachTrainingPlanNoteCommandSuccess()
        {
            GymContext context;
            ILogger<DetachTrainingPlanNoteCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<DetachTrainingPlanNoteCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint userId = 1;
            uint planId = 1;
            var src = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

            DetachTrainingPlanNoteCommand command = new DetachTrainingPlanNoteCommand(userId, planId);
            DetachTrainingPlanNoteCommandHandler handler = new DetachTrainingPlanNoteCommandHandler(
                athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);
            Assert.Null(dest.TrainingPlanNoteId);
        }


        [Fact]
        public async Task DetachWorkUnitTemplateNoteCommandSuccess()
        {
            //GymContext context;
            //ILogger<DetachWorkUnitTemplateNoteCommandHandler> logger;

            //(context, _, logger) = StaticUtilities.InitInMemoryCommandTest<DetachWorkUnitTemplateNoteCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            //var workoutRepo = new SQLWorkoutTemplateRepository(context);

            //// Test
            //uint id = 1;
            //uint workUnitPnum = 0;
            //var src = workoutRepo.Find(id).Clone() as WorkoutTemplateRoot;

            //DetachWorkUnitTemplateNoteCommand command = new DetachWorkUnitTemplateNoteCommand(id, workUnitPnum);
            //DetachWorkUnitTemplateNoteCommandHandler handler = new DetachWorkUnitTemplateNoteCommandHandler(
            //    workoutRepo, logger);

            //throw new NotImplementedException();
            //Assert.True(await handler.Handle(command, default));

            //// Check
            //var dest = workoutRepo.Find(id);
            //Assert.Null(dest.CloneWorkUnit(workUnitPnum).WorkUnitNoteId);
        }


        [Fact]
        public async Task PlanDraftExcerciseCommandSuccess()
        {
            GymContext context;
            ILogger<PlanDraftExcerciseCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<PlanDraftExcerciseCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var repo = new SQLWorkoutTemplateRepository(context);

            // Test
            uint id = 1;
            uint excerciseId = 3;
            var src = repo.Find(id).Clone() as WorkoutTemplateRoot;

            PlanDraftExcerciseCommand command = new PlanDraftExcerciseCommand(id, excerciseId);
            PlanDraftExcerciseCommandHandler handler = new PlanDraftExcerciseCommandHandler(
                repo, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = repo.Find(id);
            Assert.Equal(src.WorkUnits.Count(), dest.WorkUnits.Count() - 1);
        }


        [Fact]
        public async Task PlanTrainingWeekCommandSuccess()
        {
            GymContext context;
            ILogger<PlanTrainingWeekCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<PlanTrainingWeekCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var repo = new SQLTrainingPlanRepository(context);

            // Test
            uint id = 1;
            uint weekTypeId = 1;
            var src = repo.Find(id).Clone() as TrainingPlanRoot;

            PlanTrainingWeekCommand command = new PlanTrainingWeekCommand(id, weekTypeId);
            PlanTrainingWeekCommandHandler handler = new PlanTrainingWeekCommandHandler(
                repo, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = repo.Find(id);
            Assert.Equal(src.TrainingWeeks.Count(), dest.TrainingWeeks.Count() - 1);
        }


        [Fact]
        public void PlanTrainingWeekCommandFail()
        {
            GymContext context;

            (context, _, _) = ApplicationTestService.InitInMemoryCommandTest<PlanTrainingWeekCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var repo = new SQLTrainingPlanRepository(context);

            // Test
            uint id = 1;
            uint weekTypeId = 13;
            var src = repo.Find(id).Clone() as TrainingPlanRoot;

            PlanTrainingWeekCommand command = new PlanTrainingWeekCommand(id, weekTypeId);

            var loggerValidator = new Mock<ILogger<PlanTrainingWeekCommandValidator>>();
            PlanTrainingWeekCommandValidator validator = new PlanTrainingWeekCommandValidator(loggerValidator.Object);
            Assert.False(validator.Validate(command).IsValid);
        }


        [Fact]
        public async Task PlanWorkingSetCommandSuccess()
        {
            GymContext context;
            ILogger<PlanWorkingSetCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<PlanWorkingSetCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var repo = new SQLWorkoutTemplateRepository(context);

            // Test
            int ntests = 50;
            uint id = 1;
            uint workUnitPnum = 0;
            var src = repo.Find(id).Clone() as WorkoutTemplateRoot;

            for (int itest = 0; itest < ntests; itest++)
            {
                int reps = RandomFieldGenerator.RandomInt(1, 30);
                int? workType = RandomFieldGenerator.ChooseAmongNullable(TimeMeasureUnitEnum.List().Select(x => (int?)x.Id), 0.25f);
                int? rest = RandomFieldGenerator.RandomIntNullable(30, 360, 0.25f);
                int? restMeas = RandomFieldGenerator.ChooseAmongNullable(TimeMeasureUnitEnum.List().Select(x => (int?)x.Id), 0.25f);
                int? effort = RandomFieldGenerator.RandomIntNullable(2, 15, 0.25f);
                int? effortType = RandomFieldGenerator.RollEventWithProbability(0.25f)
                    ? null
                    : (int?)TrainingEffortTypeEnum.RM.Id;
                string tempo = RandomFieldGenerator.RollEventWithProbability(0.25f)
                    ? null
                    : TUTValue.GenericTempo;
                IEnumerable<uint?> techniques = RandomFieldGenerator.RollEventWithProbability(0.25f)
                    ? null
                    : Enumerable.Range(1, 3).Select(x => (uint?)x);

                PlanWorkingSetCommand command = new PlanWorkingSetCommand(id, workUnitPnum, reps,
                    workType, rest, restMeas, effort, effortType, tempo, techniques);
                PlanWorkingSetCommandHandler handler = new PlanWorkingSetCommandHandler(
                    repo, logger);

                Assert.True(await handler.Handle(command, default));

                // Check
                var dest = repo.Find(id);
                var wsAdded = dest.CloneWorkUnit(workUnitPnum).WorkingSets.Last();
                Assert.Equal(src.CloneAllWorkingSets().Count(), dest.CloneAllWorkingSets().Count() - itest - 1);

                Assert.Equal(
                    WSRepetitionsValue.TrackWork(reps, workType.HasValue ? WSWorkTypeEnum.From(workType.Value) : WSWorkTypeEnum.RepetitionBasedSerie)
                    , wsAdded.Repetitions);

                if (rest.HasValue)
                    Assert.Equal(
                        RestPeriodValue.SetRest(rest.Value, restMeas.HasValue ? TimeMeasureUnitEnum.From(restMeas.Value) : TimeMeasureUnitEnum.Seconds)
                        , wsAdded.Rest);

                if (effort.HasValue)
                    Assert.Equal(
                        TrainingEffortValue.FromEffort(effort.Value, effortType.HasValue ? TrainingEffortTypeEnum.From(effortType.Value) : TrainingEffortTypeEnum.IntensityPercentage)
                        , wsAdded.Effort);

                if (string.IsNullOrWhiteSpace(tempo))
                    Assert.Null(wsAdded.Tempo);
                else
                    Assert.Equal(TUTValue.PlanTUT(tempo), wsAdded.Tempo);

                if (techniques == null)
                    Assert.Empty(wsAdded.IntensityTechniqueIds);
                else
                    Assert.True(techniques.SequenceEqual(wsAdded.IntensityTechniqueIds));
            }

        }

        [Fact]
        public void PlanWorkingSetCommandFail()
        {
            GymContext context;
            ILogger<PlanWorkingSetCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<PlanWorkingSetCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var repo = new SQLWorkoutTemplateRepository(context);

            uint id = 1;
            uint workUnitPnum = 0;
            var src = repo.Find(id).Clone() as WorkoutTemplateRoot;


            int reps = 10;

            // Test wrong EffortTypeId
            int? workType = 1;
            int? rest = 120;
            int? restMeas = 1;
            int? effort = 10;
            int? effortType = -1;


            PlanWorkingSetCommand command = new PlanWorkingSetCommand(id, workUnitPnum, reps,
                workType, rest, restMeas, effort, effortType, null, null);

            var loggerValidator = new Mock<ILogger<PlanWorkingSetCommandValidator>>();
            PlanWorkingSetCommandValidator validator = new PlanWorkingSetCommandValidator(loggerValidator.Object);
            Assert.False(validator.Validate(command).IsValid);

            // Test wrong WorkTypeId
            effortType = 1;
            workType = 100;

            command = new PlanWorkingSetCommand(id, workUnitPnum, reps, workType, rest, restMeas, effort, effortType, null, null);
            Assert.False(validator.Validate(command).IsValid);

            // Test wrong TimeMEasUnitId
            workType = 1;
            restMeas = 0;

            command = new PlanWorkingSetCommand(id, workUnitPnum, reps, workType, rest, restMeas, effort, effortType, null, null);
            Assert.False(validator.Validate(command).IsValid);
        }



        [Fact]
        public async Task PlanWorkingSetEffortCommandSuccess()
        {
            GymContext context;
            ILogger<PlanWorkingSetEffortCommandHandler> logger;
            var loggerValidator = new Mock<ILogger<PlanWorkingSetEffortCommandValidator>>();

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<PlanWorkingSetEffortCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var repo = new SQLWorkoutTemplateRepository(context);

            // Test
            uint id = 2;
            uint workUnitPnum = 0;
            uint wsPnum = 0;
            int? effort = 9;
            int? effortTypeId = 2;
            var src = repo.Find(id).Clone() as WorkoutTemplateRoot;

            PlanWorkingSetEffortCommand command = new PlanWorkingSetEffortCommand(id, workUnitPnum, wsPnum, effort, effortTypeId);
            PlanWorkingSetEffortCommandHandler handler = new PlanWorkingSetEffortCommandHandler(
                repo, logger);

            PlanWorkingSetEffortCommandValidator validator = new PlanWorkingSetEffortCommandValidator(loggerValidator.Object);
            Assert.True(validator.Validate(command).IsValid);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = repo.Find(id);
            Assert.Equal(src.CloneAllWorkingSets().Count(), dest.CloneAllWorkingSets().Count());
            Assert.Equal(TrainingEffortValue.FromEffort(effort.Value, TrainingEffortTypeEnum.From(effortTypeId.Value)),
                dest.CloneWorkUnit(workUnitPnum).CloneWorkingSet(wsPnum).Effort);
        }


        [Fact]
        public async Task PlanWorkingSetRepetitionsCommandSuccess()
        {
            GymContext context;
            ILogger<PlanWorkingSetRepetitionsCommandHandler> logger;
            var loggerValidator = new Mock<ILogger<PlanWorkingSetRepetitionsCommandValidator>>();

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<PlanWorkingSetRepetitionsCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var repo = new SQLWorkoutTemplateRepository(context);

            // Test
            uint id = 1;
            uint workUnitPnum = 0;
            uint wsPnum = 0;
            int reps = 9;
            int? workTypeId = 1;
            var src = repo.Find(id).Clone() as WorkoutTemplateRoot;

            PlanWorkingSetRepetitionsCommand command = new PlanWorkingSetRepetitionsCommand(id, workUnitPnum, wsPnum, reps, workTypeId);
            PlanWorkingSetRepetitionsCommandHandler handler = new PlanWorkingSetRepetitionsCommandHandler(
                repo, logger);

            PlanWorkingSetRepetitionsCommandValidator validator = new PlanWorkingSetRepetitionsCommandValidator(loggerValidator.Object);
            Assert.True(validator.Validate(command).IsValid);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = repo.Find(id);
            Assert.Equal(src.CloneAllWorkingSets().Count(), dest.CloneAllWorkingSets().Count());
            Assert.Equal(WSRepetitionsValue.TrackWork(reps, WSWorkTypeEnum.From(workTypeId.Value)),
                dest.CloneWorkUnit(workUnitPnum).CloneWorkingSet(wsPnum).Repetitions);
        }


        [Fact]
        public async Task PlanWorkingSetTempoCommandSuccess()
        {
            GymContext context;
            ILogger<PlanWorkingSetTempoCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<PlanWorkingSetTempoCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var repo = new SQLWorkoutTemplateRepository(context);

            // Test
            uint id = 1;
            uint workUnitPnum = 0;
            uint wsPnum = 0;
            string tempo = "11X0";
            var src = repo.Find(id).Clone() as WorkoutTemplateRoot;

            PlanWorkingSetTempoCommand command = new PlanWorkingSetTempoCommand(id, workUnitPnum, wsPnum, tempo);
            PlanWorkingSetTempoCommandHandler handler = new PlanWorkingSetTempoCommandHandler(
                repo, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = repo.Find(id);
            Assert.Equal(src.CloneAllWorkingSets().Count(), dest.CloneAllWorkingSets().Count());
            Assert.Equal(TUTValue.PlanTUT(tempo), dest.CloneWorkUnit(workUnitPnum).CloneWorkingSet(wsPnum).Tempo);
        }


        [Fact]
        public async Task RemoveWorkingSetIntensityTechniqueCommandSuccess()
        {
            GymContext context;
            ILogger<RemoveWorkingSetIntensityTechniqueCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<RemoveWorkingSetIntensityTechniqueCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var repo = new SQLWorkoutTemplateRepository(context);

            // Test
            uint id = 1;
            uint workUnitPnum = 0;
            uint wsPnum = 0;
            var src = repo.Find(id).Clone() as WorkoutTemplateRoot;
            uint? intTechniqueId = src.CloneWorkingSet(workUnitPnum, wsPnum).IntensityTechniqueIds.First();

            RemoveWorkingSetIntensityTechniqueCommand command = new RemoveWorkingSetIntensityTechniqueCommand(id, workUnitPnum, wsPnum, intTechniqueId.Value);
            RemoveWorkingSetIntensityTechniqueCommandHandler handler = new RemoveWorkingSetIntensityTechniqueCommandHandler(
                repo, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = repo.Find(id);
            Assert.Equal(src.CloneAllWorkingSets().Count(), dest.CloneAllWorkingSets().Count());
            Assert.Equal(src.CloneWorkUnit(workUnitPnum).CloneWorkingSet(wsPnum).IntensityTechniqueIds.Count() - 1,
                dest.CloneWorkUnit(workUnitPnum).CloneWorkingSet(wsPnum).IntensityTechniqueIds.Count);
            Assert.DoesNotContain(intTechniqueId, dest.CloneWorkUnit(workUnitPnum).CloneWorkingSet(wsPnum).IntensityTechniqueIds);
        }


        [Fact]
        public async Task TagTrainingPlanAsHashtagCommandSuccess()
        {
            GymContext context;
            ILogger<TagTrainingPlanAsHashtagCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<TagTrainingPlanAsHashtagCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint userId = 1;
            uint planId = 1;
            uint hashtagId = 1111;

            TagTrainingPlanAsHashtagCommand command = new TagTrainingPlanAsHashtagCommand(userId, planId, hashtagId);
            TagTrainingPlanAsHashtagCommandHandler handler = new TagTrainingPlanAsHashtagCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);
            Assert.NotEmpty(dest.HashtagIds);
            Assert.Equal(hashtagId, dest.HashtagIds.Last());
            Assert.Equal(dest.HashtagIds.Count - 1,
                (int)context.TrainingPlanHashtags.Single(x => x.HashtagId == hashtagId && x.TrainingPlanId == userId).ProgressiveNumber);
        }


        [Fact]
        public async Task TagTrainingPlanAsNewHashtagCommandSuccess()
        {
            GymContext context;
            ILogger<TagTrainingPlanAsNewHashtagCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<TagTrainingPlanAsNewHashtagCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athleteRepo = new SQLAthleteRepository(context);
            var hashtagRepo = new SQLTrainingHashtagRepository(context);

            // Test
            uint userId = 1;
            uint planId = 1;
            string hashtagBody = "MyHashtag";
            var planHashtagsBefore = athleteRepo.Find(userId).TrainingPlans.Single(x => x.TrainingPlanId == planId).HashtagIds;

            TagTrainingPlanAsNewHashtagCommand command = new TagTrainingPlanAsNewHashtagCommand(userId, planId, hashtagBody);
            TagTrainingPlanAsNewHashtagCommandHandler handler = new TagTrainingPlanAsNewHashtagCommandHandler(athleteRepo, hashtagRepo, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var planHashtagsAfter = athleteRepo.Find(userId).TrainingPlans.Single(x => x.TrainingPlanId == planId).HashtagIds;
            var newHashtag = context.TrainingHashtags.Last();

            Assert.Equal(hashtagBody, newHashtag.Hashtag.Body);
            Assert.Equal("#" + hashtagBody, newHashtag.Hashtag.ToFullHashtag());

            Assert.Equal(planHashtagsBefore.Count + 1, planHashtagsAfter.Count);
            Assert.Equal(newHashtag.Id, planHashtagsAfter.Last());
            //Assert.Equal(newPlan.HashtagIds.Count - 1,
            //    (int)context.TrainingPlanHashtags.Single(x => x.HashtagId == newHashtag.Id && x.TrainingPlanId == id).ProgressiveNumber);
        }



        [Fact]
        public void TagTrainingPlanAsNewHashtagCommandFail()
        {
            string fake;
            TagTrainingPlanAsNewHashtagCommand command;
            TagTrainingPlanAsNewHashtagCommandValidator validator;

            GymContext context;

            uint userId = 1;
            uint planId = 1;
            var loggerValidator = new Mock<ILogger<TagTrainingPlanAsNewHashtagCommandValidator>>();

            // Too short Hashtag
            fake = "a";

            command = new TagTrainingPlanAsNewHashtagCommand(userId, planId, fake);
            validator = new TagTrainingPlanAsNewHashtagCommandValidator(loggerValidator.Object);
            Assert.False(validator.Validate(command).IsValid);

            // Too long Hashtag
            fake = "a".PadRight(GenericHashtagValue.DefaultMaximumLength + 1);

            command = new TagTrainingPlanAsNewHashtagCommand(userId, planId, fake);
            validator = new TagTrainingPlanAsNewHashtagCommandValidator(loggerValidator.Object);
            Assert.False(validator.Validate(command).IsValid);

            // Invalid hashtag
            fake = "my hashtag with spaces";

            command = new TagTrainingPlanAsNewHashtagCommand(userId, planId, fake);
            validator = new TagTrainingPlanAsNewHashtagCommandValidator(loggerValidator.Object);
            Assert.False(validator.Validate(command).IsValid);

            // Invalid hashtag
            fake = "myhashtagwith#hashtags";

            command = new TagTrainingPlanAsNewHashtagCommand(userId, planId, fake);
            validator = new TagTrainingPlanAsNewHashtagCommandValidator(loggerValidator.Object);
            Assert.False(validator.Validate(command).IsValid);

            // Invalid hashtag
            fake = "myhashtagwith many #errors";

            command = new TagTrainingPlanAsNewHashtagCommand(userId, planId, fake);
            validator = new TagTrainingPlanAsNewHashtagCommandValidator(loggerValidator.Object);
            Assert.False(validator.Validate(command).IsValid);
        }


        [Fact]
        public async Task UntagTrainingPlanAsHashtagCommandSuccess()
        {
            GymContext context;
            ILogger<UntagTrainingPlanAsHashtagCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<UntagTrainingPlanAsHashtagCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athleteRepo = new SQLAthleteRepository(context);
            var hashtagRepo = new SQLTrainingHashtagRepository(context);

            // Test
            uint userId = 1;
            uint planId = 1;
            uint hashtagId = athleteRepo.Find(userId).TrainingPlans.Single(x => x.TrainingPlanId == planId).HashtagIds.First().Value + 1;
            var src = athleteRepo.Find(userId).CloneTrainingPlanOrDefault(planId);

            UntagTrainingPlanAsHashtagCommand command = new UntagTrainingPlanAsHashtagCommand(userId, planId, hashtagId);
            UntagTrainingPlanAsHashtagCommandHandler handler = new UntagTrainingPlanAsHashtagCommandHandler(athleteRepo, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = athleteRepo.Find(userId).CloneTrainingPlanOrDefault(planId);

            Assert.Equal(src.HashtagIds.Count(), dest.HashtagIds.Count() + 1);
            Assert.DoesNotContain(hashtagId, dest.HashtagIds);

            //Assert.True(Enumerable.Range(0, dest.HashtagIds.Count())
            //    .SequenceEqual(context.TrainingPlanHashtags
            //        .OrderBy(x => x.ProgressiveNumber)
            //        .Where(x => x.TrainingPlanId == id).Select(y => (int)y.ProgressiveNumber)));
        }


        [Fact]
        public async Task CreateTrainingPhaseCommandSuccess()
        {
            GymContext context;
            ILogger<CreateTrainingPhaseCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<CreateTrainingPhaseCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var repo = new SQLTrainingPhaseRepository(context);

            // Test
            uint entryStatusId = 1;
            string phasename = "Local Phase";

            CreateTrainingPhaseCommand command = new CreateTrainingPhaseCommand(entryStatusId, phasename);
            CreateTrainingPhaseCommandHandler handler = new CreateTrainingPhaseCommandHandler(repo, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = context.TrainingPhases.Last();

            Assert.Equal(phasename, dest.Name);
            Assert.Equal(EntryStatusTypeEnum.From((int)entryStatusId), dest.EntryStatus);
        }


        [Fact]
        public async Task CreateTrainingPhaseCommandDuplicateFail()
        {
            GymContext context;
            ILogger<CreateTrainingPhaseCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<CreateTrainingPhaseCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var repo = new SQLTrainingPhaseRepository(context);

            // Test
            uint entryStatusId = 1;
            string phasename = repo.Find(1).Name;   // Duplicate name

            CreateTrainingPhaseCommand command = new CreateTrainingPhaseCommand(entryStatusId, phasename);
            CreateTrainingPhaseCommandHandler handler = new CreateTrainingPhaseCommandHandler(repo, logger);

            Assert.False(await handler.Handle(command, default));
        }


        [Fact]
        public async Task TagTrainingPlanWithTrainingPhaseCommandSuccess()
        {
            GymContext context;
            ILogger<TagTrainingPlanWithTrainingPhaseCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<TagTrainingPlanWithTrainingPhaseCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint userId = 1;
            uint planId = 1;
            uint phaseId = context.Athletes
                .Find(userId)
                .CloneTrainingPlanOrDefault(planId).TrainingPhaseIds
                .Last().Value + 1;

            var src = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

            TagTrainingPlanWithTrainingPhaseCommand command = new TagTrainingPlanWithTrainingPhaseCommand(userId, planId, phaseId);
            TagTrainingPlanWithTrainingPhaseCommandHandler handler = new TagTrainingPlanWithTrainingPhaseCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

            Assert.Equal(src.TrainingPhaseIds.Count() + 1, dest.TrainingPhaseIds.Count());
            Assert.Contains(phaseId, dest.TrainingPhaseIds);
        }


        [Fact]
        public async Task UntagTrainingPlanWithTrainingPhaseCommandSuccess()
        {
            GymContext context;
            ILogger<UntagTrainingPlanWithTrainingPhaseCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<UntagTrainingPlanWithTrainingPhaseCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint userId = 1;
            uint planId = 1;
            uint phaseId = context.Athletes.Find(userId).CloneTrainingPlanOrDefault(planId).TrainingPhaseIds.First().Value;

            var src = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

            UntagTrainingPlanWithTrainingPhaseCommand command = new UntagTrainingPlanWithTrainingPhaseCommand(userId, planId, phaseId);
            UntagTrainingPlanWithTrainingPhaseCommandHandler handler = new UntagTrainingPlanWithTrainingPhaseCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

            Assert.Equal(src.TrainingPhaseIds.Count() - 1, dest.TrainingPhaseIds.Count());
            Assert.DoesNotContain(phaseId, dest.TrainingPhaseIds);
        }


        [Fact]
        public async Task TagTrainingPlanWithProficiencyCommandSuccess()
        {
            GymContext context;
            ILogger<TagTrainingPlanWithProficiencyCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<TagTrainingPlanWithProficiencyCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var repo = new SQLAthleteRepository(context);

            // Test
            uint userId = 1;
            uint planId = 1;
            uint proficiencyId = context.Athletes
                .Find(userId)
                .CloneTrainingPlanOrDefault(planId).TrainingProficiencyIds
                .Last().Value + 1;

            var src = repo.Find(userId).CloneTrainingPlanOrDefault(planId);

            TagTrainingPlanWithProficiencyCommand command = new TagTrainingPlanWithProficiencyCommand(userId, planId, proficiencyId);
            TagTrainingPlanWithProficiencyCommandHandler handler = new TagTrainingPlanWithProficiencyCommandHandler(repo, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = repo.Find(userId).CloneTrainingPlanOrDefault(planId);

            Assert.Equal(src.TrainingProficiencyIds.Count() + 1, dest.TrainingProficiencyIds.Count());
            Assert.Contains(proficiencyId, dest.TrainingProficiencyIds);
        }


        [Fact]
        public async Task UntagTrainingPlanWithProficiencyCommandSuccess()
        {
            GymContext context;
            ILogger<UntagTrainingPlanWithProficiencyCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<UntagTrainingPlanWithProficiencyCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint userId = 1;
            uint planId = 1;
            uint proficiencyId = context.Athletes
                .Find(userId)
                .CloneTrainingPlanOrDefault(planId).TrainingProficiencyIds
                .Last().Value;

            var src = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

            UntagTrainingPlanWithProficiencyCommand command = new UntagTrainingPlanWithProficiencyCommand(userId, planId, proficiencyId);
            UntagTrainingPlanWithProficiencyCommandHandler handler = new UntagTrainingPlanWithProficiencyCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

            Assert.Equal(src.TrainingProficiencyIds.Count() - 1, dest.TrainingProficiencyIds.Count());
            Assert.DoesNotContain(proficiencyId, dest.TrainingProficiencyIds);
        }


        [Fact]
        public async Task TagTrainingPlanWithMuscleFocusCommandSuccess()
        {
            GymContext context;
            ILogger<TagTrainingPlanWithMuscleFocusCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<TagTrainingPlanWithMuscleFocusCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint userId = 1;
            uint planId = 1;
            uint muscleId = context.Athletes
                .Find(userId)
                .CloneTrainingPlanOrDefault(planId).MuscleFocusIds
                .Last().Value + 1;

            var src = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

            TagTrainingPlanWithMuscleFocusCommand command = new TagTrainingPlanWithMuscleFocusCommand(userId, planId, muscleId);
            TagTrainingPlanWithMuscleFocusCommandHandler handler = new TagTrainingPlanWithMuscleFocusCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

            Assert.Equal(src.MuscleFocusIds.Count() + 1, dest.MuscleFocusIds.Count());
            Assert.Contains(muscleId, dest.MuscleFocusIds);
        }


        [Fact]
        public async Task UntagTrainingPlanWithMuscleFocusCommandSuccess()
        {
            GymContext context;
            ILogger<UntagTrainingPlanWithMuscleFocusCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<UntagTrainingPlanWithMuscleFocusCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint userId = 1;
            uint planId = 1;
            uint muscleId = context.Athletes
                .Find(userId)
                .CloneTrainingPlanOrDefault(planId).MuscleFocusIds
                .Last().Value;

            var src = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

            UntagTrainingPlanWithMuscleFocusCommand command = new UntagTrainingPlanWithMuscleFocusCommand(userId, planId, muscleId);
            UntagTrainingPlanWithMuscleFocusCommandHandler handler = new UntagTrainingPlanWithMuscleFocusCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

            Assert.Equal(src.MuscleFocusIds.Count() - 1, dest.MuscleFocusIds.Count());
            Assert.DoesNotContain(muscleId, dest.MuscleFocusIds);
        }



        [Fact]
        public async Task WriteTrainingPlanNoteCommandSuccess()
        {
            GymContext context;
            ILogger<WriteTrainingPlanNoteCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<WriteTrainingPlanNoteCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);
            var notes = new SQLTrainingPlanNoteRepository(context);
            var notesBefore = context.TrainingPlanNotes.ToList();

            // Test
            uint userId = 1;
            uint planId = 1;
            string note = "my short note";

            WriteTrainingPlanNoteCommand command = new WriteTrainingPlanNoteCommand(userId, planId, note);
            WriteTrainingPlanNoteCommandHandler handler = new WriteTrainingPlanNoteCommandHandler(athletes, notes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check note added
            var notesAfter = context.TrainingPlanNotes.ToList();
            var added = notesAfter.Last();
            Assert.Equal(notesBefore.Count + 1, notesAfter.Count);
            Assert.Equal(note, added.Body.Body);

            // Check note attached to plan
            var planAfter = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

            Assert.Equal(added.Id, planAfter.TrainingPlanNoteId.Value);
        }


        [Fact]
        public async Task WriteWorkUnitTemplateNoteCommandSuccess()
        {
            GymContext context;
            ILogger<WriteWorkUnitTemplateNoteCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<WriteWorkUnitTemplateNoteCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var workouts = new SQLWorkoutTemplateRepository(context);
            var notes = new SQLWorkUnitTemplateNoteRepository(context);
            var notesBefore = context.WorkUnitTemplateNotes.ToList();

            // Test
            uint workoutId = 1;
            uint workUnitPnum = 1;
            string note = "my short work unit note!";

            WriteWorkUnitTemplateNoteCommand command = new WriteWorkUnitTemplateNoteCommand(workoutId, workUnitPnum, note);
            WriteWorkUnitTemplateNoteCommandHandler handler = new WriteWorkUnitTemplateNoteCommandHandler(workouts, notes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check note added
            var notesAfter = context.WorkUnitTemplateNotes.ToList();
            var added = notesAfter.Last();
            Assert.Equal(notesBefore.Count + 1, notesAfter.Count);
            Assert.Equal(note, added.Body.Body);

            // Check note attached to plan
            var workUnitAfter = workouts.Find(workoutId).CloneWorkUnit(workUnitPnum);
            Assert.Equal(added.Id, workUnitAfter.WorkUnitNoteId);
        }


        [Fact]
        public async Task AchieveTrainingProficiencyCommandSuccess()
        {
            GymContext context;
            ILogger<AchieveTrainingProficiencyCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<AchieveTrainingProficiencyCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint athleteId = 1;
            uint proficiencyId = 1;

            AchieveTrainingProficiencyCommand command = new AchieveTrainingProficiencyCommand(athleteId, proficiencyId);
            AchieveTrainingProficiencyCommandHandler handler = new AchieveTrainingProficiencyCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check note added
            var added = athletes.Find(athleteId).TrainingProficiencies.Last();
            Assert.Equal(proficiencyId, added.ProficiencyId);
            Assert.Equal(DateTime.UtcNow, added.StartDate, new TimeSpan(0, 0, 0, 0, 500));
            Assert.Null(added.EndDate);
        }


        [Fact]
        public async Task BookmarkTrainingPlanCommand_Success()
        {
            GymContext context;
            ILogger<BookmarkTrainingPlanCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<BookmarkTrainingPlanCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint athleteId = 1;
            uint planId = 1;
            bool isBookmarked = !athletes.Find(athleteId).CloneTrainingPlanOrDefault(planId).IsBookmarked;

            BookmarkTrainingPlanCommand command = new BookmarkTrainingPlanCommand(athleteId, planId, isBookmarked);
            BookmarkTrainingPlanCommandHandler handler = new BookmarkTrainingPlanCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check note added
            var modified = athletes.Find(athleteId).CloneTrainingPlanOrDefault(planId);
            Assert.Equal(isBookmarked, modified.IsBookmarked);
        }


        [Fact]
        public async Task ExcludeTrainingPlanFromUserLibraryCommand_NotLastOne_Success()
        {
            GymContext context;
            ILogger<ExcludeTrainingPlanFromUserLibraryCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<ExcludeTrainingPlanFromUserLibraryCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);
            var plans = new SQLTrainingPlanRepository(context);

            // Test
            uint athleteId = 1;
            uint planId = 2;        // This plan is owned by more than one users

            ExcludeTrainingPlanFromUserLibraryCommand command = new ExcludeTrainingPlanFromUserLibraryCommand(planId, athleteId);
            ExcludeTrainingPlanFromUserLibraryCommandHandler handler = new ExcludeTrainingPlanFromUserLibraryCommandHandler(athletes, plans, logger);

            Assert.True(await handler.Handle(command, default));

            // Check note added
            var modified = athletes.Find(athleteId);
            Assert.DoesNotContain(planId, modified.TrainingPlans.Select(x => x.TrainingPlanId));
            Assert.Contains(planId, context.TrainingPlans.Select(x => x.Id));
        }


        [Fact]
        public async Task ExcludeTrainingPlanFromUserLibraryCommand_LastOne_Success()
        {
            GymContext context;
            ILogger<ExcludeTrainingPlanFromUserLibraryCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<ExcludeTrainingPlanFromUserLibraryCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);
            var plans = new SQLTrainingPlanRepository(context);

            // Test
            uint athleteId = 1;
            uint planId = 1;        // This plan is owned by this user only

            ExcludeTrainingPlanFromUserLibraryCommand command = new ExcludeTrainingPlanFromUserLibraryCommand(planId, athleteId);
            ExcludeTrainingPlanFromUserLibraryCommandHandler handler = new ExcludeTrainingPlanFromUserLibraryCommandHandler(athletes, plans, logger);

            Assert.True(await handler.Handle(command, default));

            // Check note added
            var modified = athletes.Find(athleteId);
            Assert.DoesNotContain(planId, modified.TrainingPlans.Select(x => x.TrainingPlanId));
            Assert.DoesNotContain(planId, context.TrainingPlans.Select(x => x.Id));
        }


        [Fact]
        public async Task ExcludeTrainingPlanFromUserLibraryCommand_PlanNotInUserLibrary()
        {
            GymContext context;
            ILogger<ExcludeTrainingPlanFromUserLibraryCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<ExcludeTrainingPlanFromUserLibraryCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);
            var plans = new SQLTrainingPlanRepository(context);

            // Test
            uint athleteId = 1;
            uint planId = 6;        // This plan is not owned by this user

            ExcludeTrainingPlanFromUserLibraryCommand command = new ExcludeTrainingPlanFromUserLibraryCommand(planId, athleteId);
            ExcludeTrainingPlanFromUserLibraryCommandHandler handler = new ExcludeTrainingPlanFromUserLibraryCommandHandler(athletes, plans, logger);

            Assert.False(await handler.Handle(command, default));
        }


        [Fact]
        public async Task IncludeTrainingPlanInUserLibraryCommandHandler_Success()
        {
            GymContext context;
            ILogger<IncludeTrainingPlanInUserLibraryCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<IncludeTrainingPlanInUserLibraryCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint planId = 6;
            uint userId = 1;

            IncludeTrainingPlanInUserLibraryCommand command = new IncludeTrainingPlanInUserLibraryCommand(planId, userId);
            IncludeTrainingPlanInUserLibraryCommandHandler handler = new IncludeTrainingPlanInUserLibraryCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check note added
            var modified = athletes.Find(userId);
            Assert.Contains(planId, modified.TrainingPlans.Select(x => x.Id));
        }


        [Fact]
        public async Task IncludeTrainingPlanInUserLibraryCommandHandler_PlanAlreadyIncUserLibrary()
        {
            GymContext context;
            ILogger<IncludeTrainingPlanInUserLibraryCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<IncludeTrainingPlanInUserLibraryCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint planId = 1;
            uint userId = 1;
            var athleteBefore = athletes.Find(userId);

            IncludeTrainingPlanInUserLibraryCommand command = new IncludeTrainingPlanInUserLibraryCommand(planId, userId);
            IncludeTrainingPlanInUserLibraryCommandHandler handler = new IncludeTrainingPlanInUserLibraryCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check nothing happened
            var modified = athletes.Find(userId);
            Assert.True(athleteBefore.TrainingPlans.SequenceEqual(modified.TrainingPlans));
        }


        [Fact]
        public async Task MakeTrainingPlanNotVariantOfAnyCommand_Success()
        {
            GymContext context;
            ILogger<MakeTrainingPlanNotVariantOfAnyCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<MakeTrainingPlanNotVariantOfAnyCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint planId = 2;
            uint userId = 1;
            var planBefore = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

            MakeTrainingPlanNotVariantOfAnyCommand command = new MakeTrainingPlanNotVariantOfAnyCommand(planId, userId);
            MakeTrainingPlanNotVariantOfAnyCommandHandler handler = new MakeTrainingPlanNotVariantOfAnyCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check note added
            var modified = athletes.Find(userId);
            Assert.Null(modified.CloneTrainingPlanOrDefault(planId).ParentPlanId);
        }


        [Fact]
        public async Task MakeTrainingPlanVariantOfCommand_Success()
        {
            GymContext context;
            ILogger<MakeTrainingPlanVariantOfCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<MakeTrainingPlanVariantOfCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint planId = 2;
            uint userId = 1;
            uint parentId = 3;
            var planBefore = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

            MakeTrainingPlanVariantOfCommand command = new MakeTrainingPlanVariantOfCommand(planId, userId, parentId);
            MakeTrainingPlanVariantOfCommandHandler handler = new MakeTrainingPlanVariantOfCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check note added
            var modified = athletes.Find(userId);
            Assert.Equal(parentId, modified.CloneTrainingPlanOrDefault(planId).ParentPlanId);
            Assert.NotEqual(planBefore.ParentPlanId, modified.CloneTrainingPlanOrDefault(planId).ParentPlanId);
        }


        [Fact]
        public async Task PlanTrainingPhaseCommand_NoEndDateSuccess()
        {
            GymContext context;
            ILogger<PlanTrainingPhaseCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<PlanTrainingPhaseCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint userId = 1;
            uint phaseId = 1;
            uint entryStatusId = 1;
            string note = "The note";
            DateTime startDate = DateTime.UtcNow.AddDays(1);
            DateTime? endDate = null;

            PlanTrainingPhaseCommand command = new PlanTrainingPhaseCommand(userId, phaseId, entryStatusId, note, startDate, endDate);
            PlanTrainingPhaseCommandHandler handler = new PlanTrainingPhaseCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check phase started
            var modified = athletes.Find(userId);
            Assert.Equal(phaseId, modified.TrainingPhases.Last().PhaseId);
            Assert.Equal(startDate, modified.TrainingPhases.Last().StartDate, new TimeSpan(0, 0, 1));
            Assert.Null(modified.TrainingPhases.Last().EndDate);
            Assert.Equal(DateTime.UtcNow.AddDays(-1), modified.TrainingPhases.First().EndDate.Value, new TimeSpan(0, 0, 1));
            Assert.Equal(note, modified.TrainingPhases.Last().OwnerNote.Body);
        }

        [Fact]
        public async Task PlanTrainingPhaseCommand_EndDateSuccess()
        {
            GymContext context;
            ILogger<PlanTrainingPhaseCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<PlanTrainingPhaseCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint userId = 1;
            uint phaseId = 1;
            uint entryStatusId = 1;
            string note = null;
            DateTime startDate = DateTime.UtcNow.AddDays(1);
            DateTime? endDate = startDate.AddDays(100);

            PlanTrainingPhaseCommand command = new PlanTrainingPhaseCommand(userId, phaseId, entryStatusId, note, startDate, endDate);
            PlanTrainingPhaseCommandHandler handler = new PlanTrainingPhaseCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check phase started
            var modified = athletes.Find(userId);
            Assert.Equal(phaseId, modified.TrainingPhases.Last().PhaseId);
            Assert.Equal(startDate, modified.TrainingPhases.Last().StartDate, new TimeSpan(0, 0, 1));
            Assert.Equal(endDate.Value, modified.TrainingPhases.Last().EndDate.Value, new TimeSpan(0, 0, 1));
            Assert.Equal(DateTime.UtcNow.AddDays(-1), modified.TrainingPhases.First().EndDate.Value, new TimeSpan(0, 0, 1));
            Assert.Null(modified.TrainingPhases.Last().OwnerNote);
        }


        [Fact]
        public async Task RenameTrainingPlanCommand_Success()
        {
            GymContext context;
            ILogger<RenameTrainingPlanCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<RenameTrainingPlanCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint userId = 1;
            uint planId = 1;
            string name = "my plan";

            RenameTrainingPlanCommand command = new RenameTrainingPlanCommand(userId, planId, name);
            RenameTrainingPlanCommandHandler handler = new RenameTrainingPlanCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check note added
            Assert.Equal(name, athletes.Find(userId).CloneTrainingPlanOrDefault(planId).Name);
        }

        [Fact]
        public async Task RenameTrainingPlanCommand_PlanNotFound_Fail()
        {
            GymContext context;
            ILogger<RenameTrainingPlanCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<RenameTrainingPlanCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint userId = 1;
            uint planId = 6;
            string name = "my plan";

            RenameTrainingPlanCommand command = new RenameTrainingPlanCommand(userId, planId, name);
            RenameTrainingPlanCommandHandler handler = new RenameTrainingPlanCommandHandler(athletes, logger);

            Assert.False(await handler.Handle(command, default));
        }


        [Fact]
        public async Task ScheduleTrainingPlanCommand_StartDateOnly_Success()
        {
            GymContext context;
            ILogger<ScheduleTrainingPlanCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<ScheduleTrainingPlanCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var schedules = new SQLTrainingScheduleRepository(context);

            // Test
            uint planId = 1;
            uint userId = 2;
            DateTime start = new DateTime(2019, 1, 1);
            DateTime? end = null;

            ScheduleTrainingPlanCommand command = new ScheduleTrainingPlanCommand(userId, planId, start, end);
            ScheduleTrainingPlanCommandHandler handler = new ScheduleTrainingPlanCommandHandler(schedules, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var added = context.TrainingSchedules.Last();
            Assert.Equal(planId, added.TrainingPlanId);
            Assert.Equal(userId, added.AthleteId);
            Assert.Equal(start, added.StartDate);
            Assert.Equal(end, added.EndDate);
            Assert.Empty(added.Feedbacks);
        }

        [Fact]
        public async Task ScheduleTrainingPlanCommand_PlannedPeriod_Success()
        {
            GymContext context;
            ILogger<ScheduleTrainingPlanCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<ScheduleTrainingPlanCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var schedules = new SQLTrainingScheduleRepository(context);

            // Test
            uint planId = 1;
            uint userId = 2;
            DateTime start = new DateTime(2019, 1, 1);
            DateTime? end = null;

            ScheduleTrainingPlanCommand command = new ScheduleTrainingPlanCommand(userId, planId, start, end);
            ScheduleTrainingPlanCommandHandler handler = new ScheduleTrainingPlanCommandHandler(schedules, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var added = context.TrainingSchedules.Last();
            Assert.Equal(planId, added.TrainingPlanId);
            Assert.Equal(userId, added.AthleteId);
            Assert.Equal(start, added.StartDate);
            Assert.Equal(end, added.EndDate);
            Assert.Empty(added.Feedbacks);
        }

        [Fact]
        public async Task ScheduleTrainingPlanCommand_CurrentPlanNotcompleted_Success()
        {
            GymContext context;
            ILogger<ScheduleTrainingPlanCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<ScheduleTrainingPlanCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var schedules = new SQLTrainingScheduleRepository(context);

            // Test
            uint planId = 1;
            uint userId = 1;    // User1 has an ongoing plan
            DateTime start = DateTime.UtcNow.AddDays(1).Date;
            DateTime? end = null;

            var currentPlan = schedules.GetCurrentScheduleByAthleteOrDefault(userId);

            if (currentPlan == null)
                throw new ArgumentException("Test case not correct");

            ScheduleTrainingPlanCommand command = new ScheduleTrainingPlanCommand(userId, planId, start, end);
            ScheduleTrainingPlanCommandHandler handler = new ScheduleTrainingPlanCommandHandler(schedules, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var added = context.TrainingSchedules.Last();
            Assert.Equal(planId, added.TrainingPlanId);
            Assert.Equal(start, added.StartDate);
            Assert.Equal(end, added.EndDate);
            Assert.Empty(added.Feedbacks);

            // Check ongoing plan is now completed
            Assert.Equal(DateTime.UtcNow.AddDays(-1).Date, currentPlan.EndDate);
        }


        [Fact]
        public async Task StartTrainingPhaseCommand_Success()
        {
            GymContext context;
            ILogger<StartTrainingPhaseCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<StartTrainingPhaseCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var athletes = new SQLAthleteRepository(context);

            // Test
            uint userId = 1;
            uint phaseId = 1;
            uint entryStatusId = 1;
            string note = "The note";

            StartTrainingPhaseCommand command = new StartTrainingPhaseCommand(userId, phaseId, entryStatusId, note);
            StartTrainingPhaseCommandHandler handler = new StartTrainingPhaseCommandHandler(athletes, logger);

            Assert.True(await handler.Handle(command, default));

            // Check phase started
            var modified = athletes.Find(userId);
            Assert.Equal(phaseId, modified.TrainingPhases.Last().PhaseId);
            Assert.Equal(DateTime.UtcNow, modified.TrainingPhases.Last().StartDate, new TimeSpan(0, 0, 1));
            Assert.Null(modified.TrainingPhases.Last().EndDate);
            Assert.Equal(DateTime.UtcNow.AddDays(-1), modified.TrainingPhases.First().EndDate.Value, new TimeSpan(0, 0, 1));
            Assert.Equal(note, modified.TrainingPhases.Last().OwnerNote.Body);
        }


        [Fact]
        public async Task CompleteTrainingScheduleCommand_Success()
        {
            GymContext context;
            ILogger<CompleteTrainingScheduleCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<CompleteTrainingScheduleCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var schedules = new SQLTrainingScheduleRepository(context);

            // Test
            uint scheduleId = 1;
            DateTime endDate = DateTime.UtcNow.Date;

            if (schedules.Find(scheduleId).EndDate == endDate)
                throw new NotImplementedException("Wrong test case");

            CompleteTrainingScheduleCommand command = new CompleteTrainingScheduleCommand(scheduleId, endDate);
            CompleteTrainingScheduleCommandHandler handler = new CompleteTrainingScheduleCommandHandler(schedules, logger);

            Assert.True(await handler.Handle(command, default));

            // Check schedule
            var modified = schedules.Find(scheduleId);
            Assert.Equal(endDate, modified.EndDate);
        }


        [Fact]
        public async Task RescheduleTrainingScheduleCommand_Success()
        {
            GymContext context;
            ILogger<RescheduleTrainingPlanCommandHandler> logger;

            (context, _, logger) = ApplicationTestService.InitInMemoryCommandTest<RescheduleTrainingPlanCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var schedules = new SQLTrainingScheduleRepository(context);

            // Test
            uint scheduleId = 2;
            DateTime startDate = DateTime.UtcNow.Date;
            var sched = schedules.Find(scheduleId);

            if (sched.StartDate == startDate || sched.EndDate < startDate || sched.IsCompleted())
                throw new NotImplementedException("Wrong test case");

            RescheduleTrainingPlanCommand command = new RescheduleTrainingPlanCommand(scheduleId, startDate);
            RescheduleTrainingPlanCommandHandler handler = new RescheduleTrainingPlanCommandHandler(schedules, logger);

            Assert.True(await handler.Handle(command, default));

            // Check schedule
            var modified = schedules.Find(scheduleId);
            Assert.Equal(startDate, modified.StartDate);
        }


    }
}
