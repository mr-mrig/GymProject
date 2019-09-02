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



        public const int ntests = 100;

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
            IdTypeValue weekId = IdTypeValue.Create(1);

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
                        WorkoutTemplateRoot wo = StaticUtils.BuildRandomWorkout(iwo + 1, isTransient);
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
                                    wo = StaticUtils.BuildRandomWorkout(iwo + 1, isTransient);
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

                                WorkoutTemplateRoot wo = StaticUtils.BuildRandomWorkout(iwo + 1, isTransient);
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
            week = StaticUtils.BuildRandomTrainingWeek(19, 0, true, nWorkoutsMin: 3, nWorkoutsMax: 6, weekType: TrainingWeekTypeEnum.Generic, noWorkoutsProb: 0);
            uint invalidPnum = (uint)week.Workouts.Count() + 1;

            List<WorkingSetTemplateEntity> newWorkingSets = new List<WorkingSetTemplateEntity>()
            {
                StaticUtils.BuildRandomWorkingSet(1, 0, false, TrainingEffortTypeEnum.RM),
                StaticUtils.BuildRandomWorkingSet(1, 1, false, TrainingEffortTypeEnum.RM),
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
                StaticUtils.BuildRandomWorkingSet(1, 0, true, TrainingEffortTypeEnum.RM),
                StaticUtils.BuildRandomWorkingSet(1, 1, true, TrainingEffortTypeEnum.RM),
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
                        week = TrainingWeekEntity.PlanTransientFullRestWeek(weekPnum);
                    else
                        week = TrainingWeekEntity.PlanFullRestWeek(weekId, weekPnum);

                    // Check
                    CheckWeekWorkouts(initialWorkouts, week, isTransient);

                    continue;   // Skip to next test
                }

                for (int iwo = 0; iwo < initialWorkoutsNum; iwo++)
                {
                    WorkoutTemplateRoot wo = StaticUtils.BuildRandomWorkout(iwo + 1, isTransient);
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
                    WorkoutTemplateRoot wo = StaticUtils.BuildRandomWorkout(iwo + 1, isTransient);

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

            IdTypeValue planId = IdTypeValue.Create(17);
            IdTypeValue inheritedPlanScheduleId = null;
            TrainingPlanRoot plan = null;
            int constructorType;

            for (int itest = 0; itest < ntests; itest++)
            {
                List<TrainingWeekEntity> weeks = new List<TrainingWeekEntity>();

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
                TrainingPlanRoot rootPlan = TrainingPlanRoot.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, rootPlanType, noteId, null, weeks,
                                    scheduleIds, phaseIds, proficiencyIds, focusIds, childPlanIds);

                #region Check Creation Fails
                switch (testCaseProbability)
                {
                    // Null Weeks
                    case var _ when testCaseProbability < 0.05f:

                        StaticUtils.InsertRandomNullElements(weeks);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId, null, weeks,
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
                                    => TrainingPlanRoot.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId,
                                    null, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));
                                break;

                            default:

                                inheritedPlanScheduleId = scheduleIds.Count > 0 ? scheduleIds[0] : null;

                                Assert.Throws<TrainingDomainInvariantViolationException>(()
                                    => TrainingPlanRoot.SendInheritedTrainingPlan(planId, rootPlan, ownerId, messageId, inheritedPlanScheduleId));
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
                                    => TrainingPlanRoot.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId,
                                    null, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));
                                break;

                            default:

                                inheritedPlanScheduleId = scheduleIds.Count > 0 ? scheduleIds[0] : null;

                                Assert.Throws<TrainingDomainInvariantViolationException>(()
                                    => TrainingPlanRoot.SendInheritedTrainingPlan(planId, rootPlan, ownerId, messageId, inheritedPlanScheduleId));
                                break;
                        }

                        break;

                    // Null Phases
                    case var _ when testCaseProbability < 0.25f:

                        StaticUtils.InsertRandomNullElements(phaseIds);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId,
                            null, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));
                        break;

                    // Null Proficiencies
                    case var _ when testCaseProbability < 0.35f:

                        StaticUtils.InsertRandomNullElements(proficiencyIds);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId,
                            null, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));
                        break;

                    // Null Childs
                    case var _ when testCaseProbability < 0.45f:

                        StaticUtils.InsertRandomNullElements(childPlanIds);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId, null, weeks,
                            scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));

                        break;

                    // Null focus
                    case var _ when testCaseProbability < 0.55f:

                        StaticUtils.InsertRandomNullElements(focusIds);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId, null, weeks,
                            scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));

                        break;

                    // Non inherited with message attached
                    case var _ when testCaseProbability < 0.65f:

                        planType = RandomFieldGenerator.RollEventWithProbability(0.5f)
                            ? TrainingPlanTypeEnum.NotSet : TrainingPlanTypeEnum.Variant;

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId, messageId,
                            weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));

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
                            => TrainingPlanRoot.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId, null,
                            weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds));

                        break;

                    // Child Plan same as Root Plan
                    default:

                        // Violate rule for Template plans
                        childPlanIds.Add(planId);

                        Assert.Throws<TrainingDomainInvariantViolationException>(()
                            => TrainingPlanRoot.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId, null,
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

                Assert.Throws<ArgumentNullException>(() => rootPlan.PlanFullRestWeek(null));
                Assert.Throws<ArgumentException>(() => rootPlan.PlanFullRestWeek(
                    StaticUtils.BuildRandomTrainingWeek(1, 0, isTransient, weekType: TrainingWeekTypeEnum.Generic)));

                Assert.Throws<ArgumentNullException>(() => rootPlan.PlanTrainingWeek(null));

                Assert.Throws<ArgumentException>(() => rootPlan.PlanTransientTrainingWeek(TrainingWeekTypeEnum.FullRest
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

                TrainingPlanRoot plan = BuildAndCheckRandomTrainingPlan(planId, isTransient, planType);

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
                        week = StaticUtils.BuildRandomTrainingWeek(
                            RandomFieldGenerator.ChooseAmong(plan.TrainingWeeks.Select(x => (int)x.Id.Id)), 
                            srcWeeksNumber + weeksAddedCount, isTransient, weekType: weekType);
                    else
                    {
                        // Valid insertion
                        week = StaticUtils.BuildRandomTrainingWeek(srcWeeksNumber + iweek + 1, srcWeeksNumber + weeksAddedCount++, isTransient, weekType: weekType);
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
                        WorkoutTemplateRoot workout = StaticUtils.BuildRandomWorkout(1, isTransient);
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
            int idToAdd;
           
            WorkoutTemplateReferenceValue workout = week.CloneWorkout(workoutPnum);

            for (int iws = 0; iws < addWorkingSetsNumber; iws++)
            {
                if (isTransient)
                    idToAdd = 1;
                else
                {
                    IEnumerable<int> alreadyChosenIds = newWorkingSets.Union(workout.WorkingSets).Select(x => (int)x.Id.Id);

                    if (faked = RandomFieldGenerator.RollEventWithProbability(fakedOperationProbability) && workout.WorkingSets.Count > 0)
                    {
                        if (newWorkingSets.Count > 0 && RandomFieldGenerator.RollEventWithProbability(0.5f))
                            // Simulate duplicate elment in input list
                            idToAdd = RandomFieldGenerator.ChooseAmong(newWorkingSets.Select(x => (int)x.Id.Id));
                        else
                            // Simulate element already present
                            idToAdd = RandomFieldGenerator.ChooseAmong(alreadyChosenIds);

                        newWorkingSets.Add(StaticUtils.BuildRandomWorkingSet(idToAdd, workout.WorkingSets.Count + iws, isTransient, TrainingEffortTypeEnum.RM));
                        break;   // Exit loop so faked won't be overridden
                    }
                    else
                        idToAdd = RandomFieldGenerator.RandomIntValueExcluded(1, 10000, alreadyChosenIds);
                }

                newWorkingSets.Add(StaticUtils.BuildRandomWorkingSet(idToAdd, workout.WorkingSets.Count + iws, isTransient, TrainingEffortTypeEnum.RM));
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
                long idToRemove;
                int removeWorkingSetsNumber = RandomFieldGenerator.RandomInt(removeWorkingSetsMin, Math.Min(removeWorkingSetsMax, workout.WorkingSets.Count));

                for (int iws = 0; iws < removeWorkingSetsNumber; iws++)
                {
                    if (faked = RandomFieldGenerator.RollEventWithProbability(fakedOperationProbability))
                    {
                        idToRemove = RandomFieldGenerator.RandomIntValueExcluded(1, 10000, workout.WorkingSets.Select(x => (int)x.Id.Id));
                        removeWorkingSets.Add(StaticUtils.BuildRandomWorkingSet(idToRemove, workout.WorkingSets.Count, isTransient, TrainingEffortTypeEnum.RM));
                        break;
                    }

                    idToRemove = RandomFieldGenerator.ChooseAmong(
                        workout.WorkingSets.Select(x => x.Id.Id).Except(removeWorkingSets.Select(x => x.Id.Id)).ToList());

                    removeWorkingSets.Add(StaticUtils.BuildRandomWorkingSet(idToRemove, workout.WorkingSets.Count, isTransient, TrainingEffortTypeEnum.RM));
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
            int idToAdd;

            for (int iws = 0; iws < addWorkingSetsNumber; iws++)
            {
                if (isTransient)
                    idToAdd = 1;
                else
                {
                    IEnumerable<int> alreadyChosenIds = newWorkingSets.Union(srcWorkout.WorkingSets).
                        Where(x => (x?.Id?.Id ?? 0) > 0).Select(x => (int)x.Id.Id);

                    if (faked = RandomFieldGenerator.RollEventWithProbability(fakedOperationProbability))
                    {
                        if (newWorkingSets.Count > 0 &&  RandomFieldGenerator.RollEventWithProbability(0.5f))
                            // Simulate duplicate elment in input list
                            idToAdd = RandomFieldGenerator.ChooseAmong(alreadyChosenIds);
                        else
                            // Simulate element already present
                            idToAdd = RandomFieldGenerator.ChooseAmong(alreadyChosenIds);

                        if (idToAdd == 0)
                        {
                            faked = false;
                            continue;   // Skip
                        }

                        newWorkingSets.Add(StaticUtils.BuildRandomWorkingSet(idToAdd, srcWorkout.WorkingSets.Count + iws, isTransient, TrainingEffortTypeEnum.RM));
                        break;   // Exit loop so faked won't be overridden
                    }
                    else
                        idToAdd = RandomFieldGenerator.RandomIntValueExcluded(1, 10000, alreadyChosenIds);
                }

                newWorkingSets.Add(StaticUtils.BuildRandomWorkingSet(idToAdd, srcWorkout.WorkingSets.Count + iws, isTransient, TrainingEffortTypeEnum.RM));
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
                long idToRemove;
                int removeWorkingSetsNumber = RandomFieldGenerator.RandomInt(removeWorkingSetsMin, Math.Min(removeWorkingSetsMax, srcWorkout.WorkingSets.Count));

                for (int iws = 0; iws < removeWorkingSetsNumber; iws++)
                {
                    if (faked = RandomFieldGenerator.RollEventWithProbability(fakedOperationProbability))
                    {
                        idToRemove = RandomFieldGenerator.RandomIntValueExcluded(1, 10000, srcWorkout.WorkingSets.Select(x => (int)x.Id.Id));
                        removeWorkingSets.Add(StaticUtils.BuildRandomWorkingSet(idToRemove, srcWorkout.WorkingSets.Count, isTransient, TrainingEffortTypeEnum.RM));
                        break;
                    }

                    idToRemove = RandomFieldGenerator.ChooseAmong(
                        srcWorkout.WorkingSets.Where(x => (x?.Id?.Id ?? 0) > 0)
                        .Select(x => x.Id.Id).Except(removeWorkingSets.Select(x => x.Id.Id)).ToList());

                    if(idToRemove == 0)
                    {
                        faked = false;
                        continue;   // Skip
                    }

                    removeWorkingSets.Add(StaticUtils.BuildRandomWorkingSet(idToRemove, srcWorkout.WorkingSets.Count, isTransient, TrainingEffortTypeEnum.RM));
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


        internal static TrainingPlanRoot BuildAndCheckRandomTrainingPlan(
            long planIdNum, bool isTransient, TrainingPlanTypeEnum planType = null, IList<TrainingWeekEntity> weeks = null)
        {
            int planNameLengthMin = 10, planNameLengthMax = 100;
            int ownerIdMin = 50000, ownerIdMax = 55555;
            int idsSizeMin = 0, idsSizeMax = 10;
            int noteIdMin = 7000, noteIdMax = 12999;
            int messageIdMin = 6881, messageIdMax = 22441;
            int trainingWeeksMin = 0, trainingWeeksMax = 10;

            float alternativeConstructorProbabilty = 0.25f;

            IdTypeValue planId = IdTypeValue.Create(planIdNum);
            TrainingPlanRoot plan;

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
                weeks = new List<TrainingWeekEntity>();

                for (int iweek = 0; iweek < trainingWeeksNumber; iweek++)
                    weeks.Add(StaticUtils.BuildRandomTrainingWeek(iweek + 1, iweek, isTransient));
            }

            IdTypeValue messageId = null;


            if (planType == TrainingPlanTypeEnum.Inherited)
            {
                IdTypeValue traineeId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(1, 10000));
                IdTypeValue scheduleId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(1, 10000));

                TrainingPlanRoot rootPlan = TrainingPlanRoot.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId,
                    messageId, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds);

                messageId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(messageIdMin, messageIdMax));

                if(RandomFieldGenerator.RollEventWithProbability(alternativeConstructorProbabilty))
                    plan = TrainingPlanRoot.CreateTrainingPlan(planId, name, false, false, traineeId, planType, null,
                        messageId, weeks, new List<IdTypeValue>() { scheduleId });
                else
                    plan = TrainingPlanRoot.SendInheritedTrainingPlan(planId, rootPlan, traineeId, messageId, scheduleId);

                Assert.Equal(planId, plan.Id);
                Assert.Equal(name, plan.Name);
                Assert.False(plan.IsBookmarked);
                Assert.False(plan.IsTemplate);
                Assert.Equal(traineeId, plan.OwnerId);
                Assert.Equal(planType, plan.TrainingPlanType);
                Assert.Null(plan.PersonalNoteId);
                Assert.Equal(messageId, plan.AttachedMessageId);
                Assert.Single(plan.TrainingScheduleIds);
                Assert.Contains(scheduleId, plan.TrainingScheduleIds);
                Assert.Empty(plan.TrainingPhaseIds);
                Assert.Empty(plan.TrainingProficiencyIds);
                Assert.Empty(plan.MuscleFocusIds);
                Assert.Empty(plan.Hashtags);
                Assert.Empty(plan.ChildTrainingPlanIds);
                Assert.True(Enumerable.Range(0, weeks.Count).SequenceEqual(plan.TrainingWeeks.Select(x => (int)x.ProgressiveNumber)));
            }

            else if(planType == TrainingPlanTypeEnum.Variant)
            {
                TrainingPlanRoot rootPlan = TrainingPlanRoot.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId,
                    messageId, weeks, scheduleIds, phaseIds, proficiencyIds, focusIds, hashtagIds, childPlanIds);

                if (RandomFieldGenerator.RollEventWithProbability(alternativeConstructorProbabilty))
                    plan = TrainingPlanRoot.CreateTrainingPlan(planId, name, false, false, ownerId, planType, null,
                        messageId, weeks, null, phaseIds, proficiencyIds, focusIds, hashtagIds);
                else
                    plan = TrainingPlanRoot.CreateVariantTrainingPlan(planId, rootPlan);

                Assert.Equal(planId, plan.Id);
                Assert.Equal(name, plan.Name);
                Assert.False(plan.IsBookmarked);
                Assert.False(plan.IsTemplate);
                Assert.Equal(ownerId, plan.OwnerId);
                Assert.Equal(planType, plan.TrainingPlanType);
                Assert.Null(plan.PersonalNoteId);
                Assert.Equal(messageId, plan.AttachedMessageId);
                Assert.Empty(plan.TrainingScheduleIds);
                Assert.Equal(phaseIds, plan.TrainingPhaseIds);
                Assert.Equal(proficiencyIds, plan.TrainingProficiencyIds);
                Assert.Equal(focusIds, plan.MuscleFocusIds);
                Assert.Equal(hashtagIds, plan.Hashtags);
                Assert.Empty(plan.ChildTrainingPlanIds);
            }

            else
            {
                plan = TrainingPlanRoot.CreateTrainingPlan(planId, name, isBookmarked, isTemplate, ownerId, planType, noteId,
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
            }

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
            int messageIdMin = 6881, messageIdMax = 22441;
            int toRemove;

            TrainingPlanRoot planCopy = plan.Clone() as TrainingPlanRoot ??
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
            planCopy = plan.Clone() as TrainingPlanRoot ??
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
