using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class MakeTrainingPlanNotVariantOfAnyCommand : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }
        public uint UserId { get; private set; }



        public MakeTrainingPlanNotVariantOfAnyCommand(uint trainingPlanId, uint userId)
        {
            TrainingPlanId = trainingPlanId;
            UserId = userId;
        }

    }
}
