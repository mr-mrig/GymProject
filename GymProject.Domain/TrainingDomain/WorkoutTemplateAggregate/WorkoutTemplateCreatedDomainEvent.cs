using GymProject.Domain.Base.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate
{
    public class WorkoutTemplateCreatedDomainEvent : IMediatorNotification
    {



        /// <summary>
        /// Event source
        /// </summary>
        public WorkoutTemplateRoot WorkoutTemplate { get; private set; }





        /// <summary>
        /// Event contructor
        /// </summary>
        /// <param name="workoutTemplate">The event source</param>
        public WorkoutTemplateCreatedDomainEvent(WorkoutTemplateRoot workoutTemplate)
        {
            WorkoutTemplate = workoutTemplate;
        }

    }
}
