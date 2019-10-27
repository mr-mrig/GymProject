using GymProject.Application.Command;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.MediatorBehavior;
using GymProject.Application.Test.Utils;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.Test.Util;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain;
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

namespace GymProject.Application.Test.UnitTest.CQRS
{
    public class TrainingDomainCommandsTest
    {


        //private string _databaseName;


        //public TrainingDomainCommandsTest(string inMemoryDatabaseName)
        //{
        //    _databaseName = inMemoryDatabaseName;
        //}



        [Fact]
        public async Task PlanDraftWorkoutCommandSuccess()
        {
            // Mocking
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<PlanDraftWorkoutCommandHandler>>();

            // In memory DB
            DatabaseSeed seed = new DatabaseSeed(new GymContext(
                StaticUtilities.GetInMemoryIsolatedDbContextOptions<GymContext>()
                , mediator.Object
                , logger.Object));
            seed.SeedTrainingDomain();

            GymContext context = seed.Context;

            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);
            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);

            //mediator.Setup(x
            //    => x.Send(It.IsAny<PlanDraftWorkoutCommand>(), default))
            //    .Returns(Task.FromResult(true));

            // Test
            uint planId = 1;
            uint weekId = 1;
            uint weekPnum = context.TrainingWeeks.Find(weekId).ProgressiveNumber;

            PlanDraftWorkoutCommand command = new PlanDraftWorkoutCommand(planId, weekId, weekPnum);
            PlanDraftWorkoutCommandHandler handler = new PlanDraftWorkoutCommandHandler(
                workoutRepository, planRepository, logger.Object);

            Assert.True(await handler.Handle(command, default));

            // Check new workout
            WorkoutTemplateRoot added = context.WorkoutTemplates.Last();
            Assert.Equal(seed.TrainingPlans.Count(), context.TrainingPlans.Count());
            Assert.Equal(seed.WorkoutTemplates.Count() + 1, context.WorkoutTemplates.Count());
            Assert.DoesNotContain(added, seed.WorkoutTemplates);

            // Check link with plan
            TrainingWeekEntity week = context.TrainingWeeks.Find(weekId);
            Assert.Equal(added.Id, week.WorkoutIds.Last());
        }


        [Fact]
        public async Task PlanDraftWorkoutCommandFail()
        {
            // Mocking
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<PlanDraftWorkoutCommandHandler>>();

            // In memory DB
            DatabaseSeed seed = new DatabaseSeed(new GymContext(
                StaticUtilities.GetInMemoryIsolatedDbContextOptions<GymContext>()
                , mediator.Object
                , logger.Object));
            seed.SeedTrainingDomain();

            GymContext context = seed.Context;

            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);
            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);

            // Test 1: Cannot create workout
            uint fakePlanId = (uint)context.TrainingPlans.Count() + 1;
            uint weekId = 1;
            uint weekPnum = context.TrainingWeeks.Find(weekId).ProgressiveNumber;

            int workoutsNumberBefore = context.TrainingWeeks.Find(weekId).WorkoutIds.Count;

            PlanDraftWorkoutCommand command = new PlanDraftWorkoutCommand(fakePlanId, weekId, weekPnum);
            PlanDraftWorkoutCommandHandler handler = new PlanDraftWorkoutCommandHandler(
                workoutRepository, planRepository, logger.Object);

             Assert.False(await handler.Handle(command, default));

            // Check no changes
            TrainingWeekEntity week = context.TrainingWeeks.Find(weekId);
            Assert.Equal(workoutsNumberBefore, week.WorkoutIds.Count);
            Assert.Equal(seed.WorkoutTemplates.Count(), context.WorkoutTemplates.Count());


            // Test 2: Cannot link to plan
            uint planId = 1;
            weekId = 1;
            uint fakeWeekPnum = 100;

            command = new PlanDraftWorkoutCommand(planId, weekId, fakeWeekPnum);
            handler = new PlanDraftWorkoutCommandHandler(
                workoutRepository, planRepository, logger.Object);

            Assert.False(await handler.Handle(command, default));

            // Check no changes
            week = context.TrainingWeeks.Find(weekId);
            Assert.Equal(workoutsNumberBefore, week.WorkoutIds.Count);
            Assert.Equal(seed.WorkoutTemplates.Count(), context.WorkoutTemplates.Count());
        }


        //[Fact]
        //public async Task FakePlanDraftWorkoutCommandTransactionalFail()
        //{
        //    // Mocking
        //    var mediator = new Mock<IMediator>();
        //    var logger = new Mock<ILogger<TransactionBehaviour<PlanDraftWorkoutCommand, bool>>>();

        //    // In memory DB
        //    DatabaseSeed seed = new DatabaseSeed(new GymContext(
        //        StaticUtilities.GetInMemoryIsolatedDbContextOptions<GymContext>()
        //        , mediator.Object
        //        , logger.Object));
        //    seed.SeedTrainingDomain();

        //    GymContext context = seed.Context;

        //    ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);
        //    IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);

        //    // Test
        //    TransactionBehaviour<PlanDraftWorkoutCommand, bool> transactionHandler = new TransactionBehaviour<PlanDraftWorkoutCommand, bool>(context, logger.Object);


        //    uint planId = 1;
        //    uint weekId = 1;
        //    uint weekPnum = context.TrainingWeeks.Find(weekId).ProgressiveNumber;

        //    int fakeWeekPnum = 1000;
        //    int weekWorkoutsNumberBefore = context.TrainingWeeks.Find(weekId).WorkoutIds.Count;

        //    PlanDraftWorkoutCommand command = new PlanDraftWorkoutCommand(planId, weekId, weekPnum);

        //    FakePlanDraftWorkoutCommandHandler handler = new FakePlanDraftWorkoutCommandHandler(
        //        workoutRepository,
        //        planRepository,
        //        logger.Object,
        //        fakeWeekPnum);

        //    mediator.Setup(x
        //        => x.Send(It.IsAny<PlanDraftWorkoutCommand>(), default))
        //        .Returns(Task.Run(() => handler));







        //    await transactionHandler.Handle(command, default, null);

        //    //Assert.False(await handler.Handle(command, default));

        //    //// Check no changes -> both operations should fail
        //    //TrainingWeekEntity week = context.TrainingWeeks.Find(weekId);
        //    //Assert.Equal(weekWorkoutsNumberBefore, week.WorkoutIds.Count);
        //    //Assert.Equal(seed.WorkoutTemplates.Count(), context.WorkoutTemplates.Count());
        //}



        [Fact]
        public async Task DraftTrainingPlanCommandSuccess()
        {
            // Mocking
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<CreateDraftTrainingPlanCommandHandler>>();

            // In memory DB
            DatabaseSeed seed = new DatabaseSeed(new GymContext(
                StaticUtilities.GetInMemoryIsolatedDbContextOptions<GymContext>()
                , mediator.Object
                , logger.Object));
            seed.SeedTrainingDomain();

            GymContext context = seed.Context;
            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);

            // Test
            uint userId = 1;

            CreateDraftTrainingPlanCommand command = new CreateDraftTrainingPlanCommand(userId);
            CreateDraftTrainingPlanCommandHandler handler = new CreateDraftTrainingPlanCommandHandler(
                planRepository, logger.Object);

            Assert.True(await handler.Handle(command, default));

            // Check new training plan
            TrainingPlanRoot added = context.TrainingPlans.Last();
            Assert.Equal(userId, added.OwnerId);
            Assert.Empty(added.MuscleFocusIds);
            Assert.Empty(added.RelationsWithChildPlans);
            Assert.Empty(added.RelationsWithParentPlans);
            Assert.Equal(1, added.TrainingWeeks.Count);
            Assert.Equal(string.Empty, added.Name);
        }


        //[Fact]
        //public async Task DeleteTrainingPlanCommandSuccess()
        //{
        //    // Mocking
        //    var mediator = new Mock<IMediator>();
        //    var logger = new Mock<ILogger<DeleteTrainingPlanCommandHandler>>();

        //    // In memory DB
        //    DatabaseSeed seed = new DatabaseSeed(new GymContext(
        //        StaticUtilities.GetInMemoryIsolatedDbContextOptions<GymContext>()
        //        , mediator.Object
        //        , logger.Object));
        //    seed.SeedTrainingDomain();

        //    GymContext context = seed.Context;
        //    ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);
        //    IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);

        //    // Test
        //    uint planId = 1;
        //    TrainingPlanRoot removed = planRepository.Find(planId);

        //    DeleteTrainingPlanCommand command = new DeleteTrainingPlanCommand(planId);
        //    DeleteTrainingPlanCommandHandler handler = new DeleteTrainingPlanCommandHandler(
        //        workoutRepository, planRepository, logger.Object);

        //    Assert.True(await handler.Handle(command, default));

        //    // Check new training plan
        //    Assert.Equal(seed.TrainingPlans.Count() - 1, context.TrainingPlans.Count());
        //    Assert.Equal(seed.WorkoutTemplates.Count() - removed.WorkoutIds.Count(), context.WorkoutTemplates.Count());
        //    Assert.DoesNotContain(removed, context.TrainingPlans);
            
        //    foreach(uint? workoutId in removed.WorkoutIds)
        //        Assert.DoesNotContain(workoutId, context.WorkoutTemplates.ToList().Select(x => x.Id));
        //}


        [Fact]
        public async Task AddWorkingSetIntensityTechniqueCommandSuccess()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<AddWorkingSetIntensityTechniqueCommandHandler>>();

            // In memory DB
            DatabaseSeed seed = new DatabaseSeed(new GymContext(
                StaticUtilities.GetInMemoryIsolatedDbContextOptions<GymContext>()
                , mediator.Object
                , logger.Object));
            seed.SeedTrainingDomain();

            GymContext context = seed.Context;
            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);

            // Test
            uint workoutId = 1;
            uint workUnitPnum = 1;
            uint wsetPnum = 1;
            uint intensityTechniqueId = 100;
            WorkoutTemplateRoot srcWorkout = workoutRepository.Find(workoutId);

            IReadOnlyCollection<uint?> srcTechniques = srcWorkout.CloneWorkingSet(workUnitPnum, wsetPnum).IntensityTechniqueIds;

            AddWorkingSetIntensityTechniqueCommand command = new AddWorkingSetIntensityTechniqueCommand(workoutId, workUnitPnum, wsetPnum, intensityTechniqueId);
            AddWorkingSetIntensityTechniqueCommandHandler handler = new AddWorkingSetIntensityTechniqueCommandHandler(workoutRepository, logger.Object);

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
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<AttachTrainingPlanNoteCommandHandler>>();

            // In memory DB
            DatabaseSeed seed = new DatabaseSeed(new GymContext(
                StaticUtilities.GetInMemoryIsolatedDbContextOptions<GymContext>()
                , mediator.Object
                , logger.Object));
            seed.SeedTrainingDomain();

            GymContext context = seed.Context;
            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);

            // Test
            uint planId = 1;
            uint noteId = 100;
            TrainingPlanRoot srcPlan = planRepository.Find(planId).Clone() as TrainingPlanRoot;

            AttachTrainingPlanNoteCommand command = new AttachTrainingPlanNoteCommand(planId, noteId);
            AttachTrainingPlanNoteCommandHandler handler = new AttachTrainingPlanNoteCommandHandler(planRepository, logger.Object);

            Assert.True(await handler.Handle(command, default));

            // Check modifications
            TrainingPlanRoot newPlan = planRepository.Find(planId);

            Assert.Equal(noteId, newPlan.TrainingPlanNoteId);


            // Test - Clean note
            srcPlan = newPlan;
            uint finalNoteId = 456;

            command = new AttachTrainingPlanNoteCommand(planId, finalNoteId);
            handler = new AttachTrainingPlanNoteCommandHandler(planRepository, logger.Object);

            Assert.True(await handler.Handle(command, default));

            // Check modifications
            TrainingPlanRoot finalPlan = planRepository.Find(planId);

            Assert.Equal(finalNoteId, finalPlan.TrainingPlanNoteId);
        }


        [Fact]
        public async Task AttachWorkUnitTemplateNoteCommandSuccess()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<AttachWorkUnitTemplateNoteCommandHandler>>();

            // In memory DB
            DatabaseSeed seed = new DatabaseSeed(new GymContext(
                StaticUtilities.GetInMemoryIsolatedDbContextOptions<GymContext>()
                , mediator.Object
                , logger.Object));
            seed.SeedTrainingDomain();

            GymContext context = seed.Context;
            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);

            // Test
            uint workoutId = 2;
            uint workUnitPnum = 0;
            uint noteId = 1231;
            WorkoutTemplateRoot srcWorkout = workoutRepository.Find(workoutId);

            AttachWorkUnitTemplateNoteCommand command = new AttachWorkUnitTemplateNoteCommand(workoutId, workUnitPnum, noteId);
            AttachWorkUnitTemplateNoteCommandHandler handler = new AttachWorkUnitTemplateNoteCommandHandler(workoutRepository, logger.Object);

            Assert.True(await handler.Handle(command, default));

            // Check modifications
            WorkoutTemplateRoot newWorkout = workoutRepository.Find(workoutId);

            Assert.Equal(noteId, newWorkout.CloneWorkUnit(workUnitPnum).WorkUnitNoteId);
        }



        [Fact]
        public async Task CreateDraftTrainingPlanCommandSuccess()
        {
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<CreateDraftTrainingPlanCommandHandler>>();

            // In memory DB
            DatabaseSeed seed = new DatabaseSeed(new GymContext(
                StaticUtilities.GetInMemoryIsolatedDbContextOptions<GymContext>()
                , mediator.Object
                , logger.Object));
            seed.SeedTrainingDomain();

            GymContext context = seed.Context;
            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);

            // Test
            uint ownerId = 3;

            CreateDraftTrainingPlanCommand command = new CreateDraftTrainingPlanCommand(ownerId);
            CreateDraftTrainingPlanCommandHandler handler = new CreateDraftTrainingPlanCommandHandler(planRepository, logger.Object);

            Assert.True(await handler.Handle(command, default));

            // Check modifications
            TrainingPlanRoot dest = context.TrainingPlans.Last();

            Assert.NotNull(dest);
            Assert.False(dest.IsTemplate);
            Assert.False(dest.IsBookmarked);
            Assert.Empty(dest.WorkoutIds);
            Assert.Single(dest.TrainingWeeks);
            // Not testing everything ...
        }


        [Fact]
        public async Task DeleteExcerciseFromWorkoutCommandSuccess()
        {
            // Mocking
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<DeleteExcerciseFromWorkoutCommandHandler>>();

            // In memory DB
            DatabaseSeed seed = new DatabaseSeed(new GymContext(
                StaticUtilities.GetInMemoryIsolatedDbContextOptions<GymContext>()
                , mediator.Object
                , logger.Object));
            seed.SeedTrainingDomain();

            GymContext context = seed.Context;
            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);

            // Test
            uint id = 1;
            uint workUnitPnum = 1;
            WorkoutTemplateRoot src = workoutRepository.Find(id).Clone() as WorkoutTemplateRoot;

            DeleteExcerciseFromWorkoutCommand command = new DeleteExcerciseFromWorkoutCommand(id, workUnitPnum);
            DeleteExcerciseFromWorkoutCommandHandler handler = new DeleteExcerciseFromWorkoutCommandHandler(
                workoutRepository, logger.Object);

            Assert.True(await handler.Handle(command, default));

            // Check
            WorkoutTemplateRoot dest = workoutRepository.Find(id);
            Assert.Equal(src.WorkUnits.Count, dest.WorkUnits.Count + 1);
            Assert.DoesNotContain(src.CloneWorkUnit(workUnitPnum), dest.WorkUnits);
        }


        [Fact]
        public async Task DeleteExcerciseFromWorkoutCommandFail()
        {
            // Mocking
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<DeleteExcerciseFromWorkoutCommandHandler>>();

            // In memory DB
            DatabaseSeed seed = new DatabaseSeed(new GymContext(
                StaticUtilities.GetInMemoryIsolatedDbContextOptions<GymContext>()
                , mediator.Object
                , logger.Object));
            seed.SeedTrainingDomain();

            GymContext context = seed.Context;
            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);

            // Test
            uint id = 1;
            uint workUnitPnum = 12;
            WorkoutTemplateRoot src = workoutRepository.Find(id).Clone() as WorkoutTemplateRoot;

            DeleteExcerciseFromWorkoutCommand command = new DeleteExcerciseFromWorkoutCommand(id, workUnitPnum);
            DeleteExcerciseFromWorkoutCommandHandler handler = new DeleteExcerciseFromWorkoutCommandHandler(
                workoutRepository, logger.Object);

            Assert.False(await handler.Handle(command, default));

            // Check
            WorkoutTemplateRoot dest = workoutRepository.Find(id);
            Assert.Equal(src.WorkUnits.Count, dest.WorkUnits.Count);
        }


        [Fact]
        public async Task DeleteTrainingPlanCommandSuccess()
        {
            // Mocking
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<DeleteTrainingPlanCommandHandler>>();

            // In memory DB
            DatabaseSeed seed = new DatabaseSeed(new GymContext(
                StaticUtilities.GetInMemoryIsolatedDbContextOptions<GymContext>()
                , mediator.Object
                , logger.Object));
            seed.SeedTrainingDomain();

            GymContext context = seed.Context;
            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);
            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);

            // Test
            uint id = 1;
            TrainingPlanRoot src = planRepository.Find(id).Clone() as TrainingPlanRoot;

            DeleteTrainingPlanCommand command = new DeleteTrainingPlanCommand(id);
            DeleteTrainingPlanCommandHandler handler = new DeleteTrainingPlanCommandHandler(
                workoutRepository, planRepository, logger.Object);

            Assert.True(await handler.Handle(command, default));

            // Check
            foreach (uint workoutId in src.WorkoutIds.Select(x => x.Value))
                Assert.Null(workoutRepository.Find(workoutId));

            Assert.Null(planRepository.Find(id));
        }


        [Fact]
        public async Task DeleteTrainingPlanCommandFail()
        {
            // Mocking
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<DeleteTrainingPlanCommandHandler>>();

            // In memory DB
            DatabaseSeed seed = new DatabaseSeed(new GymContext(
                StaticUtilities.GetInMemoryIsolatedDbContextOptions<GymContext>()
                , mediator.Object
                , logger.Object));
            seed.SeedTrainingDomain();

            GymContext context = seed.Context;
            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);
            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);

            // Test
            uint id = uint.MaxValue;
            int planNum = context.TrainingPlans.Count();
            int woNum = context.WorkoutTemplates.Count();

            DeleteTrainingPlanCommand command = new DeleteTrainingPlanCommand(id);
            DeleteTrainingPlanCommandHandler handler = new DeleteTrainingPlanCommandHandler(
                workoutRepository, planRepository, logger.Object);

            Assert.False(await handler.Handle(command, default));

            // Check
            Assert.Equal(planNum, context.TrainingPlans.Count());
            Assert.Equal(woNum, context.WorkoutTemplates.Count());
        }
        

        [Fact]
        public async Task DeleteWorkingSetTemplateCommandSuccess()
        {
            // Mocking
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<DeleteWorkingSetTemplateCommandHandler>>();

            // In memory DB
            DatabaseSeed seed = new DatabaseSeed(new GymContext(
                StaticUtilities.GetInMemoryIsolatedDbContextOptions<GymContext>()
                , mediator.Object
                , logger.Object));
            seed.SeedTrainingDomain();

            GymContext context = seed.Context;
            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);

            // Test
            uint id = 1;
            uint workUnitPnum = 0;
            uint wsPnum = 1;
            WorkoutTemplateRoot src = workoutRepository.Find(id).Clone() as WorkoutTemplateRoot;

            DeleteWorkingSetTemplateCommand command = new DeleteWorkingSetTemplateCommand(id, workUnitPnum, wsPnum);
            DeleteWorkingSetTemplateCommandHandler handler = new DeleteWorkingSetTemplateCommandHandler(
                workoutRepository, logger.Object);

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

            (context, _, logger) = StaticUtilities.InitTest<DetachTrainingPlanNoteCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var planRepo = new SQLTrainingPlanRepository(context);

            // Test
            uint id = 1;
            var src = planRepo.Find(id).Clone() as TrainingPlanRoot;

            DetachTrainingPlanNoteCommand command = new DetachTrainingPlanNoteCommand(id);
            DetachTrainingPlanNoteCommandHandler handler = new DetachTrainingPlanNoteCommandHandler(
                planRepo, logger);

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = planRepo.Find(id);
            Assert.Null(dest.TrainingPlanNoteId);
        }


        [Fact]
        public async Task DetachWorkUnitTemplateNoteCommandSuccess()
        {
            GymContext context;
            ILogger<DetachWorkUnitTemplateNoteCommandHandler> logger;

            (context, _, logger) = StaticUtilities.InitTest<DetachWorkUnitTemplateNoteCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var workoutRepo = new SQLWorkoutTemplateRepository(context);

            // Test
            uint id = 1;
            uint workUnitPnum = 0;
            var src = workoutRepo.Find(id).Clone() as WorkoutTemplateRoot;

            DetachWorkUnitTemplateNoteCommand command = new DetachWorkUnitTemplateNoteCommand(id, workUnitPnum);
            DetachWorkUnitTemplateNoteCommandHandler handler = new DetachWorkUnitTemplateNoteCommandHandler(
                workoutRepo, logger);

            throw new NotImplementedException();
            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = workoutRepo.Find(id);
            Assert.Null(dest.CloneWorkUnit(workUnitPnum).WorkUnitNoteId);
        }


        [Fact]
        public async Task PlanDraftExcerciseCommandSuccess()
        {
            GymContext context;
            ILogger<PlanDraftExcerciseCommandHandler> logger;

            (context, _, logger) = StaticUtilities.InitTest<PlanDraftExcerciseCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

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

            (context, _, logger) = StaticUtilities.InitTest<PlanTrainingWeekCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

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
        public async Task PlanTrainingWeekCommandFail()
        {

            throw new NotImplementedException("This is done by the validator... How do we test it?");

            GymContext context;
            ILogger<PlanTrainingWeekCommandHandler> logger;

            (context, _, logger) = StaticUtilities.InitTest<PlanTrainingWeekCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var repo = new SQLTrainingPlanRepository(context);

            // Test
            uint id = 1;
            uint weekTypeId = 13;
            var src = repo.Find(id).Clone() as TrainingPlanRoot;

            PlanTrainingWeekCommand command = new PlanTrainingWeekCommand(id, weekTypeId);
            PlanTrainingWeekCommandHandler handler = new PlanTrainingWeekCommandHandler(
                repo, logger);

            Assert.False(await handler.Handle(command, default));

            // Check
            var dest = repo.Find(id);
            Assert.Equal(src.TrainingWeeks.Count(), dest.TrainingWeeks.Count());

            // Test - NULL
            uint? weekTypeId2 = null;

            command = new PlanTrainingWeekCommand(id, weekTypeId2);
            handler = new PlanTrainingWeekCommandHandler(
                repo, logger);

            Assert.False(await handler.Handle(command, default));

            // Check
            dest = repo.Find(id);
            Assert.Equal(src.TrainingWeeks.Count(), dest.TrainingWeeks.Count());
        }


        [Fact]
        public async Task PlanWorkingSetCommandSuccess()
        {
            GymContext context;
            ILogger<PlanWorkingSetCommandHandler> logger;

            (context, _, logger) = StaticUtilities.InitTest<PlanWorkingSetCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var repo = new SQLWorkoutTemplateRepository(context);

            // Test
            int ntests = 50;
            uint id = 1;
            uint workUnitPnum = 0;
            var src = repo.Find(id).Clone() as WorkoutTemplateRoot;

            for(int itest = 0; itest < ntests; itest++)
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
                        ,wsAdded.Effort);

                if (string.IsNullOrWhiteSpace(tempo))
                    Assert.Null(wsAdded.Tempo);
                else
                    Assert.Equal(TUTValue.PlanTUT(tempo) , wsAdded.Tempo);

                if (techniques == null)
                    Assert.Empty(wsAdded.IntensityTechniqueIds);
                else
                    Assert.True(techniques.SequenceEqual(wsAdded.IntensityTechniqueIds));
            }

        }

        [Fact]
        public async Task PlanWorkingSetCommandFail()
        {
            GymContext context;
            ILogger<PlanWorkingSetCommandHandler> logger;

            (context, _, logger) = StaticUtilities.InitTest<PlanWorkingSetCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            var repo = new SQLWorkoutTemplateRepository(context);

            // Test
            uint id = 1;
            uint workUnitPnum = 0;
            var src = repo.Find(id).Clone() as WorkoutTemplateRoot;


            int reps = RandomFieldGenerator.RandomInt(1, 30);
            int? workType = RandomFieldGenerator.ChooseAmongNullable(TimeMeasureUnitEnum.List().Select(x => (int?)x.Id), 0.25f);
            int? rest = RandomFieldGenerator.RandomIntNullable(30, 360, 0.25f);
            int? restMeas = RandomFieldGenerator.ChooseAmongNullable(TimeMeasureUnitEnum.List().Select(x => (int?)x.Id), 0.25f);
            int? effort = RandomFieldGenerator.RandomIntNullable(2, 15, 0.25f);

            int? effortType = 55;


            PlanWorkingSetCommand command = new PlanWorkingSetCommand(id, workUnitPnum, reps,
                workType, rest, restMeas, effort, effortType, null, null);
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

        }



        [Fact]
        public async Task PlanWorkingSetEffortCommandSuccess()
        {
            GymContext context;
            ILogger<PlanWorkingSetEffortCommandHandler> logger;

            (context, _, logger) = StaticUtilities.InitTest<PlanWorkingSetEffortCommandHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

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

            Assert.True(await handler.Handle(command, default));

            // Check
            var dest = repo.Find(id);
            Assert.Equal(src.CloneAllWorkingSets().Count(), dest.CloneAllWorkingSets().Count());
            Assert.Equal(TrainingEffortValue.FromEffort(effort.Value, TrainingEffortTypeEnum.From(effortTypeId.Value)), 
                dest.CloneWorkUnit(workUnitPnum).CloneWorkingSet(wsPnum).Effort);
        }
    }
}
