using GymProject.Domain.Base.Mediator;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;

namespace GymProject.Domain.TrainingDomain.Events
{
    public class TrainingParametersChangedDomainEvent : IMediatorNotification
    {

        /// <summary>
        /// The Workout which the training parameters have been changed of
        /// </summary>
        public WorkoutTemplateRoot ChangedWorkout;


        public TrainingParametersChangedDomainEvent(WorkoutTemplateRoot changedWorkout)
        {
            ChangedWorkout = changedWorkout;
        }
    }
}
