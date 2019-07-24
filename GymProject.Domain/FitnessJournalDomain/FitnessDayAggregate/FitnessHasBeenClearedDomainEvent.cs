﻿using GymProject.Domain.Base;
using GymProject.Domain.Base.Mediator;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class FitnessHasBeenClearedDomainEvent : IMediatorNotification
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
        /// Event for communicating that the fitness entry has been cleared: all the childs have been removed
        /// </summary>
        /// <param name="measuresEntry">The fitness day entry</param>
        /// <param name="postId">The parent Post Id</param>
        public FitnessHasBeenClearedDomainEvent(FitnessDay fitnessDay, IdType postId)
        {
            FitnessDay = fitnessDay;
            PostId = postId;
        }
    }
}
