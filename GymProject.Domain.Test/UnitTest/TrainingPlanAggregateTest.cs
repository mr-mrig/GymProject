using GymProject.Domain.Test.Util;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GymProject.Domain.Test.UnitTest
{
    public class TrainingPlanAggregateTest
    {



        public const int ntests = 50;

        private readonly ITestOutputHelper output;



        public TrainingPlanAggregateTest(ITestOutputHelper output)
        {
            this.output = output;
        }





        [Fact]
        public static void TrainingWeekFail()
        {
            int ntests = 200;        // Perform less tests for fail conditions

            int initialWorkoutMax = 5;
            int initialWorkoutsNum;

            TrainingWeekEntity week = null;
            uint? weekId = 1;

            // Create Training Week
            for (int itest = 0; itest < ntests; itest++)
            {

                bool faked = false;
                bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.1f);

                List<uint?> initialWorkoutIds = new List<uint?>();

                TrainingWeekTypeEnum weekType = TrainingWeekTypeEnum.From(
                    RandomFieldGenerator.RandomInt(1, TrainingWeekTypeEnum.Peak.Id));


                // Full rest Week with workouts
                if (weekType == TrainingWeekTypeEnum.FullRest)
                {
                    initialWorkoutsNum = RandomFieldGenerator.RandomInt(1, initialWorkoutMax);      // At least one

                    for (int iwo = 0; iwo < initialWorkoutsNum; iwo++)
                        initialWorkoutIds.Add((uint?)RandomFieldGenerator.RandomIntValueExcluded(1,7777, initialWorkoutIds.Select(x => (int)x.Value)));

                    if (isTransient)
                    {
                        Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                            TrainingWeekEntity.PlanTransientTrainingWeek(0, initialWorkoutIds, weekType));

                        week = TrainingWeekEntity.PlanTransientFullRestWeek(0);
                    }
                    else
                    {
                        Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                            TrainingWeekEntity.PlanTrainingWeek(weekId, 0, initialWorkoutIds, weekType));

                        week = TrainingWeekEntity.PlanFullRestWeek(weekId, 0);
                    }

                    // BUSINESS RULE REMOVED
                    //// Check fail when trying to switch out from Full Rest
                    //newWeekType = TrainingWeekTypeEnum.From(RandomFieldGenerator.RandomIntValueExcluded(
                    //    1, TrainingWeekTypeEnum.Peak.Id, TrainingWeekTypeEnum.FullRest.Id));

                    //Assert.Throws<TrainingDomainInvariantViolationException>(()=>
                    //    week.AssignSpecificWeekType(newWeekType));
                }
            }

            // Check fail when trying to switch to Full Rest
            week = TrainingWeekEntity.PlanTrainingWeek(weekId, 0, new List<uint?>() { 1, });

            Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                week.AssignSpecificWeekType(TrainingWeekTypeEnum.FullRest));

            // Invalid Progressive Number
            week = WorkoutTemplateAggregateBuilder.BuildRandomTrainingWeek(19, 0, true, nWorkoutsMin: 3, nWorkoutsMax: 6, weekType: TrainingWeekTypeEnum.Generic, noWorkoutsProb: 0);
            uint invalidPnum = (uint)week.WorkoutIds.Count() + 1;

            List<WorkingSetTemplateEntity> newWorkingSets = new List<WorkingSetTemplateEntity>()
            {
                WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(1, 0, false, TrainingEffortTypeEnum.RM),
                WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(1, 1, false, TrainingEffortTypeEnum.RM),
            };

            Assert.Throws<InvalidOperationException>(() => week.UnplanWorkout(invalidPnum));
        }


        [Fact]
        public static void TrainingWeekFullTest()
        {
            int initialWorkoutMin = 1, initialWorkoutMax = 10;
            int addWorkoutsMin = 2, addWorkoutsMax = 3;
            int removeWorkoutsMin = 1, removeWorkoutsMax = addWorkoutsMin + initialWorkoutMin - 1;

            for (int itest = 0; itest < ntests; itest++)
            {
                uint weekPnum = (uint)RandomFieldGenerator.RandomInt(0, 10);
                bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.1f);

                int initialWorkoutsNum = RandomFieldGenerator.RandomInt(initialWorkoutMin, initialWorkoutMax);
                List<uint?> initialWorkoutIds = new List<uint?>();
                List<uint?> workoutIds = new List<uint?>();

                TrainingWeekEntity week;
                uint? weekId = 1;

                TrainingWeekTypeEnum weekType = TrainingWeekTypeEnum.From(
                    RandomFieldGenerator.RandomIntValueExcluded(1, TrainingWeekTypeEnum.Peak.Id, TrainingWeekTypeEnum.FullRest.Id));

                // Very small chance for Full Rest weeks, as many operations on them cannot be tested
                if (RandomFieldGenerator.RollEventWithProbability(0.02f))
                    weekType = TrainingWeekTypeEnum.FullRest;

                // Create Week
                if (weekType == TrainingWeekTypeEnum.FullRest)
                {
                    if (isTransient)
                        week = TrainingWeekEntity.PlanTransientFullRestWeek(weekPnum);
                    else
                        week = TrainingWeekEntity.PlanFullRestWeek(weekId, weekPnum);

                    continue;   // Skip to next test
                }

                for (int iwo = 0; iwo < initialWorkoutsNum; iwo++)
                {
                    initialWorkoutIds.Add(
                         (uint?)RandomFieldGenerator.RandomIntValueExcluded(1, 1354325, initialWorkoutIds.Select(x => (int)x.Value)));
                }

                if (isTransient)
                    week = TrainingWeekEntity.PlanTransientTrainingWeek(weekPnum, initialWorkoutIds, weekType);
                else
                    week = TrainingWeekEntity.PlanTrainingWeek(weekId, weekPnum, initialWorkoutIds, weekType);


                Assert.True(week.WorkoutIds.SequenceEqual(initialWorkoutIds));

                // Change Week
                weekPnum = (uint)RandomFieldGenerator.RandomIntValueExcluded(0, 20, (int)weekPnum);
                week.MoveToNewProgressiveNumber(weekPnum);

                if (RandomFieldGenerator.RollEventWithProbability(0.02f))
                {
                    weekType = TrainingWeekTypeEnum.FullRest;
                    week.MarkAsFullRestWeek();

                    Assert.Equal(weekPnum, week.ProgressiveNumber);
                    Assert.Equal(weekType, week.TrainingWeekType);
                    Assert.Empty(week.WorkoutIds);

                    continue;   // Skip to next test
                }
                weekType = TrainingWeekTypeEnum.From(
                    RandomFieldGenerator.RandomIntValueExcluded(1, TrainingWeekTypeEnum.Peak.Id, TrainingWeekTypeEnum.FullRest.Id));

                week.AssignSpecificWeekType(weekType);

                Assert.Equal(weekPnum, week.ProgressiveNumber);
                Assert.Equal(weekType, week.TrainingWeekType);

                // Add Workouts
                int addWorkoutsNum = RandomFieldGenerator.RandomInt(addWorkoutsMin, addWorkoutsMax);
                workoutIds = new List<uint?>(initialWorkoutIds);

                for (int iwo = 0; iwo < addWorkoutsNum; iwo++)
                {
                    workoutIds.Add((uint?)
                        RandomFieldGenerator.RandomIntValueExcluded(1,1242135, workoutIds.Select(x => (int)x.Value)));
                    week.PlanWorkout(workoutIds.LastOrDefault().Value);

                }

                Assert.True(week.WorkoutIds.SequenceEqual(workoutIds));

                // Remove Workouts
                int removeWorkoutsNum = RandomFieldGenerator.RandomInt(removeWorkoutsMin, removeWorkoutsMax);

                for (int iwo = 0; iwo < removeWorkoutsNum; iwo++)
                {
                    uint? idToRemove = RandomFieldGenerator.ChooseAmong(workoutIds);

                    workoutIds.Remove(idToRemove);
                    week.UnplanWorkout(idToRemove.Value);

                    Assert.True(week.WorkoutIds.SequenceEqual(workoutIds));
                }
            }
        }


        [Fact]
        public void TrainingPlanFail()
        {
            int ntests = 100;
            int planNameLengthMin = 10, planNameLengthMax = 100;
            int ownerIdMin = 50000, ownerIdMax = 55555;
            int idsSizeMin = 0, idsSizeMax = 10;
            int noteIdMin = 7000, noteIdMax = 12999;
            int messageIdMin = 6881, messageIdMax = 22441;
            int trainingWeeksMin = 0, trainingWeeksMax = 10;

            uint? planId = (uint?)(17);

            for (int itest = 0; itest < ntests; itest++)
            {
                List<TrainingWeekEntity> weeks = new List<TrainingWeekEntity>();

                bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.1f);
                string name = RandomFieldGenerator.RandomTextValue(planNameLengthMin, planNameLengthMax);
                bool isBookmarked = RandomFieldGenerator.RandomBoolWithProbability(0.5f);

                uint? noteId = (uint?)(RandomFieldGenerator.RandomInt(noteIdMin, noteIdMax));
                uint? messageId = (uint?)(RandomFieldGenerator.RandomInt(messageIdMin, messageIdMax));
                uint ownerId = (uint)(RandomFieldGenerator.RandomInt(ownerIdMin, ownerIdMax));

                List<uint?> scheduleIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<uint?> phaseIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<uint?> proficiencyIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<uint?> hashtagIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<uint?> focusIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();

                float testCaseProbability = (float)RandomFieldGenerator.RandomDouble(0, 1);

                int trainingWeeksNumber = RandomFieldGenerator.RandomInt(trainingWeeksMin, trainingWeeksMax);

                for (int iweek = 0; iweek < trainingWeeksNumber; iweek++)
                    weeks.Add(WorkoutTemplateAggregateBuilder.BuildRandomTrainingWeek(iweek + 1, iweek, isTransient));

                TrainingPlanRoot validPlan = TrainingPlanRoot.CreateTrainingPlan(planId, ownerId, weeks);


                #region Check Creation Fails

                bool faked = false;
                weeks = new List<TrainingWeekEntity>();

                for (int iweek = 0; iweek < trainingWeeksNumber; iweek++)
                {
                    if (RandomFieldGenerator.RollEventWithProbability(0.2f))
                    {
                        faked = true;
                        weeks.Add(WorkoutTemplateAggregateBuilder.BuildRandomTrainingWeek(iweek + 1, (iweek + 1) * trainingWeeksNumber * 3, isTransient));
                    }
                    else
                        weeks.Add(WorkoutTemplateAggregateBuilder.BuildRandomTrainingWeek(iweek + 1, iweek, isTransient));
                }

                if (weeks.Count == 0)
                    weeks.Add(WorkoutTemplateAggregateBuilder.BuildRandomTrainingWeek(1, 1, isTransient));
                else if (!faked)
                    weeks.Add(WorkoutTemplateAggregateBuilder.BuildRandomTrainingWeek(1, trainingWeeksNumber * 3, isTransient));

                Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingPlanRoot.CreateTrainingPlan(planId, ownerId, weeks));

                #endregion


                // Check Modification Failures
                Assert.Throws<ArgumentNullException>(() => validPlan.PlanFullRestWeek(null));
                Assert.Throws<ArgumentException>(() => validPlan.PlanFullRestWeek(
                    WorkoutTemplateAggregateBuilder.BuildRandomTrainingWeek(1, 0, isTransient, weekType: TrainingWeekTypeEnum.Generic)));

                Assert.Throws<ArgumentNullException>(() => validPlan.PlanTrainingWeek(null));

                Assert.Throws<ArgumentException>(() => validPlan.PlanTransientTrainingWeek(TrainingWeekTypeEnum.FullRest
                    , new List<uint?>() { 1, }));
            }
        }


        [Fact]
        public void TrainingPlanTemplateFullTest()
        {
            long planId = 17;
            float restWeekProbability = 0.05f;

            int addWeekMin = 1, addWeekMax = 3;
            int removeWeekMin = 1, removeWeekMax = 3;
            int changeWeekMin = 1, changeWeekMax = 3;

            bool faked;
            float fakedOperationProbability = 0.05f;

            TrainingWeekTypeEnum weekType;
            TrainingWeekEntity week;
            //TrainingPlanTypeEnum planType;

            for (int itest = 0; itest < ntests; itest++)
            {
                bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.1f);
                TrainingPlanRoot plan = BuildAndCheckRandomTrainingPlan(planId, isTransient);

                // Add Training Week
                int addWeeksNumber = RandomFieldGenerator.RandomInt(addWeekMin, addWeekMax);
                int srcWeeksNumber = plan.TrainingWeeks.Count;
                int weeksAddedCount = 0;

                IList<TrainingWeekEntity> weeks = new List<TrainingWeekEntity>(plan.TrainingWeeks);

                for (int iweek = 0; iweek < addWeeksNumber; iweek++)
                {
                    faked = false;
                    // Full rest is a specific case -> choose it with a small chance
                    if (RandomFieldGenerator.RollEventWithProbability(restWeekProbability))
                        weekType = TrainingWeekTypeEnum.FullRest;
                    else
                        weekType = TrainingWeekTypeEnum.From(
                            RandomFieldGenerator.RandomIntValueExcluded(TrainingWeekTypeEnum.Generic.Id, TrainingWeekTypeEnum.Peak.Id, TrainingWeekTypeEnum.FullRest.Id));

                    if (!isTransient && plan.TrainingWeeks.Count > 0 &&
                        (faked = RandomFieldGenerator.RollEventWithProbability(fakedOperationProbability)))

                        // Fake insertion
                        week = WorkoutTemplateAggregateBuilder.BuildRandomTrainingWeek(
                            RandomFieldGenerator.ChooseAmong(plan.TrainingWeeks.Select(x => (int)x.Id)),
                            srcWeeksNumber + weeksAddedCount, isTransient, weekType: weekType);
                    else
                    {
                        // Valid insertion
                        week = WorkoutTemplateAggregateBuilder.BuildRandomTrainingWeek(srcWeeksNumber + iweek + 1, srcWeeksNumber + weeksAddedCount++, isTransient, weekType: weekType);
                        weeks.Add(week);
                    }


                    if (isTransient)
                    {
                        if (weekType == TrainingWeekTypeEnum.FullRest)
                            plan.PlanTransientFullRestWeek();
                        else
                            plan.PlanTransientTrainingWeek(week.TrainingWeekType, week.WorkoutIds);
                    }
                    else
                    {
                        if (faked)
                            Assert.Throws<ArgumentException>(() => plan.PlanTrainingWeek(week));
                        else
                            plan.PlanTrainingWeek(week);
                    }


                    CheckTrainingWeeksSequence(weeks, plan.TrainingWeeks, isTransient);
                }

                // Change Training Week
                int changeWeeksNumber = RandomFieldGenerator.RandomInt(changeWeekMin, Math.Min(changeWeekMax, plan.TrainingWeeks.Count));

                weeks = new List<TrainingWeekEntity>(plan.TrainingWeeks);

                for (int iweek = 0; iweek < changeWeeksNumber; iweek++)
                {
                    uint weekPnum = RandomFieldGenerator.ChooseAmong(plan.TrainingWeeks.Select(x => x.ProgressiveNumber));
                    week = weeks.Single(x => x.ProgressiveNumber == weekPnum);

                    if (!week.IsFullRestWeek())
                    {
                        List<uint?> workoutsIds = plan.TrainingWeeks.ElementAt((int)weekPnum).WorkoutIds.ToList();

                        // Add Workout
                        uint? toAddId = (uint?)RandomFieldGenerator.RandomIntValueExcluded(1, 1242135, workoutsIds.Select(x => (int)x.Value));

                        workoutsIds.Add(toAddId);
                        plan.PlanWorkout(weekPnum, toAddId.Value);

                        Assert.True(plan.CloneWorkouts(weekPnum).SequenceEqual(workoutsIds));

                        // Remove Workout
                        uint? toRemoveId = RandomFieldGenerator.ChooseAmong(workoutsIds);

                        workoutsIds.Remove(toRemoveId);
                        plan.UnplanWorkout(toRemoveId.Value);

                        Assert.True(plan.CloneWorkouts(weekPnum).SequenceEqual(workoutsIds));
                    }

                }

                // Remove Training Week
                int removeWeeksNumber = RandomFieldGenerator.RandomInt(removeWeekMin, Math.Min(removeWeekMax, plan.TrainingWeeks.Count));
                uint toRemovePnum;

                faked = false;
                weeks = new List<TrainingWeekEntity>(plan.TrainingWeeks);

                for (int iweek = 0; iweek < removeWeeksNumber; iweek++)
                {
                    if (faked = RandomFieldGenerator.RollEventWithProbability(fakedOperationProbability))
                    {
                        toRemovePnum = (uint)RandomFieldGenerator.RandomIntValueExcluded(1, 10000, plan.TrainingWeeks.Select(x => (int)x.ProgressiveNumber));
                        Assert.Throws<ArgumentException>(() => plan.UnplanTrainingWeek(toRemovePnum));
                    }
                    else
                    {
                        toRemovePnum = RandomFieldGenerator.ChooseAmong(plan.TrainingWeeks.Select(x => x.ProgressiveNumber));

                        week = weeks.Single(x => x.ProgressiveNumber == toRemovePnum);

                        weeks.Remove(week);
                        plan.UnplanTrainingWeek(toRemovePnum);

                        weeks = StaticUtils.ForceConsecutiveProgressiveNumbers(weeks).ToList();

                        CheckTrainingWeeksSequence(weeks, plan.TrainingWeeks, isTransient);

                        output.WriteLine($"Removed : {toRemovePnum.ToString()}");
                    }
                }

            }
        }



        #region Support Functions

        internal static void CheckWorkingSetSequence(
            IEnumerable<WorkingSetTemplateEntity> leftSequence, IEnumerable<WorkingSetTemplateEntity> rightSequence, bool isTransient)
        {
            IEnumerator<WorkingSetTemplateEntity> leftEnum = leftSequence.GetEnumerator();
            IEnumerator<WorkingSetTemplateEntity> rightEnum = rightSequence.GetEnumerator();

            Assert.Equal(leftSequence.Count(), rightSequence.Count());

            while (leftEnum.MoveNext() && rightEnum.MoveNext())
            {
                WorkoutTemplateAggregateTest.CheckWorkingSet(
                    leftEnum.Current, rightEnum.Current, isTransient);
            }
        }



        internal static TrainingPlanRoot BuildAndCheckRandomTrainingPlan(long planIdNum, bool isTransient, IList<TrainingWeekEntity> weeks = null)
        {
            int planNameLengthMin = 10, planNameLengthMax = 100;
            int ownerIdMin = 50000, ownerIdMax = 55555;
            int noteIdMin = 7000, noteIdMax = 12999;
            int trainingWeeksMin = 0, trainingWeeksMax = 10;

            //float alternativeConstructorProbabilty = 0.25f;

            string name = RandomFieldGenerator.RandomTextValue(planNameLengthMin, planNameLengthMax);
            bool isBookmarked = RandomFieldGenerator.RandomBoolWithProbability(0.5f);

            uint? planId = (uint?)(planIdNum);
            uint? noteId = (uint?)(RandomFieldGenerator.RandomInt(noteIdMin, noteIdMax));
            uint? ownerId = (uint?)(RandomFieldGenerator.RandomInt(ownerIdMin, ownerIdMax));

            int trainingWeeksNumber = RandomFieldGenerator.RandomInt(trainingWeeksMin, trainingWeeksMax);

            if (weeks == null)
            {
                weeks = new List<TrainingWeekEntity>();

                for (int iweek = 0; iweek < trainingWeeksNumber; iweek++)
                    weeks.Add(WorkoutTemplateAggregateBuilder.BuildRandomTrainingWeek(iweek + 1, iweek, isTransient));
            }

            TrainingPlanRoot plan = TrainingPlanRoot.CreateTrainingPlan(planId, ownerId.Value, weeks);

            Assert.Equal(planId, plan.Id);
            Assert.Equal(ownerId, plan.OwnerId);

            Assert.True(Enumerable.Range(0, weeks.Count).SequenceEqual(plan.TrainingWeeks.Select(x => (int)x.ProgressiveNumber)));

            Assert.Equal((float)weeks.Where(x => x?.WorkoutIds != null).DefaultIfEmpty()?.Average(x => x?.WorkoutIds.Count ?? 0)
                , plan.GetAverageWorkoutsPerWeek(), 1);

            Assert.Equal((int)weeks.Where(x => x?.WorkoutIds != null).DefaultIfEmpty()?.Min(x => x?.WorkoutIds.Count ?? 0)
                , plan.GetMinimumWorkoutsPerWeek());

            Assert.Equal((int)weeks.Where(x => x?.WorkoutIds != null).DefaultIfEmpty()?.Max(x => x?.WorkoutIds.Count ?? 0)
                , plan.GetMaximumWorkoutsPerWeek());

            CheckTrainingWeeksSequence(weeks, plan.TrainingWeeks, isTransient);

            return plan;
        }



        internal static void CheckTrainingWeeksSequence(
            IEnumerable<TrainingWeekEntity> leftSequence, IEnumerable<TrainingWeekEntity> rightSequence, bool isTransient)
        {
            IEnumerator<TrainingWeekEntity> leftEnum = leftSequence.GetEnumerator();
            IEnumerator<TrainingWeekEntity> rightEnum = rightSequence.GetEnumerator();

            Assert.Equal(leftSequence.Count(), rightSequence.Count());

            while (leftEnum.MoveNext() && rightEnum.MoveNext())
            {
                CheckTrainingWeek(leftEnum.Current, rightEnum.Current, isTransient);
            }
        }


        internal static void CheckTrainingWeek(TrainingWeekEntity left, TrainingWeekEntity right, bool isTransient)
        {
            if (!isTransient)
                Assert.Equal(left.Id, right.Id);

            Assert.Equal(left.ProgressiveNumber, right.ProgressiveNumber);
            Assert.Equal(left.TrainingWeekType, right.TrainingWeekType);

            Assert.True(left.WorkoutIds.SequenceEqual(right.WorkoutIds));
        }
        #endregion



    }
}
