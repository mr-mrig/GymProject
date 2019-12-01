using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class AttachTrainingPlanNoteCommand  : IRequest<bool>
    {


        public uint UserId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public uint TrainingPlanNoteId { get; private set; }




        public AttachTrainingPlanNoteCommand(uint userId, uint trainingPlanId, uint trainingPlanNoteId)
        {
            UserId = userId;
            TrainingPlanId = trainingPlanId;
            TrainingPlanNoteId = trainingPlanNoteId;
        }



    }
}
