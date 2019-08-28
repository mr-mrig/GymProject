using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.Utils;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.TrainingScheduleAggregate
{

    public class TrainingScheduleFeedback : Entity<IdTypeValue>, ICloneable
    {


        /// <summary>
        /// The feedback comment
        /// </summary>
        public string Comment{ get; private set; } = string.Empty;


        /// <summary>
        /// The rating of the schedule
        /// </summary>
        public RatingValue Rating { get; private set; } = null;


        /// <summary>
        /// FK to the author of the Feedback
        /// </summary>
        public IdTypeValue UserId { get; private set; } = null;





        #region Ctors

        private TrainingScheduleFeedback(IdTypeValue id, IdTypeValue authorId, RatingValue rating, string comment) : base(id)
        {
            Comment = comment;
            Rating = rating;
            UserId = authorId;

            TestBusinessRules();
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method - Created a transient object
        /// </summary>
        /// <param name="authorId">The ID of the author of the Feedback</param>
        /// <param name="rating">The rating provided</param>
        /// <param name="comment">The attached comment body</param>
        /// <returns>The TrainingScheduleFeedback instance</returns>
        public static TrainingScheduleFeedback ProvideTransientFeedback(IdTypeValue authorId, RatingValue rating, string comment)

            => ProvideFeedback(null, authorId, rating, comment);

        /// <summary>
        /// Factory method - Loads the object with the specified ID
        /// </summary>
        /// <param name="id">The Training Schedule Feedback ID</param>
        /// <param name="authorId">The ID of the author of the Feedback</param>
        /// <param name="rating">The rating provided</param>
        /// <param name="comment">The attached comment body</param>
        /// <returns>The TrainingScheduleFeedback instance</returns>
        public static TrainingScheduleFeedback ProvideFeedback(IdTypeValue id, IdTypeValue authorId, RatingValue rating, string comment)

            => new TrainingScheduleFeedback(id, authorId, rating, comment);

        #endregion



        #region Public Methods

        /// <summary>
        /// Rate the Training Schedule
        /// </summary>
        /// <param name="rating">The rating value</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void RateTrainingSchedule(RatingValue rating)
        {
            Rating = rating;
            TestBusinessRules();
        }


        /// <summary>
        /// Attach a comment
        /// </summary>
        /// <param name="comment">The body of the comment</param>
        public void WriteComment(string comment)
        {
            Comment = comment;
        }

        #endregion



        #region Business Rules Validation

        /// <summary>
        /// Rating is mandatory when providing a Feedback
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool RatingIsMandatory() => Rating != null;




        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
            if (!RatingIsMandatory())
                throw new TrainingDomainInvariantViolationException($"Rating is mandatory when providing a Feedback.");
        }

        #endregion



        #region IClonable Implementation

        public object Clone()

            => ProvideFeedback(Id, UserId, Rating, Comment);

        #endregion
    }
}


