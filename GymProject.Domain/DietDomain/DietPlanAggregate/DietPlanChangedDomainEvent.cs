using GymProject.Domain.Base;
using GymProject.Domain.Base.Mediator;

namespace GymProject.Domain.DietDomain.DietPlanAggregate
{
    public class DietPlanChangedDomainEvent : IMediatorNotification
    {


        /// <summary>
        /// The source of the event
        /// </summary>
        public DietPlan DietPlanEntry { get; private set; }

        /// <summary>
        ///  The destination of the event
        /// </summary>
        public IdType PostId { get; private set; }



        /// <summary>
        /// Event for communicating a change in the diet plan entry to the parent post.
        /// </summary>
        /// <param name="dietPlanEntry">The diet plan entry</param>
        /// <param name="postId">The parent Post Id</param>
        public DietPlanChangedDomainEvent(DietPlan dietPlanEntry, IdType postId)
        {
            DietPlanEntry = dietPlanEntry;
            PostId = postId;
        }
    }
}
