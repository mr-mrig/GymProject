using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class AttachWorkUnitTemplateNoteCommand  : IRequest<bool>
    {


        public uint WorkoutTemplateId { get; private set; }
        public uint WorkUnitProgressiveNumber { get; private set; }
        public uint WorkUnitNoteId { get; private set; }




        public AttachWorkUnitTemplateNoteCommand(uint workoutTemplateId, uint workUnitProgressiveNumber, uint workUnitNoteId)
        {
            WorkoutTemplateId = workoutTemplateId;
            WorkUnitProgressiveNumber = workUnitProgressiveNumber;
            WorkUnitNoteId = workUnitNoteId;
        }



    }
}
