using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class DetachWorkUnitTemplateNoteCommand  : IRequest<bool>
    {


        public uint WorkoutTemplateId { get; private set; }
        public uint WorkUnitProgressiveNumber { get; private set; }



        public DetachWorkUnitTemplateNoteCommand(uint workoutTemplateId, uint workUnitProgressiveNumber)
        {
            WorkoutTemplateId = workoutTemplateId;
            WorkUnitProgressiveNumber = workUnitProgressiveNumber;
        }



    }
}
