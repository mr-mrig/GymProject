using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.Test.Util;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GymProject.Domain.Test.UnitTest
{
    public class TrainingPlanAggregateTest
    {



        public const int ntests = 20;

        private readonly ITestOutputHelper output;



        public TrainingPlanAggregateTest(ITestOutputHelper output)
        {
            this.output = output;
        }





        [Fact]
        public static void TrainingWeekFail()
        {
            int ntests = 300;        // Perform less tests for fail conditions

            int initialWorkoutMax = 5;
            int initialWorkoutsNum;

            TrainingWeekEntity week = null;
            uint? weekId = 1;

            // Create Training Week
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
                        WorkoutTemplateRoot wo = WorkoutTemplateAggregateBuilder.BuildRandomWorkoutTemplate(iwo + 1, isTransient);
                        initialWorkoutsReferences.Add(WorkoutTemplateReferenceValue.BuildLinkToWorkout((uint)iwo, wo.CloneAllWorkingSets()));
                    }

                    if (isTransient)
                    {
                        Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                            TrainingWeekEntity.PlanTransientTrainingWeek(0, initialWorkoutsReferences, weekType));

                        week = TrainingWeekEntity.PlanTransientFullRestWeek(0);
                    }
                    else
                    {
                        Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                            TrainingWeekEntity.PlanTrainingWeek(weekId, 0, initialWorkoutsReferences, weekType));

                        week = TrainingWeekEntity.PlanFullRestWeek(weekId, 0);
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
                                WorkoutTemplateRoot wo;

                                if (RandomFieldGenerator.RollEventWithProbability(0.2f))
                                {
                                    initialWorkoutsReferences.Add(null);
                                    faked = true;
                                }
                                else
                                {
                                    wo = WorkoutTemplateAggregateBuilder.BuildRandomWorkoutTemplate(iwo + 1, isTransient);
                                    initialWorkoutsReferences.Add(
                                        WorkoutTemplateReferenceValue.BuildLinkToWorkout((uint)iwo, wo.CloneAllWorkingSets()));
                                }
                            }
                            if (!faked)
                                initialWorkoutsReferences[0] = null;

                            if (isTransient)
                                Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                                    TrainingWeekEntity.PlanTransientTrainingWeek(0, initialWorkoutsReferences, weekType));
                            else
                                Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                                    TrainingWeekEntity.PlanTrainingWeek(weekId, 0, initialWorkoutsReferences, weekType));
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

                                WorkoutTemplateRoot wo = WorkoutTemplateAggregateBuilder.BuildRandomWorkoutTemplate(iwo + 1, isTransient);
                                initialWorkoutsReferences.Add(WorkoutTemplateReferenceValue.BuildLinkToWorkout((uint)pnum, wo.CloneAllWorkingSets()));
                            }
                            if (!faked)
                                initialWorkoutsReferences[0] = initialWorkoutsReferences[0].MoveToNewProgressiveNumber(1000);

                            if (isTransient)
                                Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                                TrainingWeekEntity.PlanTransientTrainingWeek(0, initialWorkoutsReferences, weekType));
                            else
                                Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                                TrainingWeekEntity.PlanTrainingWeek(weekId, 0, initialWorkoutsReferences, weekType));
                            break;
                    }
                }
            }

            // Check fail when trying to switch to Full Rest
            week = TrainingWeekEntity.PlanTrainingWeek(weekId, 0,
                new List<WorkoutTemplateReferenceValue>()
                {
                    WorkoutTemplateReferenceValue.BuildLinkToWorkout(0, new List<WorkingSetTemplateEntity>())
                });

            Assert.Throws<TrainingDomainInvariantViolationException>(() =>
                week.AssignSpecificWeekType(TrainingWeekTypeEnum.FullRest));

            // Invalid Progressive Number
            week = WorkoutTemplateAggregateBuilder.BuildRandomTrainingWeek(19, 0, true, nWorkoutsMin: 3, nWorkoutsMax: 6, weekType: TrainingWeekTypeEnum.Generic, noWorkoutsProb: 0);
            uint invalidPnum = (uint)week.Workouts.Count() + 1;

            List<WorkingSetTemplateEntity> newWorkingSets = new List<WorkingSetTemplateEntity>()
            {
                WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(1, 0, false, TrainingEffortTypeEnum.RM),
                WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(1, 1, false, TrainingEffortTypeEnum.RM),
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => week.MoveWorkoutToNewProgressiveNumber(0, invalidPnum));
            Assert.Throws<ArgumentOutOfRangeException>(() => week.UnplanWorkout(invalidPnum));
            Assert.Throws<ArgumentOutOfRangeException>(() => week.AddWorkingSets(invalidPnum, newWorkingSets));
            Assert.Throws<ArgumentOutOfRangeException>(() => week.CloneWorkoutWorkingSets(invalidPnum));
            Assert.Throws<ArgumentOutOfRangeException>(() => week.RemoveWorkingSets(invalidPnum, newWorkingSets));

            // Remove Transient Working Sets
            uint validPnum = 0;

            List<WorkingSetTemplateEntity> transientWorkingSets = new List<WorkingSetTemplateEntity>()
            {
                WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(1, 0, true, TrainingEffortTypeEnum.RM),
                WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(1, 1, true, TrainingEffortTypeEnum.RM),
            };
            Assert.Throws<InvalidOperationException>(() => week.RemoveWorkingSets(validPnum, newWorkingSets));
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
                List<WorkoutTemplateRoot> initialWorkouts = new List<WorkoutTemplateRoot>();
                List<WorkoutTemplateRoot> workouts = new List<WorkoutTemplateRoot>();
                List<WorkoutTemplateReferenceValue> initialWorkoutsReferences = new List<WorkoutTemplateReferenceValue>();

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

                    // Check
                    CheckWeekWorkouts(initialWorkouts, week, isTransient);

                    continue;   // Skip to next test
                }

                for (int iwo = 0; iwo < initialWorkoutsNum; iwo++)
                {
                    WorkoutTemplateRoot wo = WorkoutTemplateAggregateBuilder.BuildRandomWorkoutTemplate(iwo + 1, isTransient);
                    initialWorkouts.Add(wo);
                    initialWorkoutsReferences.Add(WorkoutTemplateReferenceValue.BuildLinkToWorkout((uint)iwo, wo.CloneAllWorkingSets()));
                }

                if (isTransient)
                    week = TrainingWeekEntity.PlanTransientTrainingWeek(weekPnum, initialWorkoutsReferences, weekType);
                else
                    week = TrainingWeekEntity.PlanTrainingWeek(weekId, weekPnum, initialWorkoutsReferences, weekType);


                // Check
                CheckWeekWorkouts(initialWorkouts, week, isTransient);

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

                workouts = new List<WorkoutTemplateRoot>(initialWorkouts);

                // Add Workouts
                int addWorkoutsNum = RandomFieldGenerator.RandomInt(addWorkoutsMin, addWorkoutsMax);

                for (int iwo = 0; iwo < addWorkoutsNum; iwo++)
                {
                    WorkoutTemplateRoot wo = WorkoutTemplateAggregateBuilder.BuildRandomWorkoutTemplate(iwo + 1, isTransient);

                    workouts.Add(wo);
                    week.PlanWorkout(wo.CloneAllWorkingSets().ToList());

                    // Check
                    CheckWeekWorkouts(workouts, week, isTransient);
                }

                // Remove Workouts
                int removeWorkoutsNum = RandomFieldGenerator.RandomInt(removeWorkoutsMin, removeWorkoutsMax);

                for (int iwo = 0; iwo < removeWorkoutsNum; iwo++)
                {
                    uint removePnum = RandomFieldGenerator.ChooseAmong(week.Workouts.Select(x => x.ProgressiveNumber).ToList());

                    workouts.RemoveAt((int)removePnum);
                    week.UnplanWorkout(removePnum);

                    // Check 
                    CheckWeekWorkouts(workouts, week, isTransient);
                }

                // Change Workouts
                int changeWorkoutsNum = RandomFieldGenerator.RandomInt(changeWorkoutMin, workouts.Count);

                for (int iwo = 0; iwo < changeWorkoutsNum; iwo++)
                    CheckWeekWorkoutChanges(week, isTransient);

            }
        }


        [Fact]
        public void TrainingPlanFail()
        {
            int ntests = 300;
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
                uint? ownerId = (uint?)(RandomFieldGenerator.RandomInt(ownerIdMin, ownerIdMax));

                List<uint?> scheduleIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<uint?> phaseIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<uint?> proficiencyIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<uint?> hashtagIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<uint?> focusIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();

                float testCaseProbability = (float)RandomFieldGenerator.RandomDouble(0, 1);

                int trainingWeeksNumber = RandomFieldGenerator.RandomInt(trainingWeeksMin, trainingWeeksMax);

                for (int iweek = 0; iweek < trainingWeeksNumber; iweek++)
                    weeks.Add(WorkoutTemplateAggregateBuilder.BuildRandomTrainingWeek(iweek + 1, iweek, isTransient));

                List<TrainingPlanRelation> childPlansRelations = StaticUtils.BuildTrainingPlanRelations(planId).ToList();

                uint? validChildId = (uint?)(RandomFieldGenerator.RandomIntValueExcluded(1, 99999,
                            childPlansRelations.Select(x => (int)x.ChildPlanId).Union(new List<int>() { (int)planId })));

                TrainingPlanRoot validPlan = TrainingPlanRoot.CreateTrainingPlan(
                    planId, name, isBookmarked, ownerId, noteId, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlansRelations);


                #region Check Creation Fails
                switch (testCaseProbability)
                {
                    // Null Weeks
                    case var _ when testCaseProbability < 0.05f:

                        StaticUtils.InsertRandomNullElements(weeks);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(
                                    planId, name, isBookmarked, ownerId, noteId, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlansRelations));
                        break;

                    // Null owner
                    case var _ when testCaseProbability < 0.1f:

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(
                                    planId, name, isBookmarked, null, noteId, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlansRelations));
                        break;

                    // Null Schedules
                    case var _ when testCaseProbability < 0.2f:

                        StaticUtils.InsertRandomNullIds(scheduleIds);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(
                                    planId, name, isBookmarked, ownerId, noteId, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlansRelations));

                        break;

                    // Null Phases
                    case var _ when testCaseProbability < 0.25f:

                        StaticUtils.InsertRandomNullIds(phaseIds);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(
                                    planId, name, isBookmarked, ownerId, noteId, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlansRelations));
                        break;

                    // Null Proficiencies
                    case var _ when testCaseProbability < 0.3f:

                        StaticUtils.InsertRandomNullIds(proficiencyIds);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(
                                    planId, name, isBookmarked, ownerId, noteId, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlansRelations));
                        break;

                    // Null Childs
                    case var _ when testCaseProbability < 0.35f:

                        StaticUtils.InsertRandomNullElements(childPlansRelations);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(
                                    planId, name, isBookmarked, ownerId, noteId, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlansRelations));
                        break;

                    // Null focus
                    case var _ when testCaseProbability < 0.45f:

                        StaticUtils.InsertRandomNullIds(focusIds);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(
                                planId, name, isBookmarked, ownerId, noteId, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlansRelations));
                        break;

                    // Duplicate relations
                    case var _ when testCaseProbability < 0.65f:

                        int duplicatesNumber = RandomFieldGenerator.RandomInt(1, childPlansRelations.Count);

                        for (int irel = 0; irel < duplicatesNumber; irel++)
                            childPlansRelations.Add(StaticUtils.BuildTrainingPlanRelation(planId, 
                                RandomFieldGenerator.ChooseAmong(childPlansRelations.Select(x => x.ChildPlanId))));

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(
                                    planId, name, isBookmarked, ownerId, noteId, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlansRelations));
                        break;

                    // Non inherited with message attached
                    case var _ when testCaseProbability < 0.65f:

                        TrainingPlanTypeEnum childType = RandomFieldGenerator.RollEventWithProbability(0.5f)
                            ? TrainingPlanTypeEnum.NotSet 
                            : TrainingPlanTypeEnum.Variant;

                        childPlansRelations.Add(StaticUtils.BuildTrainingPlanRelation(planId, validChildId, childType, messageId));

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(
                                    planId, name, isBookmarked, ownerId, noteId, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlansRelations));
                        break;

                    // Parent plan different from the current one
                    case var _ when testCaseProbability < 0.75f:

                        uint? fakeId = (uint?)(RandomFieldGenerator.RandomIntValueExcluded(1, 9999, (int)planId));

                        childPlansRelations.Add(StaticUtils.BuildTrainingPlanRelation(fakeId, validChildId));

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(
                                    planId, name, isBookmarked, ownerId, noteId, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlansRelations));
                        break;

                    // Non Consecutive Numbers
                    case var _ when testCaseProbability < 0.9f:

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

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(
                                    planId, name, isBookmarked, ownerId, noteId, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlansRelations));

                        break;

                    // Child Plan same as Root Plan
                    default:

                        Assert.Throws<TrainingDomainInvariantViolationException>(() => StaticUtils.BuildTrainingPlanRelation(planId, planId));
                        break;
                }
                #endregion


                #region Check Modifications Fails

                Assert.Throws<ArgumentNullException>(() => validPlan.TagAs(null));
                Assert.Throws<ArgumentNullException>(() => validPlan.TagPhase(null));
                Assert.Throws<ArgumentNullException>(() => validPlan.LinkTargetProficiency(null));
                Assert.Throws<ArgumentNullException>(() => validPlan.FocusOnMuscle(null));
                Assert.Throws<ArgumentNullException>(() => validPlan.ScheduleTraining(null));

                Assert.Throws<ArgumentNullException>(() => validPlan.Untag(null));
                Assert.Throws<ArgumentNullException>(() => validPlan.UntagPhase(null));
                Assert.Throws<ArgumentNullException>(() => validPlan.UnlinkTargetProficiency(null));
                Assert.Throws<ArgumentNullException>(() => validPlan.UnfocusMuscle(null));

                Assert.Throws<ArgumentNullException>(() => validPlan.PlanFullRestWeek(null));
                Assert.Throws<ArgumentException>(() => validPlan.PlanFullRestWeek(
                    WorkoutTemplateAggregateBuilder.BuildRandomTrainingWeek(1, 0, isTransient, weekType: TrainingWeekTypeEnum.Generic)));

                Assert.Throws<ArgumentNullException>(() => validPlan.PlanTrainingWeek(null));

                Assert.Throws<ArgumentException>(() => validPlan.PlanTransientTrainingWeek(TrainingWeekTypeEnum.FullRest
                    , new List<WorkoutTemplateReferenceValue>() { WorkoutTemplateReferenceValue.BuildLinkToWorkout(0, null) }));

                //Assert.Throws<ArgumentException>(() => rootPlan.RemoveHashtag((uint?)(
                //    RandomFieldGenerator.RandomIntValueExcluded(1, 10000, rootPlan.Hashtags.Select(x => (int)x)))));

                //Assert.Throws<ArgumentException>(() => rootPlan.UnlinkTargetPhase((uint?)(
                //    RandomFieldGenerator.RandomIntValueExcluded(1, 10000, rootPlan.TrainingPhaseIds.Select(x => (int)x)))));

                //Assert.Throws<ArgumentException>(() => rootPlan.UnlinkTargetProficiency((uint?)(
                //    RandomFieldGenerator.RandomIntValueExcluded(1, 10000, rootPlan.TrainingProficiencyIds.Select(x => (int)x)))));

                //Assert.Throws<ArgumentException>(() => rootPlan.RemoveFocusToMuscle((uint?)(
                //    RandomFieldGenerator.RandomIntValueExcluded(1, 10000, rootPlan.MuscleFocusIds.Select(x => (int)x)))));

                //if(rootPlanType != null && rootPlanType != TrainingPlanTypeEnum.Variant && rootPlanType != TrainingPlanTypeEnum.NotSet)
                //    Assert.Throws<InvalidOperationException>(() => plan.MarkAsVariant());
                #endregion
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
            float inheritedPlanProbability = 0.25f;
            float variantPlanProbability = 0.25f;

            TrainingWeekTypeEnum weekType;
            TrainingWeekEntity week;
            TrainingPlanTypeEnum planType;

            for (int itest = 0; itest < ntests; itest++)
            {
                bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.1f);

                // Build the Plan according to randomic type
                float rolledChance = (float)RandomFieldGenerator.RandomDouble(0, 1);

                if (rolledChance <= inheritedPlanProbability)
                    planType = TrainingPlanTypeEnum.Inherited;

                else if (rolledChance <= variantPlanProbability)
                    planType = TrainingPlanTypeEnum.Variant;
                else
                    planType = TrainingPlanTypeEnum.NotSet;

                TrainingPlanRoot plan = BuildAndCheckRandomTrainingPlan(planId, isTransient);

                // Change Plan
                CheckTrainingPlanChanges(plan);

                // Add Training Week
                int addWeeksNumber = RandomFieldGenerator.RandomInt(addWeekMin, addWeekMax);
                int srcWeeksNumber = plan.TrainingWeeks.Count;
                int weeksAddedCount = 0;

                IList<TrainingWeekEntity> weeks = new List<TrainingWeekEntity>(plan.TrainingWeeks);

                for(int iweek = 0; iweek <addWeeksNumber; iweek++)
                {
                    faked = false;
                    // Full rest is a specific case -> choose it with a small chance
                    if (RandomFieldGenerator.RollEventWithProbability(restWeekProbability))
                        weekType = TrainingWeekTypeEnum.FullRest;
                    else
                        weekType = TrainingWeekTypeEnum.From(
                            RandomFieldGenerator.RandomIntValueExcluded(TrainingWeekTypeEnum.Generic.Id, TrainingWeekTypeEnum.Peak.Id, TrainingWeekTypeEnum.FullRest.Id));

                    if(!isTransient && plan.TrainingWeeks.Count > 0 &&
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
                            plan.PlanTransientTrainingWeek(week.TrainingWeekType, week.Workouts.Select(
                                x => WorkoutTemplateReferenceValue.BuildLinkToWorkout(x.ProgressiveNumber, x.WorkingSets)).ToList());
                    }
  
                    else
                    {
                        if(faked)
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

                    if(!week.IsFullRestWeek())
                    {
                        // Value Copy
                        //IList<WorkoutTemplateReferenceValue> workouts = plan.TrainingWeeks.ElementAt((int)weekPnum).Workouts
                        //    .Select(x => WorkoutTemplateReferenceValue.BuildLinkToWorkout(x.ProgressiveNumber, x.WorkingSets)).ToList();
                        IList<WorkoutTemplateReferenceValue> workouts = plan.TrainingWeeks.ElementAt((int)weekPnum).Workouts.ToList();

                        // Add Workout
                        WorkoutTemplateRoot workout = WorkoutTemplateAggregateBuilder.BuildRandomWorkoutTemplate(1, isTransient);
                        plan.PlanWorkout(weekPnum, workout.CloneAllWorkingSets());

                        week = plan.TrainingWeeks.Single(x => x.ProgressiveNumber == weekPnum);     // Keep Updated

                        CheckWeekWorkouts(workouts.Union(new List<WorkoutTemplateReferenceValue>() {
                            WorkoutTemplateReferenceValue.BuildLinkToWorkout((uint)workouts.Count(), workout.CloneAllWorkingSets()) }), week, isTransient);

                        Assert.Equal((float)(weeks.Sum(x => x.Workouts.Count) + 1) / (float)weeks.Count, plan.GetAverageWorkoutsPerWeek(), 1);

                        // Move Workout
                        uint srcPnum = (uint)RandomFieldGenerator.RandomInt(0, week.Workouts.Count - 1);
                        uint destPnum = (uint)RandomFieldGenerator.RandomInt(0, week.Workouts.Count - 1);
                        plan.MoveWorkoutToNewProgressiveNumber(weekPnum, srcPnum, destPnum);

                        WorkoutTemplateReferenceValue srcWorkout = plan.CloneWorkout(weekPnum, srcPnum);
                        WorkoutTemplateReferenceValue destWorkout = plan.CloneWorkout(weekPnum, destPnum);

                        CheckWorkingSetSequence(srcWorkout.WorkingSets, week.Workouts.ToList()[(int)destPnum].WorkingSets, isTransient);
                        CheckWorkingSetSequence(destWorkout.WorkingSets, week.Workouts.ToList()[(int)srcPnum].WorkingSets, isTransient);

                        // Remove Workout
                        //workouts = plan.TrainingWeeks.ElementAt((int)weekPnum).Workouts
                        //    .Select(x => WorkoutTemplateReferenceValue.BuildLinkToWorkout(x.ProgressiveNumber, x.WorkingSets)).ToList();
                        workouts = plan.TrainingWeeks.ElementAt((int)weekPnum).Workouts.ToList();

                        uint pnumToRemove = (uint)RandomFieldGenerator.RandomInt(0, week.Workouts.Count - 1);
                        plan.UnplanWorkout(weekPnum, pnumToRemove);

                        IList<WorkoutTemplateReferenceValue> workoutsLeft = workouts.Where(x => x.ProgressiveNumber != pnumToRemove).ToList();
                        workoutsLeft = StaticUtils.ForceConsecutiveProgressiveNumbers(workoutsLeft).ToList();

                        week = plan.TrainingWeeks.Single(x => x.ProgressiveNumber == weekPnum);     // Keep Updated

                        CheckWeekWorkouts(workoutsLeft, week, isTransient);

                        Assert.Equal((float)(weeks.Sum(x => x.Workouts.Count)) / (float)weeks.Count, plan.GetAverageWorkoutsPerWeek(), 1);

                        if (week.Workouts.Count > 0)
                            CheckPlanWorkoutChanges(plan, weekPnum, isTransient);
                    }

                }

                // Remove Training Week
                int removeWeeksNumber = RandomFieldGenerator.RandomInt(removeWeekMin, Math.Min(removeWeekMax, plan.TrainingWeeks.Count));
                uint toRemovePnum;

                faked = false;
                weeks = new List<TrainingWeekEntity>(plan.TrainingWeeks);

                for (int iweek = 0; iweek < removeWeeksNumber; iweek++)
                {
                    if(faked = RandomFieldGenerator.RollEventWithProbability(fakedOperationProbability))
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



        internal static void CheckPlanWorkoutChanges(TrainingPlanRoot plan, uint weekPnum, bool isTransient)
        {
            bool faked = false;

            int addWorkingSetsMin = 1, addWorkingSetsMax = 2;
            int removeWorkingSetsMin = 1, removeWorkingSetsMax = 2;

            float fakedOperationProbability = 0.05f;

            ICollection<WorkingSetTemplateEntity> newWorkingSets = new List<WorkingSetTemplateEntity>();
            ICollection<WorkingSetTemplateEntity> removeWorkingSets = new List<WorkingSetTemplateEntity>();


            TrainingWeekEntity week = plan.TrainingWeeks.Single(x => x.ProgressiveNumber == weekPnum);
            IList<WorkoutTemplateReferenceValue> workouts = week.Workouts.ToList();

            // Add Working Sets
            uint workoutPnum = (uint)RandomFieldGenerator.RandomInt(0, week.Workouts.Count - 1);
            int addWorkingSetsNumber = RandomFieldGenerator.RandomInt(addWorkingSetsMin, addWorkingSetsMax);
            uint idToAdd;
           
            WorkoutTemplateReferenceValue workout = week.CloneWorkout(workoutPnum);

            for (int iws = 0; iws < addWorkingSetsNumber; iws++)
            {
                if (isTransient)
                    idToAdd = 1;
                else
                {
                    IEnumerable<int> alreadyChosenIds = newWorkingSets.Union(workout.WorkingSets).Select(x => (int)x.Id.Value);

                    if (faked = RandomFieldGenerator.RollEventWithProbability(fakedOperationProbability) && workout.WorkingSets.Count > 0)
                    {
                        if (newWorkingSets.Count > 0 && RandomFieldGenerator.RollEventWithProbability(0.5f))
                            // Simulate duplicate elment in input list
                            idToAdd = RandomFieldGenerator.ChooseAmong(newWorkingSets.Select(x => x.Id.Value));
                        else
                            // Simulate element already present
                            idToAdd = (uint)RandomFieldGenerator.ChooseAmong(alreadyChosenIds);

                        newWorkingSets.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(idToAdd, workout.WorkingSets.Count + iws, isTransient, TrainingEffortTypeEnum.RM));
                        break;   // Exit loop so faked won't be overridden
                    }
                    else
                        idToAdd = (uint)RandomFieldGenerator.RandomIntValueExcluded(1, 10000, alreadyChosenIds);
                }
                newWorkingSets.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(idToAdd, workout.WorkingSets.Count + iws, isTransient, TrainingEffortTypeEnum.RM));
            }

            if (faked)
                Assert.Throws<ArgumentException>(() => plan.AddWorkingSets(weekPnum, workoutPnum, newWorkingSets));
            else
            {
                plan.AddWorkingSets(weekPnum, workoutPnum, newWorkingSets);
                workouts[(int)workoutPnum] = workouts.First(x => x.ProgressiveNumber == workoutPnum).AddWorkingSets(newWorkingSets);
                
                week = plan.TrainingWeeks.Single(x => x.ProgressiveNumber == weekPnum);     // Keep Updated

                CheckWeekWorkouts(workouts, week, isTransient);
            }



            // Remove Working sets
            workout = week.CloneWorkout(workoutPnum);
            faked = false;

            if (!isTransient)
            {
                workout = week.CloneWorkout(workoutPnum);    // Refresh it
                uint idToRemove;
                int removeWorkingSetsNumber = RandomFieldGenerator.RandomInt(removeWorkingSetsMin, Math.Min(removeWorkingSetsMax, workout.WorkingSets.Count));

                for (int iws = 0; iws < removeWorkingSetsNumber; iws++)
                {
                    if (faked = RandomFieldGenerator.RollEventWithProbability(fakedOperationProbability))
                    {
                        idToRemove = (uint)RandomFieldGenerator.RandomIntValueExcluded(1, 10000, workout.WorkingSets.Select(x => (int)x.Id.Value));
                        removeWorkingSets.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(idToRemove, workout.WorkingSets.Count, isTransient, TrainingEffortTypeEnum.RM));
                        break;
                    }

                    idToRemove = RandomFieldGenerator.ChooseAmong(
                        workout.WorkingSets.Select(x => x.Id.Value).Except(removeWorkingSets.Select(x => x.Id.Value)).ToList());

                    removeWorkingSets.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(idToRemove, workout.WorkingSets.Count, isTransient, TrainingEffortTypeEnum.RM));
                }
                if (faked)
                    Assert.Throws<ArgumentException>(() => plan.RemoveWorkingSets(weekPnum, workoutPnum, removeWorkingSets));
                else
                {
                    plan.RemoveWorkingSets(weekPnum, workoutPnum, removeWorkingSets);
                    CheckWorkingSetSequence(workout.WorkingSets.Except(removeWorkingSets), plan.CloneWorkout(weekPnum, workoutPnum).WorkingSets, isTransient);
                }
            }

            // Move Workouts
            uint destPnum = RandomFieldGenerator.ChooseAmong(week.Workouts.Select(x => x.ProgressiveNumber).ToList());
            uint srcPnum = workoutPnum;

            workout = week.CloneWorkout(srcPnum);    // Refresh it
            WorkoutTemplateReferenceValue destWorkout = week.Workouts.ToList()[(int)destPnum];

            week.MoveWorkoutToNewProgressiveNumber(srcPnum, destPnum);

            CheckWorkingSetSequence(workout.WorkingSets, week.Workouts.ToList()[(int)destPnum].WorkingSets, isTransient);
            CheckWorkingSetSequence(destWorkout.WorkingSets, week.Workouts.ToList()[(int)srcPnum].WorkingSets, isTransient);

            Assert.True(Enumerable.Range(0, workouts.Count()).ToList().SequenceEqual
                (week.Workouts.Select(x => (int)x.ProgressiveNumber)));
        }


        internal static void CheckWeekWorkoutChanges(TrainingWeekEntity week, bool isTransient)
        {
            bool faked = false;

            int addWorkingSetsMin = 1, addWorkingSetsMax = 2;
            int removeWorkingSetsMin = 1, removeWorkingSetsMax = 2;

            float fakedOperationProbability = 0.05f;

            IEnumerable<WorkoutTemplateReferenceValue> workouts = new List<WorkoutTemplateReferenceValue>(week.Workouts);
            uint srcPnum = RandomFieldGenerator.ChooseAmong(week.Workouts.Select(x => x.ProgressiveNumber).ToList());

            ICollection<WorkingSetTemplateEntity> newWorkingSets = new List<WorkingSetTemplateEntity>();
            ICollection<WorkingSetTemplateEntity> removeWorkingSets = new List<WorkingSetTemplateEntity>();

            WorkoutTemplateReferenceValue srcWorkout = week.CloneWorkout(srcPnum);


            // Add Working Sets
            int addWorkingSetsNumber = RandomFieldGenerator.RandomInt(addWorkingSetsMin, addWorkingSetsMax);
            uint idToAdd;

            for (int iws = 0; iws < addWorkingSetsNumber; iws++)
            {
                if (isTransient)
                    idToAdd = 1;
                else
                {
                    IEnumerable<int> alreadyChosenIds = newWorkingSets.Union(srcWorkout.WorkingSets).
                        Where(x => (x?.Id ?? 0) > 0).Select(x => (int)x.Id);

                    if (faked = RandomFieldGenerator.RollEventWithProbability(fakedOperationProbability))
                    {
                        if (newWorkingSets.Count > 0 &&  RandomFieldGenerator.RollEventWithProbability(0.5f))
                            // Simulate duplicate elment in input list
                            idToAdd = (uint)RandomFieldGenerator.ChooseAmong(alreadyChosenIds);
                        else
                            // Simulate element already present
                            idToAdd = (uint)RandomFieldGenerator.ChooseAmong(alreadyChosenIds);

                        if (idToAdd == 0)
                        {
                            faked = false;
                            continue;   // Skip
                        }

                        newWorkingSets.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(idToAdd, srcWorkout.WorkingSets.Count + iws, isTransient, TrainingEffortTypeEnum.RM));
                        break;   // Exit loop so faked won't be overridden
                    }
                    else
                        idToAdd = (uint)RandomFieldGenerator.RandomIntValueExcluded(1, 10000, alreadyChosenIds);
                }

                newWorkingSets.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(idToAdd, srcWorkout.WorkingSets.Count + iws, isTransient, TrainingEffortTypeEnum.RM));
            }

            if (faked)
                Assert.Throws<ArgumentException>(() => week.AddWorkingSets(srcPnum, newWorkingSets));
            else
            {
                week.AddWorkingSets(srcPnum, newWorkingSets);
                CheckWorkingSetSequence(srcWorkout.WorkingSets.Union(newWorkingSets), week.CloneWorkout(srcPnum).WorkingSets, isTransient);
            }


            // Remove Working sets
            faked = false;

            if (!isTransient)
            {
                srcWorkout = week.CloneWorkout(srcPnum);    // Refresh it
                uint idToRemove;
                int removeWorkingSetsNumber = RandomFieldGenerator.RandomInt(removeWorkingSetsMin, Math.Min(removeWorkingSetsMax, srcWorkout.WorkingSets.Count));

                for (int iws = 0; iws < removeWorkingSetsNumber; iws++)
                {
                    if (faked = RandomFieldGenerator.RollEventWithProbability(fakedOperationProbability))
                    {
                        idToRemove = (uint)RandomFieldGenerator.RandomIntValueExcluded(1, 10000, srcWorkout.WorkingSets.Select(x => (int)x.Id));
                        removeWorkingSets.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(idToRemove, srcWorkout.WorkingSets.Count, isTransient, TrainingEffortTypeEnum.RM));
                        break;
                    }

                    idToRemove = RandomFieldGenerator.ChooseAmong(
                        srcWorkout.WorkingSets.Where(x => (x?.Id ?? 0) > 0)
                        .Select(x => x.Id.Value).Except(removeWorkingSets.Select(x => x.Id.Value)).ToList());

                    if(idToRemove == 0)
                    {
                        faked = false;
                        continue;   // Skip
                    }

                    removeWorkingSets.Add(WorkoutTemplateAggregateBuilder.BuildRandomWorkingSetTemplate(idToRemove, srcWorkout.WorkingSets.Count, isTransient, TrainingEffortTypeEnum.RM));
                }
                if (faked)
                    Assert.Throws<ArgumentException>(() => week.RemoveWorkingSets(srcPnum, removeWorkingSets));
                else
                {
                    week.RemoveWorkingSets(srcPnum, removeWorkingSets);
                    CheckWorkingSetSequence(srcWorkout.WorkingSets.Except(removeWorkingSets), week.CloneWorkout(srcPnum).WorkingSets, isTransient);
                }
            }

            // Move Workouts
            uint destPnum = RandomFieldGenerator.ChooseAmong(week.Workouts.Select(x => x.ProgressiveNumber).ToList());

            srcWorkout = week.CloneWorkout(srcPnum);    // Refresh it
            WorkoutTemplateReferenceValue destWorkout = week.Workouts.ToList()[(int)destPnum];

            week.MoveWorkoutToNewProgressiveNumber(srcPnum, destPnum);

            CheckWorkingSetSequence(srcWorkout.WorkingSets, week.Workouts.ToList()[(int)destPnum].WorkingSets, isTransient);
            CheckWorkingSetSequence(destWorkout.WorkingSets, week.Workouts.ToList()[(int)srcPnum].WorkingSets, isTransient);

            Assert.True(Enumerable.Range(0, workouts.Count()).ToList().SequenceEqual
                (week.Workouts.Select(x => (int)x.ProgressiveNumber)));
        }



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


        internal static void CheckWorkingSetSequence(
            IEnumerable<WorkoutTemplateRoot> workouts, TrainingWeekEntity week, bool isTransient)

            => CheckWorkingSetSequence(
                workouts.SelectMany(x => x.CloneAllWorkingSets()),
                week.Workouts.SelectMany(x => x.WorkingSets),
                isTransient);


        internal static void CheckWorkingSetSequence(
            IEnumerable<WorkoutTemplateReferenceValue> workouts, TrainingWeekEntity week, bool isTransient)

            => CheckWorkingSetSequence(
                workouts.SelectMany(x => x.WorkingSets),
                week.Workouts.SelectMany(x => x.WorkingSets),
                isTransient);


        internal static void CheckWeekWorkouts(IEnumerable<WorkoutTemplateRoot> workouts, TrainingWeekEntity week, bool isTransient)
        {
            CheckWorkingSetSequence(workouts, week, isTransient);

            Assert.True(Enumerable.Range(0, workouts.Count()).ToList().SequenceEqual
                (week.Workouts.Select(x => (int)x.ProgressiveNumber)));

            // Global Training Parameters
            WorkoutTemplateAggregateTest.CheckTrainingParameters(
                workouts.SelectMany(x => x.CloneAllWorkingSets()),
                week.TrainingVolume, week.TrainingDensity, week.TrainingIntensity);

            // Single Workout Training Parameters
            foreach (WorkoutTemplateReferenceValue workout in week.Workouts)
            {
                WorkoutTemplateAggregateTest.CheckTrainingParameters(
                    workout.WorkingSets,
                    week.GetWorkoutTrainingVolume(workout.ProgressiveNumber),
                    week.GetWorkoutTrainingDensity(workout.ProgressiveNumber),
                    week.GetWorkoutTrainingIntensity(workout.ProgressiveNumber));
            }
        }


        internal static void CheckWeekWorkouts(IEnumerable<WorkoutTemplateReferenceValue> workouts, TrainingWeekEntity week, bool isTransient)
        {
            CheckWorkingSetSequence(workouts, week, isTransient);

            Assert.True(Enumerable.Range(0, workouts.Count()).ToList().SequenceEqual
                (week.Workouts.Select(x => (int)x.ProgressiveNumber)));

            // Global Training Parameters
            WorkoutTemplateAggregateTest.CheckTrainingParameters(
                workouts.SelectMany(x => x.WorkingSets),
                week.TrainingVolume, week.TrainingDensity, week.TrainingIntensity);

            // Single Workout Training Parameters
            foreach (WorkoutTemplateReferenceValue workout in week.Workouts)
            {
                WorkoutTemplateAggregateTest.CheckTrainingParameters(
                    workout.WorkingSets,
                    week.GetWorkoutTrainingVolume(workout.ProgressiveNumber),
                    week.GetWorkoutTrainingDensity(workout.ProgressiveNumber),
                    week.GetWorkoutTrainingIntensity(workout.ProgressiveNumber));
            }
        }


        internal static TrainingPlanRoot BuildAndCheckRandomTrainingPlan(long planIdNum, bool isTransient, IList<TrainingWeekEntity> weeks = null)
        {
            int planNameLengthMin = 10, planNameLengthMax = 100;
            int ownerIdMin = 50000, ownerIdMax = 55555;
            int idsSizeMin = 0, idsSizeMax = 10;
            int noteIdMin = 7000, noteIdMax = 12999;
            int trainingWeeksMin = 0, trainingWeeksMax = 10;

            //float alternativeConstructorProbabilty = 0.25f;

            string name = RandomFieldGenerator.RandomTextValue(planNameLengthMin, planNameLengthMax);
            bool isBookmarked = RandomFieldGenerator.RandomBoolWithProbability(0.5f);

            uint? planId = (uint?)(planIdNum);
            uint? noteId = (uint?)(RandomFieldGenerator.RandomInt(noteIdMin, noteIdMax));
            uint? ownerId = (uint?)(RandomFieldGenerator.RandomInt(ownerIdMin, ownerIdMax));

            List<uint?> scheduleIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
            List<uint?> phaseIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
            List<uint?> proficiencyIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
            List<uint?> hashtagIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
            List<uint?> focusIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();

            int trainingWeeksNumber = RandomFieldGenerator.RandomInt(trainingWeeksMin, trainingWeeksMax);

            if (weeks == null)
            {
                weeks = new List<TrainingWeekEntity>();

                for (int iweek = 0; iweek < trainingWeeksNumber; iweek++)
                    weeks.Add(WorkoutTemplateAggregateBuilder.BuildRandomTrainingWeek(iweek + 1, iweek, isTransient));
            }


            //if (planType == TrainingPlanTypeEnum.Inherited)
            //{
            //    uint? traineeId = (uint?)(RandomFieldGenerator.RandomInt(1, 10000));
            //    uint? scheduleId = (uint?)(RandomFieldGenerator.RandomInt(1, 10000));

            //    TrainingPlanRoot rootPlan = TrainingPlanRoot.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId,
            //        messageId, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds);

            //    messageId = (uint?)(RandomFieldGenerator.RandomInt(messageIdMin, messageIdMax));

            //    if(RandomFieldGenerator.RollEventWithProbability(alternativeConstructorProbabilty))
            //        plan = TrainingPlanRoot.CreateTrainingPlan(planId, name, false, false, traineeId, planType, null,
            //            messageId, weeks, new List<uint?>() { scheduleId });
            //    else
            //        plan = TrainingPlanRoot.SendInheritedTrainingPlan(planId, rootPlan, traineeId, messageId, scheduleId);

            //    Assert.Equal(planId, plan.Id);
            //    Assert.Equal(name, plan.Name);
            //    Assert.False(plan.IsBookmarked);
            //    Assert.False(plan.IsTemplate);
            //    Assert.Equal(traineeId, plan.OwnerId);
            //    Assert.Equal(planType, plan.TrainingPlanType);
            //    Assert.Null(plan.PersonalNoteId);
            //    Assert.Equal(messageId, plan.AttachedMessageId);
            //    Assert.Single(plan.TrainingScheduleIds);
            //    Assert.Contains(scheduleId, plan.TrainingScheduleIds);
            //    Assert.Empty(plan.TrainingPhaseIds);
            //    Assert.Empty(plan.TrainingProficiencyIds);
            //    Assert.Empty(plan.MuscleFocusIds);
            //    Assert.Empty(plan.HashtagIds);
            //    Assert.Empty(plan.ChildTrainingPlanIds);
            //    Assert.True(Enumerable.Range(0, weeks.Count).SequenceEqual(plan.TrainingWeeks.Select(x => (int)x.ProgressiveNumber)));
            //}


            List<TrainingPlanRelation> childPlansRelations = StaticUtils.BuildTrainingPlanRelations(planId).ToList();

            TrainingPlanRoot plan = TrainingPlanRoot.CreateTrainingPlan(planId, name, isBookmarked, ownerId, noteId,
                weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlansRelations);

            Assert.Equal(planId, plan.Id);
            Assert.Equal(name, plan.Name);
            Assert.Equal(isBookmarked, plan.IsBookmarked);
            Assert.Equal(childPlansRelations.Count(x => x.ChildPlanType == TrainingPlanTypeEnum.Variant) > 0, plan.IsTemplate);
            Assert.Equal(ownerId, plan.OwnerId);
            Assert.Equal(noteId, plan.PersonalNoteId);

            Assert.True(scheduleIds.SequenceEqual(plan.TrainingScheduleIds));
            Assert.True(phaseIds.SequenceEqual(plan.TrainingPhaseIds));
            Assert.True(proficiencyIds.SequenceEqual(plan.TrainingProficiencyIds));
            Assert.True(focusIds.SequenceEqual(plan.MuscleFocusIds));
            Assert.True(hashtagIds.SequenceEqual(plan.HashtagIds));
            Assert.True(childPlansRelations.SequenceEqual(plan.RelationsWithChildPlans));

            Assert.True(Enumerable.Range(0, weeks.Count).SequenceEqual(plan.TrainingWeeks.Select(x => (int)x.ProgressiveNumber)));

            Assert.Equal((float)weeks.Where(x => x?.Workouts != null).DefaultIfEmpty()?.Average(x => x?.Workouts.Count ?? 0)
                , plan.GetAverageWorkoutsPerWeek(), 1);

            Assert.Equal((int)weeks.Where(x => x?.Workouts != null).DefaultIfEmpty()?.Min(x => x?.Workouts.Count ?? 0)
                , plan.GetMinimumWorkoutsPerWeek());

            Assert.Equal((int)weeks.Where(x => x?.Workouts != null).DefaultIfEmpty()?.Max(x => x?.Workouts.Count ?? 0)
                , plan.GetMaximumWorkoutsPerWeek());

            WorkoutTemplateAggregateTest.CheckTrainingParameters(weeks.SelectMany(x => x.CloneAllWorkingSets()),
                plan.TrainingVolume, plan.TrainingDensity, plan.TrainingIntensity);

            CheckTrainingWeeksSequence(weeks, plan.TrainingWeeks, isTransient);

            return plan;
        }


        internal static void CheckTrainingPlanChanges(TrainingPlanRoot plan)
        {
            int planNameLengthMin = 10, planNameLengthMax = 100;
            int ownerIdMin = 50000, ownerIdMax = 55555;
            int idsMin = 1, idsMax = 999999;
            int noteIdMin = 7000, noteIdMax = 12999;
            int toRemove;

            TrainingPlanRoot planCopy = plan.Clone() as TrainingPlanRoot ??
                throw new ArgumentException("Error while cloning the input training plan");

            bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.1f);
            string name = RandomFieldGenerator.RandomTextValue(planNameLengthMin, planNameLengthMax);
            bool isBookmarked = RandomFieldGenerator.RandomBoolWithProbability(0.5f);

            uint? noteId = (uint?)(RandomFieldGenerator.RandomInt(noteIdMin, noteIdMax));
            uint? ownerId = (uint?)(RandomFieldGenerator.RandomInt(ownerIdMin, ownerIdMax));

            uint? scheduleId = (uint?)(RandomFieldGenerator.RandomIntValueExcluded(idsMin, idsMax, plan.TrainingScheduleIds.Select(x => (int)x)));
            uint? phaseId = (uint?)(RandomFieldGenerator.RandomIntValueExcluded(idsMin, idsMax, plan.TrainingPhaseIds.Select(x => (int)x)));
            uint? proficiencyId = (uint?)(RandomFieldGenerator.RandomIntValueExcluded(idsMin, idsMax, plan.TrainingProficiencyIds.Select(x => (int)x)));
            uint? hashtagId = (uint?)(RandomFieldGenerator.RandomIntValueExcluded(idsMin, idsMax, plan.HashtagIds.Select(x => (int)x)));
            uint? focusId = (uint?)(RandomFieldGenerator.RandomIntValueExcluded(idsMin, idsMax, plan.MuscleFocusIds.Select(x => (int)x)));
            uint? childId = (uint?)(RandomFieldGenerator.RandomIntValueExcluded(idsMin, idsMax, 
                plan.RelationsWithChildPlans.Select(x => (int)x.ChildPlanId.Value).Union(new List<int>() { (int)plan.Id.Value })));

            TrainingPlanRelation childPlanRelation = StaticUtils.BuildTrainingPlanRelation(plan.Id, childId);


            // Change simple fields
            plan.GiveName(name);
            plan.WriteNote(noteId);

            Assert.Equal(name, plan.Name);
            Assert.Equal(noteId, plan.PersonalNoteId);

            // Add IDs
            plan.TagAs(hashtagId);
            plan.TagPhase(phaseId);
            plan.LinkTargetProficiency(proficiencyId);
            plan.FocusOnMuscle(focusId);

            Assert.Contains(hashtagId, plan.HashtagIds);
            Assert.Equal(planCopy.HashtagIds.Count + 1, plan.HashtagIds.Count);
            Assert.Contains(phaseId, plan.TrainingPhaseIds);
            Assert.Equal(planCopy.TrainingPhaseIds.Count + 1, plan.TrainingPhaseIds.Count);
            Assert.Contains(proficiencyId, plan.TrainingProficiencyIds);
            Assert.Equal(planCopy.TrainingProficiencyIds.Count + 1, plan.TrainingProficiencyIds.Count);
            Assert.Contains(focusId, plan.MuscleFocusIds);
            Assert.Equal(planCopy.MuscleFocusIds.Count + 1, plan.MuscleFocusIds.Count);

            // Add with logic
            plan.ChangeBookmarkedFlag(isBookmarked);
            plan.AttachChildPlan(childId, childPlanRelation.ChildPlanType, childPlanRelation.MessageId);
            plan.ScheduleTraining(scheduleId);

            Assert.Equal(isBookmarked, plan.IsBookmarked);
            Assert.Equal(plan.RelationsWithChildPlans.Count(x => x.ChildPlanType == TrainingPlanTypeEnum.Variant) > 0, plan.IsTemplate);
            Assert.Contains(childPlanRelation, plan.RelationsWithChildPlans);
            Assert.Equal(planCopy.RelationsWithChildPlans.Count() + 1, plan.RelationsWithChildPlans.Count());
            Assert.Contains(scheduleId, plan.TrainingScheduleIds);
            Assert.Equal(planCopy.TrainingScheduleIds.Count + 1, plan.TrainingScheduleIds.Count);

            // Update copy
            planCopy = plan.Clone() as TrainingPlanRoot ??
                throw new ArgumentException("Error while cloning the input training plan");


            // Duplicate Add -> No change
            if (plan.HashtagIds.Count > 0)
            {
                plan.TagAs(RandomFieldGenerator.ChooseAmong(plan.HashtagIds));
                Assert.Equal(planCopy.HashtagIds.Count, plan.HashtagIds.Count);
            }
            if (plan.TrainingPhaseIds.Count > 0)
            {
                plan.TagPhase(RandomFieldGenerator.ChooseAmong(plan.TrainingPhaseIds));
                Assert.Equal(planCopy.TrainingPhaseIds.Count, plan.TrainingPhaseIds.Count);
            }
            if (plan.TrainingProficiencyIds.Count > 0)
            {
                plan.LinkTargetProficiency(RandomFieldGenerator.ChooseAmong(plan.TrainingProficiencyIds));
                Assert.Equal(planCopy.TrainingProficiencyIds.Count, plan.TrainingProficiencyIds.Count);
            }
            if (plan.MuscleFocusIds.Count > 0)
            {
                plan.FocusOnMuscle(RandomFieldGenerator.ChooseAmong(plan.MuscleFocusIds));
                Assert.Equal(planCopy.MuscleFocusIds.Count, plan.MuscleFocusIds.Count);
            }
            if (plan.RelationsWithChildPlans.Count > 0)
            {
                TrainingPlanTypeEnum relationType = TrainingPlanTypeEnum.From(RandomFieldGenerator.RandomInt(1, 2));

                plan.AttachChildPlan(RandomFieldGenerator.ChooseAmong(plan.RelationsWithChildPlans.Select(x => x.ChildPlanId)),  relationType);
                Assert.Equal(planCopy.RelationsWithChildPlans.Count, plan.RelationsWithChildPlans.Count);
            }
            if (plan.TrainingScheduleIds.Count > 0)
            {
                plan.ScheduleTraining(RandomFieldGenerator.ChooseAmong(plan.TrainingScheduleIds));
                Assert.Equal(planCopy.TrainingScheduleIds.Count, plan.TrainingScheduleIds.Count);
            }


            // Remove
            plan.CleanNote();
            Assert.Null(plan.PersonalNoteId);

            // Remove Hashtags
            toRemove = RandomFieldGenerator.RandomInt(1, plan.HashtagIds.Count);

            for (int i = 0; i < toRemove; i++)
            {
                uint? idToRemove = RandomFieldGenerator.ChooseAmong(plan.HashtagIds);
                plan.Untag(idToRemove);

                Assert.Equal(planCopy.HashtagIds.Count - i - 1, plan.HashtagIds.Count);
                Assert.DoesNotContain(idToRemove, plan.HashtagIds);
            }

            // Remove Phases
            toRemove = RandomFieldGenerator.RandomInt(1, plan.TrainingPhaseIds.Count);

            for (int i = 0; i < toRemove; i++)
            {
                uint? idToRemove = RandomFieldGenerator.ChooseAmong(plan.TrainingPhaseIds);
                plan.UntagPhase(idToRemove);

                Assert.Equal(planCopy.TrainingPhaseIds.Count - i - 1, plan.TrainingPhaseIds.Count);
                Assert.DoesNotContain(idToRemove, plan.TrainingPhaseIds);
            }

            // Remove Proficiencies
            toRemove = RandomFieldGenerator.RandomInt(1, plan.TrainingProficiencyIds.Count);

            for (int i = 0; i < toRemove; i++)
            {
                uint? idToRemove = RandomFieldGenerator.ChooseAmong(plan.TrainingProficiencyIds);
                plan.UnlinkTargetProficiency(idToRemove);

                Assert.Equal(planCopy.TrainingProficiencyIds.Count - i - 1, plan.TrainingProficiencyIds.Count);
                Assert.DoesNotContain(idToRemove, plan.TrainingProficiencyIds);
            }

            // Remove Focus
            toRemove = RandomFieldGenerator.RandomInt(1, plan.MuscleFocusIds.Count);

            for (int i = 0; i < toRemove; i++)
            {
                uint? idToRemove = RandomFieldGenerator.ChooseAmong(plan.MuscleFocusIds);
                plan.UnfocusMuscle(idToRemove);

                Assert.Equal(planCopy.MuscleFocusIds.Count - i - 1, plan.MuscleFocusIds.Count);
                Assert.DoesNotContain(idToRemove, plan.MuscleFocusIds);
            }

            // Remove Child Plan
            toRemove = RandomFieldGenerator.RandomInt(1, plan.RelationsWithChildPlans.Count);

            for (int i = 0; i < toRemove; i++)
            {
                uint? idToRemove = RandomFieldGenerator.ChooseAmong(plan.RelationsWithChildPlans.Select(x => x.ChildPlanId));
                plan.DetachChildPlan(idToRemove);

                Assert.Equal(planCopy.RelationsWithChildPlans.Count - i - 1, plan.RelationsWithChildPlans.Count);
                Assert.DoesNotContain(idToRemove, plan.RelationsWithChildPlans.Select(x => x.ChildPlanId));
            }

            if (plan.RelationsWithChildPlans.Count == 0)
                Assert.False(plan.IsTemplate);
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

            CheckWeekWorkouts(left.Workouts, right, isTransient);
        }
        #endregion



    }
}
