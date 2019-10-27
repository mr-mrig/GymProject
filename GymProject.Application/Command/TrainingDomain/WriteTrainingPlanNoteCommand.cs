using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class WriteTrainingPlanNoteCommand  : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }
        public string NoteBody { get; private set; }




        public WriteTrainingPlanNoteCommand(uint trainingPlanId, string noteBody)
        {
            TrainingPlanId = trainingPlanId;
            NoteBody = noteBody;
        }



    }
}
