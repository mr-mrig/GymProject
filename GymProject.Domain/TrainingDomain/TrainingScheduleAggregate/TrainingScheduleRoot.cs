using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GymProject.Domain.TrainingDomain.TrainingScheduleAggregate
{
    public class TrainingScheduleRoot : Entity<uint?>, IAggregateRoot, ICloneable
    {



        ///// <summary>
        ///// The period which the Training Plan has been scheduled to 
        ///// </summary>
        //public DateRangeValue ScheduledPeriod { get; private set; } = null;


        /// <summary>
        /// The schedule start date
        /// </summary>
        public DateTime StartDate { get; private set; }


        /// <summary>
        /// The schedule end date, might be unbounded
        /// </summary>
        public DateTime? EndDate { get; private set; }


        /// <summary>
        /// FK to the Training Plan which this schedule refers to
        /// </summary>
        public uint? TrainingPlanId { get; private set; } = null;


        /// <summary>
        /// FK to the Athlete which follows this schedule
        /// </summary>
        public uint? AthleteId { get; private set; } = null;



        private List<TrainingScheduleFeedbackEntity> _feedbacks = new List<TrainingScheduleFeedbackEntity>();

        /// <summary>
        /// The Feedbacks of the Training Schedule
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<TrainingScheduleFeedbackEntity> Feedbacks
        {
            get => _feedbacks?.Clone().ToList().AsReadOnly() 
                ?? new List<TrainingScheduleFeedbackEntity>().AsReadOnly();
        }


        #region Ctors

        private TrainingScheduleRoot() : base(null)
        {

        }


        private TrainingScheduleRoot(uint? id, uint? athleteId, uint? trainingPlanId, DateTime startDate, DateTime? endDate, IEnumerable<TrainingScheduleFeedbackEntity> feedbacks) : base(id)
        {
            StartDate = startDate;
            EndDate = endDate;
            AthleteId = athleteId;
            TrainingPlanId = trainingPlanId;

            _feedbacks = feedbacks?.Clone().ToList() ?? new List<TrainingScheduleFeedbackEntity>();

            TestBusinessRules();
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The object ID</param>
        /// <param name="trainingPlanId">The ID of the Training Plan which the schedule refers to</param>
        /// <param name="startDate">The schedule start date</param>
        /// <param name="endDate">The schedule end date, might be null in case of plans with no fixed number of weeks</param>
        /// <param name="feedbacks">The Feedbacks list</param>
        /// <param name="athleteId">The ID of the athlete which is following this schedule</param>
        /// <returns>The TrainingSchedule instance</returns>
        public static TrainingScheduleRoot ScheduleTrainingPlan(uint? id, uint? athleteId, uint? trainingPlanId, DateTime startDate, DateTime? endDate, IEnumerable<TrainingScheduleFeedbackEntity> feedbacks = null)

            => new TrainingScheduleRoot(id, athleteId, trainingPlanId, startDate, endDate, feedbacks);


        /// <summary>
        /// Factory method - Transient object
        /// </summary>
        /// <param name="trainingPlanId">The ID of the Training Plan which the schedule refers to</param>
        /// <param name="startDate">The schedule start date</param>
        /// <param name="endDate">The schedule end date, might be null in case of plans with no fixed number of weeks</param>
        /// <param name="feedbacks">The Feedbacks list</param>
        /// <param name="athleteId">The ID of the athlete which is following this schedule</param>
        /// <returns>The TrainingSchedule instance</returns>
        public static TrainingScheduleRoot ScheduleTrainingPlan(uint? athleteId,uint? trainingPlanId, DateTime startDate, DateTime? endDate, IEnumerable<TrainingScheduleFeedbackEntity> feedbacks = null)

            => ScheduleTrainingPlan(null, athleteId, trainingPlanId, startDate, endDate, feedbacks);

        #endregion



        #region Public Methods

        /// <summary>
        /// Check if the Schedule has been completed or is it still ongoing.
        /// </summary>
        /// <returns>True if the training plan has been completed</returns>
        public bool IsCompleted()

            => EndDate != null && EndDate <= DateTime.Now;


        /// <summary>
        /// Reschedule the Training Plan to the day specified.
        /// This action cannot been performed on a completed schedule: cannot rewrite history.
        /// </summary>
        /// <param name="firstDate">The day the Training Schedule starts</param>
        /// <exception cref="InvalidOperationException">If rescheduling a schedule whch has already been completed</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If edge dates are cronologically invalid</exception>
        public void Reschedule(DateTime firstDate)
        {
            if (IsCompleted())
                throw new InvalidOperationException("Cannot reschedule what has already been completed");

            StartDate = firstDate;
            TestBusinessRules();
        }


        /// <summary>
        /// Mark the Training Schedule as Completed.
        /// </summary>
        /// <param name="lastDay">The the day the Training Schedule ends</param>
        public void Complete(DateTime lastDay)
        {
            EndDate = lastDay;
            TestBusinessRules();
        }

        #endregion


        #region Feebacks Methods

        /// <summary>
        /// Changes a Feedback which has already been provided.
        /// The feedback to be replaced is fetched by the user ID - which must be unique for each Schedule
        /// NOTE: there is no way to change the owner of a feeedbak, hence that field is used only to find the feedback to be replaced.
        /// </summary>
        /// <param name="userId">The ID of the author of the Feedback</param>
        /// <param name="newRating">The Feedback rating</param>
        /// <param name="newComment">The Feedback comment body</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If business rules not met</exception>
        /// <exception cref="ArgumentNullException">If Owner ID is null</exception>
        /// <exception cref="InvalidOperationException">If the Feedback with the specified User couldn't be found</exception>
        public void ChangeFeedback(uint? userId, RatingValue newRating, string newComment)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId), $"Trying to change the Feedback of a NULL User.");

            TrainingScheduleFeedbackEntity feedback = FindUserFeedback(userId);

            feedback?.RateTrainingSchedule(newRating);
            feedback?.WriteComment(newComment);

            TestBusinessRules();
        }


        /// <summary>
        /// Changes a Feedback which has already been provided.
        /// The input feedback replaces the one in the feedback list with the same ID - which must be present in the list.
        /// NOTE: there is no way to change the owner of a feeedbak, hence that field will be ignored.
        /// </summary>
        /// <param name="updatedFeedback">The new feedback</param>
        /// <exception cref="ArgumentNullException">If trying to remove a NULL Feedback</exception>
        /// <exception cref="InvalidOperationException">If zero or more than one Feedback from the same user</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void ChangeFeedback(TrainingScheduleFeedbackEntity updatedFeedback)
        {
            if (updatedFeedback == null)
                throw new ArgumentNullException(nameof(updatedFeedback), $"Trying to change a NULL Feedback.");

            TrainingScheduleFeedbackEntity toChange = FindFeedback(updatedFeedback);

            toChange?.RateTrainingSchedule(updatedFeedback.Rating);
            toChange?.WriteComment(updatedFeedback.Comment.Body);

            TestBusinessRules();
        }


        /// <summary>
        /// Provide a Feedback to the Training Schedule
        /// </summary>
        /// <param name="feedback">The Feedback to be added</param>
        /// <exception cref="ArgumentNullException">If trying to add a NULL Feedback</exception>
        /// <exception cref="ArgumentException">If the input Feedback is already present in the list</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void ProvideFeedback(TrainingScheduleFeedbackEntity feedback)
        {
            if(feedback == null)
                throw new ArgumentNullException(nameof(feedback), $"Trying to add a NULL Feedback.");

            if (_feedbacks.Contains(feedback))
                throw new ArgumentException($"The Training Schedule Feedback (Id = {feedback.ToString()}) is already present in the list", nameof(feedback));

            _feedbacks.Add(feedback.Clone() as TrainingScheduleFeedbackEntity);

            TestBusinessRules();
        }


        ///// <summary>
        ///// Remove the Feedback from the Training Schedule ones
        ///// </summary>
        ///// <param name="feedbackId">The ID of the feedback to be removed</param>
        ///// <exception cref="ArgumentNullException">If trying to remove a NULL Feedback</exception>
        ///// <exception cref="InvalidOperationException">If zero or more than one Feedback from the same user</exception>
        ///// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        //public void RemoveFeedback(uint feedbackId)
        //{
        //    TrainingScheduleFeedbackEntity toRemove = Feedbacks.SingleOrDefault(x => x.Id == feedbackId);

        //    if (toRemove != null && _feedbacks.Remove(toRemove))
        //        TestBusinessRules();
        //}


        /// <summary>
        /// Remove the Feedback from the Training Schedule ones
        /// </summary>
        /// <param name="userId">The ID of the author of the Feedback to be removed</param>
        /// <exception cref="ArgumentNullException">If trying to remove a NULL Feedback</exception>
        /// <exception cref="InvalidOperationException">If the input Feedback is already present in the list</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void RemoveFeedback(uint? userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId), $"Trying to remove a Feedback of a NULL Author.");

            //TrainingScheduleFeedback toRemove = FindUserFeedbackOrDefault(userId) ??
            //    throw new ArgumentException($"The Training Schedule Feedback (UserId = {userId.ToString()}) is not present in the list.", nameof(userId));

            TrainingScheduleFeedbackEntity toRemove = FindUserFeedback(userId);

            if (_feedbacks.Remove(toRemove))
                TestBusinessRules();
        }


        /// <summary>
        /// Get a copy of the Feedback provided by the specified user
        /// </summary>
        /// <param name="authorId">The ID of the author of the Feedback</param>
        /// <exception cref="InvalidOperationException">If zero or more than one Feedback from the same user</exception>
        /// <returns>A copy of the TrainingScheduleFeedback object/returns>
        public TrainingScheduleFeedbackEntity CloneFeedback(uint? authorId)

            => _feedbacks.Single(x => x.UserId == authorId)?.Clone() as TrainingScheduleFeedbackEntity;

        #endregion


        #region Private Methods

        /// <summary>
        /// Find the Feedback provided by the specified user
        /// </summary>
        /// <param name="authorId">The ID of the author of the Feedback</param>
        /// <exception cref="InvalidOperationException">If zero or more than one Feedback from the same user</exception>
        /// <returns>The TrainingScheduleFeedback object/returns>
        private TrainingScheduleFeedbackEntity FindUserFeedback(uint? authorId)

            => _feedbacks.Single(x => x.UserId == authorId);


        /// <summary>
        /// Find the Feedback provided by the specified user, or NULL if not found
        /// </summary>
        /// <param name="authorId">The ID of the author of the Feedback</param>
        /// <returns>The TrainingScheduleFeedback object or NULL/returns>
        private TrainingScheduleFeedbackEntity FindUserFeedbackOrDefault(uint? authorId)

            => _feedbacks.SingleOrDefault(x => x.UserId == authorId);


        /// <summary>
        /// Find the Feedback searching for the ID, if possible, or for the Author ID
        /// </summary>
        /// <param name="feedback">The Feedback to be fetched</param>
        /// <exception cref="InvalidOperationException">If zero or more than one Feedback from the same user</exception>
        /// <returns>The TrainingScheduleFeedback object/returns>
        private TrainingScheduleFeedbackEntity FindFeedback(TrainingScheduleFeedbackEntity feedback)
        {
            if (feedback.IsTransient())

                // Find by Author ID
                return FindUserFeedback(feedback.UserId);

            else
                // Find by ID
                return _feedbacks.Single(x => x == feedback);
        }


        #endregion


        #region Business Rules Validation

        /// <summary>
        /// The Schedule must refer to a non-NULL Training Plan.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NonNullTrainingPlan() => TrainingPlanId != null;


        /// <summary>
        /// The Training Schedule must have no NULL Feedbacks.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NonNullFeedbacks() => _feedbacks.All(x => x != null);


        /// <summary>
        /// The Training Schedule cannot have Feebacks with duplicate IDs.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoDuplicateFeedbacks() => !_feedbacks.ContainsDuplicates();


        /// <summary>
        /// The Training Plan must have a valid start date.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool SchedulingStartDateIsValid() => StartDate != null;


        /// <summary>
        /// Start Date must prcede End Date
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool StartDateMustPrecedeEndDate() => EndDate == null || StartDate <= EndDate;


        /// <summary>
        /// A single User can provide one Training Scheule Feedback only.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool OneUserProvidesOnlyOneFeedback() => _feedbacks.GroupBy(x => x.UserId).All(g => g.Count() == 1);




        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
            //if (!NonNullTrainingPlan())
            //    throw new TrainingDomainInvariantViolationException($"The Schedule must refer to a non-NULL Training Plan.");

            //if (!NonNullFeedbacks())
            //    throw new TrainingDomainInvariantViolationException($"The Training Schedule must have no NULL Feedbacks.");

            if (!NoDuplicateFeedbacks())
                throw new TrainingDomainInvariantViolationException($"The Training Schedule cannot have Feebacks with duplicate IDs.");

            if (!SchedulingStartDateIsValid())
                throw new TrainingDomainInvariantViolationException($"The Training Plan must have a valid start date.");
            
            if (!StartDateMustPrecedeEndDate())
                throw new TrainingDomainInvariantViolationException($"Start Date must prcede End Date.");

            if (!OneUserProvidesOnlyOneFeedback())
                throw new TrainingDomainInvariantViolationException($"A single User can provide one Training Scheule Feedback only.");
        }

        #endregion


        #region IClonable Implementation

        public object Clone()

            => ScheduleTrainingPlan(Id, TrainingPlanId, StartDate, EndDate, _feedbacks);

        #endregion
    }
}
