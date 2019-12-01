using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class UntagTrainingPlanAsHashtagCommand : IRequest<bool>
    {


        public uint UserId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public uint TrainingHashtagId { get; private set; }




        public UntagTrainingPlanAsHashtagCommand(uint userId, uint trainingPlanId, uint trainingHashtagId)
        {
            UserId = userId;
            TrainingPlanId = trainingPlanId;
            TrainingHashtagId = trainingHashtagId;
        }

    }
}
