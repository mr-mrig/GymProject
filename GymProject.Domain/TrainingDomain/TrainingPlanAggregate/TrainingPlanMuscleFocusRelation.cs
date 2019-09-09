﻿using GymProject.Domain.Base;
using System.Collections.Generic;

namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
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
        public TrainingPlanRoot TrainingPlan { get; private set; } = null;

        /// <summary>
        /// Navigation Property to the Muscle
        /// </summary>
        public uint? MuscleId { get; private set; } = null;




        #region Ctors

        private TrainingPlanMuscleFocusRelation() { }


        private TrainingPlanMuscleFocusRelation(TrainingPlanRoot trainingPlan, uint? muscleId)
        {
            TrainingPlan = trainingPlan;
            MuscleId = muscleId;
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
        public static TrainingPlanMuscleFocusRelation BuildLink(TrainingPlanRoot trainingPlan, uint? muscleId)

            => new TrainingPlanMuscleFocusRelation(trainingPlan, muscleId);

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TrainingPlan;
            yield return MuscleId;
            //yield return TrainingPlanId;  // Not necessary
        }
    }
}
