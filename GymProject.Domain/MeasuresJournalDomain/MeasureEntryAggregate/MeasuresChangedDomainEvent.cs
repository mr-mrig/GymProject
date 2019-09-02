using GymProject.Domain.Base;
using GymProject.Domain.Base.Mediator;

namespace GymProject.Domain.MeasuresJournalDomain.MeasureEntryAggregate
{
    public class MeasuresChangedDomainEvent : IMediatorNotification
    {


        /// <summary>
        /// The source of the event
        /// </summary>
        public DailyMeasuresEntity MeasuresEntry { get; private set; }

        /// <summary>
        ///  The destination of the event
        /// </summary>
        public IdTypeValue PostId { get; private set; }



        /// <summary>
        /// Event for communicating a change in the measures entry to the parent post.
        /// </summary>
        /// <param name="measuresEntry">The measure entry</param>
        /// <param name="postId">The parent Post Id</param>
        public MeasuresChangedDomainEvent(DailyMeasuresEntity measuresEntry, IdTypeValue postId)
        {
            MeasuresEntry = measuresEntry;
            PostId = postId;
        }
    }
}
