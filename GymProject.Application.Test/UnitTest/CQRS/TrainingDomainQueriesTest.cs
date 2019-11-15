using GymProject.Application.Command;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.MediatorBehavior;
using GymProject.Application.Queries.TrainingDomain;
using GymProject.Application.Test.Utils;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.Test.Util;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Infrastructure.Persistence.EFContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GymProject.Application.Test.UnitTest.CQRS
{
    public class TrainingDomainQueriesTest
    {



        [Fact]
        public async Task GetTraininPlansSummariesTest()
        {
            IEnumerable<TrainingPlanSummaryDto> results;
            uint id;

            GymContext context = await ApplicationTestService.InitQueryTest();
            TrainingQueryWrapper queries = new TrainingQueryWrapper(ApplicationUnitTestContext.SQLiteDbTestConnectionString);

            // Dummy case:  No results
            id = uint.MaxValue;
            results = await queries.GetTraininPlansSummaries(id);
            // Check
            Assert.Empty(results);

            // Standard case
            id = 1;
            results = await queries.GetTraininPlansSummaries(id);

            // Get expected results
            var planIds = context.TrainingPlans.Where(x => x.OwnerId == id).Select(x => x.Id);
            var nhashtags = context.TrainingPlanHashtags.Count(x => planIds.Contains(x.TrainingPlanId));
            var nphases = context.TrainingPlanPhases.Count(x => planIds.Contains(x.TrainingPlanId));
            var nproficiencies = context.TrainingPlanProficiencies.Count(x => planIds.Contains(x.TrainingPlanId));

            // Check
            Assert.Equal(planIds.Count(), results.Count());
            Assert.Equal(nhashtags, results.SelectMany(x => x.Hashtags).Count());
            Assert.Equal(nphases, results.SelectMany(x => x.TargetPhases).Count());
            Assert.Equal(nproficiencies, results.SelectMany(x => x.TargetProficiencies).Count());

            // Ad-hoc case
            uint specialId = 1;
            Assert.Equal(4.5f, results.Single(x => x.TrainingPlanId == specialId).AvgWorkoutDays.Value, 2);
            Assert.Equal(36, results.Single(x => x.TrainingPlanId == specialId).AvgWorkingSets.Value);

            // Not testable yet
            //Assert.Equal(todo, results.Single(x => x.TrainingPlanId == specialId).AvgIntensityPercentage);
            //Assert.Equal(todo, results.Single(x => x.TrainingPlanId == specialId).LastWorkoutTimestamp);
        }


        [Fact]
        public async Task GetTraininPlanPlannedWorkoutDays()
        {
            IEnumerable<WorkoutFullPlanDto> results;
            string workoutName;

            GymContext context = await ApplicationTestService.InitQueryTest();
            TrainingQueryWrapper queries = new TrainingQueryWrapper(ApplicationUnitTestContext.SQLiteDbTestConnectionString);

            //List<uint> weekIds = context.TrainingPlans.Find(planId).TrainingWeeks as List<uint>;
            List<uint> weekIds = new List<uint> { 14, 1, 6, 10 };

            // Dummy case1:  Fake weeks -> No results
            results = await queries.GetTraininPlanPlannedWorkoutDays(new List<uint>() { uint.MaxValue }, "");
            Assert.Equal(default, results.First().TrainingWeekId);

            // Dummy case2:  Fake name -> No results
            workoutName = "FAKE NAME";
            results = await queries.GetTraininPlanPlannedWorkoutDays(weekIds, workoutName);
            Assert.Equal(default, results.First().TrainingWeekId);

            // Standard case
            workoutName = "DAY A";
            results = await queries.GetTraininPlanPlannedWorkoutDays(weekIds, workoutName);

            Assert.NotEmpty(results);

            // Get expected results
            //var planIds = context.TrainingPlans.Where(x => x.OwnerId == weekIds).Select(x => x.Id);
            //var nhashtags = context.TrainingPlanHashtags.Count(x => planIds.Contains(x.TrainingPlanId));
            //var nphases = context.TrainingPlanPhases.Count(x => planIds.Contains(x.TrainingPlanId));
            //var nproficiencies = context.TrainingPlanProficiencies.Count(x => planIds.Contains(x.TrainingPlanId));

            //// Check
            //Assert.Equal(planIds.Count(), results.Count());
            //Assert.Equal(nhashtags, results.SelectMany(x => x.Hashtags).Count());
            //Assert.Equal(nphases, results.SelectMany(x => x.TargetPhases).Count());
            //Assert.Equal(nproficiencies, results.SelectMany(x => x.TargetProficiencies).Count());

            //// Ad-hoc case
            //uint specialId = 1;
            //Assert.Equal(4.5f, results.Single(x => x.TrainingPlanId == specialId).AvgWorkoutDays.Value, 2);
            //Assert.Equal(36, results.Single(x => x.TrainingPlanId == specialId).AvgWorkingSets.Value);

        }



    }
}
