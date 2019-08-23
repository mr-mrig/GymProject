using GymProject.Domain.Base;
using GymProject.Domain.Test.Util;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace GymProject.Domain.Test.UnitTest
{
    public class TrainingPlanAggregateTest
    {



        public const int ntests = 200;



        [Fact]
        public static void TrainingWeekFail()
        {
            int ntests = 300;        // Perform less tests for fail conditions

            int initialWorkoutMax = 5;
            int initialWorkoutsNum;

            TrainingWeekTemplate week = null;
            IdTypeValue weekId = IdTypeValue.Create(1);
            TrainingWeekTypeEnum newWeekType;

            for (int itest = 0; itest < ntests; itest++)
            {

                bool faked = false;
                bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.1f);

                List<WorkoutTemplateReferenceValue> initialWorkoutsReferences = new List<WorkoutTemplateReferenceValue>();

                TrainingWeekTypeEnum weekType = TrainingWeekTypeEnum.From(
                    RandomFieldGenerator.RandomInt(1, TrainingWeekTypeEnum.Peak.Id));


                // Full rest Week with workouts
                if (weekType == TrainingWeekTypeEnum.FullRest)
                {
                    initialWorkoutsNum = RandomFieldGenerator.RandomInt(1, initialWorkoutMax);      // At least one

                    for (int iwo = 0; iwo < initialWorkoutsNum; iwo++)
                    {
                        WorkoutTemplate wo = StaticUtils.BuildRandomWorkout(iwo + 1, isTransient);
                        initialWorkoutsReferences.Add(WorkoutTemplateReferenceValue.BuildLinkToWorkout((uint)iwo, wo.CloneAllWorkingSets()));
                    }

                    if (isTransient)
                    {
                        Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                            TrainingWeekTemplate.PlanTransientTrainingWeek(0, initialWorkoutsReferences, weekType));

                        week = TrainingWeekTemplate.PlanTransientFullRestWeek(0);
                    }
                    else
                    {
                        Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                            TrainingWeekTemplate.PlanTrainingWeek(weekId, 0, initialWorkoutsReferences, weekType));

                        week = TrainingWeekTemplate.PlanFullRestWeek(weekId, 0);
                    }

                    // BUSINESS RULE REMOVED
                    //// Check fail when trying to switch out from Full Rest
                    //newWeekType = TrainingWeekTypeEnum.From(RandomFieldGenerator.RandomIntValueExcluded(
                    //    1, TrainingWeekTypeEnum.Peak.Id, TrainingWeekTypeEnum.FullRest.Id));

                    //Assert.Throws<TrainingDomainInvariantViolationException>(()=>
                    //    week.AssignSpecificWeekType(newWeekType));
                }
                else
                {
                    float testCaseProbability = (float)RandomFieldGenerator.RandomDouble(0, 1);
                    initialWorkoutsNum = RandomFieldGenerator.RandomInt(1, initialWorkoutMax);


                    switch (testCaseProbability)
                    {
                        // BUSINESS RULE REMOVED
                        // Non rest week with no workouts - empty
                        //case var _ when testCaseProbability < 0.2f:

                        //    if (isTransient)
                        //        Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                        //        TrainingWeekTemplate.PlanTransientTrainingWeek(0, new List<WorkoutTemplateReferenceValue>(), weekType));
                        //    else
                        //        Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                        //        TrainingWeekTemplate.PlanTrainingWeek(weekId, 0, new List<WorkoutTemplateReferenceValue>(), weekType));

                        //    break;

                        // BUSINESS RULE REMOVED
                        //// Non rest week with no workouts - null
                        //case var _ when testCaseProbability < 0.4f:

                        //    if (isTransient)
                        //        Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                        //        TrainingWeekTemplate.PlanTransientTrainingWeek(0, null, weekType));
                        //    else
                        //        Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                        //        TrainingWeekTemplate.PlanTrainingWeek(weekId, 0, null, weekType));

                        //    break;
                        //case var _ when testCaseProbability < 0.6f:


                        // Null workouts
                        case var _ when testCaseProbability < 0.3f:

                            for (int iwo = 0; iwo < initialWorkoutsNum; iwo++)
                            {
                                WorkoutTemplate wo;

                                if (RandomFieldGenerator.RollEventWithProbability(0.2f))
                                {
                                    initialWorkoutsReferences.Add(null);
                                    faked = true;
                                }
                                else
                                {
                                    wo = StaticUtils.BuildRandomWorkout(iwo + 1, isTransient);
                                    initialWorkoutsReferences.Add(
                                        WorkoutTemplateReferenceValue.BuildLinkToWorkout((uint)iwo, wo.CloneAllWorkingSets()));
                                }
                            }
                            if (!faked)
                                initialWorkoutsReferences[0] = null;

                            if (isTransient)
                                Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                                TrainingWeekTemplate.PlanTransientTrainingWeek(0, initialWorkoutsReferences, weekType));
                            else
                                Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                                TrainingWeekTemplate.PlanTrainingWeek(weekId, 0, initialWorkoutsReferences, weekType));
                            break;

                        // Invalid Progressive Numbers
                        default:

                            for (int iwo = 0; iwo < initialWorkoutsNum; iwo++)
                            {
                                int pnum;

                                if (RandomFieldGenerator.RollEventWithProbability(0.2f))
                                {
                                    pnum = (iwo + 1) * initialWorkoutMax;
                                    faked = true;
                                }
                                else
                                    pnum = iwo;

                                WorkoutTemplate wo = StaticUtils.BuildRandomWorkout(iwo + 1, isTransient);
                                initialWorkoutsReferences.Add(WorkoutTemplateReferenceValue.BuildLinkToWorkout((uint)pnum, wo.CloneAllWorkingSets()));
                            }
                            if (!faked)
                                initialWorkoutsReferences[0] = initialWorkoutsReferences[0].MoveToNewProgressiveNumber(1000);

                            if (isTransient)
                                Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                                TrainingWeekTemplate.PlanTransientTrainingWeek(0, initialWorkoutsReferences, weekType));
                            else
                                Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                                TrainingWeekTemplate.PlanTrainingWeek(weekId, 0, initialWorkoutsReferences, weekType));
                            break;
                    }
                }
            }

            // Check fail when trying to switch to Full Rest
            week = TrainingWeekTemplate.PlanTrainingWeek(weekId, 0,
                new List<WorkoutTemplateReferenceValue>()
                {
                    WorkoutTemplateReferenceValue.BuildLinkToWorkout(0, new List<WorkingSetTemplate>())
                });

            Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                week.AssignSpecificWeekType(TrainingWeekTypeEnum.FullRest));
        }


        [Fact]
        public static void TrainingWeekFullTest()
        {
            int initialWorkoutMin = 1, initialWorkoutMax = 10;
            int addWorkoutsMin = 2, addWorkoutsMax = 3;
            int removeWorkoutsMin = 1, removeWorkoutsMax = addWorkoutsMin + initialWorkoutMin - 1;
            int changeWorkoutMin = 1;

            for (int itest = 0; itest < ntests; itest++)
            {
                uint weekPnum = (uint)RandomFieldGenerator.RandomInt(0, 10);
                bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.1f);

                int initialWorkoutsNum = RandomFieldGenerator.RandomInt(initialWorkoutMin, initialWorkoutMax);
                List<WorkoutTemplate> initialWorkouts = new List<WorkoutTemplate>();
                List<WorkoutTemplate> workouts = new List<WorkoutTemplate>();
                List<WorkoutTemplateReferenceValue> initialWorkoutsReferences = new List<WorkoutTemplateReferenceValue>();

                TrainingWeekTemplate week;
                IdTypeValue weekId = IdTypeValue.Create(1);

                TrainingWeekTypeEnum weekType = TrainingWeekTypeEnum.From(
                    RandomFieldGenerator.RandomIntValueExcluded(1, TrainingWeekTypeEnum.Peak.Id, TrainingWeekTypeEnum.FullRest.Id));

                // Very small chance for Full Rest weeks, as many operations on them cannot be tested
                if (RandomFieldGenerator.RollEventWithProbability(0.02f))
                    weekType = TrainingWeekTypeEnum.FullRest;

                // Create Week
                if (weekType == TrainingWeekTypeEnum.FullRest)
                {
                    if (isTransient)
                        week = TrainingWeekTemplate.PlanTransientFullRestWeek(weekPnum);
                    else
                        week = TrainingWeekTemplate.PlanFullRestWeek(weekId, weekPnum);

                    // Check Week
                    Assert.NotNull(week);
                    Assert.Equal(weekPnum, week.ProgressiveNumber);
                    Assert.Equal(weekType, week.TrainingWeekType);
                    Assert.Equal(initialWorkouts.Count, week.Workouts.Count);

                    CheckWorkingSetSequence(initialWorkouts, week, isTransient);

                    Assert.True(Enumerable.Range(0, initialWorkouts.Count).ToList().SequenceEqual
                        (week.Workouts.Select(x => (int)x.ProgressiveNumber)));

                    continue;   // Skip to next test
                }

                for (int iwo = 0; iwo < initialWorkoutsNum; iwo++)
                {
                    WorkoutTemplate wo = StaticUtils.BuildRandomWorkout(iwo + 1, isTransient);
                    initialWorkouts.Add(wo);
                    initialWorkoutsReferences.Add(WorkoutTemplateReferenceValue.BuildLinkToWorkout((uint)iwo, wo.CloneAllWorkingSets()));
                }

                if (isTransient)
                    week = TrainingWeekTemplate.PlanTransientTrainingWeek(weekPnum, initialWorkoutsReferences, weekType);
                else
                    week = TrainingWeekTemplate.PlanTrainingWeek(weekId, weekPnum, initialWorkoutsReferences, weekType);


                // Check Week
                Assert.NotNull(week);
                Assert.Equal(weekPnum, week.ProgressiveNumber);
                Assert.Equal(weekType, week.TrainingWeekType);
                Assert.Equal(initialWorkouts.Count, week.Workouts.Count);

                CheckWorkingSetSequence(initialWorkouts, week, isTransient);

                Assert.True(Enumerable.Range(0, initialWorkouts.Count).ToList().SequenceEqual
                    (week.Workouts.Select(x => (int)x.ProgressiveNumber)));

                // Change Week
                weekPnum = (uint)RandomFieldGenerator.RandomIntValueExcluded(0, 20, (int)weekPnum);
                week.MoveToNewProgressiveNumber(weekPnum);

                if (RandomFieldGenerator.RollEventWithProbability(0.02f))
                {
                    weekType = TrainingWeekTypeEnum.FullRest;
                    week.MarkAsFullRestWeek();

                    Assert.Equal(weekPnum, week.ProgressiveNumber);
                    Assert.Equal(weekType, week.TrainingWeekType);
                    Assert.Empty(week.Workouts);

                    continue;   // Skip to next test
                }
                weekType = TrainingWeekTypeEnum.From(
                    RandomFieldGenerator.RandomIntValueExcluded(1, TrainingWeekTypeEnum.Peak.Id, TrainingWeekTypeEnum.FullRest.Id));

                week.AssignSpecificWeekType(weekType);

                Assert.Equal(weekPnum, week.ProgressiveNumber);
                Assert.Equal(weekType, week.TrainingWeekType);

                workouts = new List<WorkoutTemplate>(initialWorkouts);

                // Add Workouts
                int addWorkoutsNum = RandomFieldGenerator.RandomInt(addWorkoutsMin, addWorkoutsMax);

                for (int iwo = 0; iwo < addWorkoutsNum; iwo++)
                {
                    WorkoutTemplate wo = StaticUtils.BuildRandomWorkout(iwo + 1, isTransient);

                    workouts.Add(wo);
                    week.PlanWorkout(wo.CloneAllWorkingSets().ToList());

                    CheckWorkingSetSequence(workouts, week, isTransient);

                    Assert.True(Enumerable.Range(0, workouts.Count).ToList().SequenceEqual
                        (week.Workouts.Select(x => (int)x.ProgressiveNumber)));
                }

                // Remove Workouts
                int removeWorkoutsNum = RandomFieldGenerator.RandomInt(removeWorkoutsMin, removeWorkoutsMax);

                for (int iwo = 0; iwo < removeWorkoutsNum; iwo++)
                {
                    uint removePnum = RandomFieldGenerator.ChooseAmong(week.Workouts.Select(x => x.ProgressiveNumber).ToList());

                    workouts.RemoveAt((int)removePnum);
                    week.UnplanWorkout(removePnum);

                    CheckWorkingSetSequence(workouts, week, isTransient);

                    Assert.True(Enumerable.Range(0, workouts.Count).ToList().SequenceEqual
                        (week.Workouts.Select(x => (int)x.ProgressiveNumber)));
                }

                // Change Workouts
                int changeWorkoutsNum = RandomFieldGenerator.RandomInt(changeWorkoutMin, workouts.Count);

                for (int iwo = 0; iwo < changeWorkoutsNum; iwo++)
                {
                    uint srcPnum = RandomFieldGenerator.ChooseAmong(week.Workouts.Select(x => x.ProgressiveNumber).ToList());
                    uint destPnum = RandomFieldGenerator.ChooseAmong(week.Workouts.Select(x => x.ProgressiveNumber).ToList());

                    WorkoutTemplateReferenceValue src = week.Workouts.ToList()[(int)srcPnum];
                    WorkoutTemplateReferenceValue dest = week.Workouts.ToList()[(int)destPnum];

                    week.MoveWorkoutToNewProgressiveNumber(srcPnum, destPnum);

                    CheckWorkingSetSequence(src.WorkingSets, week.Workouts.ToList()[(int)destPnum].WorkingSets, isTransient);
                    CheckWorkingSetSequence(dest.WorkingSets, week.Workouts.ToList()[(int)srcPnum].WorkingSets, isTransient);

                    Assert.True(Enumerable.Range(0, workouts.Count).ToList().SequenceEqual
                        (week.Workouts.Select(x => (int)x.ProgressiveNumber)));
                }
            }
        }





        #region Support Functions

        internal static void CheckWorkingSetSequence(
            IEnumerable<WorkingSetTemplate> leftSequence, IEnumerable<WorkingSetTemplate> rightSequence, bool isTransient)
        {
            IEnumerator<WorkingSetTemplate> leftEnum = leftSequence.GetEnumerator();
            IEnumerator<WorkingSetTemplate> rightEnum = rightSequence.GetEnumerator();

            Assert.Equal(leftSequence.Count(), rightSequence.Count());

            while (leftEnum.MoveNext() && rightEnum.MoveNext())
            {
                WorkoutTemplateAggregateTest.CheckWorkingSet(
                    leftEnum.Current, rightEnum.Current, isTransient);
            }
        }


        internal static void CheckWorkingSetSequence(
            IEnumerable<WorkoutTemplate> workouts, TrainingWeekTemplate week, bool isTransient)

            => CheckWorkingSetSequence(
                workouts.SelectMany(x => x.CloneAllWorkingSets()),
                week.Workouts.SelectMany(x => x.WorkingSets),
                isTransient);


        #endregion



    }
}
