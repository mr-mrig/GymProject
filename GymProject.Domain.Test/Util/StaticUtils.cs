using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using GymProject.Domain.Test.UnitTest;
using GymProject.Infrastructure.Utils;
using GymProject.Infrastructure.Persistence.EFContext.Model;

namespace GymProject.Domain.Test.Util
{
    internal static class StaticUtils
    {

        /// <summary>
        /// Checks current timestamp is as expected
        /// </summary>
        /// <param name="toBeChecked">The datetime object storing the current timestamp</param>
        internal static void CheckTimestamp(DateTime toBeChecked, bool isUtc = true)
        {
            //Assert.InRange(toBeChecked, DateTime.Now.Subtract(TimeSpan.FromSeconds(1)), DateTime.Now);
            CheckDateTimes(toBeChecked, isUtc ? DateTime.UtcNow : DateTime.Now);
        }


        /// <summary>
        /// Checks the datetimes are eqivalent but for a specific margin
        /// </summary>
        /// <param name="left">The first datetime</param>
        /// <param name="right">The second datetime</param>
        /// <param name="toleranceMilliseconds">The tolerance in milliseconds for the datetimes to be considered equals</param>
        internal static void CheckDateTimes(DateTime? left, DateTime? right, int toleranceMilliseconds = 100)
        {
            if (left.HasValue && right.HasValue)
                Assert.Equal(left.Value, right.Value, new TimeSpan(toleranceMilliseconds * 10));
            else
                Assert.Equal(left, right);
        }
        

        /// <summary>
        /// Checks two DateRangeValues are equals
        /// </summary>
        /// <param name="left">The first DateRangeValue</param>
        /// <param name="right">The second DateRangeValue</param>
        /// <param name="strictlyEqual">Wether to use strictly equality comparator or to left a small precision margin</param>
        internal static void CheckDateRangeValue(DateRangeValue left, DateRangeValue right, bool strictlyEqual = false)
        {
            DateTime leftStart = left.Start.HasValue ? left.Start.Value : DateTime.MinValue;
            DateTime leftEnd = left.End.HasValue ? left.End.Value : DateTime.MinValue;
            DateTime rightStart = right.Start.HasValue ? right.Start.Value : DateTime.MinValue;
            DateTime rightEnd = right.End.HasValue ? right.End.Value : DateTime.MinValue;

            if(strictlyEqual)
            {
                Assert.Equal(leftStart, rightStart);
                Assert.Equal(leftEnd, rightEnd);
            }
            else
            {
                Assert.Equal(leftStart, rightStart, new TimeSpan(TimeSpan.TicksPerSecond / 2));
                Assert.Equal(leftEnd, rightEnd, new TimeSpan(TimeSpan.TicksPerSecond / 2));
            }

        }


        /// <summary>
        /// Checks that the DateRangeVaues in the lists are sequentially equal
        /// </summary>
        /// <param name="left">The first DateRangeValue list</param>
        /// <param name="right">The second DateRangeValue list</param>
        /// <param name="strictlyEqual">Wether to use strictly equality comparator or to left a small precision margin</param>
        internal static void CheckDateRangeValuesInLists(IEnumerable<DateRangeValue> left, IEnumerable<DateRangeValue> right, bool strictlyEqual = false)
        {
            var leftEnumerator = left.GetEnumerator();
            var rightEnumerator = right.GetEnumerator();

            Assert.Equal(left.Count(), right.Count());

            while (leftEnumerator.MoveNext() && rightEnumerator.MoveNext())
            {
                CheckDateRangeValue(leftEnumerator.Current, rightEnumerator.Current, strictlyEqual);
            }
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


        public static IEnumerable<WorkoutTemplateReferenceEntity> ForceConsecutiveProgressiveNumbers(IEnumerable<WorkoutTemplateReferenceEntity> input)
        {
            IEnumerable<WorkoutTemplateReferenceEntity> result = input.OrderBy(x => x.ProgressiveNumber).ToList();

            // Just overwrite all the progressive numbers
            for (int iws = 0; iws < result.Count(); iws++)
            {
                WorkoutTemplateReferenceEntity ws = result.ElementAt(iws);
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
        internal static TrainingEffortValue ComputeAverageEffort(IEnumerable<IFullWorkingSet> workingSets, TrainingEffortTypeEnum mainEffortType = null)
        {
            TrainingEffortValue result = null;
            WSRepetitionsValue avgReps;

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
                IEnumerable<IFullWorkingSet> rpeSets = workingSets.Where(x => x.Effort.IsRPE());

                if (rpeSets.Count() == 0)
                    avgReps = null;
                else
                    avgReps = WSRepetitionsValue.TrackRepetitionSerie((uint)rpeSets.Average(x => x.ToRepetitions()));

                result = TrainingEffortValue.AsRM((workingSets.Where(x => x.Effort.IsRM()).Sum(x => x.Effort.Value)
                    + workingSets.Where(x => x.Effort.IsRPE()).Sum(x => x.Effort.ToRm(avgReps).Value)
                    + workingSets.Where(x => x.Effort.IsIntensityPercentage()).Sum(x => x.Effort.ToRm().Value)) / workingSets.Count());
            }

            else if (mainEffortType == TrainingEffortTypeEnum.RPE)
            {
                WSRepetitionsValue avgRepsInt;
                WSRepetitionsValue avgRepsRM;

                IEnumerable<IFullWorkingSet> intSets = workingSets.Where(x => x.Effort.IsIntensityPercentage());

                if (intSets.Count() == 0)
                    avgRepsInt = null;
                else
                    avgRepsInt = WSRepetitionsValue.TrackRepetitionSerie((uint)intSets.Average(x => x.ToRepetitions()));

                IEnumerable<IFullWorkingSet> rmSets = workingSets.Where(x => x.Effort.IsRM());

                if (rmSets.Count() == 0)
                    avgRepsRM = null;
                else
                    avgRepsRM = WSRepetitionsValue.TrackRepetitionSerie((uint)rmSets.Average(x => x.ToRepetitions()));


                result = TrainingEffortValue.AsRPE((workingSets.Where(x => x.Effort.IsRM()).Sum(x => x.Effort.ToRPE(avgRepsRM).Value)
                    + workingSets.Where(x => x.Effort.IsRPE()).Sum(x => x.Effort.Value)
                    + workingSets.Where(x => x.Effort.IsIntensityPercentage()).Sum(x => x.Effort.ToRPE(avgRepsInt).Value)) / workingSets.Count());
            }

            else if (mainEffortType == TrainingEffortTypeEnum.IntensityPercentage)
            {
                IEnumerable<IFullWorkingSet> rpeSets = workingSets.Where(x => x.Effort.IsRPE());

                if (rpeSets.Count() == 0)
                    avgReps = null;
                else
                    avgReps = WSRepetitionsValue.TrackRepetitionSerie((uint)rpeSets.Average(x => x.ToRepetitions()));

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


        internal static void InsertRandomNullIds(IList<uint?> inputList)
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


        internal static ICollection<uint?> BuildIdsCollection(int minCollectionSize, int maxCollectionSize, int minId = 1, int maxId = 99999)
        {
            int nElments = RandomFieldGenerator.RandomInt(minCollectionSize, maxCollectionSize);
            List<uint?> result = new List<uint?>();

            for (int iel = 0; iel < nElments; iel++)
                //result.Add((uint?)(iel + 1));
                result.Add((uint?)(RandomFieldGenerator.RandomInt(minId, maxId)));

            return result;
        }


        //internal static ICollection<uint?> BuildRandomIdsCollection(int minNumber, int maxNumber)
        //{
        //    int nElments = RandomFieldGenerator.RandomInt(minNumber, maxNumber);
        //    List<uint?> result = new List<uint?>();

        //    for (int iel = 0; iel < nElments; iel++)
        //        result.Add((uint?)(iel + 1));

        //    return result;
        //}


        internal static void RemoveFromIdsCollection(ICollection<uint?> inputIds, int numberOfElemntsToRemove, Action<uint?> removeFunction)
        {
            for (int i = 0; i < numberOfElemntsToRemove; i++)
            {
                uint? toRemoveId = RandomFieldGenerator.ChooseAmong(inputIds.ToList());
                removeFunction(toRemoveId);
            }
        }




        internal static TrainingScheduleFeedbackEntity BuildRandomFeedback(uint id, bool isTransient, uint? userId = null,
            RatingValue rating = null, PersonalNoteValue commentBody = null, IEnumerable<uint?> userIdsToExclude = null)
        {
            // Used to avoid collision -> ensure the Business Rule is always met
            IEnumerable<int> userIdNumbersToExclude = userIdsToExclude?.Select(x => (int)(x ?? 0)) ?? new List<int>();

            userId = userId ?? (uint)(RandomFieldGenerator.RandomIntValueExcluded(1, 99999, userIdNumbersToExclude));
            rating = rating ?? RatingValue.Rate(RandomFieldGenerator.RandomFloat(0, 5));
            commentBody = commentBody ?? PersonalNoteValue.Write(RandomFieldGenerator.RandomTextValue(0, PersonalNoteValue.DefaultMaximumLength));

            return isTransient 
                ? TrainingScheduleFeedbackEntity.ProvideTransientFeedback(userId.Value, rating, commentBody)
                : TrainingScheduleFeedbackEntity.ProvideFeedback(id, userId.Value, rating, commentBody);
        }


        //internal static TrainingScheduleRoot BuildRandomSchedule(uint id, bool isTransient, bool checkAsserts = true)
        //{
        //    TrainingScheduleRoot result;
        //    int feedbacksMin = 0, feedbacksMax = 5;

        //    float rightUnboundedScheduleProbability = 0.5f;

        //    int feedbacksNumber = RandomFieldGenerator.RandomInt(feedbacksMin, feedbacksMax);

        //    List<TrainingScheduleFeedbackEntity> initialFeedbacks = new List<TrainingScheduleFeedbackEntity>();

        //    // Init Feedbacks
        //    if (RandomFieldGenerator.RollEventWithProbability(0.1f))
        //        initialFeedbacks = null;
        //    else
        //    {
        //        for (uint ifeed = 0; ifeed < feedbacksNumber; ifeed++)
        //            initialFeedbacks.Add(BuildRandomFeedback(ifeed + 1, isTransient));
        //    }

        //    // Create the Schedule
        //    uint planId = (uint)(RandomFieldGenerator.RandomInt(1, 1000000));
        //    uint athleteId = (uint)(RandomFieldGenerator.RandomInt(1, 1000000));

        //    DateTime startDate = RandomFieldGenerator.RandomDate(1000);

        //    DateRangeValue period = RandomFieldGenerator.RollEventWithProbability(rightUnboundedScheduleProbability)
        //        ? DateRangeValue.RangeStartingFrom(startDate)
        //        : DateRangeValue.RangeBetween(startDate, startDate.AddDays(RandomFieldGenerator.RandomInt(14, 100)));
                       
        //    if (isTransient)
        //        result = TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, period, initialFeedbacks);
        //    else
        //        result = TrainingScheduleRoot.ScheduleTrainingPlan(id, planId, period, initialFeedbacks);

        //    initialFeedbacks = initialFeedbacks ?? new List<TrainingScheduleFeedbackEntity>();

        //    if (checkAsserts)
        //    {
        //        Assert.Equal(period, result.ScheduledPeriod);
        //        Assert.Equal(planId, result.TrainingPlanId);

        //        if(!isTransient)
        //            Assert.Equal(id, result.Id);

        //        TrainingScheduleAggregateTest.CheckFeedbacks(initialFeedbacks, result.Feedbacks);

        //        TrainingScheduleAggregateTest.CheckFeedbacks(initialFeedbacks, 
        //            result.Feedbacks.Select(x => result.CloneFeedback(x.UserId)));
        //    }


        //    return result;
        //}

    }
}
