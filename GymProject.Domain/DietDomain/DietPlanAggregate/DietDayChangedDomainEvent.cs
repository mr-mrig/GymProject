//using GymProject.Domain.Base;
//using MediatR;

//namespace GymProject.Domain.DietDomain.DietPlanAggregate
//{
//    public class FitnessDayChangedDomainEvent : INotification
//    {


//        /// <summary>
//        /// The source of the event
//        /// </summary>
//        public DietPlan DietPlan { get; private set; }

//        /// <summary>
//        ///  The destination of the event
//        /// </summary>
//        public IdType PostId { get; private set; }



//        /// <summary>
//        /// Event for communicating a change in the diet plan entry to the parent post.
//        /// </summary>
//        /// <param name="measuresEntry">The diet plan entry</param>
//        /// <param name="postId">The parent Post Id</param>
//        public FitnessDayChangedDomainEvent(DietPlan dietPlan, IdType postId)
//        {
//            DietPlan = dietPlan;
//            PostId = postId;
//        }
//    }
//}
