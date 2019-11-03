using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class TagTrainingPlanAsHashtagCommand : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }
        public uint TrainingHashtagId { get; private set; }




        public TagTrainingPlanAsHashtagCommand(uint trainingPlanId, uint trainingHashtagId)
        {
            TrainingPlanId = trainingPlanId;
            TrainingHashtagId = trainingHashtagId;
        }

    }
}
