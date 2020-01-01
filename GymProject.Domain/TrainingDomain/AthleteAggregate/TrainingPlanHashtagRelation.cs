using GymProject.Domain.Base;
using System.Collections.Generic;

namespace GymProject.Domain.TrainingDomain.AthleteAggregate
{
    public class TrainingPlanHashtagRelation : RelationEntity
    {



        /// <summary>
        /// /// The Training Plan ID
        /// </summary>
        public uint? UserTrainingPlanId { get; private set; } = null;

        /// <summary>
        /// Navigation Property to the Training Plan
        /// </summary>
        public UserTrainingPlanEntity UserTrainingPlan { get; private set; } = null;

        /// <summary>
        /// Navigation Property to the Hashtag
        /// </summary>
        public uint? HashtagId{ get; private set; } = null;

        /// <summary>
        /// Used to give a priority in order to sort the Hashtags when displaying them to the user
        /// </summary>
        public uint ProgressiveNumber { get; internal set; }




        #region Ctors

        private TrainingPlanHashtagRelation() { }


        private TrainingPlanHashtagRelation(UserTrainingPlanEntity userTrainingPlan, uint progressiveNumber, uint? hashtagId)
        {
            UserTrainingPlan = userTrainingPlan;
            HashtagId = hashtagId;
            UserTrainingPlanId = userTrainingPlan.Id;
            ProgressiveNumber = progressiveNumber;
        }

        #endregion


        #region Factories

        /// <summary>
        /// Perform a link between the two entities, by applying a Many-to-Many relation
        /// </summary>
        /// <param name="trainingPlan">The left entity</param>
        /// <param name="hashtagId">The right entity</param>
        /// <param name="progressiveNumber">The progressive number which identifies the display order</param>
        /// <returns>The TrainingPlanHashtag isntance</returns>
        public static TrainingPlanHashtagRelation BuildLink(UserTrainingPlanEntity trainingPlan, uint progressiveNumber, uint? hashtagId)

            => new TrainingPlanHashtagRelation(trainingPlan, progressiveNumber, hashtagId);

        #endregion


        protected override IEnumerable<object> GetIdentifyingFields()
        {
            yield return UserTrainingPlanId;
            yield return HashtagId;
        }

    }
}
