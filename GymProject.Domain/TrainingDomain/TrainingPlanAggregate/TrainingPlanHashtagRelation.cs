using GymProject.Domain.Base;
using System.Collections.Generic;

namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
{
    public class TrainingPlanHashtagRelation : ValueObject
    {



        /// <summary>
        /// /// The Training Plan ID
        /// </summary>
        public uint TrainingPlanId { get; private set; }

        /// <summary>
        /// Navigation Property to the Training Plan
        /// </summary>
        public TrainingPlanRoot TrainingPlan { get; private set; } = null;

        /// <summary>
        /// Navigation Property to the Hashtag
        /// </summary>
        public uint HashtagId{ get; private set; }




        #region Ctors

        private TrainingPlanHashtagRelation() { }


        private TrainingPlanHashtagRelation(TrainingPlanRoot trainingPlan, uint hashtagId)
        {
            TrainingPlan = trainingPlan;
            HashtagId = hashtagId;
            TrainingPlanId = trainingPlan.Id;
        }

        #endregion


        #region Factories

        /// <summary>
        /// Perform a link between the two entities, by applying a Many-to-Many relation
        /// </summary>
        /// <param name="trainingPlan">The left entity</param>
        /// <param name="hashtagId">Tha right entity</param>
        /// <returns>The TrainingPlanHashtag isntance</returns>
        public static TrainingPlanHashtagRelation BuildLink(TrainingPlanRoot trainingPlan, uint hashtagId)

            => new TrainingPlanHashtagRelation(trainingPlan, hashtagId);

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TrainingPlan; 
            yield return HashtagId; 
            //yield return TrainingPlanId;  // Not necessary
        }
    }
}
