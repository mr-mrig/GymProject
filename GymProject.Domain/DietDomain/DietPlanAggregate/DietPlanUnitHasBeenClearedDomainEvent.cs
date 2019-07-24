﻿using GymProject.Domain.Base.Mediator;

namespace GymProject.Domain.DietDomain.DietPlanAggregate
{
    public class DietPlanUnitHasBeenClearedDomainEvent : IMediatorNotification
    {


        /// <summary>
        /// The source of the event
        /// </summary>
        public DietPlanUnit DietPlanUnit { get; private set; }




        /// <summary>
        /// Event for communicating that the diet plan unit has been cleared: all the childs have been removed
        /// </summary>
        /// <param name="dietPlanUnit">The DietPlanUnit object</param>
        public DietPlanUnitHasBeenClearedDomainEvent(DietPlanUnit dietPlanUnit)
        {
            DietPlanUnit = dietPlanUnit;
        }
    }
}
