using GymProject.Domain.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.MeasuresJournalDomain.MeasureEntryAggregate
{
    public class MeasuresChangedDomainEvent : INotification
    {


        /// <summary>
        /// The source of the event
        /// </summary>
        public MeasuresEntry MeasuresEntry { get; private set; }

        /// <summary>
        ///  The destination of the event
        /// </summary>
        public IdType PostId { get; private set; }



        /// <summary>
        /// Event for communicating a change in the measures entry to the parent post.
        /// </summary>
        /// <param name="measuresEntry">The measure entry</param>
        /// <param name="postId">The parent Post Id</param>
        public MeasuresChangedDomainEvent(MeasuresEntry measuresEntry, IdType postId)
        {
            MeasuresEntry = measuresEntry;
            PostId = postId;
        }
    }
}
