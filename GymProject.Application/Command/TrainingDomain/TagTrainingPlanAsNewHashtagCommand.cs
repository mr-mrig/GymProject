using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class TagTrainingPlanAsNewHashtagCommand : IRequest<bool>
    {


        public uint AthleteId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public string HashtagBody { get; private set; }




        public TagTrainingPlanAsNewHashtagCommand(uint athleteId, uint trainingPlanId, string hashtagBody)
        {
            AthleteId = athleteId;
            TrainingPlanId = trainingPlanId;
            HashtagBody = hashtagBody;
        }

    }
}
