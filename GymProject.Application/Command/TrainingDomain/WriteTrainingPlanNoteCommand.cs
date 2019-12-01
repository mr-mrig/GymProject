using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class WriteTrainingPlanNoteCommand  : IRequest<bool>
    {


        public uint UserId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public string NoteBody { get; private set; }




        public WriteTrainingPlanNoteCommand(uint userId, uint trainingPlanId, string noteBody)
        {
            UserId = userId;
            TrainingPlanId = trainingPlanId;
            NoteBody = noteBody;
        }



    }
}
