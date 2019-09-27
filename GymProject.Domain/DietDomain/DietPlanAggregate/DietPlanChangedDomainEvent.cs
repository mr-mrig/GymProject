using GymProject.Domain.Base;
using GymProject.Domain.Base.Mediator;
using MediatR;

namespace GymProject.Domain.DietDomain.DietPlanAggregate
{
    public class DietPlanChangedDomainEvent : INotification
    {


        /// <summary>
        /// The source of the event
        /// </summary>
        public DietPlanRoot DietPlanEntry { get; private set; }

        /// <summary>
        ///  The destination of the event
        /// </summary>
        public uint? PostId { get; private set; }



        /// <summary>
        /// Event for communicating a change in the diet plan entry to the parent post.
        /// </summary>
        /// <param name="dietPlanEntry">The diet plan entry</param>
        /// <param name="postId">The parent Post Id</param>
        public DietPlanChangedDomainEvent(DietPlanRoot dietPlanEntry, uint? postId)
        {
            DietPlanEntry = dietPlanEntry;
            PostId = postId;
        }
    }
}
