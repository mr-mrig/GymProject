using GymProject.Application.DomainEventHandler;
using GymProject.Application.Test.Utils;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using GymProject.Infrastructure.Persistence.SqlRepository.TrainingDomain;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace GymProject.Application.Test.UnitTest.CQRS.TrainingDomain
{
    public class TrainingDomainEventsHandlersTest
    {




        [Fact]
        public async Task TrainingPlanRemovedFromLibrary_NotLastOne()
        {
            GymContext context;
            ILogger<TrainingPlanRemovedFromLibraryDomainEventHandler> logger;

            (context, _, logger) = await ApplicationTestService.InitInMemoryCommandTest<TrainingPlanRemovedFromLibraryDomainEventHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            IAthleteRepository athleteRepo = new SQLAthleteRepository(context);
            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);
            IWorkoutTemplateRepository workoutRepo = new SQLWorkoutTemplateRepository(context);

            // Test
            uint athleteId = 1;
            uint planId = 2;        // This plan is shared by more than une user

            // Not testing event deployment
            //ExcludeTrainingPlanFromUserLibraryCommand command = new ExcludeTrainingPlanFromUserLibraryCommand(planId, athleteId);
            //ExcludeTrainingPlanFromUserLibraryCommandHandler handler = 

            var plansBefore = context.TrainingPlans.ToList();

            TrainingPlanRemovedFromLibraryDomainEvent @event = new TrainingPlanRemovedFromLibraryDomainEvent(planId);
            TrainingPlanRemovedFromLibraryDomainEventHandler handler = new TrainingPlanRemovedFromLibraryDomainEventHandler(planRepository, athleteRepo, workoutRepo, logger);

            await handler.Handle(@event, default);

            // Check nothing happened
            var plansAfter = context.TrainingPlans.ToList();
            Assert.True(plansBefore.SequenceEqual(plansAfter));
        }

        [Fact]
        public async Task DraftTrainingPlanCommandSuccess_LastOne()
        {
            //throw new NotImplementedException("Cannot test this without routing the event as the athletesWithPlan is not coherent");

            GymContext context;
            ILogger<TrainingPlanRemovedFromLibraryDomainEventHandler> logger;

            (context, _, logger) = await ApplicationTestService.InitInMemoryCommandTest<TrainingPlanRemovedFromLibraryDomainEventHandler>(MethodBase.GetCurrentMethod().DeclaringType.Name);

            IAthleteRepository athleteRepo = new SQLAthleteRepository(context);
            ITrainingPlanRepository planRepository = new SQLTrainingPlanRepository(context);
            IWorkoutTemplateRepository workoutRepo = new SQLWorkoutTemplateRepository(context);

            // Test
            uint planId = 1;    // This plan belongs to user1 library only
            var toRemove = context.TrainingPlans.Find(planId);

            TrainingPlanRemovedFromLibraryDomainEvent @event = new TrainingPlanRemovedFromLibraryDomainEvent(planId);
            TrainingPlanRemovedFromLibraryDomainEventHandler handler = new TrainingPlanRemovedFromLibraryDomainEventHandler(planRepository, athleteRepo, workoutRepo, logger);

            await handler.Handle(@event, default);

            // Check the Training Plan Template and all its workouts have been removed
            var plansAfter = context.TrainingPlans.ToList();
            Assert.DoesNotContain(toRemove, plansAfter);
            Assert.DoesNotContain(context.WorkoutTemplates.Select(x => x.Id), (wo) => toRemove.WorkoutIds.Contains(wo));
        }





    }
}
