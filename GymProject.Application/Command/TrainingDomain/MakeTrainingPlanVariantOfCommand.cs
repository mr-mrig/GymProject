using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class MakeTrainingPlanVariantOfCommand : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }
        public uint UserId { get; private set; }
        public uint ParentPlanId { get; private set; }



        public MakeTrainingPlanVariantOfCommand(uint trainingPlanId, uint userId, uint parentPlanId)
        {
            TrainingPlanId = trainingPlanId;
            UserId = userId;
            ParentPlanId = parentPlanId;
        }

    }
}
