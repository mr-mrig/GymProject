using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class UntagTrainingPlanAsHashtagCommand : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }
        public uint TrainingHashtagId { get; private set; }




        public UntagTrainingPlanAsHashtagCommand(uint trainingPlanId, uint trainingHashtagId)
        {
            TrainingPlanId = trainingPlanId;
            TrainingHashtagId = trainingHashtagId;
        }

    }
}
