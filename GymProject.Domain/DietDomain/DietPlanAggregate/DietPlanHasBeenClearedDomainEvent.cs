using GymProject.Domain.Base;
using GymProject.Domain.Base.Mediator;
using MediatR;

namespace GymProject.Domain.DietDomain.DietPlanAggregate
{
    public class DietPlanHasBeenClearedDomainEvent : INotification
    {


        /// <summary>
        /// The source of the event
        /// </summary>
        public DietPlanRoot DietPlan { get; private set; }

        /// <summary>
        ///  The destination of the event
        /// </summary>
        public uint? PostId { get; private set; }



        /// <summary>
        /// Event for communicating that the diet plan unit has been cleared: all the childs have been removed
        /// </summary>
        /// <param name="dietPlan">The DietPlanUnit object</param>
        /// <param name="postId">The reference to the root Post</param>
        public DietPlanHasBeenClearedDomainEvent(DietPlanRoot dietPlan, uint? postId)
        {
            DietPlan = dietPlan;
            PostId = postId;
        }
    }
}
