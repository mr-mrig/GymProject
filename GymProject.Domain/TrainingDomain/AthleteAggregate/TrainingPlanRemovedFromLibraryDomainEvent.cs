using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.TrainingDomain.AthleteAggregate
{
    public class TrainingPlanRemovedFromLibraryDomainEvent : INotification
    {

        public uint TrainingPlanId { get; set; }



        public TrainingPlanRemovedFromLibraryDomainEvent(uint trainingPlanId)
        {
            TrainingPlanId = trainingPlanId;
        }
    }
}
