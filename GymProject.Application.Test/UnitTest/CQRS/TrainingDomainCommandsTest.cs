using GymProject.Application.Command;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.MediatorBehavior;
using GymProject.Application.Test.Utils;
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
using System.Linq;
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
            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);
            IWorkoutTemplateRepository workoutRepository = new SQLWorkoutTemplateRepository(context);

            // Test
            uint planId = 1;
            TrainingPlanRoot removed = planRepository.Find(planId);

            DeleteTrainingPlanCommand command = new DeleteTrainingPlanCommand(planId);
            DeleteTrainingPlanCommandHandler handler = new DeleteTrainingPlanCommandHandler(
                workoutRepository, planRepository, logger.Object);

            Assert.True(await handler.Handle(command, default));

            // Check new training plan
            Assert.Equal(seed.TrainingPlans.Count() - 1, context.TrainingPlans.Count());
            Assert.Equal(seed.WorkoutTemplates.Count() - removed.WorkoutIds.Count(), context.WorkoutTemplates.Count());
            Assert.DoesNotContain(removed, context.TrainingPlans);
            
            foreach(uint? workoutId in removed.WorkoutIds)
                Assert.DoesNotContain(workoutId, context.WorkoutTemplates.ToList().Select(x => x.Id));
        }


    }
}
