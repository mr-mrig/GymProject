using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
using GymProject.Domain.Test.Util;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using GymProject.Domain.Utils.Extensions;
using GymProject.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GymProject.Domain.Test.UnitTest
{
    public class TrainingScheduleAggregateTest
    {



        public const int ntests = 1000;



        [Fact]
        public void ScheduleTrainingPlan_DuplicateFeedbacksOnlyFail()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime? endDate = null;
            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, 1, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, 2, RatingValue.Rate(4), null),
            };

            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate, endDate, feedbakcs));
        }

        [Fact]
        public void ScheduleTrainingPlan_DuplicateFeedbacksFail()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime? endDate = null;
            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, 1, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(2, 1, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(3, 1, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, 2, RatingValue.Rate(4), null),
            };

            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate, endDate, feedbakcs));
        }

        [Fact]
        public void ScheduleTrainingPlan_UserWithMoreThanOneFeedbackFail()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime? endDate = null;
            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, 1, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(2, 4, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(3, 12, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(4, 1, RatingValue.Rate(4), null),
            };

            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate, endDate, feedbakcs));
        }

        [Fact]
        public void ScheduleTrainingPlan_UserWithMoreThanOneFeedbackOnlyFail()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime? endDate = null;
            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, 1, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(4, 1, RatingValue.Rate(4), null),
            };

            Assert.Throws<TrainingDomainInvariantViolationException>(() => TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate, endDate, feedbakcs));
        }

        [Fact]
        public void ScheduleTrainingPlan_StartingFromDateSuccess()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime? endDate = null;
            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, 1, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(2, 2, RatingValue.Rate(4), null),
            };

            TrainingScheduleRoot schedule = TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate, endDate, feedbakcs);

            Assert.Equal(athleteId, schedule.AthleteId);
            Assert.Equal(planId, schedule.TrainingPlanId);
            Assert.Equal(startDate, schedule.StartDate);
            Assert.Equal(endDate, schedule.EndDate);
            Assert.Equal(feedbakcs, schedule.Feedbacks);
            Assert.False(schedule.IsCompleted());
        }

        [Fact]
        public void ScheduleTrainingPlan_PlannedPastPeriodSuccess()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime endDate = new DateTime(2019, 6, 6);
            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, 1, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(2, 2, RatingValue.Rate(4), null),
            };

            TrainingScheduleRoot schedule = TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate, endDate, feedbakcs);

            Assert.Equal(athleteId, schedule.AthleteId);
            Assert.Equal(planId, schedule.TrainingPlanId);
            Assert.Equal(startDate, schedule.StartDate);
            Assert.Equal(endDate, schedule.EndDate);
            Assert.Equal(feedbakcs, schedule.Feedbacks);
            Assert.True(schedule.IsCompleted());
        }

        [Fact]
        public void ScheduleTrainingPlan_PlannedPeriodSuccess()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            DateTime startDate = DateTime.UtcNow;
            DateTime? endDate = DateTime.UtcNow.AddDays(100);
            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, 1, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(2, 2, RatingValue.Rate(4), null),
            };

            TrainingScheduleRoot schedule = TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate, endDate, feedbakcs);

            Assert.Equal(athleteId, schedule.AthleteId);
            Assert.Equal(planId, schedule.TrainingPlanId);
            Assert.Equal(startDate, schedule.StartDate);
            Assert.Equal(endDate, schedule.EndDate);
            Assert.Equal(feedbakcs, schedule.Feedbacks);
            Assert.False(schedule.IsCompleted());
        }

        [Fact]
        public void ScheduleTrainingPlan_Reschedule_StartingFromSuccess()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime? endDate = null;
            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, 1, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(2, 2, RatingValue.Rate(4), null),
            };
            DateTime newDate = DateTime.UtcNow.AddDays(1);

            TrainingScheduleRoot schedule = TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate,endDate, feedbakcs);
            schedule.Reschedule(newDate);

            Assert.Equal(newDate, schedule.StartDate);
            Assert.Null(schedule.EndDate);
        }

        [Fact]
        public void ScheduleTrainingPlan_Reschedule_PlannedCompletedPeriodFail()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime endDate = new DateTime(2019, 6, 6);
            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, 1, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(2, 2, RatingValue.Rate(4), null),
            };
            DateTime newDate = DateTime.UtcNow.AddDays(1);

            TrainingScheduleRoot schedule = TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate, endDate, feedbakcs);
            Assert.Throws<InvalidOperationException>(() => schedule.Reschedule(newDate));
        }

        [Fact]
        public void ScheduleTrainingPlan_Reschedule_InvalidPeriodFail()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime? endDate = DateTime.UtcNow.AddDays(1);
            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, 1, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(2, 2, RatingValue.Rate(4), null),
            };
            DateTime invalidNewStartDate = endDate.Value.AddDays(1);

            TrainingScheduleRoot schedule = TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate, endDate, feedbakcs);
            Assert.Throws<TrainingDomainInvariantViolationException>(() => schedule.Reschedule(invalidNewStartDate));
        }

        [Fact]
        public void ScheduleTrainingPlan_Reschedule_PlannedPeriodSuccess()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            DateTime startDate = DateTime.UtcNow.AddDays(1);
            DateTime endDate = DateTime.UtcNow.AddDays(100);
            DateTime newStartDate = endDate.AddDays(-10);
            
            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, 1, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(2, 2, RatingValue.Rate(4), null),
            };
            TrainingScheduleRoot schedule = TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate, endDate, feedbakcs);
            schedule.Reschedule(newStartDate);

            Assert.Equal(newStartDate, schedule.StartDate);
            Assert.Equal(endDate, schedule.EndDate);
        }

        [Fact]
        public void ScheduleTrainingPlan_ProvideFeedback_UserWithMoreThanOneFeedbackFail()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime endDate = new DateTime(2019, 6, 6);
            TrainingScheduleFeedbackEntity feedback = TrainingScheduleFeedbackEntity.ProvideFeedback(100, 1, RatingValue.Rate(1), null);

            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, 1, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(2, 2, RatingValue.Rate(4), null),
            };
            TrainingScheduleRoot schedule = TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate, endDate, feedbakcs);
            Assert.Throws<TrainingDomainInvariantViolationException>(() => schedule.ProvideFeedback(feedback));
        }

        [Fact]
        public void ScheduleTrainingPlan_ProvideFeedback_Success()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime endDate = new DateTime(2019, 6, 6);
            TrainingScheduleFeedbackEntity feedback = TrainingScheduleFeedbackEntity.ProvideFeedback(100, 100, RatingValue.Rate(1), PersonalNoteValue.Write("note1"));

            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, 1, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(2, 2, RatingValue.Rate(4), null),
            };
            TrainingScheduleRoot schedule = TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate, endDate, feedbakcs);
            schedule.ProvideFeedback(feedback);

            TrainingScheduleFeedbackEntity added = schedule.Feedbacks.Last();
            Assert.Equal(feedback, added);
            Assert.Equal(feedback.Rating, added.Rating);
            Assert.Equal(feedback.Comment, added.Comment);
            Assert.Equal(feedback.UserId, added.UserId);
        }

        [Fact]
        public void ScheduleTrainingPlan_ChangeFeedback_FetchByFeedbackIdSuccess()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            uint oldAuthorId = 1;
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime endDate = new DateTime(2019, 6, 6);
            TrainingScheduleFeedbackEntity feedback = TrainingScheduleFeedbackEntity.ProvideFeedback(1, oldAuthorId + 1, RatingValue.Rate(1), PersonalNoteValue.Write("note1"));

            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, oldAuthorId, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(2, 2, RatingValue.Rate(4), null),
            };
            TrainingScheduleRoot schedule = TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate, endDate, feedbakcs);
            schedule.ChangeFeedback(feedback);

            TrainingScheduleFeedbackEntity modified = schedule.CloneFeedback(oldAuthorId);
            Assert.Equal(feedback.Rating, modified.Rating);
            Assert.Equal(feedback.Comment, modified.Comment);
            Assert.Equal(oldAuthorId, modified.UserId);     // The author has not been overwritten
        }

        [Fact]
        public void ScheduleTrainingPlan_ChangeFeedback_FetchByAuthorSuccess()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            uint authorId = 1;
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime endDate = new DateTime(2019, 6, 6);
            TrainingScheduleFeedbackEntity feedback = TrainingScheduleFeedbackEntity.ProvideFeedback(1, authorId + 1, RatingValue.Rate(1), PersonalNoteValue.Write("note1"));

            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, authorId, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(2, 2, RatingValue.Rate(4), null),
            };
            TrainingScheduleRoot schedule = TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate, endDate, feedbakcs);
            schedule.ChangeFeedback(authorId, feedback.Rating, feedback.Comment.Body);

            TrainingScheduleFeedbackEntity modified = schedule.CloneFeedback(authorId);
            Assert.Equal(feedback.Rating, modified.Rating);
            Assert.Equal(feedback.Comment, modified.Comment);
        }

        [Fact]
        public void ScheduleTrainingPlan_RemoveFeedback_Success()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            uint authorId = 1;
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime endDate = new DateTime(2019, 6, 6);

            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, authorId, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(2, 2, RatingValue.Rate(4), null),
            };
            TrainingScheduleRoot schedule = TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate, endDate, feedbakcs);
            schedule.RemoveFeedback(authorId);

            Assert.Empty(schedule.Feedbacks.Where(x => x.UserId == authorId));
        }

        [Fact]
        public void ScheduleTrainingPlan_RemoveFeedback_FeedbackNotFoundFail()
        {
            uint? athleteId = 1;
            uint? planId = 12;
            uint authorId = 1;
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime endDate = new DateTime(2019, 6, 6);

            List<TrainingScheduleFeedbackEntity> feedbakcs = new List<TrainingScheduleFeedbackEntity>
            {
                TrainingScheduleFeedbackEntity.ProvideFeedback(1, authorId, RatingValue.Rate(4), null),
                TrainingScheduleFeedbackEntity.ProvideFeedback(2, 2, RatingValue.Rate(4), null),
            };
            TrainingScheduleRoot schedule = TrainingScheduleRoot.ScheduleTrainingPlan(athleteId, planId, startDate, endDate, feedbakcs);
            Assert.Throws<InvalidOperationException>(() => schedule.RemoveFeedback(authorId + 1000));

            // Nothing happens
            Assert.Empty(schedule.Feedbacks.Where(x => x.UserId == authorId + 1000));
        }


    }
}