using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.Test.UnitTestEnvironment;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanNoteAggregate;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Domain.TrainingDomain.WorkUnitTemplateNote;
using GymProject.Infrastructure.Persistence.EFContext;
using GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain;
using GymProject.Infrastructure.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GymProject.Application.Test.UnitTest.CQRS.TrainingDomain
{





    public class TrainingDomainCommandsTest
    {






        [Fact]
        public async Task PlanDraftWorkoutCommand_Success()
        {
            var logger = new Mock<ILogger<PlanDraftWorkoutCommandHandler>>().Object;
            uint weekPnum, lastWoId;
            int workoutsNumberBefore;
            int trainingPlansNumberBefore;
            ITrainingPlanRepository planRepository;
            IWorkoutTemplateRepository workoutRepository;
            IEnumerable<uint?> planWorkouts;
            WorkoutTemplateRoot added;
            int destWoCount, destPlansCount;

            // Arrange
            uint planId = 1;
            uint weekId = 1;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (var context = await factory.CreateContextAsync())
                {
                    planRepository = new SQLTrainingPlanRepository(context);
                    workoutRepository = new SQLWorkoutTemplateRepository(context);

                    planWorkouts = planRepository.Find(planId).WorkoutIds;
                    weekPnum = context.TrainingWeeks.Find(weekId).ProgressiveNumber;
                    workoutsNumberBefore = context.WorkoutTemplates.Count();
                    trainingPlansNumberBefore = context.TrainingPlans.Count();
                }
                // Test
                using (var context = await factory.CreateContextAsync())
                {
                    planRepository = new SQLTrainingPlanRepository(context);
                    workoutRepository = new SQLWorkoutTemplateRepository(context);

                    PlanDraftWorkoutCommand command = new PlanDraftWorkoutCommand(planId, weekId, weekPnum);
                    PlanDraftWorkoutCommandHandler handler = new PlanDraftWorkoutCommandHandler(
                        workoutRepository, planRepository, logger);

                    Assert.True(await handler.Handle(command, default));
                    added = context.WorkoutTemplates.ToList().Last();
                    destPlansCount = context.TrainingPlans.Count();
                    destWoCount = context.WorkoutTemplates.Count();
                    lastWoId = context.TrainingWeeks.Find(weekId).WorkoutIds.Last().Value;
                }
                // Check
                using (var context = await factory.CreateContextAsync())
                {
                    planRepository = new SQLTrainingPlanRepository(context); 
                    var dest = planRepository.Find(planId).WorkoutIds;

                    Assert.Equal(trainingPlansNumberBefore, destPlansCount);
                    Assert.Equal(workoutsNumberBefore + 1, destWoCount);
                    Assert.Equal(planWorkouts.Count() + 1, dest.Count);
                    Assert.Contains(lastWoId, dest);

                    // Check link with plan
                    TrainingWeekEntity week = context.TrainingWeeks.Find(weekId);
                    Assert.Equal(added.Id, week.WorkoutIds.Last());
                }
            }
        }

        [Fact]
        public async Task PlanDraftWorkoutCommand_Fail()
        {
            var logger = new Mock<ILogger<PlanDraftWorkoutCommandHandler>>().Object;
            int trainingPlansNumberBefore;
            int workoutsNumberBefore;
            ITrainingPlanRepository planRepository;
            IWorkoutTemplateRepository workoutRepository;
            uint weekId, weekPnum;

            // Arrange
            uint fakePlanId;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (var context = await factory.CreateContextAsync())
                {
                    planRepository = new SQLTrainingPlanRepository(context);
                    workoutRepository = new SQLWorkoutTemplateRepository(context);

                    fakePlanId = (uint)context.TrainingPlans.Count() + 1;
                    weekId = 1;
                    weekPnum = context.TrainingWeeks.Find(weekId).ProgressiveNumber;
                }

                // Test 1: Cannot create workout
                using (var context = await factory.CreateContextAsync())
                {
                    planRepository = new SQLTrainingPlanRepository(context);
                    workoutRepository = new SQLWorkoutTemplateRepository(context);


                    workoutsNumberBefore = context.WorkoutTemplates.Count();
                    trainingPlansNumberBefore = context.TrainingPlans.Count();

                    PlanDraftWorkoutCommand command = new PlanDraftWorkoutCommand(fakePlanId, weekId, weekPnum);
                    PlanDraftWorkoutCommandHandler handler = new PlanDraftWorkoutCommandHandler(
                        workoutRepository, planRepository, logger);

                    Assert.False(await handler.Handle(command, default));
                }
                // Check no changes
                using (GymContext context = await factory.CreateContextAsync())
                {
                    planRepository = new SQLTrainingPlanRepository(context);
                    workoutRepository = new SQLWorkoutTemplateRepository(context);

                    Assert.Equal(trainingPlansNumberBefore, context.TrainingPlans.Count());
                    Assert.Equal(workoutsNumberBefore, context.WorkoutTemplates.Count());
                }
                // Test 2: Cannot link to plan
                using (var context = await factory.CreateContextAsync())
                {
                    planRepository = new SQLTrainingPlanRepository(context);
                    workoutRepository = new SQLWorkoutTemplateRepository(context);

                    uint planId = 1;
                    weekId = 1;
                    uint fakeWeekPnum = 100;

                    PlanDraftWorkoutCommand command = new PlanDraftWorkoutCommand(planId, weekId, fakeWeekPnum);
                    PlanDraftWorkoutCommandHandler handler = new PlanDraftWorkoutCommandHandler(
                        workoutRepository, planRepository, logger);

                    Assert.False(await handler.Handle(command, default));
                }
                // Check no changes
                using (GymContext context = await factory.CreateContextAsync())
                {
                    Assert.Equal(workoutsNumberBefore, context.WorkoutTemplates.Count());
                }
            }
        }

        [Fact]
        public async Task DraftTrainingPlanCommand_Success()
        {
            var logger = new Mock<ILogger<CreateDraftTrainingPlanCommandHandler>>().Object;
            int workoutsNumberBefore;
            IAthleteRepository athleteRepo;
            ITrainingPlanRepository planRepository;
            uint addedId;

            // Arrange
            uint userId = 1;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athleteRepo = new SQLAthleteRepository(context);
                    planRepository = new SQLTrainingPlanRepository(context);
                    workoutsNumberBefore = context.TrainingPlans.Count();
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athleteRepo = new SQLAthleteRepository(context);
                    planRepository = new SQLTrainingPlanRepository(context);

                    CreateDraftTrainingPlanCommand command = new CreateDraftTrainingPlanCommand(userId);
                    CreateDraftTrainingPlanCommandHandler handler = new CreateDraftTrainingPlanCommandHandler(planRepository, athleteRepo, logger);

                    Assert.True(await handler.Handle(command, default));
                    addedId = context.TrainingPlans.ToList().Last().Id.Value;
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    planRepository = new SQLTrainingPlanRepository(context);
                    athleteRepo = new SQLAthleteRepository(context);

                    // Check new training plan
                    //int workoutsNumberAfter = context.TrainingPlans.Count();
                    var added = planRepository.Find(addedId);
                    var dest = athleteRepo.Find(userId);

                    //Assert.Equal(workoutsNumberBefore + 1, workoutsNumberAfter);
                    Assert.Equal(userId, added.OwnerId);
                    Assert.Equal(1, added.TrainingWeeks.Count);
                    Assert.Empty(added.TrainingWeeks.SelectMany(x => x.WorkoutIds));

                    // Check training plan in user library
                    Assert.Contains(added.Id.Value, dest.TrainingPlans.Select(x => x.TrainingPlanId));
                }
            }
        }

        [Fact]
        public async Task AddWorkingSetIntensityTechniqueCommand_Success()
        {
            var logger = new Mock<ILogger<AddWorkingSetIntensityTechniqueCommandHandler>>().Object;
            IReadOnlyCollection<uint?> srcTechniques;
            IWorkoutTemplateRepository workoutRepository;

            // Arrange 
            uint workoutId = 1;
            uint workUnitPnum = 1;
            uint wsetPnum = 1;
            uint intensityTechniqueId = 2;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);
                    srcTechniques = workoutRepository.Find(workoutId).CloneWorkingSet(workUnitPnum, wsetPnum).IntensityTechniqueIds;

                    if (srcTechniques.Contains(intensityTechniqueId))
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);

                    AddWorkingSetIntensityTechniqueCommand command = new AddWorkingSetIntensityTechniqueCommand(workoutId, workUnitPnum, wsetPnum, intensityTechniqueId);
                    AddWorkingSetIntensityTechniqueCommandHandler handler = new AddWorkingSetIntensityTechniqueCommandHandler(workoutRepository, logger);

                    Assert.True(await handler.Handle(command, default));
                }

                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);

                    WorkoutTemplateRoot newWorkout = workoutRepository.Find(workoutId);
                    IReadOnlyCollection<uint?> destTechniques = newWorkout.CloneWorkingSet(workUnitPnum, wsetPnum).IntensityTechniqueIds;

                    Assert.Equal(srcTechniques.Count() + 1, destTechniques.Count);
                    Assert.Equal(intensityTechniqueId, destTechniques.Last().Value);
                    Assert.True(srcTechniques.SequenceEqual(destTechniques.Take(destTechniques.Count - 1)));
                }
            }
        }

        [Fact]
        public async Task AttachTrainingPlanNoteCommand_Success()
        {
            ILogger<AttachTrainingPlanNoteCommandHandler> logger = new Mock<ILogger<AttachTrainingPlanNoteCommandHandler>>().Object;
            UserTrainingPlanEntity src;
            IAthleteRepository athletes;

            // Arrange
            uint planId = 1;
            uint userId = 1;
            uint noteId = 2;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    src = athletes.Find(planId).CloneTrainingPlanOrDefault(planId);

                    if (src.TrainingPlanNoteId != null && src.TrainingPlanNoteId == noteId)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    AttachTrainingPlanNoteCommand command = new AttachTrainingPlanNoteCommand(userId, planId, noteId);
                    AttachTrainingPlanNoteCommandHandler handler = new AttachTrainingPlanNoteCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    Assert.Equal(noteId, athletes.Find(userId).CloneTrainingPlanOrDefault(planId).TrainingPlanNoteId);
                }
            }
        }

        [Fact]
        public async Task AttachTrainingPlanNoteCommand_NoteIdNotFound_Fail()
        {
            ILogger<AttachTrainingPlanNoteCommandHandler> logger = new Mock<ILogger<AttachTrainingPlanNoteCommandHandler>>().Object;
            IAthleteRepository athletes;

            // Arrange
            uint planId = 1;
            uint userId = 1;
            uint noteId = 456;
            UserTrainingPlanEntity src;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    src = athletes.Find(planId).CloneTrainingPlanOrDefault(planId);

                    if (src.TrainingPlanNoteId != null && src.TrainingPlanNoteId == noteId)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    AttachTrainingPlanNoteCommand command = new AttachTrainingPlanNoteCommand(userId, planId, noteId);
                    AttachTrainingPlanNoteCommandHandler handler = new AttachTrainingPlanNoteCommandHandler(athletes, logger);

                    Assert.False(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    Assert.Equal(src.TrainingPlanNoteId, athletes.Find(userId).CloneTrainingPlanOrDefault(planId).TrainingPlanNoteId);
                }
            }
        }

        [Fact]
        public async Task AttachWorkUnitTemplateNoteCommand_Success()
        {
            var logger = new Mock<ILogger<AttachWorkUnitTemplateNoteCommandHandler>>().Object;
            IWorkoutTemplateRepository workoutRepository;

            // Arrange
            uint workoutId = 2;
            uint workUnitPnum = 0;
            uint noteId = 2;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);

                    var src = workoutRepository.Find(workoutId);

                    if (src.CloneWorkUnit(workUnitPnum).WorkUnitNoteId != null && src.CloneWorkUnit(workUnitPnum).WorkUnitNoteId == noteId)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);

                    AttachWorkUnitTemplateNoteCommand command = new AttachWorkUnitTemplateNoteCommand(workoutId, workUnitPnum, noteId);
                    AttachWorkUnitTemplateNoteCommandHandler handler = new AttachWorkUnitTemplateNoteCommandHandler(workoutRepository, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);
                    WorkoutTemplateRoot newWorkout = workoutRepository.Find(workoutId);

                    Assert.Equal(noteId, newWorkout.CloneWorkUnit(workUnitPnum).WorkUnitNoteId);
                }
            }
        }

        [Fact]
        public async Task CreateDraftTrainingPlanCommand_Success()
        {
            var logger = new Mock<ILogger<CreateDraftTrainingPlanCommandHandler>>().Object;
            IAthleteRepository athleteRepo;
            ITrainingPlanRepository planRepository;

            // Arrange
            uint ownerId = 3;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athleteRepo = new SQLAthleteRepository(context);
                    planRepository = new SQLTrainingPlanRepository(context);
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athleteRepo = new SQLAthleteRepository(context);
                    planRepository = new SQLTrainingPlanRepository(context);

                    CreateDraftTrainingPlanCommand command = new CreateDraftTrainingPlanCommand(ownerId);
                    CreateDraftTrainingPlanCommandHandler handler = new CreateDraftTrainingPlanCommandHandler(planRepository, athleteRepo, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athleteRepo = new SQLAthleteRepository(context);

                    // Check Training Plan Created
                    TrainingPlanRoot dest = context.TrainingPlans.ToList().Last();

                    Assert.NotNull(dest);
                    Assert.Empty(dest.WorkoutIds);
                    Assert.Empty(dest.TrainingWeeks);
                    // Not testing everything ...

                    // Check Training Plan in User Library
                    AthleteRoot destAthlete = athleteRepo.Find(ownerId);

                    Assert.Contains(dest.Id.Value, destAthlete.TrainingPlans.Select(x => x.TrainingPlanId));
                }
            }
        }


        [Fact]
        public async Task DeleteExcerciseFromWorkoutCommand_Success()
        {
            var logger = new Mock<ILogger<DeleteExcerciseFromWorkoutCommandHandler>>().Object;
            IWorkoutTemplateRepository workoutRepository;

            // Arrange
            uint id = 1;
            uint workUnitPnum = 0;
            WorkoutTemplateRoot src;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);
                    src = workoutRepository.Find(id).Clone() as WorkoutTemplateRoot;
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);

                    DeleteExcerciseFromWorkoutCommand command = new DeleteExcerciseFromWorkoutCommand(id, workUnitPnum);
                    DeleteExcerciseFromWorkoutCommandHandler handler = new DeleteExcerciseFromWorkoutCommandHandler(
                        workoutRepository, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);
                    WorkoutTemplateRoot dest = workoutRepository.Find(id);

                    Assert.Equal(src.WorkUnits.Count, dest.WorkUnits.Count + 1);
                    Assert.DoesNotContain(src.CloneWorkUnit(workUnitPnum), dest.WorkUnits);
                    // Check progressive numbers are consistent
                    Assert.True(Enumerable.Range(0, src.WorkUnits.Count)
                        .SequenceEqual(src.WorkUnits.Select(x => (int)x.ProgressiveNumber).OrderBy(x => x)));
                }
            }
        }

        [Fact]
        public async Task DeleteExcerciseFromWorkoutCommand_WorkUnitNotFound_Fail()
        {
            var logger = new Mock<ILogger<DeleteExcerciseFromWorkoutCommandHandler>>().Object;
            WorkoutTemplateRoot src;
            IWorkoutTemplateRepository workoutRepository;

            // Arrange
            uint id = 1;
            uint workUnitPnum = 12;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);
                    src = workoutRepository.Find(id).Clone() as WorkoutTemplateRoot;
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);

                    DeleteExcerciseFromWorkoutCommand command = new DeleteExcerciseFromWorkoutCommand(id, workUnitPnum);
                    DeleteExcerciseFromWorkoutCommandHandler handler = new DeleteExcerciseFromWorkoutCommandHandler(
                        workoutRepository, logger);

                    Assert.False(await handler.Handle(command, default));
                }

                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    // No cheanges
                    workoutRepository = new SQLWorkoutTemplateRepository(context);

                    WorkoutTemplateRoot dest = workoutRepository.Find(id);
                    Assert.Equal(src.WorkUnits.Count, dest.WorkUnits.Count);
                }
            }
        }

        [Fact]
        public async Task DeleteTrainingPlanCommand_Success()
        {
            var logger = new Mock<ILogger<DeleteTrainingPlanCommandHandler>>().Object;
            TrainingPlanRoot src;
            IWorkoutTemplateRepository workoutRepository;
            ITrainingPlanRepository planRepository;

            // Arrange
            uint id = 1;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);
                    planRepository = new SQLTrainingPlanRepository(context);
                    src = planRepository.Find(id).Clone() as TrainingPlanRoot;
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);
                    planRepository = new SQLTrainingPlanRepository(context);

                    DeleteTrainingPlanCommand command = new DeleteTrainingPlanCommand(id);
                    DeleteTrainingPlanCommandHandler handler = new DeleteTrainingPlanCommandHandler(
                        workoutRepository, planRepository, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);
                    planRepository = new SQLTrainingPlanRepository(context);

                    foreach (uint workoutId in src.WorkoutIds.Select(x => x.Value))
                        Assert.Null(workoutRepository.Find(workoutId));

                    Assert.Null(planRepository.Find(id));
                    Assert.DoesNotContain(id, context.TrainingPlans.Select(x => x.Id.Value));
                }
            }
        }

        [Fact]
        public async Task DeleteTrainingPlanCommand_PlanNotFound_Fail()
        {
            var logger = new Mock<ILogger<DeleteTrainingPlanCommandHandler>>().Object;
            int planNum;
            int woNum;
            IWorkoutTemplateRepository workoutRepository;
            ITrainingPlanRepository planRepository;

            // Arrange
            uint id = uint.MaxValue;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);
                    planRepository = new SQLTrainingPlanRepository(context);

                    planNum = context.TrainingPlans.Count();
                    woNum = context.WorkoutTemplates.Count();
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);
                    planRepository = new SQLTrainingPlanRepository(context);

                    DeleteTrainingPlanCommand command = new DeleteTrainingPlanCommand(id);
                    DeleteTrainingPlanCommandHandler handler = new DeleteTrainingPlanCommandHandler(
                        workoutRepository, planRepository, logger);

                    Assert.False(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    // No changes
                    Assert.Equal(planNum, context.TrainingPlans.Count());
                    Assert.Equal(woNum, context.WorkoutTemplates.Count());
                }
            }
        }

        [Fact]
        public async Task DeleteWorkingSetTemplateCommand_Success()
        {
            var logger = new Mock<ILogger<DeleteWorkingSetTemplateCommandHandler>>().Object;
            WorkoutTemplateRoot src;
            IWorkoutTemplateRepository workoutRepository;

            // Arrange
            uint id = 1;
            uint workUnitPnum = 0;
            uint wsPnum = 1;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);
                    src = workoutRepository.Find(id).Clone() as WorkoutTemplateRoot;
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);

                    DeleteWorkingSetTemplateCommand command = new DeleteWorkingSetTemplateCommand(id, workUnitPnum, wsPnum);
                    DeleteWorkingSetTemplateCommandHandler handler = new DeleteWorkingSetTemplateCommandHandler(
                        workoutRepository, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepository = new SQLWorkoutTemplateRepository(context);

                    WorkoutTemplateRoot dest = workoutRepository.Find(id);
                    Assert.Equal(src.CloneAllWorkingSets().Count(), dest.CloneAllWorkingSets().Count() + 1);
                }
            }
        }

        [Fact]
        public async Task DetachTrainingPlanNoteCommand_Success()
        {
            var logger = new Mock<ILogger<DetachTrainingPlanNoteCommandHandler>>().Object;
            IAthleteRepository athletes;

            // Arrange
            uint userId = 1;
            uint planId = 1;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    if (athletes.Find(userId).CloneTrainingPlanOrDefault(planId).TrainingPlanNoteId == null)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    DetachTrainingPlanNoteCommand command = new DetachTrainingPlanNoteCommand(userId, planId);
                    DetachTrainingPlanNoteCommandHandler handler = new DetachTrainingPlanNoteCommandHandler(
                        athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    var dest = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);
                    Assert.Null(dest.TrainingPlanNoteId);
                }
            }
        }

        [Fact]
        public async Task DetachWorkUnitTemplateNoteCommand_Success()
        {
            var logger = new Mock<ILogger<DetachWorkUnitTemplateNoteCommandHandler>>().Object;
            IWorkoutTemplateRepository workoutRepo;

            // Arrange
            uint id = 1;
            uint workUnitPnum = 0;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepo = new SQLWorkoutTemplateRepository(context);
                    var src = workoutRepo.Find(id).Clone() as WorkoutTemplateRoot;

                    if (src.CloneWorkUnit(workUnitPnum).WorkUnitNoteId == null)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepo = new SQLWorkoutTemplateRepository(context);

                    DetachWorkUnitTemplateNoteCommand command = new DetachWorkUnitTemplateNoteCommand(id, workUnitPnum);
                    DetachWorkUnitTemplateNoteCommandHandler handler = new DetachWorkUnitTemplateNoteCommandHandler(
                        workoutRepo, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workoutRepo = new SQLWorkoutTemplateRepository(context);

                    var dest = workoutRepo.Find(id);
                    Assert.Null(dest.CloneWorkUnit(workUnitPnum).WorkUnitNoteId);
                }
            }
        }

        [Fact]
        public async Task PlanDraftExcerciseCommand_Success()
        {
            var logger = new Mock<ILogger<PlanDraftExcerciseCommandHandler>>().Object;
            WorkoutTemplateRoot src;

            // Arrange
            uint id = 1;
            uint excerciseId = 3;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var repo = new SQLWorkoutTemplateRepository(context);
                    src = repo.Find(id).Clone() as WorkoutTemplateRoot;
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var repo = new SQLWorkoutTemplateRepository(context);

                    PlanDraftExcerciseCommand command = new PlanDraftExcerciseCommand(id, excerciseId);
                    PlanDraftExcerciseCommandHandler handler = new PlanDraftExcerciseCommandHandler(
                        repo, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var repo = new SQLWorkoutTemplateRepository(context);
                    var dest = repo.Find(id);
                    Assert.Equal(src.WorkUnits.Count() + 1, dest.WorkUnits.Count());
                }
            }
        }

        [Fact]
        public async Task PlanTrainingWeekCommand_Success()
        {
            var logger = new Mock<ILogger<PlanTrainingWeekCommandHandler>>().Object;
            TrainingPlanRoot src;
            ITrainingPlanRepository repo;

            // Arrange
            uint id = 1;
            uint weekTypeId = 1;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLTrainingPlanRepository(context);
                    src = repo.Find(id).Clone() as TrainingPlanRoot;
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLTrainingPlanRepository(context);

                    PlanTrainingWeekCommand command = new PlanTrainingWeekCommand(id, weekTypeId);
                    PlanTrainingWeekCommandHandler handler = new PlanTrainingWeekCommandHandler(
                        repo, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLTrainingPlanRepository(context);
                    var dest = repo.Find(id);

                    Assert.Equal(src.TrainingWeeks.Count(), dest.TrainingWeeks.Count() - 1);
                }
            }
        }

        [Fact]
        public async Task PlanWorkingSetCommand_DraftWorkingSet_Success()
        {
            var logger = new Mock<ILogger<PlanWorkingSetCommandHandler>>().Object;
            WorkoutTemplateRoot src;
            IWorkoutTemplateRepository repo;

            // Arrange
            uint id = 1;
            uint workUnitPnum = 0;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);
                    src = repo.Find(id).Clone() as WorkoutTemplateRoot;
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);

                    PlanWorkingSetCommand command = new PlanWorkingSetCommand(id, workUnitPnum, null, null, null, null, null, null, null, null);
                    PlanWorkingSetCommandHandler handler = new PlanWorkingSetCommandHandler(repo, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);
                    var dest = repo.Find(id);
                    var wsAdded = dest.CloneWorkUnit(workUnitPnum).WorkingSets.Last();

                    Assert.Equal(src.CloneAllWorkingSets().Count() + 1, dest.CloneAllWorkingSets().Count());
                    Assert.Null(wsAdded.Repetitions);
                    Assert.Null(wsAdded.Rest);
                    Assert.Null(wsAdded.Effort);
                    Assert.Null(wsAdded.Tempo?.TUT);
                    Assert.Empty(wsAdded.IntensityTechniqueIds);
                }
            }
        }

        [Fact]
        public async Task PlanWorkingSetCommand_FullWorkingSet_Success()
        {
            var logger = new Mock<ILogger<PlanWorkingSetCommandHandler>>().Object;
            WorkoutTemplateRoot src;
            IWorkoutTemplateRepository repo;

            // Arrange
            uint id = 1;
            uint workUnitPnum = 0;
            int reps = 1;
            int workTypeId = WSWorkTypeEnum.RepetitionBasedSerie.Id;
            int rest = 90;
            int restMeasId = 1;
            int effort = 9;
            int effortTypeId = TrainingEffortTypeEnum.RM.Id;
            string tempo = TUTValue.GenericTempo;
            IEnumerable<uint?> techniques = new List<uint?> { 1, 3, };

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);
                    src = repo.Find(id).Clone() as WorkoutTemplateRoot;
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);

                    PlanWorkingSetCommand command = new PlanWorkingSetCommand(id, workUnitPnum, reps,
                        workTypeId, rest, restMeasId, effort, effortTypeId, tempo, techniques);
                    PlanWorkingSetCommandHandler handler = new PlanWorkingSetCommandHandler(
                        repo, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);
                    var dest = repo.Find(id);
                    var wsAdded = dest.CloneWorkUnit(workUnitPnum).WorkingSets.Last();

                    Assert.Equal(src.CloneAllWorkingSets().Count() + 1, dest.CloneAllWorkingSets().Count());
                    Assert.Equal(WSRepetitionsValue.TrackWork(reps, WSWorkTypeEnum.From(workTypeId)), wsAdded.Repetitions);
                    Assert.Equal(RestPeriodValue.SetRest(rest, TimeMeasureUnitEnum.From(restMeasId)), wsAdded.Rest);
                    Assert.Equal(TrainingEffortValue.FromEffort(effort, TrainingEffortTypeEnum.From(effortTypeId)), wsAdded.Effort);
                    Assert.Equal(TUTValue.PlanTUT(tempo), wsAdded.Tempo);
                    Assert.True(techniques.SequenceEqual(wsAdded.IntensityTechniqueIds));
                }
            }
        }

        [Fact]
        public async Task PlanWorkingSetEffortCommand_Success()
        {
            var logger = new Mock<ILogger<PlanWorkingSetEffortCommandHandler>>().Object;
            WorkoutTemplateRoot src;
            IWorkoutTemplateRepository repo;

            // Arrange
            uint id = 2;
            uint workUnitPnum = 0;
            uint wsPnum = 0;
            int? effort = 9;
            int? effortTypeId = 2;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);
                    src = repo.Find(id).Clone() as WorkoutTemplateRoot;

                    if (src.CloneWorkingSet(workUnitPnum, wsPnum).Effort.EffortType.Id != effortTypeId)
                        throw new Exception("Invalid test");    // This means there's something wrong with the context, as everything is added as Reps!
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);

                    PlanWorkingSetEffortCommand command = new PlanWorkingSetEffortCommand(id, workUnitPnum, wsPnum, effort, effortTypeId);
                    PlanWorkingSetEffortCommandHandler handler = new PlanWorkingSetEffortCommandHandler(repo, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);
                    var dest = repo.Find(id);

                    Assert.Equal(src.CloneAllWorkingSets().Count(), dest.CloneAllWorkingSets().Count());
                    Assert.Equal(TrainingEffortValue.FromEffort(effort.Value, TrainingEffortTypeEnum.From(effortTypeId.Value)),
                        dest.CloneWorkUnit(workUnitPnum).CloneWorkingSet(wsPnum).Effort);
                }
            }
        }

        [Fact]
        public async Task PlanWorkingSetEffortCommand_ChangeEffortType_Success()
        {
            var logger = new Mock<ILogger<PlanWorkingSetEffortCommandHandler>>().Object;
            WorkoutTemplateRoot src;
            IWorkoutTemplateRepository repo;

            // Arrange
            uint id = 2;
            uint workUnitPnum = 0;
            uint wsPnum = 0;
            int? effort = 9;
            int? effortTypeId = 3;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);
                    src = repo.Find(id).Clone() as WorkoutTemplateRoot;

                    if (src.CloneWorkingSet(workUnitPnum, wsPnum).Effort.EffortType.Id == effortTypeId)
                        throw new Exception("Invalid test");    // This means there's something wrong with the context, as everything is added as Reps!
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);

                    PlanWorkingSetEffortCommand command = new PlanWorkingSetEffortCommand(id, workUnitPnum, wsPnum, effort, effortTypeId);
                    PlanWorkingSetEffortCommandHandler handler = new PlanWorkingSetEffortCommandHandler(repo, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);
                    var dest = repo.Find(id);

                    Assert.Equal(src.CloneAllWorkingSets().Count(), dest.CloneAllWorkingSets().Count());
                    Assert.Equal(TrainingEffortValue.FromEffort(effort.Value, TrainingEffortTypeEnum.From(effortTypeId.Value)),
                        dest.CloneWorkUnit(workUnitPnum).CloneWorkingSet(wsPnum).Effort);
                }
            }
        }

        [Fact]
        public async Task PlanWorkingSetRepetitionsCommand_Success()
        {
            var logger = new Mock<ILogger<PlanWorkingSetRepetitionsCommandHandler>>().Object;
            WorkoutTemplateRoot src;
            IWorkoutTemplateRepository repo;

            // Arrange
            uint id = 1;
            uint workUnitPnum = 0;
            uint wsPnum = 0;
            int reps = 9;
            int? workTypeId = WSWorkTypeEnum.RepetitionBasedSerie.Id;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);
                    src = repo.Find(id).Clone() as WorkoutTemplateRoot;

                    if (src.CloneWorkingSet(workUnitPnum, wsPnum).Repetitions.WorkType.Id != workTypeId)
                        throw new Exception("Invalid test");    // This means there's something wrong with the context, as everything is added as Reps!
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);

                    PlanWorkingSetRepetitionsCommand command = new PlanWorkingSetRepetitionsCommand(id, workUnitPnum, wsPnum, reps, workTypeId);
                    PlanWorkingSetRepetitionsCommandHandler handler = new PlanWorkingSetRepetitionsCommandHandler(repo, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);
                    var dest = repo.Find(id);

                    Assert.Equal(src.CloneAllWorkingSets().Count(), dest.CloneAllWorkingSets().Count());
                    Assert.Equal(WSRepetitionsValue.TrackWork(reps, WSWorkTypeEnum.From(workTypeId.Value)),
                        dest.CloneWorkUnit(workUnitPnum).CloneWorkingSet(wsPnum).Repetitions);
                }
            }
        }

        [Fact]
        public async Task PlanWorkingSetRepetitionsCommand_ChangeWorkType_Success()
        {
            var logger = new Mock<ILogger<PlanWorkingSetRepetitionsCommandHandler>>().Object;
            WorkoutTemplateRoot src;
            IWorkoutTemplateRepository repo;

            // Arrange
            uint id = 1;
            uint workUnitPnum = 0;
            uint wsPnum = 0;
            int reps = 30;
            int? workTypeId = WSWorkTypeEnum.TimeBasedSerie.Id;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);
                    src = repo.Find(id).Clone() as WorkoutTemplateRoot;

                    if (src.CloneWorkingSet(workUnitPnum, wsPnum).Repetitions.WorkType.Id == workTypeId)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);

                    PlanWorkingSetRepetitionsCommand command = new PlanWorkingSetRepetitionsCommand(id, workUnitPnum, wsPnum, reps, workTypeId);
                    PlanWorkingSetRepetitionsCommandHandler handler = new PlanWorkingSetRepetitionsCommandHandler(repo, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);
                    var dest = repo.Find(id);

                    Assert.Equal(src.CloneAllWorkingSets().Count(), dest.CloneAllWorkingSets().Count());
                    Assert.Equal(WSRepetitionsValue.TrackWork(reps, WSWorkTypeEnum.From(workTypeId.Value)),
                        dest.CloneWorkUnit(workUnitPnum).CloneWorkingSet(wsPnum).Repetitions);
                }
            }
        }

        [Fact]
        public async Task PlanWorkingSetTempoCommand_Success()
        {
            var logger = new Mock<ILogger<PlanWorkingSetTempoCommandHandler>>().Object;
            WorkoutTemplateRoot src;
            IWorkoutTemplateRepository repo;

            // Arrange
            uint id = 1;
            uint workUnitPnum = 0;
            uint wsPnum = 0;
            string tempo = "11X0";

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);
                    src = repo.Find(id).Clone() as WorkoutTemplateRoot;
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);

                    PlanWorkingSetTempoCommand command = new PlanWorkingSetTempoCommand(id, workUnitPnum, wsPnum, tempo);
                    PlanWorkingSetTempoCommandHandler handler = new PlanWorkingSetTempoCommandHandler(repo, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);
                    var dest = repo.Find(id);

                    Assert.Equal(src.CloneAllWorkingSets().Count(), dest.CloneAllWorkingSets().Count());
                    Assert.Equal(TUTValue.PlanTUT(tempo), dest.CloneWorkUnit(workUnitPnum).CloneWorkingSet(wsPnum).Tempo);
                }
            }
        }

        [Fact]
        public async Task RemoveWorkingSetIntensityTechniqueCommand_Success()
        {
            var logger = new Mock<ILogger<RemoveWorkingSetIntensityTechniqueCommandHandler>>().Object;
            WorkoutTemplateRoot src;
            uint? intTechniqueId;
            IWorkoutTemplateRepository repo;

            // Arrange
            uint id = 1;
            uint workUnitPnum = 0;
            uint wsPnum = 0;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);
                    src = repo.Find(id).Clone() as WorkoutTemplateRoot;
                    intTechniqueId = src.CloneWorkingSet(workUnitPnum, wsPnum).IntensityTechniqueIds.FirstOrDefault();

                    if (intTechniqueId == null)
                        throw new Exception("Invalid Test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);

                    RemoveWorkingSetIntensityTechniqueCommand command = new RemoveWorkingSetIntensityTechniqueCommand(id, workUnitPnum, wsPnum, intTechniqueId.Value);
                    RemoveWorkingSetIntensityTechniqueCommandHandler handler = new RemoveWorkingSetIntensityTechniqueCommandHandler(repo, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLWorkoutTemplateRepository(context);
                    var dest = repo.Find(id);

                    Assert.Equal(src.CloneAllWorkingSets().Count(), dest.CloneAllWorkingSets().Count());
                    Assert.Equal(src.CloneWorkUnit(workUnitPnum).CloneWorkingSet(wsPnum).IntensityTechniqueIds.Count() - 1,
                        dest.CloneWorkUnit(workUnitPnum).CloneWorkingSet(wsPnum).IntensityTechniqueIds.Count);
                    Assert.DoesNotContain(intTechniqueId, dest.CloneWorkUnit(workUnitPnum).CloneWorkingSet(wsPnum).IntensityTechniqueIds);
                }
            }
        }

        [Fact]
        public async Task TagTrainingPlanAsHashtagCommand_Success()
        {
            var logger = new Mock<ILogger<TagTrainingPlanAsHashtagCommandHandler>>().Object;
            UserTrainingPlanEntity src;
            IAthleteRepository athletes;

            // Arrange
            uint userId = 1;
            uint planId = 1;
            uint hashtagId = 1;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    src = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

                    if (src.HashtagIds.Contains(hashtagId))
                        throw new Exception("Invalid Test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    TagTrainingPlanAsHashtagCommand command = new TagTrainingPlanAsHashtagCommand(userId, planId, hashtagId);
                    TagTrainingPlanAsHashtagCommandHandler handler = new TagTrainingPlanAsHashtagCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    var dest = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

                    //RIGM: why are they sorted? This is not an issue, though it makes no sense
                    Assert.NotEmpty(dest.HashtagIds);
                    Assert.Equal(hashtagId, dest.HashtagIds.First());
                    Assert.Equal(src.HashtagIds.Count + 1, dest.HashtagIds.Count);
                    //Assert.Equal(dest.HashtagIds.Count - 1,
                    //    (int)context.TrainingPlanHashtags.Single(x => x.HashtagId == hashtagId && x.UserTrainingPlanId == userId).ProgressiveNumber);
                }
            }
        }

        [Fact]
        public async Task TagTrainingPlanAsNewHashtagCommand_Success()
        {
            var logger = new Mock<ILogger<TagTrainingPlanAsNewHashtagCommandHandler>>().Object;
            UserTrainingPlanEntity src;
            ITrainingHashtagRepository hashtags;
            IAthleteRepository athletes;

            // Arrange
            uint userId = 1;
            uint planId = 1;
            string hashtagBody = "MyHashtag";

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    hashtags = new SQLTrainingHashtagRepository(context);
                    src = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    hashtags = new SQLTrainingHashtagRepository(context);

                    TagTrainingPlanAsNewHashtagCommand command = new TagTrainingPlanAsNewHashtagCommand(userId, planId, hashtagBody);
                    TagTrainingPlanAsNewHashtagCommandHandler handler = new TagTrainingPlanAsNewHashtagCommandHandler(athletes, hashtags, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    hashtags = new SQLTrainingHashtagRepository(context);
                    var dest = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);
                    var newHashtag = context.TrainingHashtags.ToList().Last();

                    Assert.Equal(hashtagBody, newHashtag.Hashtag.Body);
                    Assert.Equal("#" + hashtagBody, newHashtag.Hashtag.ToFullHashtag());

                    Assert.Equal(src.HashtagIds.Count + 1, dest.HashtagIds.Count);
                    Assert.Equal(newHashtag.Id, dest.HashtagIds.Last());
                }
            }
        }

        [Fact]
        public async Task UntagTrainingPlanAsHashtagCommand_Success()
        {
            var logger = new Mock<ILogger<UntagTrainingPlanAsHashtagCommandHandler>>().Object;
            UserTrainingPlanEntity src;
            IAthleteRepository athleteRepo;
            ITrainingHashtagRepository hashtagRepo;

            // Arrange
            uint userId = 1;
            uint planId = 1;
            uint hashtagId = 2;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athleteRepo = new SQLAthleteRepository(context);
                    hashtagRepo = new SQLTrainingHashtagRepository(context);
                    src = athleteRepo.Find(userId).CloneTrainingPlanOrDefault(planId);
                    //uint hashtagId = athleteRepo.Find(userId).TrainingPlans.Single(x => x.TrainingPlanId == planId).HashtagIds.First().Value + 1;

                    if (!src.HashtagIds.Contains(hashtagId))
                        throw new Exception("Invalid Test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athleteRepo = new SQLAthleteRepository(context);
                    hashtagRepo = new SQLTrainingHashtagRepository(context);

                    UntagTrainingPlanAsHashtagCommand command = new UntagTrainingPlanAsHashtagCommand(userId, planId, hashtagId);
                    UntagTrainingPlanAsHashtagCommandHandler handler = new UntagTrainingPlanAsHashtagCommandHandler(athleteRepo, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athleteRepo = new SQLAthleteRepository(context);
                    var dest = athleteRepo.Find(userId).CloneTrainingPlanOrDefault(planId);

                    Assert.Equal(src.HashtagIds.Count(), dest.HashtagIds.Count() + 1);
                    Assert.DoesNotContain(hashtagId, dest.HashtagIds);

                    //Assert.True(Enumerable.Range(0, dest.HashtagIds.Count())
                    //    .SequenceEqual(context.TrainingPlanHashtags
                    //        .OrderBy(x => x.ProgressiveNumber)
                    //        .Where(x => x.UserTrainingPlan.TrainingPlanId == planId).Select(y => (int)y.ProgressiveNumber)));
                }
            }
        }


        [Fact]
        public async Task CreateTrainingPhaseCommand_Success()
        {
            var logger = new Mock<ILogger<CreateTrainingPhaseCommandHandler>>().Object;
            uint idAdded;

            // Arrange
            uint entryStatusId = 1;
            string phasename = "Local Phase";

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var repo = new SQLTrainingPhaseRepository(context);

                    CreateTrainingPhaseCommand command = new CreateTrainingPhaseCommand(entryStatusId, phasename);
                    CreateTrainingPhaseCommandHandler handler = new CreateTrainingPhaseCommandHandler(repo, logger);

                    Assert.True(await handler.Handle(command, default));
                    idAdded = context.TrainingPhases.ToList().Last().Id.Value;
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var repo = new SQLTrainingPhaseRepository(context);
                    var dest = repo.Find(idAdded);

                    Assert.Equal(phasename, dest.Name);
                    Assert.Equal(EntryStatusTypeEnum.From((int)entryStatusId), dest.EntryStatus);
                }
            }
        }

        [Fact]
        public async Task CreateTrainingPhaseCommand_DuplicatePhaseName_Fail()
        {
            var logger = new Mock<ILogger<CreateTrainingPhaseCommandHandler>>().Object;

            // Arrange
            uint entryStatusId = 1;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var repo = new SQLTrainingPhaseRepository(context);
                    string phasename = repo.Find(1).Name;   // Duplicate name

                    CreateTrainingPhaseCommand command = new CreateTrainingPhaseCommand(entryStatusId, phasename);
                    CreateTrainingPhaseCommandHandler handler = new CreateTrainingPhaseCommandHandler(repo, logger);

                    Assert.False(await handler.Handle(command, default));
                }
            }
        }

        [Fact]
        public async Task TagTrainingPlanWithTrainingPhaseCommand_Success()
        {
            var logger = new Mock<ILogger<TagTrainingPlanWithTrainingPhaseCommandHandler>>().Object;
            UserTrainingPlanEntity src;
            IAthleteRepository athletes;

            // Arrange
            uint userId = 1;
            uint planId = 1;
            uint phaseId = 3;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    src = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

                    if (src.TrainingPhaseIds.Contains(phaseId))
                        throw new Exception("Invalid Test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    TagTrainingPlanWithTrainingPhaseCommand command = new TagTrainingPlanWithTrainingPhaseCommand(userId, planId, phaseId);
                    TagTrainingPlanWithTrainingPhaseCommandHandler handler = new TagTrainingPlanWithTrainingPhaseCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    var dest = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

                    Assert.Equal(src.TrainingPhaseIds.Count() + 1, dest.TrainingPhaseIds.Count());
                    Assert.Contains(phaseId, dest.TrainingPhaseIds);
                }
            }
        }

        [Fact]
        public async Task UntagTrainingPlanWithTrainingPhaseCommand_Success()
        {
            var logger = new Mock<ILogger<UntagTrainingPlanWithTrainingPhaseCommandHandler>>().Object;
            UserTrainingPlanEntity src;
            IAthleteRepository athletes;

            // Arrange
            uint userId = 1;
            uint planId = 1;
            uint phaseId = 1;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    src = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

                    if (!src.TrainingPhaseIds.Contains(phaseId))
                        throw new Exception("Invalid Test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    UntagTrainingPlanWithTrainingPhaseCommand command = new UntagTrainingPlanWithTrainingPhaseCommand(userId, planId, phaseId);
                    UntagTrainingPlanWithTrainingPhaseCommandHandler handler = new UntagTrainingPlanWithTrainingPhaseCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    var dest = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

                    Assert.Equal(src.TrainingPhaseIds.Count() - 1, dest.TrainingPhaseIds.Count());
                    Assert.DoesNotContain(phaseId, dest.TrainingPhaseIds);
                }
            }
        }

        [Fact]
        public async Task TagTrainingPlanWithProficiencyCommand_Success()
        {
            var logger = new Mock<ILogger<TagTrainingPlanWithProficiencyCommandHandler>>().Object;
            UserTrainingPlanEntity src;
            IAthleteRepository repo;

            // Arrange
            uint userId = 1;
            uint planId = 1;
            uint proficiencyId = 3;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLAthleteRepository(context);
                    src = repo.Find(userId).CloneTrainingPlanOrDefault(planId);

                    if (src.TrainingProficiencyIds.Contains(proficiencyId))
                        throw new Exception("Invalid Test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLAthleteRepository(context);

                    TagTrainingPlanWithProficiencyCommand command = new TagTrainingPlanWithProficiencyCommand(userId, planId, proficiencyId);
                    TagTrainingPlanWithProficiencyCommandHandler handler = new TagTrainingPlanWithProficiencyCommandHandler(repo, logger);

                    Assert.True(await handler.Handle(command, default));

                    // Check
                }
                using (GymContext context = await factory.CreateContextAsync())
                {
                    repo = new SQLAthleteRepository(context);
                    var dest = repo.Find(userId).CloneTrainingPlanOrDefault(planId);

                    Assert.Equal(src.TrainingProficiencyIds.Count() + 1, dest.TrainingProficiencyIds.Count());
                    Assert.Contains(proficiencyId, dest.TrainingProficiencyIds);
                }
            }
        }

        [Fact]
        public async Task UntagTrainingPlanWithProficiencyCommand_Success()
        {
            var logger = new Mock<ILogger<UntagTrainingPlanWithProficiencyCommandHandler>>().Object;
            UserTrainingPlanEntity src;
            IAthleteRepository athletes;

            // Arrange
            uint userId = 1;
            uint planId = 1;
            uint proficiencyId = 1;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    src = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

                    if (!src.TrainingProficiencyIds.Contains(proficiencyId))
                        throw new Exception("Invalid Test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    UntagTrainingPlanWithProficiencyCommand command = new UntagTrainingPlanWithProficiencyCommand(userId, planId, proficiencyId);
                    UntagTrainingPlanWithProficiencyCommandHandler handler = new UntagTrainingPlanWithProficiencyCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    var dest = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

                    Assert.Equal(src.TrainingProficiencyIds.Count() - 1, dest.TrainingProficiencyIds.Count());
                    Assert.DoesNotContain(proficiencyId, dest.TrainingProficiencyIds);
                }
            }
        }

        [Fact]
        public async Task TagTrainingPlanWithMuscleFocusCommand_Success()
        {
            var logger = new Mock<ILogger<TagTrainingPlanWithMuscleFocusCommandHandler>>().Object;
            UserTrainingPlanEntity src;
            IAthleteRepository athletes;

            // Arrange
            uint userId = 1;
            uint planId = 1;
            uint muscleId = 2;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    src = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

                    if (src.MuscleFocusIds.Contains(muscleId))
                        throw new Exception("Invalid Test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    TagTrainingPlanWithMuscleFocusCommand command = new TagTrainingPlanWithMuscleFocusCommand(userId, planId, muscleId);
                    TagTrainingPlanWithMuscleFocusCommandHandler handler = new TagTrainingPlanWithMuscleFocusCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    var dest = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

                    Assert.Equal(src.MuscleFocusIds.Count() + 1, dest.MuscleFocusIds.Count());
                    Assert.Contains(muscleId, dest.MuscleFocusIds);
                }
            }
        }

        [Fact]
        public async Task UntagTrainingPlanWithMuscleFocusCommand_Success()
        {
            var logger = new Mock<ILogger<UntagTrainingPlanWithMuscleFocusCommandHandler>>().Object;
            UserTrainingPlanEntity src;
            IAthleteRepository athletes;

            // Arrange
            uint userId = 1;
            uint planId = 1;
            uint muscleId = 1;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    src = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

                    if (!src.MuscleFocusIds.Contains(muscleId))
                        throw new Exception("Invalid Test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    UntagTrainingPlanWithMuscleFocusCommand command = new UntagTrainingPlanWithMuscleFocusCommand(userId, planId, muscleId);
                    UntagTrainingPlanWithMuscleFocusCommandHandler handler = new UntagTrainingPlanWithMuscleFocusCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    var dest = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

                    Assert.Equal(src.MuscleFocusIds.Count() - 1, dest.MuscleFocusIds.Count());
                    Assert.DoesNotContain(muscleId, dest.MuscleFocusIds);
                }
            }
        }

        [Fact]
        public async Task WriteTrainingPlanNoteCommand_Success()
        {
            var logger = new Mock<ILogger<WriteTrainingPlanNoteCommandHandler>>().Object;
            List<TrainingPlanNoteRoot> notesBefore;
            IAthleteRepository athletes;
            ITrainingPlanNoteRepository notes;

            // Arrange
            uint userId = 1;
            uint planId = 1;
            string note = "my short note";

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    notes = new SQLTrainingPlanNoteRepository(context);
                    notesBefore = context.TrainingPlanNotes.ToList();
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    notes = new SQLTrainingPlanNoteRepository(context);

                    WriteTrainingPlanNoteCommand command = new WriteTrainingPlanNoteCommand(userId, planId, note);
                    WriteTrainingPlanNoteCommandHandler handler = new WriteTrainingPlanNoteCommandHandler(athletes, notes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    // Check note added
                    var notesAfter = context.TrainingPlanNotes.ToList();
                    var added = notesAfter.Last();
                    Assert.Equal(notesBefore.Count + 1, notesAfter.Count);
                    Assert.Equal(note, added.Body.Body);

                    // Check note attached to plan
                    var planAfter = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

                    Assert.Equal(added.Id, planAfter.TrainingPlanNoteId.Value);
                }
            }
        }

        [Fact]
        public async Task WriteWorkUnitTemplateNoteCommand_Success()
        {
            var logger = new Mock<ILogger<WriteWorkUnitTemplateNoteCommandHandler>>().Object;
            int srcNotesCount;
            uint idAdded;
            IWorkoutTemplateRepository workouts;
            IWorkUnitTemplateNoteRepository notes;

            // Arrange
            uint workoutId = 1;
            uint workUnitPnum = 1;
            string note = "my short work unit note";

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workouts = new SQLWorkoutTemplateRepository(context);
                    notes = new SQLWorkUnitTemplateNoteRepository(context);
                    srcNotesCount = context.WorkUnitTemplateNotes.Count();
                    idAdded = context.WorkUnitTemplateNotes.Max(x => x.Id).Value + 1;
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workouts = new SQLWorkoutTemplateRepository(context);
                    notes = new SQLWorkUnitTemplateNoteRepository(context);

                    WriteWorkUnitTemplateNoteCommand command = new WriteWorkUnitTemplateNoteCommand(workoutId, workUnitPnum, note);
                    WriteWorkUnitTemplateNoteCommandHandler handler = new WriteWorkUnitTemplateNoteCommandHandler(workouts, notes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    workouts = new SQLWorkoutTemplateRepository(context);
                    notes = new SQLWorkUnitTemplateNoteRepository(context);

                    // Check note added
                    var destNotesCount = context.WorkUnitTemplateNotes.Count();
                    var added = notes.Find(idAdded);
                    Assert.Equal(srcNotesCount + 1, destNotesCount);
                    Assert.Equal(note, added.Body.Body);

                    // Check note attached to plan
                    var workUnitAfter = workouts.Find(workoutId).CloneWorkUnit(workUnitPnum);
                    Assert.Equal(idAdded, workUnitAfter.WorkUnitNoteId);
                }
            }
        }

        [Fact]
        public async Task AchieveTrainingProficiencyCommand_TwoProficienciesStartingTheSameDay_Success()
        {
            var logger = new Mock<ILogger<AchieveTrainingProficiencyCommandHandler>>().Object;
            int srcProficienciesCount = 0;
            IAthleteRepository athletes;

            // Arrange
            uint athleteId = 1;         // It has another proficiency started today -> conflict
            uint proficiencyId = 1;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    srcProficienciesCount = athletes.Find(athleteId).TrainingProficiencies.Count;
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    AchieveTrainingProficiencyCommand command = new AchieveTrainingProficiencyCommand(athleteId, proficiencyId);
                    AchieveTrainingProficiencyCommandHandler handler = new AchieveTrainingProficiencyCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    var dest = athletes.Find(athleteId);
                    var added = dest.TrainingProficiencies.Last();

                    Assert.Equal(proficiencyId, added.ProficiencyId);
                    Assert.Equal(DateTime.UtcNow.Date, added.StartDate, new TimeSpan(0, 0, 0, 0, 500));
                    Assert.Null(added.EndDate);
                    Assert.Equal(srcProficienciesCount, dest.TrainingProficiencies.Count);
                }
            }
        }

        [Fact]
        public async Task AchieveTrainingProficiencyCommand_FirstProficiency_Success()
        {
            var logger = new Mock<ILogger<AchieveTrainingProficiencyCommandHandler>>().Object;

            // Arrange
            uint athleteId = 2;
            uint proficiencyId = 1;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);

                    AchieveTrainingProficiencyCommand command = new AchieveTrainingProficiencyCommand(athleteId, proficiencyId);
                    AchieveTrainingProficiencyCommandHandler handler = new AchieveTrainingProficiencyCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);
                    var added = athletes.Find(athleteId).TrainingProficiencies.Last();

                    Assert.Equal(proficiencyId, added.ProficiencyId);
                    Assert.Equal(DateTime.UtcNow.Date, added.StartDate, new TimeSpan(0, 0, 0, 0, 500));
                    Assert.Null(added.EndDate);
                }
            }
        }

        [Fact]
        public async Task BookmarkTrainingPlanCommand_Success()
        {
            var logger = new Mock<ILogger<BookmarkTrainingPlanCommandHandler>>().Object;
            IAthleteRepository athletes;

            // Arrange
            uint athleteId = 1;
            uint planId = 1;
            bool isBookmarked;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    isBookmarked = !athletes.Find(athleteId)
                        .CloneTrainingPlanOrDefault(planId).IsBookmarked;
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    BookmarkTrainingPlanCommand command = new BookmarkTrainingPlanCommand(athleteId, planId, isBookmarked);
                    BookmarkTrainingPlanCommandHandler handler = new BookmarkTrainingPlanCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    var modified = athletes.Find(athleteId).CloneTrainingPlanOrDefault(planId);
                    Assert.Equal(isBookmarked, modified.IsBookmarked);
                }
            }
        }

        [Fact]
        public async Task ExcludeTrainingPlanFromUserLibraryCommand_NotLastOne_Success()
        {
            var logger = new Mock<ILogger<ExcludeTrainingPlanFromUserLibraryCommandHandler>>().Object;
            ITrainingPlanRepository plans;
            IAthleteRepository athletes;

            // Arrange
            uint athleteId = 1;
            uint planId = 2;        // This plan is owned by more than one users

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    if (athletes.Find(athleteId).CloneTrainingPlanOrDefault(planId) == default
                        && athletes.CountAthletesWithTrainingPlanInLibrary(planId) < 2)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    plans = new SQLTrainingPlanRepository(context);

                    ExcludeTrainingPlanFromUserLibraryCommand command = new ExcludeTrainingPlanFromUserLibraryCommand(planId, athleteId);
                    ExcludeTrainingPlanFromUserLibraryCommandHandler handler = new ExcludeTrainingPlanFromUserLibraryCommandHandler(athletes, plans, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    var modified = athletes.Find(athleteId);

                    Assert.DoesNotContain(planId, modified.TrainingPlans.Select(x => x.TrainingPlanId));
                    Assert.Contains(planId, context.TrainingPlans.Select(x => x.Id));
                }
            }
        }

        [Fact]
        public async Task ExcludeTrainingPlanFromUserLibraryCommand_LastOne_Success()
        {
            var logger = new Mock<ILogger<ExcludeTrainingPlanFromUserLibraryCommandHandler>>().Object;
            IAthleteRepository athletes;
            ITrainingPlanRepository plans;

            // Arrange
            uint athleteId = 1;
            uint planId = 1;        // This plan is owned by this user only

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    plans = new SQLTrainingPlanRepository(context);

                    if (athletes.Find(athleteId).CloneTrainingPlanOrDefault(planId) == default
                        && athletes.CountAthletesWithTrainingPlanInLibrary(planId) == 1)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    plans = new SQLTrainingPlanRepository(context);

                    ExcludeTrainingPlanFromUserLibraryCommand command = new ExcludeTrainingPlanFromUserLibraryCommand(planId, athleteId);
                    ExcludeTrainingPlanFromUserLibraryCommandHandler handler = new ExcludeTrainingPlanFromUserLibraryCommandHandler(athletes, plans, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    var modified = athletes.Find(athleteId);

                    Assert.DoesNotContain(planId, modified.TrainingPlans.Select(x => x.TrainingPlanId));
                    Assert.DoesNotContain(planId, context.TrainingPlans.Select(x => x.Id));
                }
            }
        }

        [Fact]
        public async Task ExcludeTrainingPlanFromUserLibraryCommand_PlanNotInUserLibrary()
        {
            var logger = new Mock<ILogger<ExcludeTrainingPlanFromUserLibraryCommandHandler>>().Object;

            // Arrange
            uint athleteId = 1;
            uint planId = 6;        // This plan is not owned by this user

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);
                    var plans = new SQLTrainingPlanRepository(context);

                    if (athletes.Find(athleteId).CloneTrainingPlanOrDefault(planId) != default)
                        throw new Exception("Invalid test");

                    ExcludeTrainingPlanFromUserLibraryCommand command = new ExcludeTrainingPlanFromUserLibraryCommand(planId, athleteId);
                    ExcludeTrainingPlanFromUserLibraryCommandHandler handler = new ExcludeTrainingPlanFromUserLibraryCommandHandler(athletes, plans, logger);

                    Assert.False(await handler.Handle(command, default));
                }
            }
        }

        [Fact]
        public async Task IncludeTrainingPlanInUserLibraryCommandHandler_Success()
        {
            var logger = new Mock<ILogger<IncludeTrainingPlanInUserLibraryCommandHandler>>().Object;
            IAthleteRepository athletes;

            // Arrange
            uint athleteId = 1;
            uint planId = 6;        // This plan is not owned by this user

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    if (athletes.Find(athleteId).CloneTrainingPlanOrDefault(planId) != default)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    IncludeTrainingPlanInUserLibraryCommand command = new IncludeTrainingPlanInUserLibraryCommand(planId, athleteId);
                    IncludeTrainingPlanInUserLibraryCommandHandler handler = new IncludeTrainingPlanInUserLibraryCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    var modified = athletes.Find(athleteId);
                    Assert.Contains(planId, modified.TrainingPlans.Select(x => x.TrainingPlanId));
                }
            }
        }

        [Fact]
        public async Task IncludeTrainingPlanInUserLibraryCommandHandler_PlanAlreadyInUserLibrary()
        {
            var logger = new Mock<ILogger<IncludeTrainingPlanInUserLibraryCommandHandler>>().Object;
            AthleteRoot src;
            IAthleteRepository athletes;

            // Arrange
            uint planId = 1;
            uint userId = 1;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using(GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    src = athletes.Find(userId).Clone() as AthleteRoot;

                    if (src.CloneTrainingPlanOrDefault(planId) == default)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    IncludeTrainingPlanInUserLibraryCommand command = new IncludeTrainingPlanInUserLibraryCommand(planId, userId);
                    IncludeTrainingPlanInUserLibraryCommandHandler handler = new IncludeTrainingPlanInUserLibraryCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    // Check nothing happened
                    athletes = new SQLAthleteRepository(context);
                    var modified = athletes.Find(userId);
                    Assert.True(src.TrainingPlans.SequenceEqual(modified.TrainingPlans));
                }
            }
        }

        [Fact]
        public async Task MakeTrainingPlanNotVariantOfAnyCommand_Success()
        {
            var logger = new Mock<ILogger<MakeTrainingPlanNotVariantOfAnyCommandHandler>>().Object;
            UserTrainingPlanEntity src;
            IAthleteRepository athletes;

            // Arrange
            uint planId = 2;
            uint userId = 1;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    src = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

                    if (src.ParentPlanId == null)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    MakeTrainingPlanNotVariantOfAnyCommand command = new MakeTrainingPlanNotVariantOfAnyCommand(planId, userId);
                    MakeTrainingPlanNotVariantOfAnyCommandHandler handler = new MakeTrainingPlanNotVariantOfAnyCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check 
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    var modified = athletes.Find(userId);
                    Assert.Null(modified.CloneTrainingPlanOrDefault(planId).ParentPlanId);
                }
            }
        }

        [Fact]
        public async Task MakeTrainingPlanVariantOfCommand_Success()
        {
            var logger = new Mock<ILogger<MakeTrainingPlanVariantOfCommandHandler>>().Object;
            UserTrainingPlanEntity src;
            IAthleteRepository athletes;

            // Arrange
            uint planId = 2;
            uint userId = 1;
            uint parentId = 3;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    src = athletes.Find(userId).CloneTrainingPlanOrDefault(planId);

                    if (src.ParentPlanId == parentId)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    MakeTrainingPlanVariantOfCommand command = new MakeTrainingPlanVariantOfCommand(planId, userId, parentId);
                    MakeTrainingPlanVariantOfCommandHandler handler = new MakeTrainingPlanVariantOfCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check 
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    var modified = athletes.Find(userId);

                    Assert.Equal(parentId, modified.CloneTrainingPlanOrDefault(planId).ParentPlanId);
                    Assert.NotEqual(src.ParentPlanId, modified.CloneTrainingPlanOrDefault(planId).ParentPlanId);
                }
            }
        }

        [Fact]
        public async Task PlanTrainingPhaseCommand_NoEndDateSuccess()
        {
            var logger = new Mock<ILogger<PlanTrainingPhaseCommandHandler>>().Object;
            IAthleteRepository athletes;

            // Arrange
            uint userId = 2;
            uint phaseId = 1;
            uint entryStatusId = 1;
            string note = "The note";
            DateTime startDate = DateTime.UtcNow.AddDays(1);
            DateTime? endDate = null;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    if (athletes.Find(userId).TrainingPhases.Count != 1)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);

                    PlanTrainingPhaseCommand command = new PlanTrainingPhaseCommand(userId, phaseId, entryStatusId, note, startDate, endDate);
                    PlanTrainingPhaseCommandHandler handler = new PlanTrainingPhaseCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    athletes = new SQLAthleteRepository(context);
                    var phases = athletes.Find(userId).TrainingPhases;

                    Assert.Equal(phaseId, phases.First().PhaseId);
                    Assert.Equal(startDate.Date, phases.First().StartDate);
                    Assert.Equal(phases.Last().StartDate.AddDays(-1), phases.First().EndDate); // Works only if two phases
                    Assert.Equal(note, phases.First().OwnerNote.Body);
                }
            }
        }

        [Fact]
        public async Task PlanTrainingPhaseCommand_EndDateSuccess()
        {
            var logger = new Mock<ILogger<PlanTrainingPhaseCommandHandler>>().Object;

            // Arrange
            uint userId = 2;
            uint phaseId = 1;
            uint entryStatusId = 1;
            string note = null;
            DateTime startDate = DateTime.UtcNow.AddDays(1);
            DateTime? endDate = startDate.AddDays(10);

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);

                    if (athletes.Find(userId).TrainingPhases.Count != 1)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);

                    PlanTrainingPhaseCommand command = new PlanTrainingPhaseCommand(userId, phaseId, entryStatusId, note, startDate, endDate);
                    PlanTrainingPhaseCommandHandler handler = new PlanTrainingPhaseCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);
                    var newPhase = athletes.Find(userId).TrainingPhases.First();    // Why is this sorted?

                    Assert.Equal(phaseId, newPhase.PhaseId);
                    Assert.Equal(startDate.Date, newPhase.StartDate);
                    Assert.Equal(endDate.Value.Date, newPhase.EndDate.Value);
                    Assert.Null(newPhase.OwnerNote);
                }
            }
        }

        [Fact]
        public async Task RenameTrainingPlanCommand_Success()
        {
            var logger = new Mock<ILogger<RenameTrainingPlanCommandHandler>>().Object;

            // Arrange
            uint userId = 1;
            uint planId = 1;
            string name = "My new renamed plan...";

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);

                    RenameTrainingPlanCommand command = new RenameTrainingPlanCommand(userId, planId, name);
                    RenameTrainingPlanCommandHandler handler = new RenameTrainingPlanCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);
                    Assert.Equal(name, athletes.Find(userId).CloneTrainingPlanOrDefault(planId).Name);
                }
            }
        }

        [Fact]
        public async Task RenameTrainingPlanCommand_PlanNotFound_Fail()
        {
            var logger = new Mock<ILogger<RenameTrainingPlanCommandHandler>>().Object;

            // Arrange
            uint userId = 1;
            uint planId = 6;
            string name = "My new renamed plan...";

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);

                    if (athletes.Find(userId).CloneTrainingPlanOrDefault(planId) != default)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);

                    RenameTrainingPlanCommand command = new RenameTrainingPlanCommand(userId, planId, name);
                    RenameTrainingPlanCommandHandler handler = new RenameTrainingPlanCommandHandler(athletes, logger);

                    Assert.False(await handler.Handle(command, default));
                }
            }
        }

        [Fact]
        public async Task ScheduleTrainingPlanCommand_StartDateOnly_Success()
        {
            var logger = new Mock<ILogger<ScheduleTrainingPlanCommandHandler>>().Object;

            // Arrange
            uint planId = 1;
            uint userId = 2;
            DateTime start = new DateTime(2019, 1, 1);
            DateTime? end = null;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var schedules = new SQLTrainingScheduleRepository(context);

                    ScheduleTrainingPlanCommand command = new ScheduleTrainingPlanCommand(userId, planId, start, end);
                    ScheduleTrainingPlanCommandHandler handler = new ScheduleTrainingPlanCommandHandler(schedules, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var added = context.TrainingSchedules.ToList().Last();

                    Assert.Equal(planId, added.TrainingPlanId);
                    Assert.Equal(userId, added.AthleteId);
                    Assert.Equal(start, added.StartDate);
                    Assert.Equal(end, added.EndDate);
                    Assert.Empty(added.Feedbacks);
                }
            }
        }

        [Fact]
        public async Task ScheduleTrainingPlanCommand_PlannedPeriod_Success()
        {
            var logger = new Mock<ILogger<ScheduleTrainingPlanCommandHandler>>().Object;

            // Arrange
            uint planId = 1;
            uint userId = 2;
            DateTime start = new DateTime(2019, 1, 1);
            DateTime? end = null;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var schedules = new SQLTrainingScheduleRepository(context);
                    ScheduleTrainingPlanCommand command = new ScheduleTrainingPlanCommand(userId, planId, start, end);
                    ScheduleTrainingPlanCommandHandler handler = new ScheduleTrainingPlanCommandHandler(schedules, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var added = context.TrainingSchedules.ToList().Last();

                    Assert.Equal(planId, added.TrainingPlanId);
                    Assert.Equal(userId, added.AthleteId);
                    Assert.Equal(start, added.StartDate);
                    Assert.Equal(end, added.EndDate);
                    Assert.Empty(added.Feedbacks);
                }
            }
        }

        [Fact]
        public async Task ScheduleTrainingPlanCommand_CurrentPlanNotcompleted_Success()
        {
            var logger = new Mock<ILogger<ScheduleTrainingPlanCommandHandler>>().Object;
            TrainingScheduleRoot currentPlan;

            // Arrange
            uint planId = 1;
            uint userId = 1;    // User has an ongoing plan
            DateTime start = DateTime.UtcNow.AddDays(1).Date;
            DateTime? end = null;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var schedules = new SQLTrainingScheduleRepository(context);
                    currentPlan = schedules.GetCurrentScheduleByAthleteOrDefault(userId);

                    if (currentPlan == null)
                        throw new ArgumentException("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var schedules = new SQLTrainingScheduleRepository(context);
                    ScheduleTrainingPlanCommand command = new ScheduleTrainingPlanCommand(userId, planId, start, end);
                    ScheduleTrainingPlanCommandHandler handler = new ScheduleTrainingPlanCommandHandler(schedules, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check schedule added
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var schedules = new SQLTrainingScheduleRepository(context);
                    var added = context.TrainingSchedules.ToList().Last();

                    Assert.Equal(planId, added.TrainingPlanId);
                    Assert.Equal(start, added.StartDate);
                    Assert.Equal(end, added.EndDate);
                    Assert.Empty(added.Feedbacks);
                }
                // Check ongoing plan is now completed
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var schedules = new SQLTrainingScheduleRepository(context);
                    var prevCurrentPlan = schedules.Find(currentPlan.Id.Value);

                    Assert.Equal(DateTime.UtcNow.AddDays(-1).Date, prevCurrentPlan.EndDate);
                }
            }
        }

        [Fact]
        public async Task StartTrainingPhaseCommand_FirstPhase_Success()
        {
            var logger = new Mock<ILogger<StartTrainingPhaseCommandHandler>>().Object;

            // Arrange
            uint userId = 3;        // User with no phases started yet
            uint phaseId = 1;
            uint entryStatusId = 1;
            string note = "The training phase new note";

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);

                    if (athletes.Find(userId).TrainingPhases.Count > 0)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);
                    StartTrainingPhaseCommand command = new StartTrainingPhaseCommand(userId, phaseId, entryStatusId, note);
                    StartTrainingPhaseCommandHandler handler = new StartTrainingPhaseCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);
                    var modified = athletes.Find(userId);
                    var phase = modified.TrainingPhases.Last();

                    Assert.Equal(phaseId, phase.PhaseId);
                    Assert.Equal(DateTime.UtcNow.Date, phase.StartDate);
                    Assert.Null(modified.TrainingPhases.Last().EndDate);
                    Assert.Equal(phase, modified.CurrentTrainingPhase);
                }
            }
        }
        [Fact]
        public async Task StartTrainingPhaseCommand_FirstPhaseWithNoComment_Success()
        {
            var logger = new Mock<ILogger<StartTrainingPhaseCommandHandler>>().Object;

            // Arrange
            uint userId = 3;        // User with no phases started yet
            uint phaseId = 1;
            uint entryStatusId = 1;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);

                    if (athletes.Find(userId).TrainingPhases.Count > 0)
                        throw new Exception("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);
                    StartTrainingPhaseCommand command = new StartTrainingPhaseCommand(userId, phaseId, entryStatusId, null);
                    StartTrainingPhaseCommandHandler handler = new StartTrainingPhaseCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);
                    var modified = athletes.Find(userId);
                    var phase = modified.TrainingPhases.Last();

                    Assert.Equal(phaseId, phase.PhaseId);
                    Assert.Equal((int)entryStatusId, phase.EntryStatus.Id);
                    Assert.Equal(DateTime.UtcNow.Date, phase.StartDate);
                    Assert.Null(modified.TrainingPhases.Last().EndDate);
                    Assert.Equal(phase, modified.CurrentTrainingPhase);
                }
            }
        }

        [Fact]
        public async Task StartTrainingPhaseCommand_FuturePhaseAlreadyPlanned_Success()
        {
            var logger = new Mock<ILogger<StartTrainingPhaseCommandHandler>>().Object;
            DateTime nextPhaseStart;

            // Arrange
            uint userId = 2;        // Only one phase planned in the future
            uint phaseId = 1;
            uint entryStatusId = 2;
            string note = "The training phase new note";

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);
                    var src = athletes.Find(userId).TrainingPhases;

                    if (src.Count != 1 || src.First().StartDate.Date <= DateTime.UtcNow.Date)
                        throw new Exception("Invalid test");

                    nextPhaseStart = src.FirstOrDefault().StartDate;
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);
                    StartTrainingPhaseCommand command = new StartTrainingPhaseCommand(userId, phaseId, entryStatusId, note);
                    StartTrainingPhaseCommandHandler handler = new StartTrainingPhaseCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);
                    var modified = athletes.Find(userId);

                    //RIGM: why are the TrainingPhases sorted by StartDate? This is not an issue, though it makes no sense
                    var newPhase = modified.TrainingPhases.First();
                    Assert.Equal(phaseId, newPhase.PhaseId);
                    Assert.Equal((int)entryStatusId, newPhase.EntryStatus.Id);
                    Assert.Equal(DateTime.UtcNow.Date, newPhase.StartDate);
                    Assert.Equal(nextPhaseStart.AddDays(-1), newPhase.EndDate);
                }
            }
        }

        [Fact]
        public async Task StartTrainingPhaseCommand_TwoPhasesStartingTheSameDay_Fail()
        {
            var logger = new Mock<ILogger<StartTrainingPhaseCommandHandler>>().Object;
            uint srcPhaseId;

            // Arrange
            uint userId = 1;        // This user has a phase starting today
            uint phaseId = 2;
            uint entryStatusId = 1;
            string note = "The training phase new note";

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);
                    var src = athletes.Find(userId).TrainingPhases;

                    if (src.All(x => x.StartDate != DateTime.UtcNow.Date))
                        throw new Exception("Invalid test");

                    srcPhaseId = src.SingleOrDefault(x => x.StartDate == DateTime.UtcNow.Date).PhaseId.Value;
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);
                    StartTrainingPhaseCommand command = new StartTrainingPhaseCommand(userId, phaseId, entryStatusId, note);
                    StartTrainingPhaseCommandHandler handler = new StartTrainingPhaseCommandHandler(athletes, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var athletes = new SQLAthleteRepository(context);
                    var modified = athletes.Find(userId);

                    //RIGM: why are the TrainingPhases sorted by StartDate? This is not an issue, though it makes no sense
                    var newPhase = modified.TrainingPhases.First();
                    Assert.Equal(phaseId, newPhase.PhaseId);
                    Assert.Equal(DateTime.UtcNow.Date, newPhase.StartDate);
                    Assert.Equal(modified.TrainingPhases.Last().StartDate.AddDays(-1), newPhase.EndDate);   // Works only with two phases
                    Assert.Equal(note, newPhase.OwnerNote.Body);

                    Assert.DoesNotContain(srcPhaseId, modified.TrainingPhases.Select(x => x.PhaseId.Value));
                }
            }
        }

        [Fact]
        public async Task CompleteTrainingScheduleCommand_Success()
        {
            var logger = new Mock<ILogger<CompleteTrainingScheduleCommandHandler>>().Object;

            // Arrange
            uint scheduleId = 1;
            DateTime endDate = DateTime.UtcNow.Date;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var schedules = new SQLTrainingScheduleRepository(context);

                    if (schedules.Find(scheduleId).EndDate == endDate)
                        throw new NotImplementedException("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var schedules = new SQLTrainingScheduleRepository(context);
                    CompleteTrainingScheduleCommand command = new CompleteTrainingScheduleCommand(scheduleId, endDate);
                    CompleteTrainingScheduleCommandHandler handler = new CompleteTrainingScheduleCommandHandler(schedules, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check
                using(GymContext context = await factory.CreateContextAsync())
                {
                    var schedules = new SQLTrainingScheduleRepository(context);
                    var modified = schedules.Find(scheduleId);
                    Assert.Equal(endDate, modified.EndDate);
                }
            }
        }

        [Fact]
        public async Task RescheduleTrainingScheduleCommand_Success()
        {
            var logger = new Mock<ILogger<RescheduleTrainingPlanCommandHandler>>().Object;

            // Arrange
            uint scheduleId = 2;
            DateTime startDate = DateTime.UtcNow.Date;

            using (InMemoryDbContextFactory factory = new InMemoryDbContextFactory())
            {
                // State before
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var schedules = new SQLTrainingScheduleRepository(context);
                    var sched = schedules.Find(scheduleId);

                    if (sched.StartDate == startDate || sched.EndDate < startDate || sched.IsCompleted())
                        throw new NotImplementedException("Invalid test");
                }
                // Test
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var schedules = new SQLTrainingScheduleRepository(context);

                    RescheduleTrainingPlanCommand command = new RescheduleTrainingPlanCommand(scheduleId, startDate);
                    RescheduleTrainingPlanCommandHandler handler = new RescheduleTrainingPlanCommandHandler(schedules, logger);

                    Assert.True(await handler.Handle(command, default));
                }
                // Check 
                using (GymContext context = await factory.CreateContextAsync())
                {
                    var schedules = new SQLTrainingScheduleRepository(context);
                    var modified = schedules.Find(scheduleId);

                    Assert.Equal(startDate, modified.StartDate);
                }
            }
        }


        [Fact]
        public async Task AAA_BenchmarkRepository()
        {
            //int ntests = 1000;

            //// Arrange
            //uint id = 1;

            //using (SQLDbContextFactory factory = new SQLDbContextFactory())
            //{

            //    Stopwatch dapperTime = new Stopwatch();
            //    dapperTime.Start();

            //    for (int i = 0; i < ntests; i++)
            //    {
            //        // Dapper
            //        using (GymContext context = await factory.CreateContextAsync())
            //        {
            //            SQLWorkoutTemplateRepository repo = new SQLWorkoutTemplateRepository(context);
            //            repo.FindWithDapper(id);
            //        }
            //    }
            //    dapperTime.Stop();

            //    Stopwatch efTime = new Stopwatch();
            //    efTime.Start();

            //    for (int i = 0; i < ntests; i++)
            //    {
            //        // EF CORE
            //        using (GymContext context = await factory.CreateContextAsync())
            //        {
            //            SQLWorkoutTemplateRepository repo = new SQLWorkoutTemplateRepository(context);
            //            repo.Find(id);
            //        }
            //    }
            //    efTime.Stop();

            //    Assert.Equal(efTime.ElapsedMilliseconds, dapperTime.ElapsedMilliseconds);
            //}
            //throw new Exception();
        }
    }
}
