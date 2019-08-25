using GymProject.Domain.Base.Mediator;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;

namespace GymProject.Domain.TrainingDomain.Events
{
    public class TrainingParametersChangedDomainEvent : IMediatorNotification
    {

        /// <summary>
        /// The Workout which the training parameters have been changed of
        /// </summary>
        public WorkoutTemplate ChangedWorkout;


        public TrainingParametersChangedDomainEvent(WorkoutTemplate changedWorkout)
        {
            ChangedWorkout = changedWorkout;
        }
    }
}
