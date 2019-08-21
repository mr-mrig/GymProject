using GymProject.Domain.Base;
using GymProject.Domain.Base.Mediator;

namespace GymProject.Domain.MeasuresJournalDomain.MeasureEntryAggregate
{
    public class MeasuresHaveBeenClearedDomainEvent : IMediatorNotification
    {


        /// <summary>
        /// The source of the event
        /// </summary>
        public MeasuresEntry MeasuresEntry { get; private set; }

        /// <summary>
        ///  The destination of the event
        /// </summary>
        public IdTypeValue PostId { get; private set; }



        /// <summary>
        /// Event for communicating that the measures entry has been cleared: all the childs have been removed
        /// </summary>
        /// <param name="measuresEntry">The measure entry</param>
        /// <param name="postId">The parent Post Id</param>
        public MeasuresHaveBeenClearedDomainEvent(MeasuresEntry measuresEntry, IdTypeValue postId)
        {
            MeasuresEntry = measuresEntry;
            PostId = postId;
        }
    }
}
