using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class TagTrainingPlanAsNewHashtagCommand : IRequest<bool>
    {


        public uint UserId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public string HashtagBody { get; private set; }




        public TagTrainingPlanAsNewHashtagCommand(uint userId, uint trainingPlanId, string hashtagBody)
        {
            UserId = userId;
            TrainingPlanId = trainingPlanId;
            HashtagBody = hashtagBody;
        }

    }
}
