using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class AttachTrainingPlanNoteCommand  : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }
        public uint TrainingPlanNoteId { get; private set; }




        public AttachTrainingPlanNoteCommand(uint trainingPlanId, uint trainingPlanNoteId)
        {
            TrainingPlanId = trainingPlanId;
            TrainingPlanNoteId = trainingPlanNoteId;
        }



    }
}
