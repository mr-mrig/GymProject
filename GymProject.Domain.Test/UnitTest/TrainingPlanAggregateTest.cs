﻿using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
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

                    // Check
                    CheckWeekWorkouts(initialWorkouts, week, isTransient);

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

                workouts = new List<WorkoutTemplate>(initialWorkouts);

                // Add Workouts
                int addWorkoutsNum = RandomFieldGenerator.RandomInt(addWorkoutsMin, addWorkoutsMax);

                for (int iwo = 0; iwo < addWorkoutsNum; iwo++)
                {
                    WorkoutTemplate wo = StaticUtils.BuildRandomWorkout(iwo + 1, isTransient);

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


        [Fact]
        public static void TrainingPlanFail()
        {
            int ntests = 300;
            int planNameLengthMin = 10, planNameLengthMax = 100;
            int ownerIdMin = 50000, ownerIdMax = 55555;
            int idsSizeMin = 0, idsSizeMax = 10;
            int noteIdMin = 7000, noteIdMax = 12999;
            int messageIdMin = 6881, messageIdMax = 22441;
            int trainingWeeksMin = 0, trainingWeeksMax = 10;

            IdTypeValue planId = IdTypeValue.Create(17);
            IdTypeValue inheritedPlanScheduleId = null;
            TrainingPlan plan = null;
            int constructorType;

            for (int itest = 0; itest < ntests; itest++)
            {
                List<TrainingWeekTemplate> weeks = new List<TrainingWeekTemplate>();

                bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.1f);
                string name = RandomFieldGenerator.RandomTextValue(planNameLengthMin, planNameLengthMax);
                bool isBookmarked = RandomFieldGenerator.RandomBoolWithProbability(0.5f);
                bool isTemplate = RandomFieldGenerator.RandomBoolWithProbability(0.5f);

                IdTypeValue noteId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(noteIdMin, noteIdMax));
                IdTypeValue messageId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(messageIdMin, messageIdMax));
                IdTypeValue ownerId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(ownerIdMin, ownerIdMax));

                List<IdTypeValue> scheduleIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<IdTypeValue> phaseIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<IdTypeValue> proficiencyIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<IdTypeValue> hashtagIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<IdTypeValue> focusIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<IdTypeValue> childPlanIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();

                float testCaseProbability = (float)RandomFieldGenerator.RandomDouble(0, 1);
                float constructorTypeProbability = (float)RandomFieldGenerator.RandomDouble(0, 1);


                TrainingPlanTypeEnum planType = RandomFieldGenerator.RollEventWithProbability(0.1f)
                    ? null
                    : TrainingPlanTypeEnum.From(
                        RandomFieldGenerator.RandomInt(TrainingPlanTypeEnum.NotSet.Id, TrainingPlanTypeEnum.Inherited.Id));

                TrainingPlanTypeEnum rootPlanType = planType;

                int trainingWeeksNumber = RandomFieldGenerator.RandomInt(trainingWeeksMin, trainingWeeksMax);

                for (int iweek = 0; iweek < trainingWeeksNumber; iweek++)
                    weeks.Add(StaticUtils.BuildRandomTrainingWeek(iweek + 1, iweek, isTransient));

                // Root plan - just in case it will be needed
                TrainingPlan rootPlan = TrainingPlan.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, rootPlanType, noteId, null, weeks,
                                    scheduleIds, phaseIds, proficiencyIds, focusIds, childPlanIds);

                #region Check Creation Fails
                switch (testCaseProbability)
                {
                    // Null Weeks
                    case var _ when testCaseProbability < 0.05f:

                        StaticUtils.InsertRandomNullElements(weeks);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlan.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId, null, weeks,
                            scheduleIds, phaseIds, proficiencyIds, focusIds, childPlanIds));

                        // Cannot test non-template plans: rootPlan would have raised the esception if it had null weeks
                        break;

                    // Null owner
                    case var _ when testCaseProbability < 0.1f:

                        constructorType = constructorTypeProbability < 0.5f ? 0 : 1;
                        ownerId = null;

                        switch (constructorType)
                        {
                            case 0:

                                Assert.Throws<TrainingDomainInvariantViolationException>(()
                                    => TrainingPlan.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId,
                                    null, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));
                                break;

                            default:

                                inheritedPlanScheduleId = scheduleIds.Count > 0 ? scheduleIds[0] : null;

                                Assert.Throws<TrainingDomainInvariantViolationException>(()
                                    => TrainingPlan.SendInheritedTrainingPlan(planId, rootPlan, ownerId, messageId, inheritedPlanScheduleId));
                                break;
                        }

                        break;

                    // Null Schedules
                    case var _ when testCaseProbability < 0.2f:

                        constructorType = constructorTypeProbability < 0.5f ? 0 : 1;
                        ownerId = null;

                        switch (constructorType)
                        {
                            case 0:

                                Assert.Throws<TrainingDomainInvariantViolationException>(()
                                    => TrainingPlan.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId,
                                    null, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));
                                break;

                            default:

                                inheritedPlanScheduleId = scheduleIds.Count > 0 ? scheduleIds[0] : null;

                                Assert.Throws<TrainingDomainInvariantViolationException>(()
                                    => TrainingPlan.SendInheritedTrainingPlan(planId, rootPlan, ownerId, messageId, inheritedPlanScheduleId));
                                break;
                        }

                        break;

                    // Null Phases
                    case var _ when testCaseProbability < 0.25f:

                        StaticUtils.InsertRandomNullElements(phaseIds);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlan.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId,
                            null, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));
                        break;

                    // Null Proficiencies
                    case var _ when testCaseProbability < 0.35f:

                        StaticUtils.InsertRandomNullElements(proficiencyIds);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlan.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId,
                            null, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));
                        break;

                    // Null Childs
                    case var _ when testCaseProbability < 0.45f:

                        StaticUtils.InsertRandomNullElements(childPlanIds);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlan.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId, null, weeks,
                            scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));

                        break;

                    // Null focus
                    case var _ when testCaseProbability < 0.55f:

                        StaticUtils.InsertRandomNullElements(focusIds);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlan.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId, null, weeks,
                            scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));

                        break;

                    // Non inherited with message attached
                    case var _ when testCaseProbability < 0.65f:

                        planType = RandomFieldGenerator.RollEventWithProbability(0.5f)
                            ? TrainingPlanTypeEnum.NotSet : TrainingPlanTypeEnum.Variant;

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlan.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId, messageId,
                            weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));

                        break;

                    // Non Consecutive Numbers
                    case var _ when testCaseProbability < 0.9f:

                        bool faked = false;
                        weeks = new List<TrainingWeekTemplate>();

                        for (int iweek = 0; iweek < trainingWeeksNumber; iweek++)
                        {
                            if (RandomFieldGenerator.RollEventWithProbability(0.2f))
                            {
                                faked = true;
                                weeks.Add(StaticUtils.BuildRandomTrainingWeek(iweek + 1, (iweek + 1) * trainingWeeksNumber * 3, isTransient));
                            }
                            else
                                weeks.Add(StaticUtils.BuildRandomTrainingWeek(iweek + 1, iweek, isTransient));
                        }

                        if (weeks.Count == 0)
                            weeks.Add(StaticUtils.BuildRandomTrainingWeek(1, 1, isTransient));
                        else if (!faked)
                            weeks.Add(StaticUtils.BuildRandomTrainingWeek(1, trainingWeeksNumber * 3, isTransient));

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlan.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId, null,
                            weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));

                        break;

                    // Child Plan same as Root Plan
                    default:

                        // Violate rule for Template plans
                        childPlanIds.Add(planId);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlan.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId, null,
                            weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));
                        break;
                }
                #endregion


                #region Check Modifications Fails

                Assert.Throws<ArgumentNullException>(() => rootPlan.AddHashtag(null));
                Assert.Throws<ArgumentNullException>(() => rootPlan.LinkTargetPhase(null));
                Assert.Throws<ArgumentNullException>(() => rootPlan.LinkTargetProficiency(null));
                Assert.Throws<ArgumentNullException>(() => rootPlan.GiveFocusToMuscle(null));
                Assert.Throws<ArgumentNullException>(() => rootPlan.ScheduleTraining(null));

                Assert.Throws<ArgumentNullException>(() => rootPlan.RemoveHashtag(null));
                Assert.Throws<ArgumentNullException>(() => rootPlan.UnlinkTargetPhase(null));
                Assert.Throws<ArgumentNullException>(() => rootPlan.UnlinkTargetProficiency(null));
                Assert.Throws<ArgumentNullException>(() => rootPlan.RemoveFocusToMuscle(null));

                Assert.Throws<ArgumentNullException>(() => rootPlan.AddFullRestWeek(null));
                Assert.Throws<ArgumentException>(() => rootPlan.AddFullRestWeek(
                    StaticUtils.BuildRandomTrainingWeek(1, 0, isTransient, weekType: TrainingWeekTypeEnum.Generic)));

                Assert.Throws<ArgumentNullException>(() => rootPlan.AddTrainingWeek(null));

                Assert.Throws<ArgumentException>(() => rootPlan.AddTrainingWeek(TrainingWeekTypeEnum.FullRest
                    , new List<WorkoutTemplateReferenceValue>() { WorkoutTemplateReferenceValue.BuildLinkToWorkout(0, null) }));

                //Assert.Throws<ArgumentException>(() => rootPlan.RemoveHashtag(IdTypeValue.Create(
                //    RandomFieldGenerator.RandomIntValueExcluded(1, 10000, rootPlan.Hashtags.Select(x => (int)x.Id)))));

                //Assert.Throws<ArgumentException>(() => rootPlan.UnlinkTargetPhase(IdTypeValue.Create(
                //    RandomFieldGenerator.RandomIntValueExcluded(1, 10000, rootPlan.TrainingPhaseIds.Select(x => (int)x.Id)))));

                //Assert.Throws<ArgumentException>(() => rootPlan.UnlinkTargetProficiency(IdTypeValue.Create(
                //    RandomFieldGenerator.RandomIntValueExcluded(1, 10000, rootPlan.TrainingProficiencyIds.Select(x => (int)x.Id)))));

                //Assert.Throws<ArgumentException>(() => rootPlan.RemoveFocusToMuscle(IdTypeValue.Create(
                //    RandomFieldGenerator.RandomIntValueExcluded(1, 10000, rootPlan.MuscleFocusIds.Select(x => (int)x.Id)))));

                if(rootPlanType != null && rootPlanType != TrainingPlanTypeEnum.Variant && rootPlanType != TrainingPlanTypeEnum.NotSet)
                    Assert.Throws<InvalidOperationException>(() => rootPlan.MarkAsVariant());
                #endregion
            }
        }


        [Fact]
        public static void TrainingPlanTemplateFullTest()
        {
            long planId = 17;

            for (int itest = 0; itest < ntests; itest++)
            {
                // Build the Plan and check the correctness
                TrainingPlan plan = BuildAndCheckRandomTrainingPlan(planId, TrainingPlanTypeEnum.NotSet);

                // Change Plan
                CheckTrainingPlanChanges(plan);

                // Add Training Week


                // Remove Training Week


                // Change Training Week
            }
        }


        [Fact]
        public static void TrainingPlanInheritedFullTest()
        {
            int planNameLengthMin = 10, planNameLengthMax = 100;
            int ownerIdMin = 50000, ownerIdMax = 55555;
            int idsSizeMin = 0, idsSizeMax = 10;
            int noteIdMin = 7000, noteIdMax = 12999;
            int messageIdMin = 6881, messageIdMax = 22441;
            int trainingWeeksMin = 0, trainingWeeksMax = 10;

            float mainConstructorProb = 0.2f;

            IdTypeValue planId = IdTypeValue.Create(17);
            IdTypeValue inheritedPlanScheduleId = null;
            TrainingPlan plan = null;

            TrainingPlanTypeEnum planType = TrainingPlanTypeEnum.Inherited;

            for (int itest = 0; itest < ntests; itest++)
            {
                List<TrainingWeekTemplate> weeks = new List<TrainingWeekTemplate>();

                bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.1f);
                string name = RandomFieldGenerator.RandomTextValue(planNameLengthMin, planNameLengthMax);
                bool isBookmarked = RandomFieldGenerator.RandomBoolWithProbability(0.5f);
                bool isTemplate = RandomFieldGenerator.RandomBoolWithProbability(0.5f);

                IdTypeValue noteId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(noteIdMin, noteIdMax));
                IdTypeValue messageId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(messageIdMin, messageIdMax));
                IdTypeValue ownerId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(ownerIdMin, ownerIdMax));

                List<IdTypeValue> scheduleIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<IdTypeValue> phaseIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<IdTypeValue> proficiencyIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<IdTypeValue> hashtagIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<IdTypeValue> focusIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<IdTypeValue> childPlanIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();

                int trainingWeeksNumber = RandomFieldGenerator.RandomInt(trainingWeeksMin, trainingWeeksMax);

                for (int iweek = 0; iweek < trainingWeeksNumber; iweek++)
                    weeks.Add(StaticUtils.BuildRandomTrainingWeek(iweek + 1, iweek, isTransient));

                // Root plan - just in case it will be needed
                TrainingPlan rootPlan = TrainingPlan.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId, null, weeks,
                                    scheduleIds, phaseIds, proficiencyIds, focusIds, childPlanIds);

                if (RandomFieldGenerator.RollEventWithProbability(mainConstructorProb))
                    plan = TrainingPlan.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId, null,
                        weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, childPlanIds);
                else
                    plan = null;

                throw new NotImplementedException();
            }
        }


        [Fact]
        public static void TrainingPlanVariantFullTest()
        {
            int planNameLengthMin = 10, planNameLengthMax = 100;
            int ownerIdMin = 50000, ownerIdMax = 55555;
            int idsSizeMin = 0, idsSizeMax = 10;
            int noteIdMin = 7000, noteIdMax = 12999;
            int messageIdMin = 6881, messageIdMax = 22441;
            int trainingWeeksMin = 0, trainingWeeksMax = 10;

            float mainConstructorProb = 0.2f;

            IdTypeValue planId = IdTypeValue.Create(17);
            IdTypeValue inheritedPlanScheduleId = null;
            TrainingPlan plan = null;

            TrainingPlanTypeEnum planType = TrainingPlanTypeEnum.Variant;

            for (int itest = 0; itest < ntests; itest++)
            {
                List<TrainingWeekTemplate> weeks = new List<TrainingWeekTemplate>();

                bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.1f);
                string name = RandomFieldGenerator.RandomTextValue(planNameLengthMin, planNameLengthMax);
                bool isBookmarked = RandomFieldGenerator.RandomBoolWithProbability(0.5f);
                bool isTemplate = RandomFieldGenerator.RandomBoolWithProbability(0.5f);

                IdTypeValue noteId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(noteIdMin, noteIdMax));
                IdTypeValue messageId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(messageIdMin, messageIdMax));
                IdTypeValue ownerId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(ownerIdMin, ownerIdMax));

                List<IdTypeValue> scheduleIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<IdTypeValue> phaseIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<IdTypeValue> proficiencyIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<IdTypeValue> hashtagIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<IdTypeValue> focusIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
                List<IdTypeValue> childPlanIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();

                int trainingWeeksNumber = RandomFieldGenerator.RandomInt(trainingWeeksMin, trainingWeeksMax);

                for (int iweek = 0; iweek < trainingWeeksNumber; iweek++)
                    weeks.Add(StaticUtils.BuildRandomTrainingWeek(iweek + 1, iweek, isTransient));

                // Root plan - just in case it will be needed
                TrainingPlan rootPlan = TrainingPlan.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId, null, weeks,
                                    scheduleIds, phaseIds, proficiencyIds, focusIds, childPlanIds);

                if (RandomFieldGenerator.RollEventWithProbability(mainConstructorProb))
                    plan = TrainingPlan.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId, null,
                        weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, childPlanIds);
                else
                    plan = null;

                throw new NotImplementedException();
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


        internal static void CheckWorkingSetSequence(
            IEnumerable<WorkoutTemplateReferenceValue> workouts, TrainingWeekTemplate week, bool isTransient)

            => CheckWorkingSetSequence(
                workouts.SelectMany(x => x.WorkingSets),
                week.Workouts.SelectMany(x => x.WorkingSets),
                isTransient);


        internal static void CheckWeekWorkouts(IEnumerable<WorkoutTemplate> workouts, TrainingWeekTemplate week, bool isTransient)
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


        internal static void CheckWeekWorkouts(IEnumerable<WorkoutTemplateReferenceValue> workouts, TrainingWeekTemplate week, bool isTransient)
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


        internal static TrainingPlan BuildAndCheckRandomTrainingPlan(
            long planIdNum, TrainingPlanTypeEnum planType = null, IList<TrainingWeekTemplate> weeks = null)
        {
            int planNameLengthMin = 10, planNameLengthMax = 100;
            int ownerIdMin = 50000, ownerIdMax = 55555;
            int idsSizeMin = 0, idsSizeMax = 10;
            int noteIdMin = 7000, noteIdMax = 12999;
            int messageIdMin = 6881, messageIdMax = 22441;
            int trainingWeeksMin = 0, trainingWeeksMax = 10;

            IdTypeValue planId = IdTypeValue.Create(planIdNum);

            bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.1f);
            string name = RandomFieldGenerator.RandomTextValue(planNameLengthMin, planNameLengthMax);
            bool isBookmarked = RandomFieldGenerator.RandomBoolWithProbability(0.5f);
            bool isTemplate = RandomFieldGenerator.RandomBoolWithProbability(0.5f);

            IdTypeValue noteId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(noteIdMin, noteIdMax));
            IdTypeValue ownerId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(ownerIdMin, ownerIdMax));

            List<IdTypeValue> scheduleIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
            List<IdTypeValue> phaseIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
            List<IdTypeValue> proficiencyIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
            List<IdTypeValue> hashtagIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
            List<IdTypeValue> focusIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();
            List<IdTypeValue> childPlanIds = StaticUtils.BuildIdsCollection(idsSizeMin, idsSizeMax).ToList();

            if (planType == null)
            {
                planType = RandomFieldGenerator.RollEventWithProbability(0.1f)
                    ? null
                    : TrainingPlanTypeEnum.From(
                        RandomFieldGenerator.RandomInt(TrainingPlanTypeEnum.NotSet.Id, TrainingPlanTypeEnum.Inherited.Id));
            }

            int trainingWeeksNumber = RandomFieldGenerator.RandomInt(trainingWeeksMin, trainingWeeksMax);

            if (weeks == null)
            {
                weeks = new List<TrainingWeekTemplate>();

                for (int iweek = 0; iweek < trainingWeeksNumber; iweek++)
                    weeks.Add(StaticUtils.BuildRandomTrainingWeek(iweek + 1, iweek, isTransient));
            }

            IdTypeValue messageId = null;

            if (planType == TrainingPlanTypeEnum.Inherited)
                messageId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(messageIdMin, messageIdMax)); ;

            TrainingPlan plan = TrainingPlan.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId,
                messageId, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds);


            Assert.Equal(planId, plan.Id);
            Assert.Equal(name, plan.Name);
            Assert.Equal(isBookmarked, plan.IsBookmarked);
            Assert.Equal(isTemplate, plan.IsTemplate);
            Assert.Equal(ownerId, plan.OwnerId);
            Assert.Equal(planType ?? TrainingPlanTypeEnum.NotSet, plan.TrainingPlanType);
            Assert.Equal(noteId, plan.PersonalNoteId);
            Assert.Equal(messageId, plan.AttachedMessageId);
            Assert.Equal(scheduleIds, plan.TrainingScheduleIds);
            Assert.Equal(phaseIds, plan.TrainingPhaseIds);
            Assert.Equal(proficiencyIds, plan.TrainingProficiencyIds);
            Assert.Equal(focusIds, plan.MuscleFocusIds);
            Assert.Equal(hashtagIds, plan.Hashtags);
            Assert.Equal(childPlanIds, plan.ChildTrainingPlanIds);
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


        internal static void CheckTrainingPlanChanges(TrainingPlan plan)
        {
            int planNameLengthMin = 10, planNameLengthMax = 100;
            int ownerIdMin = 50000, ownerIdMax = 55555;
            int idsMin = 1, idsMax = 999999;
            int noteIdMin = 7000, noteIdMax = 12999;
            int messageIdMin = 6881, messageIdMax = 22441;
            int toRemove;

            TrainingPlan planCopy = plan.Clone() as TrainingPlan ??
                throw new ArgumentException("Error while cloning the input training plan");

            bool isTransient = RandomFieldGenerator.RollEventWithProbability(0.1f);
            string name = RandomFieldGenerator.RandomTextValue(planNameLengthMin, planNameLengthMax);
            bool isBookmarked = RandomFieldGenerator.RandomBoolWithProbability(0.5f);

            IdTypeValue noteId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(noteIdMin, noteIdMax));
            IdTypeValue ownerId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(ownerIdMin, ownerIdMax));

            IdTypeValue scheduleId = IdTypeValue.Create(RandomFieldGenerator.RandomIntValueExcluded(idsMin, idsMax, plan.TrainingScheduleIds.Select(x => (int)x.Id)));
            IdTypeValue phaseId = IdTypeValue.Create(RandomFieldGenerator.RandomIntValueExcluded(idsMin, idsMax, plan.TrainingPhaseIds.Select(x => (int)x.Id)));
            IdTypeValue proficiencyId = IdTypeValue.Create(RandomFieldGenerator.RandomIntValueExcluded(idsMin, idsMax, plan.TrainingProficiencyIds.Select(x => (int)x.Id)));
            IdTypeValue hashtagId = IdTypeValue.Create(RandomFieldGenerator.RandomIntValueExcluded(idsMin, idsMax, plan.Hashtags.Select(x => (int)x.Id)));
            IdTypeValue focusId = IdTypeValue.Create(RandomFieldGenerator.RandomIntValueExcluded(idsMin, idsMax, plan.MuscleFocusIds.Select(x => (int)x.Id)));
            IdTypeValue childPlanId = IdTypeValue.Create(RandomFieldGenerator.RandomIntValueExcluded(idsMin, idsMax,
                plan.ChildTrainingPlanIds.Select(x => (int)x.Id).Union(new List<int>() { (int)plan.Id.Id } )));

            // Change simple fields
            plan.GiveName(name);
            plan.WriteNote(noteId);

            Assert.Equal(name, plan.Name);
            Assert.Equal(noteId, plan.PersonalNoteId);

            // Add IDs
            plan.AddHashtag(hashtagId);
            plan.LinkTargetPhase(phaseId);
            plan.LinkTargetProficiency(proficiencyId);
            plan.GiveFocusToMuscle(focusId);

            Assert.Contains(hashtagId, plan.Hashtags);
            Assert.Equal(planCopy.Hashtags.Count + 1, plan.Hashtags.Count);
            Assert.Contains(phaseId, plan.TrainingPhaseIds);
            Assert.Equal(planCopy.TrainingPhaseIds.Count + 1, plan.TrainingPhaseIds.Count);
            Assert.Contains(proficiencyId, plan.TrainingProficiencyIds);
            Assert.Equal(planCopy.TrainingProficiencyIds.Count + 1, plan.TrainingProficiencyIds.Count);
            Assert.Contains(focusId, plan.MuscleFocusIds);
            Assert.Equal(planCopy.MuscleFocusIds.Count + 1, plan.MuscleFocusIds.Count);

            // Add with logic
            plan.ChangeBookmarkedFlag(isBookmarked);
            plan.AttachChildToTemplatePlan(childPlanId);
            plan.ScheduleTraining(scheduleId);

            Assert.Equal(isBookmarked, plan.IsBookmarked);
            Assert.True(plan.IsTemplate);
            Assert.Contains(childPlanId, plan.ChildTrainingPlanIds);
            Assert.Equal(planCopy.ChildTrainingPlanIds.Count + 1, plan.ChildTrainingPlanIds.Count);
            Assert.Contains(scheduleId, plan.TrainingScheduleIds);
            Assert.Equal(planCopy.TrainingScheduleIds.Count + 1, plan.TrainingScheduleIds.Count);

            // Update copy
            planCopy = plan.Clone() as TrainingPlan ??
                throw new ArgumentException("Error while cloning the input training plan");


            // Duplicate Add -> No change
            if (plan.Hashtags.Count > 0)
            {
                plan.AddHashtag(RandomFieldGenerator.ChooseAmong(plan.Hashtags.ToList()));
                Assert.Equal(planCopy.Hashtags.Count, plan.Hashtags.Count);
            }
            if (plan.TrainingPhaseIds.Count > 0)
            {
                plan.LinkTargetPhase(RandomFieldGenerator.ChooseAmong(plan.TrainingPhaseIds.ToList()));
                Assert.Equal(planCopy.TrainingPhaseIds.Count, plan.TrainingPhaseIds.Count);
            }
            if (plan.TrainingProficiencyIds.Count > 0)
            {
                plan.LinkTargetProficiency(RandomFieldGenerator.ChooseAmong(plan.TrainingProficiencyIds.ToList()));
                Assert.Equal(planCopy.TrainingProficiencyIds.Count, plan.TrainingProficiencyIds.Count);
            }
            if (plan.MuscleFocusIds.Count > 0)
            {
                plan.GiveFocusToMuscle(RandomFieldGenerator.ChooseAmong(plan.MuscleFocusIds.ToList()));
                Assert.Equal(planCopy.MuscleFocusIds.Count, plan.MuscleFocusIds.Count);
            }
            if (plan.ChildTrainingPlanIds.Count > 0)
            {
                plan.AttachChildToTemplatePlan(RandomFieldGenerator.ChooseAmong(plan.ChildTrainingPlanIds.ToList()));
                Assert.Equal(planCopy.ChildTrainingPlanIds.Count, plan.ChildTrainingPlanIds.Count);
            }
            if (plan.TrainingScheduleIds.Count > 0)
            {
                plan.ScheduleTraining(RandomFieldGenerator.ChooseAmong(plan.TrainingScheduleIds.ToList()));
                Assert.Equal(planCopy.TrainingScheduleIds.Count, plan.TrainingScheduleIds.Count);
            }


            // Remove
            plan.CleanNote();
            Assert.Null(plan.PersonalNoteId);

            // Remove Hashtags
            toRemove = RandomFieldGenerator.RandomInt(1, plan.Hashtags.Count);

            for (int i = 0; i < toRemove; i++)
            {
                IdTypeValue idToRemove = RandomFieldGenerator.ChooseAmong(plan.Hashtags.ToList());
                plan.RemoveHashtag(idToRemove);

                Assert.Equal(planCopy.Hashtags.Count - i - 1, plan.Hashtags.Count);
                Assert.DoesNotContain(idToRemove, plan.Hashtags.ToList());
            }
            //StaticUtils.CheckRemoveFromIdsCollection(plan.Hashtags.ToList(), RandomFieldGenerator.RandomInt(1, plan.Hashtags.Count), plan.RemoveHashtag);

            // Remove Phases
            toRemove = RandomFieldGenerator.RandomInt(1, plan.TrainingPhaseIds.Count);

            for (int i = 0; i < toRemove; i++)
            {
                IdTypeValue idToRemove = RandomFieldGenerator.ChooseAmong(plan.TrainingPhaseIds.ToList());
                plan.UnlinkTargetPhase(idToRemove);

                Assert.Equal(planCopy.TrainingPhaseIds.Count - i - 1, plan.TrainingPhaseIds.Count);
                Assert.DoesNotContain(idToRemove, plan.TrainingPhaseIds.ToList());
            }

            // Remove Proficiencies
            toRemove = RandomFieldGenerator.RandomInt(1, plan.TrainingProficiencyIds.Count);

            for (int i = 0; i < toRemove; i++)
            {
                IdTypeValue idToRemove = RandomFieldGenerator.ChooseAmong(plan.TrainingProficiencyIds.ToList());
                plan.UnlinkTargetProficiency(idToRemove);

                Assert.Equal(planCopy.TrainingProficiencyIds.Count - i - 1, plan.TrainingProficiencyIds.Count);
                Assert.DoesNotContain(idToRemove, plan.TrainingProficiencyIds.ToList());
            }

            // Remove Focus
            toRemove = RandomFieldGenerator.RandomInt(1, plan.MuscleFocusIds.Count);

            for (int i = 0; i < toRemove; i++)
            {
                IdTypeValue idToRemove = RandomFieldGenerator.ChooseAmong(plan.MuscleFocusIds.ToList());
                plan.RemoveFocusToMuscle(idToRemove);

                Assert.Equal(planCopy.MuscleFocusIds.Count - i - 1, plan.MuscleFocusIds.Count);
                Assert.DoesNotContain(idToRemove, plan.MuscleFocusIds.ToList());
            }

            // Remove Child Plan
            toRemove = RandomFieldGenerator.RandomInt(1, plan.ChildTrainingPlanIds.Count);

            for (int i = 0; i < toRemove; i++)
            {
                IdTypeValue idToRemove = RandomFieldGenerator.ChooseAmong(plan.ChildTrainingPlanIds.ToList());
                plan.DetachChildToTemplatePlan(idToRemove);

                Assert.Equal(planCopy.ChildTrainingPlanIds.Count - i - 1, plan.ChildTrainingPlanIds.Count);
                Assert.DoesNotContain(idToRemove, plan.ChildTrainingPlanIds.ToList());
            }

            if (plan.ChildTrainingPlanIds.Count == 0)
                Assert.False(plan.IsTemplate);

            if (plan.TrainingPlanType == TrainingPlanTypeEnum.NotSet && plan.TrainingPlanType == TrainingPlanTypeEnum.Variant)
            {
                if(RandomFieldGenerator.RollEventWithProbability(0.5f))
                {
                    plan.MarkAsVariant();
                    Assert.Equal(TrainingPlanTypeEnum.Variant, plan.TrainingPlanType);
                }
            }
        }


        internal static void CheckTrainingWeeksSequence(
            IEnumerable<TrainingWeekTemplate> leftSequence, IEnumerable<TrainingWeekTemplate> rightSequence, bool isTransient)
        {
            IEnumerator<TrainingWeekTemplate> leftEnum = leftSequence.GetEnumerator();
            IEnumerator<TrainingWeekTemplate> rightEnum = rightSequence.GetEnumerator();

            Assert.Equal(leftSequence.Count(), rightSequence.Count());

            while (leftEnum.MoveNext() && rightEnum.MoveNext())
            {
                CheckTrainingWeek(leftEnum.Current, rightEnum.Current, isTransient);
            }
        }


        internal static void CheckTrainingWeek(TrainingWeekTemplate left, TrainingWeekTemplate right, bool isTransient)
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
