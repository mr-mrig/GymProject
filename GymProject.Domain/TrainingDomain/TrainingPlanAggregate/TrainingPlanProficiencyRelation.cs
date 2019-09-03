using GymProject.Domain.Base;
using System.Collections.Generic;

namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
{
    public class TrainingPlanProficiencyRelation : ValueObject
    {



        /// <summary>
        /// The Training Plan ID
        /// </summary>
        public IdTypeValue TrainingPlanId { get; private set; } = null;

        /// <summary>
        /// Navigation Property to the Training Plan
        /// </summary>
        public TrainingPlanRoot TrainingPlan { get; private set; } = null;

        /// <summary>
        /// Navigation Property to the Training Proficiency
        /// </summary>
        public IdTypeValue TrainingProficiencyId { get; private set; } = null;




        #region Ctors

        private TrainingPlanProficiencyRelation() { }


        private TrainingPlanProficiencyRelation(TrainingPlanRoot trainingPlan, IdTypeValue proficiencyId)
        {
            TrainingPlan = trainingPlan;
            TrainingProficiencyId = proficiencyId;
            TrainingPlanId = trainingPlan.Id;
        }

        #endregion


        #region Factories

        /// <summary>
        /// Perform a link between the two entities, by applying a Many-to-MAny relation
        /// </summary>
        /// <param name="trainingPlan">The left entity</param>
        /// <param name="proficiencyId">Tha right entity</param>
        /// <returns>The TrainingPlanMuscleFocus isntance</returns>
        public static TrainingPlanProficiencyRelation BuildLink(TrainingPlanRoot trainingPlan, IdTypeValue proficiencyId)

            => new TrainingPlanProficiencyRelation(trainingPlan, proficiencyId);

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TrainingPlan;
            yield return TrainingProficiencyId;
            //yield return TrainingPlanId;  // Not necessary
        }
    }
}
