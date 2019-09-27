﻿using GymProject.Domain.Base;
using GymProject.Domain.Base.Mediator;
using MediatR;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class FitnessHasBeenClearedDomainEvent : INotification
    {


        /// <summary>
        /// The source of the event
        /// </summary>
        public DailyFitnessRoot FitnessDay { get; private set; }

        /// <summary>
        ///  The destination of the event
        /// </summary>
        public uint? PostId { get; private set; }



        /// <summary>
        /// Event for communicating that the fitness entry has been cleared: all the childs have been removed
        /// </summary>
        /// <param name="measuresEntry">The fitness day entry</param>
        /// <param name="postId">The parent Post Id</param>
        public FitnessHasBeenClearedDomainEvent(DailyFitnessRoot fitnessDay, uint? postId)
        {
            FitnessDay = fitnessDay;
            PostId = postId;
        }
    }
}
