using GymProject.Domain.Base;
using GymProject.Domain.Base.Mediator;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class FitnessDayChangedDomainEvent : IMediatorNotification
    {


        /// <summary>
        /// The source of the event
        /// </summary>
        public FitnessDay FitnessDay { get; private set; }

        /// <summary>
        ///  The destination of the event
        /// </summary>
        public IdType PostId { get; private set; }



        /// <summary>
        /// Event for communicating a change in the measures entry to the parent post.
        /// </summary>
        /// <param name="measuresEntry">The measure entry</param>
        /// <param name="postId">The parent Post Id</param>
        public FitnessDayChangedDomainEvent(FitnessDay fitnessDayEntry, IdType postId)
        {
            FitnessDay = fitnessDayEntry;
            PostId = postId;
        }
    }
}
