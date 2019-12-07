using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class UntagTrainingPlanAsHashtagCommand : IRequest<bool>
    {


        public uint AthleteId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public uint TrainingHashtagId { get; private set; }




        public UntagTrainingPlanAsHashtagCommand(uint athleteId, uint trainingPlanId, uint trainingHashtagId)
        {
            AthleteId = athleteId;
            TrainingPlanId = trainingPlanId;
            TrainingHashtagId = trainingHashtagId;
        }

    }
}
