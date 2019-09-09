//using GymProject.Domain.Base;
//using GymProject.Domain.SharedKernel;
//using GymProject.Domain.SharedKernel.Exceptions;
//using GymProject.Domain.Test.Util;
//using GymProject.Domain.TrainingDomain.Common;
//using GymProject.Domain.TrainingDomain.Exceptions;
//using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
//using GymProject.Domain.Utils.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Xunit;

//namespace GymProject.Domain.Test.UnitTest
//{
//    public class TrainingScheduleAggregateTest
//    {



//        public const int ntests = 500;





//        [Fact]
//        public static void TrainingScheduleFail()
//        {
//            float isTransientProbability = 0.1f;

//            for (int itest = 0; itest < ntests; itest++)
//            {
//                bool isTransient = RandomFieldGenerator.RollEventWithProbability(isTransientProbability);

//                CheckTrainingFeedbackFail(isTransient);
//                CheckTrainingScheduleFail(isTransient);

//                TrainingScheduleRoot schedule = StaticUtils.BuildRandomSchedule(1, isTransient, false);

//                // Add Feedback
//                Assert.Throws<ArgumentNullException>(() => schedule.ProvideFeedback(null));

//                if (schedule.Feedbacks.Count > 0)
//                {
//                    TrainingScheduleFeedbackEntity firstElement = schedule.Feedbacks.ToList()[0];

//                    if (!isTransient)
//                    {
//                        // Double id
//                        Assert.Throws<ArgumentException>(() => schedule.ProvideFeedback(firstElement));

//                        // Double user
//                        Assert.Throws<TrainingDomainInvariantViolationException>(() => schedule.ProvideFeedback(
//                            TrainingScheduleFeedbackEntity.ProvideFeedback(
//                                IdTypeValue.Create(RandomFieldGenerator.RandomIntValueExcluded(1, 100000, (int)firstElement.Id.Id)),
//                                firstElement.UserId, RatingValue.Rate(3), null)));
//                    }
//                }

//                // Remove Feedback
//                TrainingScheduleFeedbackEntity toRemove = null;
//                IdTypeValue idToRemove = null;
//                IdTypeValue fakeUserId = IdTypeValue.Create(RandomFieldGenerator.RandomIntValueExcluded(1, 10000, schedule.Feedbacks.Select(x => (int)x.UserId.Id)));
//                long fakeFeedbackId = isTransient
//                    ? 1
//                    : RandomFieldGenerator.RandomIntValueExcluded(1, 10000, schedule.Feedbacks.Select(x => (int)x.Id.Id));

//                // Remove NULLs
//                Assert.Throws<ArgumentNullException>(() => schedule.RemoveFeedback(toRemove));
//                Assert.Throws<ArgumentNullException>(() => schedule.RemoveFeedback(idToRemove));

//                // Remove not present
//                if (!isTransient)
//                {
//                    toRemove = StaticUtils.BuildRandomFeedback(
//                        RandomFieldGenerator.RandomIntValueExcluded(1, 10000, schedule.Feedbacks.Select(x => (int)x.Id.Id)), isTransient, fakeUserId);

//                    Assert.Throws<InvalidOperationException>(() => schedule.RemoveFeedback(toRemove));

//                    idToRemove = IdTypeValue.Create(fakeFeedbackId);

//                    Assert.Throws<InvalidOperationException>(() => schedule.RemoveFeedback(idToRemove));
//                }

//                // Change Feedback
//                TrainingScheduleFeedbackEntity temp = StaticUtils.BuildRandomFeedback(fakeFeedbackId, isTransient, fakeUserId);

//                Assert.Throws<ArgumentNullException>(() => schedule.ChangeFeedback(null, temp.Rating, temp.Comment.Body));
//                Assert.Throws<InvalidOperationException>(() => schedule.ChangeFeedback(fakeUserId, temp.Rating, temp.Comment.Body));

//                Assert.Throws<InvalidOperationException>(() => schedule.ChangeFeedback(temp));
//            }
//        }



//        [Fact]
//        public static void TrainingScheduleFullTest()
//        {
//            int addFeedbacksMin = 1, addFeedbacksMax = 3;
//            int changeFeedbacksMin = 1, changeFeedbacksMax = 3;

//            float isTransientProbability = 0.1f;

//            List<TrainingScheduleFeedbackEntity> feedbacks;
//            List<IdTypeValue> feedbacksIds;
//            List<IdTypeValue> feedbacksUserIds;

//            for (int itest = 0; itest < ntests; itest++)
//            {
//                bool isTransient = RandomFieldGenerator.RollEventWithProbability(isTransientProbability);

//                TrainingScheduleRoot schedule = StaticUtils.BuildRandomSchedule(1, isTransient, true);

//                feedbacksIds = schedule.Feedbacks.Select(x => x?.Id).ToList();
//                feedbacksUserIds = schedule.Feedbacks.Select(x => x?.UserId).ToList();

//                // Change Schedule
//                int daysTimelapse = 100;
//                DateTime newStartDate = RandomFieldGenerator.RandomDate(daysTimelapse);
//                DateTime newEndDate = newStartDate.AddDays(RandomFieldGenerator.RandomInt(30, daysTimelapse));

//                if (newStartDate > schedule.ScheduledPeriod.End)
//                    Assert.Throws<ValueObjectInvariantViolationException>(() => schedule.Reschedule(newStartDate));
//                else
//                {
//                    schedule.Reschedule(newStartDate);
//                    Assert.Equal(newStartDate, schedule.ScheduledPeriod.Start);
//                }
                    
//                schedule.Complete(newEndDate);
//                Assert.Equal(newEndDate, schedule.ScheduledPeriod.End);

//                if(newEndDate < DateTime.Now)
//                    Assert.True(schedule.IsCompleted());
//                else
//                    Assert.False(schedule.IsCompleted());

//                // Add Feebacks
//                feedbacks = new List<TrainingScheduleFeedbackEntity>(schedule.Feedbacks);

//                int addFeedbacksNumber = RandomFieldGenerator.RandomInt(addFeedbacksMin, addFeedbacksMax);

//                for (int ifeed = 0; ifeed < addFeedbacksNumber; ifeed++)
//                {
//                    TrainingScheduleFeedbackEntity toAdd = StaticUtils.BuildRandomFeedback(
//                        RandomFieldGenerator.RandomIntValueExcluded(1, 999999, 
//                        feedbacksIds.Select(x => (int)(x?.Id ?? 1))), isTransient, userIdsToExclude: feedbacksUserIds);

//                    feedbacksIds.Add(toAdd?.Id);
//                    feedbacksUserIds.Add(toAdd?.UserId);

//                    feedbacks.Add(toAdd);
//                    schedule.ProvideFeedback(toAdd);
//                }
//                CheckFeedbacks(feedbacks, schedule);

//                // Change Feedbacks
//                feedbacks = new List<TrainingScheduleFeedbackEntity>(schedule.Feedbacks);

//                int changeFeedbacksNumber = RandomFieldGenerator.RandomInt(changeFeedbacksMin, changeFeedbacksMax);

//                for (int ifeed = 0; ifeed < changeFeedbacksNumber; ifeed++)
//                {
//                    TrainingScheduleFeedbackEntity toChange;
//                    IdTypeValue userId = RandomFieldGenerator.ChooseAmong(feedbacks.Select(x => x.UserId));
//                    TrainingScheduleFeedbackEntity originalFeedback = schedule.CloneFeedback(userId);


//                    if (isTransient)
//                    {
//                        toChange = StaticUtils.BuildRandomFeedback(1, isTransient, userId);

//                        if (RandomFieldGenerator.RollEventWithProbability(0.5f))
//                            schedule.ChangeFeedback(userId, toChange.Rating, toChange.Comment.Body);
//                        else
//                            schedule.ChangeFeedback(toChange);
//                    }
//                    else
//                    {
//                        toChange = StaticUtils.BuildRandomFeedback(originalFeedback.Id.Id, isTransient, originalFeedback.UserId);

//                        schedule.ChangeFeedback(toChange);
//                    }
//                    CheckFeedback(toChange, schedule.CloneFeedback(userId));
//                }

//                // Remove Feedbacks
//                feedbacks = new List<TrainingScheduleFeedbackEntity>(schedule.Feedbacks);

//                int removeeFeedbacksNumber = RandomFieldGenerator.RandomInt(changeFeedbacksMin, Math.Min(changeFeedbacksMax, feedbacks.Count));

//                for (int ifeed = 0; ifeed < removeeFeedbacksNumber; ifeed++)
//                {
//                    IdTypeValue userId = RandomFieldGenerator.ChooseAmong(feedbacks.Select(x => x.UserId));
//                    TrainingScheduleFeedbackEntity toRemove = schedule.CloneFeedback(userId);

//                    if (RandomFieldGenerator.RollEventWithProbability(0.5f))
//                        schedule.RemoveFeedback(userId);
//                    else
//                        schedule.RemoveFeedback(toRemove);

//                    feedbacks.Remove(feedbacks.Single(x => x.UserId == userId));        // Keep it updated

//                    CheckFeedbacks(feedbacks, schedule);
//                    Assert.DoesNotContain(toRemove, schedule.Feedbacks);
//                }
//            }
//        }






//        #region Support Methods

//        internal static void CheckFeedbacks(IEnumerable<TrainingScheduleFeedbackEntity> left, TrainingScheduleRoot schedule)

//            => CheckFeedbacks(left, schedule.Feedbacks);


//        internal static void CheckFeedbacks(IEnumerable<TrainingScheduleFeedbackEntity> left, IEnumerable<TrainingScheduleFeedbackEntity>right)
//        {
//            Assert.Equal(left.Count(), right.Count());

//            IEnumerator<TrainingScheduleFeedbackEntity> leftEnumerator = left.GetEnumerator();
//            IEnumerator<TrainingScheduleFeedbackEntity> rightEnumerator = right.GetEnumerator();

//            while (leftEnumerator.MoveNext() && rightEnumerator.MoveNext())
//            {
//                CheckFeedback(leftEnumerator.Current, rightEnumerator.Current);
//            }
//        }


//        internal static void CheckFeedback(TrainingScheduleFeedbackEntity left, TrainingScheduleFeedbackEntity right)
//        {
//            Assert.Equal(left.Comment, right.Comment);
//            Assert.Equal(left.Rating, right.Rating);
//            Assert.Equal(left.UserId, right.UserId);
//        }


//        internal static void CheckTrainingFeedbackFail(bool isTransient)
//        {
//            RatingValue rating = RatingValue.Rate(RandomFieldGenerator.RandomFloat(0, 5));
//            PersonalNoteValue commentBody = PersonalNoteValue.Write(RandomFieldGenerator.RandomTextValue(0, PersonalNoteValue.DefaultMaximumLength));
//            IdTypeValue userId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(0, 99999));

//            if (isTransient)
//            {
//                Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingScheduleFeedbackEntity.ProvideTransientFeedback(null, rating, commentBody));
//                Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingScheduleFeedbackEntity.ProvideTransientFeedback(userId, null, commentBody));

//            }
//            else
//            {
//                IdTypeValue feedbackId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(0, 99999));

//                Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingScheduleFeedbackEntity.ProvideFeedback(feedbackId, null, rating, commentBody));
//                Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingScheduleFeedbackEntity.ProvideFeedback(feedbackId, userId, null, commentBody));
//            }
//        }


//        internal static void CheckTrainingScheduleFail(bool isTransient)
//        {
//            bool faked = false;
//            int feedbacksMin = 0, feedbacksMax = 5;

//            List<TrainingScheduleFeedbackEntity> validFeedbacks = new List<TrainingScheduleFeedbackEntity>();
//            List<TrainingScheduleFeedbackEntity> duplicateFeedbacks = new List<TrainingScheduleFeedbackEntity>();
//            List<TrainingScheduleFeedbackEntity> nullFeedbacks = new List<TrainingScheduleFeedbackEntity>();
//            List<TrainingScheduleFeedbackEntity> duplicateOwnerFeedbacks = new List<TrainingScheduleFeedbackEntity>();

//            int feedbacksNumber = RandomFieldGenerator.RandomInt(feedbacksMin, feedbacksMax);

//            // Valid Feedbacks and Feedbacks with NULL values
//            for (int ifeed = 0; ifeed < feedbacksNumber; ifeed++)
//            {
//                if (RandomFieldGenerator.RollEventWithProbability(0.5f))
//                {
//                    nullFeedbacks.Add(null);
//                    faked = true;
//                }
//                else
//                    nullFeedbacks.Add(StaticUtils.BuildRandomFeedback(ifeed + 1, isTransient));

//                validFeedbacks.Add(StaticUtils.BuildRandomFeedback(ifeed + 1, isTransient));
//                duplicateOwnerFeedbacks.Add(StaticUtils.BuildRandomFeedback(ifeed + 1, isTransient));
//                duplicateFeedbacks.Add(StaticUtils.BuildRandomFeedback(ifeed + 1, isTransient));
//            }

//            // Ensure there are always duplicates in this list
//            if (!faked)
//                nullFeedbacks.Add(null);

//            // More Feedbacks with the same owner
//            if (feedbacksNumber > 0)
//                duplicateOwnerFeedbacks.Add(StaticUtils.BuildRandomFeedback(feedbacksNumber + 1, isTransient,
//                    duplicateOwnerFeedbacks[RandomFieldGenerator.RandomInt(0, feedbacksNumber - 1)].UserId));

//            // Feedbacks with Duplicates
//            if (!isTransient)
//            {
//                for (int ifeed = 0; ifeed < feedbacksNumber; ifeed++)
//                {
//                    if (RandomFieldGenerator.RollEventWithProbability(0.5f))
//                    {
//                        if (ifeed > 0)
//                            duplicateFeedbacks.Add(StaticUtils.BuildRandomFeedback(
//                                RandomFieldGenerator.ChooseAmong(duplicateFeedbacks.Select(x => x.Id.Id)), isTransient));
//                        else
//                            duplicateFeedbacks.Add(StaticUtils.BuildRandomFeedback(1, isTransient));
//                    }
//                    else
//                        duplicateFeedbacks.Add(StaticUtils.BuildRandomFeedback(ifeed + 1, isTransient));
//                }
//            }

//            // Create the Schedule
//            IdTypeValue planId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(1, 1000000));
//            DateRangeValue validPeriod = DateRangeValue.RangeBetween(DateTime.Now, DateTime.Now.AddDays(10));
//            DateRangeValue fakePeriod = DateRangeValue.RangeUpTo(DateTime.Now);

//            if (isTransient)
//            {
//                Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingScheduleRoot.ScheduleTrainingPlanTransient(null, validPeriod, validFeedbacks));
//                Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingScheduleRoot.ScheduleTrainingPlanTransient(planId, fakePeriod, validFeedbacks));

//                if (feedbacksNumber == 0)
//                {
//                    nullFeedbacks.Add(null);
//                    Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingScheduleRoot.ScheduleTrainingPlanTransient(planId, fakePeriod, nullFeedbacks));
//                }
//                else
//                    Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingScheduleRoot.ScheduleTrainingPlanTransient(planId, fakePeriod, nullFeedbacks));
//            }
//            else
//            {
//                IdTypeValue schedId = IdTypeValue.Create(RandomFieldGenerator.RandomInt(1, 1000000));

//                Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingScheduleRoot.ScheduleTrainingPlan(schedId, null, validPeriod, validFeedbacks));
//                Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingScheduleRoot.ScheduleTrainingPlan(schedId, planId, fakePeriod, validFeedbacks));
//                Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingScheduleRoot.ScheduleTrainingPlanTransient(planId, validPeriod, nullFeedbacks));

//                if (feedbacksNumber > 0)
//                    Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingScheduleRoot.ScheduleTrainingPlanTransient(planId, validPeriod, duplicateFeedbacks));
//            }
//        }
//        #endregion


//    }
//}