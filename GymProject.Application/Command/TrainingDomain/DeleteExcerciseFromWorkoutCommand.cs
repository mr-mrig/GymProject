using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class DeleteExcerciseFromWorkoutCommand : IRequest<bool>
    {


        public uint WorkoutTemplateId { get; private set; }
        public uint WorkUnitId { get; private set; }



        public DeleteExcerciseFromWorkoutCommand(uint workoutTemplateId, uint workUnitId)
        {
            WorkoutTemplateId = workoutTemplateId;
            WorkUnitId = workUnitId;
        }




    }
}
