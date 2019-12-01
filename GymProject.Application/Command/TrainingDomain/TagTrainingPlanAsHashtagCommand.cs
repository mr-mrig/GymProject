using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class TagTrainingPlanAsHashtagCommand : IRequest<bool>
    {


        public uint UserId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public uint TrainingHashtagId { get; private set; }




        public TagTrainingPlanAsHashtagCommand(uint userId, uint trainingPlanId, uint trainingHashtagId)
        {
            UserId = userId;
            TrainingPlanId = trainingPlanId;
            TrainingHashtagId = trainingHashtagId;
        }

    }
}
