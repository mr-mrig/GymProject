using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using GymProject.Domain.Utils.Extensions;
using Xunit;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using GymProject.Domain.Test.UnitTest;

namespace GymProject.Domain.Test.Util
{
    internal static class StaticUtils
    {

        /// <summary>
        /// Checks current timestamp is as expected
        /// </summary>
        /// <param name="toBeChecked">The datetime object storing the current timestamp</param>
        internal static void CheckCurrentTimestamp(DateTime toBeChecked)
        {
            Assert.InRange(toBeChecked, DateTime.Now.Subtract(TimeSpan.FromSeconds(1)), DateTime.Now);
        }

        /// <summary>
        /// Checks that two values are inside a specific tolerance.
        /// This should be used to check conversions where rounding can lead to precision issues
        /// </summary>
        /// <param name="srcValue">The original value</param>
        /// <param name="convertedValue">The converted value</param>
        /// <param name="srcUnitMeasId">The Meas Unit ID of the original value</param>
        /// <param name="convertedUnitMeasId">The Meas Unit ID of the converted  value</param>
        /// <param name="tolerance">The tolerance as a [0-1] float - default = 1.5% </param>
        internal static void CheckConversions(float srcValue, float convertedValue, int srcUnitMeasId = -1, int convertedUnitMeasId = -1, float tolerance = 0.02f)
        {
            Assert.InRange(convertedValue, srcValue * (1 - tolerance), srcValue * (1 + tolerance));

            if (srcUnitMeasId > 0 && convertedUnitMeasId > 0)
                Assert.Equal(srcUnitMeasId, convertedUnitMeasId);
        }




        public static IEnumerable<WorkingSetTemplateEntity> ForceConsecutiveProgressiveNumbers(IEnumerable<WorkingSetTemplateEntity> input)
        {
            IEnumerable<WorkingSetTemplateEntity> result = input.OrderBy(x => x.ProgressiveNumber).ToList();

            // Just overwrite all the progressive numbers
            for (int iws = 0; iws < result.Count(); iws++)
            {
                WorkingSetTemplateEntity ws = result.ElementAt(iws);
                ws.MoveToNewProgressiveNumber((uint)iws);
            }
            return result;
        }


        public static IEnumerable<WorkoutTemplateReferenceValue> ForceConsecutiveProgressiveNumbers(IEnumerable<WorkoutTemplateReferenceValue> input)
        {
            IEnumerable<WorkoutTemplateReferenceValue> result = input.OrderBy(x => x.ProgressiveNumber).ToList();

            // Just overwrite all the progressive numbers
            for (int iws = 0; iws < result.Count(); iws++)
            {
                WorkoutTemplateReferenceValue ws = result.ElementAt(iws);
                ws.MoveToNewProgressiveNumber((uint)iws);
            }
            return result;
        }


        public static IEnumerable<TrainingWeekEntity> ForceConsecutiveProgressiveNumbers(IEnumerable<TrainingWeekEntity> input)
        {
            IEnumerable<TrainingWeekEntity> result = input.OrderBy(x => x.ProgressiveNumber).ToList();

            // Just overwrite all the progressive numbers
            for (int iws = 0; iws < result.Count(); iws++)
            {
                TrainingWeekEntity ws = result.ElementAt(iws);
                ws.MoveToNewProgressiveNumber((uint)iws);
            }
            return result;
        }


        /// <summary>
        /// Compute the average effort of the specified working sets
        /// </summary>
        /// <param name="workingSets">The input working sets</param>
        /// <param name="mainEffortType">The main effort type of the WSs as the most used one - optional</param>
        /// <returns>The average effort value</returns>
        internal static TrainingEffortValue ComputeAverageEffort(IEnumerable<IWorkingSet> workingSets, TrainingEffortTypeEnum mainEffortType = null)
        {
            TrainingEffortValue result = null;
            WSRepetitionValue avgReps;

            // Get the main effort type if not specified, as the most used effort among the WSs ones
            if (mainEffortType == null)
            {
                mainEffortType = workingSets?.GroupBy(x => x.Effort.EffortType)?.Select(x
                 => new
                 {
                     Counter = x.Count(),
                     EffortType = x.Key
                 })?.OrderByDescending(x => x.Counter)?.FirstOrDefault()?.EffortType;
            }


            if (mainEffortType == TrainingEffortTypeEnum.RM)
            {
                IEnumerable<IWorkingSet> rpeSets = workingSets.Where(x => x.Effort.IsRPE());

                if (rpeSets.Count() == 0)
                    avgReps = null;
                else
                    avgReps = WSRepetitionValue.TrackRepetitionSerie((uint)rpeSets.Average(x => x.ToRepetitions()));

                result = TrainingEffortValue.AsRM((workingSets.Where(x => x.Effort.IsRM()).Sum(x => x.Effort.Value)
                    + workingSets.Where(x => x.Effort.IsRPE()).Sum(x => x.Effort.ToRm(avgReps).Value)
                    + workingSets.Where(x => x.Effort.IsIntensityPercentage()).Sum(x => x.Effort.ToRm().Value)) / workingSets.Count());
            }

            else if (mainEffortType == TrainingEffortTypeEnum.RPE)
            {
                WSRepetitionValue avgRepsInt;
                WSRepetitionValue avgRepsRM;

                IEnumerable<IWorkingSet> intSets = workingSets.Where(x => x.Effort.IsIntensityPercentage());

                if (intSets.Count() == 0)
                    avgRepsInt = null;
                else
                    avgRepsInt = WSRepetitionValue.TrackRepetitionSerie((uint)intSets.Average(x => x.ToRepetitions()));

                IEnumerable<IWorkingSet> rmSets = workingSets.Where(x => x.Effort.IsRM());

                if (rmSets.Count() == 0)
                    avgRepsRM = null;
                else
                    avgRepsRM = WSRepetitionValue.TrackRepetitionSerie((uint)rmSets.Average(x => x.ToRepetitions()));


                result = TrainingEffortValue.AsRPE((workingSets.Where(x => x.Effort.IsRM()).Sum(x => x.Effort.ToRPE(avgRepsRM).Value)
                    + workingSets.Where(x => x.Effort.IsRPE()).Sum(x => x.Effort.Value)
                    + workingSets.Where(x => x.Effort.IsIntensityPercentage()).Sum(x => x.Effort.ToRPE(avgRepsInt).Value)) / workingSets.Count());
            }

            else if (mainEffortType == TrainingEffortTypeEnum.IntensityPerc)
            {
                IEnumerable<IWorkingSet> rpeSets = workingSets.Where(x => x.Effort.IsRPE());

                if (rpeSets.Count() == 0)
                    avgReps = null;
                else
                    avgReps = WSRepetitionValue.TrackRepetitionSerie((uint)rpeSets.Average(x => x.ToRepetitions()));

                result = TrainingEffortValue.AsRM((workingSets.Where(x => x.Effort.IsRM()).Sum(x => x.Effort.ToIntensityPercentage().Value)
                    + workingSets.Where(x => x.Effort.IsRPE()).Sum(x => x.Effort.ToIntensityPercentage(avgReps).Value)
                    + workingSets.Where(x => x.Effort.IsIntensityPercentage()).Sum(x => x.Effort.Value)) / workingSets.Count());
            }

            return result;
        }



        internal static void InsertRandomNullElements<T>(IList<T> inputList) where T : class
        {
            if (inputList.Count == 0)

                inputList.Add(null);
            else
            {
                // Quick way to provide at least one null element
                int nullIndex1 = RandomFieldGenerator.RandomInt(0, inputList.Count - 1);
                int nullIndex2 = inputList.Count > 1 ?
                    (RandomFieldGenerator.RollEventWithProbability(0.5f)
                    ? RandomFieldGenerator.RandomIntValueExcluded(0, inputList.Count - 1, nullIndex1)
                    : nullIndex1)    // Just one
                    : nullIndex1;

                inputList[nullIndex1] = null;
                inputList[nullIndex2] = null;
            }
        }


        internal static ICollection<IdTypeValue> BuildIdsCollection(int minNumber, int maxNumber)
        {
            int nElments = RandomFieldGenerator.RandomInt(minNumber, maxNumber);
            List<IdTypeValue> result = new List<IdTypeValue>();

            for (int iel = 0; iel < nElments; iel++)
                result.Add(IdTypeValue.Create(iel + 1));

            return result;
        }


        internal static void RemoveFromIdsCollection(ICollection<IdTypeValue> inputIds, int numberOfElemntsToRemove, Action<IdTypeValue> removeFunction)
        {
            for (int i = 0; i < numberOfElemntsToRemove; i++)
            {
                IdTypeValue toRemoveId = RandomFieldGenerator.ChooseAmong(inputIds.ToList());
                removeFunction(toRemoveId);
            }
        }






        internal static TrainingWeekEntity BuildRandomTrainingWeek(long id, int progn, bool isTransient,
            int nWorkoutsMin = 3, int nWorkoutsMax = 9, TrainingWeekTypeEnum weekType = null, float noWorkoutsProb = 0.05f)
        {
            float workoutWithNoWorkingSetsProbability = 0.05f;
            int workingSetsMin = 10, workingSetsMax = 30;

            // Workouts
            List<WorkoutTemplateReferenceValue> workouts = new List<WorkoutTemplateReferenceValue>();

            // Week Type
            if (weekType == null)
            {
                int? weekTypeId = RandomFieldGenerator.RandomIntNullable(0, TrainingWeekTypeEnum.Peak.Id, 0.1f);

                if (weekTypeId == null)
                    weekType = TrainingWeekTypeEnum.Generic;
                else
                    weekType = TrainingWeekTypeEnum.From(weekTypeId.Value);
            }

            // Buil Workouts
            int? nWorkouts = RandomFieldGenerator.RandomIntNullable(nWorkoutsMin, nWorkoutsMax, noWorkoutsProb);

            if (nWorkouts == null || weekType == TrainingWeekTypeEnum.FullRest)
                nWorkouts = 0;

            for (int iwo = 0; iwo < nWorkouts; iwo++)
            {
                //long strongId = ((id - 1) * wuIdOffset + iwu + 1);      // Easiest to read
                long strongId = ((id - 1) * nWorkoutsMax + iwo + 1);      // Smallest possible

                // Build working sets
                int workingSetsNumber = RandomFieldGenerator.RollEventWithProbability(workoutWithNoWorkingSetsProbability)
                    ? 0
                    : RandomFieldGenerator.RandomInt(workingSetsMin, workingSetsMax);

                List<WorkingSetTemplateEntity> workoutSets = new List<WorkingSetTemplateEntity>();

                for (int iws = 0; iws < workingSetsNumber; iws++)
                {
                    TrainingEffortTypeEnum effortType = TrainingEffortTypeEnum.From(RandomFieldGenerator.RandomInt(1, 3));

                    workoutSets.Add(BuildRandomWorkingSet(strongId * iws + 1, iwo, isTransient, effortType));
                }

                //if (workoutSets.ContainsDuplicates())
                //    System.Diagnostics.Debugger.Break();

                workouts.Add(
                    WorkoutTemplateReferenceValue.BuildLinkToWorkout((uint)iwo, workoutSets));
            }

            // Create the Week
            if (isTransient)
                return TrainingWeekEntity.PlanTransientTrainingWeek((uint)progn, workouts, weekType);

            else

                return TrainingWeekEntity.PlanTrainingWeek(IdTypeValue.Create(id), (uint)progn, workouts, weekType);
        }


        internal static WorkoutTemplateRoot BuildRandomWorkout(long id, bool isTransient,
            int nWorkUnitsMin = 3, int nWorkUnitsMax = 7, WeekdayEnum specificDay = null, float emptyWorkoutProb = 0.05f)
        {
            // Work Units
            List<WorkUnitTemplateEntity> workUnits = new List<WorkUnitTemplateEntity>();

            int? nWorkUnits = RandomFieldGenerator.RandomIntNullable(nWorkUnitsMin, nWorkUnitsMax, emptyWorkoutProb);

            if (nWorkUnits == null)
                nWorkUnits = 0;


            for (int iwu = 0; iwu < nWorkUnits; iwu++)
            {
                //long strongId = ((id - 1) * wuIdOffset + iwu + 1);      // Easiest to read
                long strongId = ((id - 1) * nWorkUnitsMax + iwu + 1);      // Smallest possible

                workUnits.Add(BuildRandomWorkUnit(strongId, iwu, isTransient));
            }

            // Specific Day
            if (specificDay == null)
            {
                int? weekDayId = RandomFieldGenerator.RandomIntNullable(WeekdayEnum.Monday.Id, WeekdayEnum.AllTheWeek, 0.1f);

                if (weekDayId == null)
                    specificDay = WeekdayEnum.Generic;
                else
                    specificDay = WeekdayEnum.From(weekDayId.Value);
            }

            if (isTransient)

                return WorkoutTemplateRoot.PlanTransientWorkout(
                    workUnits: workUnits,
                    workoutName: RandomFieldGenerator.RandomTextValue(4, 5, 0.05f),
                    weekday: specificDay
                    );
            else
                return WorkoutTemplateRoot.PlanWorkout(
                    id: IdTypeValue.Create(id),
                    workUnits: workUnits,
                    workoutName: RandomFieldGenerator.RandomTextValue(4, 5, 0.05f),
                    weekday: specificDay
                    );
        }



        internal static WorkUnitTemplateEntity BuildRandomWorkUnit(long id, int progn, bool isTransient,
            int wsNumMin = 2, int wsNumMax = 7, int excerciseIdMin = 1, int excerciseIdMax = 500,
            int ownerNoteIdMin = 10, int ownerNoteIdMax = 500)
        {
            List<WorkingSetTemplateEntity> workingSets = new List<WorkingSetTemplateEntity>();
            TrainingEffortTypeEnum effortType;

            int wuIntTechniquesMin = 0, wuIntTechniquesMax = 3;
            int intTechniqueIdMin = 1, intTechniqueIdMax = 1000;


            //int wsIdOffset = 100;       // Used to ensure no ID collisions among WSs of different WUs

            // Build randomic Effort
            if (id % 2 == 0)
                effortType = TrainingEffortTypeEnum.IntensityPerc;

            else if (id % 3 == 0)
                effortType = TrainingEffortTypeEnum.RM;

            else
                effortType = TrainingEffortTypeEnum.RPE;

            // Build randomic TUT
            List<TUTValue> tuts = new List<TUTValue>()
            {
                TUTValue.PlanTUT("1010"),
                TUTValue.PlanTUT("3030"),
                TUTValue.PlanTUT("5151"),
                TUTValue.PlanTUT("30x0"),
                TUTValue.SetGenericTUT(),
            };

            // Build randomic WU intensity techniques
            int wuIntTechniquesNum = RandomFieldGenerator.RandomInt(wuIntTechniquesMin, wuIntTechniquesMax);
            List<IdTypeValue> wuIntensityTechniques = new List<IdTypeValue>();
            for (int i = 0; i < wuIntTechniquesNum; i++)
                wuIntensityTechniques.Add(IdTypeValue.Create(RandomFieldGenerator.RandomIntValueExcluded(intTechniqueIdMin, intTechniqueIdMax, wuIntensityTechniques.Select(x => (int)x.Id))));

            // Add randomic Working Sets
            int iwsMax = RandomFieldGenerator.RandomInt(wsNumMin, wsNumMax);

            for (int iws = 0; iws < iwsMax; iws++)
            {
                //long strongId = ((id - 1) * wsIdOffset + iws + 1);      // Easiest to read
                long strongId = ((id - 1) * wsNumMax + iws + 1);      // Smallest possible

                workingSets.Add(BuildRandomWorkingSet(strongId, iws, isTransient, effortType, tutToChooseAmong: tuts));
            }

            if (isTransient)

                return WorkUnitTemplateEntity.PlanTransientWorkUnit(
                progressiveNumber: (uint)progn,
                excerciseId: IdTypeValue.Create(RandomFieldGenerator.RandomInt(excerciseIdMin, excerciseIdMax)),
                workingSets: workingSets,
                workUnitIntensityTechniqueIds: wuIntensityTechniques,
                ownerNoteId: IdTypeValue.Create(RandomFieldGenerator.RandomInt(ownerNoteIdMin, ownerNoteIdMax))
            );
            else

                return WorkUnitTemplateEntity.PlanWorkUnit(
                    id: IdTypeValue.Create(id),
                    progressiveNumber: (uint)progn,
                    excerciseId: IdTypeValue.Create(RandomFieldGenerator.RandomInt(excerciseIdMin, excerciseIdMax)),
                    workingSets: workingSets,
                    workUnitIntensityTechniqueIds: wuIntensityTechniques,
                    ownerNoteId: IdTypeValue.Create(RandomFieldGenerator.RandomInt(ownerNoteIdMin, ownerNoteIdMax))
                );
        }


        internal static WorkingSetTemplateEntity BuildRandomWorkingSet(long id, int progn, bool isTransient, TrainingEffortTypeEnum effortType,
            int repsMin = 1, int repsMax = 20, bool repetitionsSerie = true, float amrapProbability = 0.1f, int restMin = 5, int restMax = 500,
            IList<TUTValue> tutToChooseAmong = null, IList<IdTypeValue> techniquesId = null)
        {
            float rpeMin = 1, rpeMax = 11;
            float rmMin = 1, rmMax = 20;
            float intPercMin = 50, intPercMax = 105;

            TrainingEffortValue effort;
            WSRepetitionValue serie;

            switch (effortType)
            {
                case var _ when effortType == TrainingEffortTypeEnum.IntensityPerc:

                    effort = TrainingEffortValue.AsIntensityPerc(RandomFieldGenerator.RandomFloat(intPercMin, intPercMax));
                    break;

                case var _ when effortType == TrainingEffortTypeEnum.RM:

                    effort = TrainingEffortValue.AsRM(RandomFieldGenerator.RandomFloat(rmMin, rmMax));
                    break;

                case var _ when effortType == TrainingEffortTypeEnum.RPE:

                    effort = TrainingEffortValue.AsRPE((float)CommonUtilities.RoundToPointFive(RandomFieldGenerator.RandomFloat(rpeMin, rpeMax)));
                    break;

                default:

                    effort = null;
                    break;
            }

            if (repetitionsSerie)
            {
                if (RandomFieldGenerator.RandomDouble(0, 1) < amrapProbability)
                {
                    serie = WSRepetitionValue.TrackAMRAP();
                    effort = effortType == TrainingEffortTypeEnum.RPE ? TrainingEffortValue.AsRM(10) : effort; // Couldn't resolve this
                    //effort = effortType == TrainingEffortTypeEnum.IntensityPerc ? effort.ToIntensityPercentage(serie) : effort.ToRm(serie);
                }
                else
                    serie = WSRepetitionValue.TrackRepetitionSerie((uint)RandomFieldGenerator.RandomInt(repsMin, repsMax));
            }
            else
                serie = WSRepetitionValue.TrackTimedSerie((uint)RandomFieldGenerator.RandomInt(repsMin, repsMax));

            if (isTransient)

                return WorkingSetTemplateEntity.PlanTransientWorkingSet(
                    (uint)progn,
                    serie,
                    RestPeriodValue.SetRestSeconds((uint)RandomFieldGenerator.RandomInt(restMin, restMax)),
                    effort,
                    tutToChooseAmong == null ? null : tutToChooseAmong[RandomFieldGenerator.RandomInt(0, tutToChooseAmong.Count - 1)],
                    techniquesId
                );
            else

                return WorkingSetTemplateEntity.PlanWorkingSet(
                    IdTypeValue.Create(id),
                    (uint)progn,
                    serie,
                    RestPeriodValue.SetRestSeconds((uint)RandomFieldGenerator.RandomInt(restMin, restMax)),
                    effort,
                    tutToChooseAmong == null ? null : tutToChooseAmong[RandomFieldGenerator.RandomInt(0, tutToChooseAmong.Count - 1)],
                    techniquesId
                );
        }


        internal static TrainingScheduleFeedbackEntity BuildRandomFeedback(long id, bool isTransient, IdTypeValue userId = null,
            RatingValue rating = null, PersonalNoteValue commentBody = null, IEnumerable<IdTypeValue> userIdsToExclude = null)
        {
            // Used to avoid collision -> ensure the Business Rule is always met
            IEnumerable<int> userIdNumbersToExclude = userIdsToExclude?.Select(x => (int)(x?.Id ?? 0)) ?? new List<int>();

            userId = userId ?? IdTypeValue.Create(RandomFieldGenerator.RandomIntValueExcluded(1, 99999, userIdNumbersToExclude));
            rating = rating ?? RatingValue.Rate(RandomFieldGenerator.RandomFloat(0, 5));
            commentBody = commentBody ?? PersonalNoteValue.Write(RandomFieldGenerator.RandomTextValue(0, PersonalNoteValue.DefaultMaximumLength));

            return isTransient 
                ? TrainingScheduleFeedbackEntity.ProvideTransientFeedback(userId, rating, commentBody)
                : TrainingScheduleFeedbackEntity.ProvideFeedback(IdTypeValue.Create(id), userId, rating, commentBody);
        }


        internal static TrainingScheduleRoot BuildRandomSchedule(long id, bool isTransient, bool checkAsserts = true)
        {
            TrainingScheduleRoot result;
            int feedbacksMin = 0, feedbacksMax = 5;

            float rightUnboundedScheduleProbability = 0.5f;

            int feedbacksNumber = RandomFieldGenerator.RandomInt(feedbacksMin, feedbacksMax);

            List<TrainingScheduleFeedbackEntity> initialFeedbacks = new List<TrainingScheduleFeedbackEntity>();

            // Init Feedbacks
            if (RandomFieldGenerator.RollEventWithProbability(0.1f))
                initialFeedbacks = null;
            else
            {
                for (int ifeed = 0; ifeed < feedbacksNumber; ifeed++)
                    initialFeedbacks.Add(BuildRandomFeedback(ifeed + 1, isTransient));
            }

            // Create the Schedule
            IdTypeValue planId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(1, 1000000));

            DateTime startDate = RandomFieldGenerator.RandomDate(1000);

            DateRangeValue period = RandomFieldGenerator.RollEventWithProbability(rightUnboundedScheduleProbability)
                ? DateRangeValue.RangeStartingFrom(startDate)
                : DateRangeValue.RangeBetween(startDate, startDate.AddDays(RandomFieldGenerator.RandomInt(14, 100)));
                       
            if (isTransient)
                result = TrainingScheduleRoot.ScheduleTrainingPlanTransient(planId, period, initialFeedbacks);
            else
                result = TrainingScheduleRoot.ScheduleTrainingPlan(IdTypeValue.Create(id), planId, period, initialFeedbacks);

            initialFeedbacks = initialFeedbacks ?? new List<TrainingScheduleFeedbackEntity>();

            if (checkAsserts)
            {
                Assert.Equal(period, result.ScheduledPeriod);
                Assert.Equal(planId, result.TrainingPlanId);

                if(!isTransient)
                    Assert.Equal(IdTypeValue.Create(id), result.Id);

                TrainingScheduleAggregateTest.CheckFeedbacks(initialFeedbacks, result.Feedbacks);

                TrainingScheduleAggregateTest.CheckFeedbacks(initialFeedbacks, 
                    result.Feedbacks.Select(x => result.CloneFeedback(x.UserId)));
            }


            return result;
        }


    }
}
