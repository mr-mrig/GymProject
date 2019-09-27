using GymProject.Domain.Base;
using GymProject.Domain.Base.Mediator;
using MediatR;

namespace GymProject.Domain.MeasuresJournalDomain.MeasureEntryAggregate
{
    public class MeasuresHaveBeenClearedDomainEvent : INotification
    {


        /// <summary>
        /// The source of the event
        /// </summary>
        public DailyMeasuresEntity MeasuresEntry { get; private set; }

        /// <summary>
        ///  The destination of the event
        /// </summary>
        public uint? PostId { get; private set; }



        /// <summary>
        /// Event for communicating that the measures entry has been cleared: all the childs have been removed
        /// </summary>
        /// <param name="measuresEntry">The measure entry</param>
        /// <param name="postId">The parent Post Id</param>
        public MeasuresHaveBeenClearedDomainEvent(DailyMeasuresEntity measuresEntry, uint? postId)
        {
            MeasuresEntry = measuresEntry;
            PostId = postId;
        }
    }
}
