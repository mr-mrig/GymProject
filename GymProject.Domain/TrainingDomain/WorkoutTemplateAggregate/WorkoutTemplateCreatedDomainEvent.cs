using GymProject.Domain.Base.Mediator;
using MediatR;

namespace GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate
{
    public class WorkoutTemplateCreatedDomainEvent : INotification
    {



        /// <summary>
        /// Event source
        /// </summary>
        public uint? WorkoutTemplateId { get; private set; }


        /// <summary>
        /// Event source
        /// </summary>
        public uint? TrainingPlanId { get; private set; }


        /// <summary>
        /// Event source
        /// </summary>
        public uint TrainingWeekProgressiveNumber { get; private set; }





        /// <summary>
        /// Event contructor
        /// </summary>
        public WorkoutTemplateCreatedDomainEvent(uint? workoutTemplateId, uint? trainingPlanId, uint trainingWeekProgressiveNumber)
        {
            WorkoutTemplateId = workoutTemplateId;
            TrainingPlanId = trainingPlanId;
            TrainingWeekProgressiveNumber = trainingWeekProgressiveNumber;
        }

    }
}
