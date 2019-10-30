using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class TagTrainingPlanCommand : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }
        public uint TrainingHashtagId { get; private set; }




        public TagTrainingPlanCommand(uint trainingPlanId, uint trainingHashtagId)
        {
            TrainingPlanId = trainingPlanId;
            TrainingHashtagId = trainingHashtagId;
        }

    }
}
