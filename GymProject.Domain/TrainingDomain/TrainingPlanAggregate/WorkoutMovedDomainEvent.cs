using GymProject.Domain.Base.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
{
    public class WorkoutMovedDomainEvent : IMediatorNotification
    {

        public workout Order { get; }

        public OrderCancelledDomainEvent(Order order)
        {
            Order = order;
        }


    }
}
