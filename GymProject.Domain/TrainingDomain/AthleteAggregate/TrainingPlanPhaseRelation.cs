using GymProject.Domain.Base;
using System.Collections.Generic;

namespace GymProject.Domain.TrainingDomain.AthleteAggregate
{
    public class TrainingPlanPhaseRelation : ValueObject
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
        /// Navigation Property to the Training Phase
        /// </summary>
        public uint? TrainingPhaseId { get; private set; } = null;




        #region Ctors

        private TrainingPlanPhaseRelation() { }


        private TrainingPlanPhaseRelation(UserTrainingPlanEntity trainingPlan, uint? phaseId)
        {
            TrainingPlan = trainingPlan;
            TrainingPhaseId = phaseId;
            TrainingPlanId = trainingPlan.Id;
        }

        #endregion


        #region Factories

        /// <summary>
        /// Perform a link between the two entities, by applying a Many-to-Many relation
        /// </summary>
        /// <param name="trainingPlan">The left entity</param>
        /// <param name="phaseId">Tha right entity</param>
        /// <returns>The TrainingPlanMuscleFocus isntance</returns>
        public static TrainingPlanPhaseRelation BuildLink(UserTrainingPlanEntity trainingPlan, uint? phaseId)

            => new TrainingPlanPhaseRelation(trainingPlan, phaseId);

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TrainingPlan;
            yield return TrainingPhaseId;
            //yield return TrainingPlanId;  // Not necessary
        }
    }
}
