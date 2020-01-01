using GymProject.Domain.Base;
using System.Collections.Generic;

namespace GymProject.Domain.TrainingDomain.AthleteAggregate
{
    public class TrainingPlanPhaseRelation : RelationEntity
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
        /// Navigation Property to the Training Phase
        /// </summary>
        public uint? TrainingPhaseId { get; private set; } = null;




        #region Ctors

        private TrainingPlanPhaseRelation() { }


        private TrainingPlanPhaseRelation(UserTrainingPlanEntity userTrainingPlan, uint? phaseId)
        {
            UserTrainingPlan = userTrainingPlan;
            TrainingPhaseId = phaseId;
            UserTrainingPlanId = userTrainingPlan.Id;
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

        protected override IEnumerable<object> GetIdentifyingFields()
        {
            yield return UserTrainingPlanId;
            yield return TrainingPhaseId;
        }
    }
}
