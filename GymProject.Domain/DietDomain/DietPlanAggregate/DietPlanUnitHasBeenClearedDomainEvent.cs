using GymProject.Domain.Base.Mediator;
using MediatR;

namespace GymProject.Domain.DietDomain.DietPlanAggregate
{
    public class DietPlanUnitHasBeenClearedDomainEvent : INotification
    {


        /// <summary>
        /// The source of the event
        /// </summary>
        public DietPlanUnitEntity DietPlanUnit { get; private set; }




        /// <summary>
        /// Event for communicating that the diet plan unit has been cleared: all the childs have been removed
        /// </summary>
        /// <param name="dietPlanUnit">The DietPlanUnit object</param>
        public DietPlanUnitHasBeenClearedDomainEvent(DietPlanUnitEntity dietPlanUnit)
        {
            DietPlanUnit = dietPlanUnit;
        }
    }
}
