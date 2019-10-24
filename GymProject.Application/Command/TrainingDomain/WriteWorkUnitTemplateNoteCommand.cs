using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class WriteWorkUnitTemplateNoteCommand  : IRequest<bool>
    {


        public uint WorkoutTemplateId { get; private set; }
        public uint WorkUnitProgressiveNumber { get; private set; }
        public string NoteBody { get; private set; }




        public WriteWorkUnitTemplateNoteCommand(uint workoutTemplateId, uint workUnitProgressiveNumber, string noteBody)
        {
            WorkoutTemplateId = workoutTemplateId;
            WorkUnitProgressiveNumber = workUnitProgressiveNumber;
            NoteBody = noteBody;
        }



    }
}
