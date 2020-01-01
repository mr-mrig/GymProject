using GymProject.Domain.Base;
using System.Collections.Generic;

namespace GymProject.Domain.TrainingDomain.AthleteAggregate
{
    public class TrainingPlanMuscleFocusRelation : RelationEntity
    {



        /// <summary>
        /// The Training Plan ID
        /// </summary>
        public uint? UserTrainingPlanId { get; private set; } = null;

        /// <summary>
        /// Navigation Property to the Training Plan
        /// </summary>
        public UserTrainingPlanEntity UserTrainingPlan { get; private set; } = null;

        /// <summary>
        /// Navigation Property to the Muscle
        /// </summary>
        public uint? MuscleGroupId { get; private set; } = null;




        #region Ctors

        private TrainingPlanMuscleFocusRelation() { }


        private TrainingPlanMuscleFocusRelation(UserTrainingPlanEntity userTrainingPlan, uint? muscleId)
        {
            UserTrainingPlan = userTrainingPlan;
            MuscleGroupId = muscleId;
            UserTrainingPlanId = userTrainingPlan.Id;
        }

        #endregion


        #region Factories

        /// <summary>
        /// Perform a link between the two entities, by applying a Many-to-Many relation
        /// </summary>
        /// <param name="trainingPlan">The left entity</param>
        /// <param name="muscleId">Tha right entity</param>
        /// <returns>The TrainingPlanMuscleFocus isntance</returns>
        public static TrainingPlanMuscleFocusRelation BuildLink(UserTrainingPlanEntity trainingPlan, uint? muscleId)

            => new TrainingPlanMuscleFocusRelation(trainingPlan, muscleId);


        #endregion


        protected override IEnumerable<object> GetIdentifyingFields()
        {
            yield return MuscleGroupId;
            yield return UserTrainingPlanId;
        }

    }
}
