﻿using GymProject.Domain.Base;
using System.Collections.Generic;

namespace GymProject.Domain.TrainingDomain.AthleteAggregate
{
    public class TrainingPlanProficiencyRelation : ValueObject
    {



        /// <summary>
        /// The Training Plan ID
        /// </summary>
        public uint? TrainingPlanId { get; private set; } = null;

        /// <summary>
        /// Navigation Property to the Training Plan
        /// </summary>
        public UserTrainingPlanEntity UserTrainingPlan { get; private set; } = null;

        /// <summary>
        /// Navigation Property to the Training Proficiency
        /// </summary>
        public uint? TrainingProficiencyId { get; private set; } = null;




        #region Ctors

        private TrainingPlanProficiencyRelation() { }


        private TrainingPlanProficiencyRelation(UserTrainingPlanEntity userTrainingPlan, uint? proficiencyId)
        {
            UserTrainingPlan = userTrainingPlan;
            TrainingProficiencyId = proficiencyId;
            TrainingPlanId = userTrainingPlan.Id;
        }

        #endregion


        #region Factories

        /// <summary>
        /// Perform a link between the two entities, by applying a Many-to-MAny relation
        /// </summary>
        /// <param name="trainingPlan">The left entity</param>
        /// <param name="proficiencyId">Tha right entity</param>
        /// <returns>The TrainingPlanMuscleFocus isntance</returns>
        public static TrainingPlanProficiencyRelation BuildLink(UserTrainingPlanEntity trainingPlan, uint? proficiencyId)

            => new TrainingPlanProficiencyRelation(trainingPlan, proficiencyId);

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return UserTrainingPlan;
            yield return TrainingProficiencyId;
            //yield return TrainingPlanId;  // Not necessary
        }
    }
}