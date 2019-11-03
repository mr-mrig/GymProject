using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class TagTrainingPlanAsNewHashtagCommand : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }
        public string HashtagBody { get; private set; }




        public TagTrainingPlanAsNewHashtagCommand(uint trainingPlanId, string hashtagBody)
        {
            TrainingPlanId = trainingPlanId;
            HashtagBody = hashtagBody;
        }

    }
}
