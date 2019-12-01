using GymProject.Domain.Base;
using System.Collections.Generic;

namespace GymProject.Domain.TrainingDomain.AthleteAggregate
{
    public class TrainingPlanMuscleFocusRelation : ValueObject
    {



        /// <summary>
        /// The Training Plan ID
        /// </summary>
        public uint? TrainingPlanId { get; private set; } = null;

        /// <summary>
        /// Navigation Property to the Training Plan
        /// </summary>
        public UserTrainingPlanEntity TrainingPlan { get; private set; } = null;

        /// <summary>
        /// Navigation Property to the Muscle
        /// </summary>
        public uint? MuscleGroupId { get; private set; } = null;




        #region Ctors

        private TrainingPlanMuscleFocusRelation() { }


        private TrainingPlanMuscleFocusRelation(UserTrainingPlanEntity trainingPlan, uint? muscleId)
        {
            TrainingPlan = trainingPlan;
            MuscleGroupId = muscleId;
            TrainingPlanId = trainingPlan.Id;
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



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TrainingPlan;
            yield return MuscleGroupId;
            //yield return TrainingPlanId;  // Not necessary
        }
    }
}
