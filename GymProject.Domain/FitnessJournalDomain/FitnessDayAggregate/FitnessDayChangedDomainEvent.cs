using GymProject.Domain.Base;
using GymProject.Domain.Base.Mediator;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class FitnessDayChangedDomainEvent : IMediatorNotification
    {


        /// <summary>
        /// The source of the event
        /// </summary>
        public DailyFitnessRoot FitnessDay { get; private set; }

        /// <summary>
        ///  The destination of the event
        /// </summary>
        public IdTypeValue PostId { get; private set; }



        /// <summary>
        /// Event for communicating a change in the measures entry to the parent post.
        /// </summary>
        /// <param name="measuresEntry">The measure entry</param>
        /// <param name="postId">The parent Post Id</param>
        public FitnessDayChangedDomainEvent(DailyFitnessRoot fitnessDayEntry, IdTypeValue postId)
        {
            FitnessDay = fitnessDayEntry;
            PostId = postId;
        }
    }
}
