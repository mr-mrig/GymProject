using GymProject.Domain.Base;
using GymProject.Domain.DietDomain.DietPlanAggregate;
using MediatR;

namespace GymProject.Domain.DietDomain.DietPlanAggregate
{
    public class DietPlanUnitHasBeenClearedDomainEvent : INotification
    {


        /// <summary>
        /// The source of the event
        /// </summary>
        public DietPlanUnit DietPlanUnit { get; private set; }

        /// <summary>
        ///  The destination of the event
        /// </summary>
        public IdType PostId { get; private set; }



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
