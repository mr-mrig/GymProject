using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class RenameTrainingPlanCommand : IRequest<bool>
    {


        public uint AthleteId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public string TrainingPlanName { get; private set; }




        public RenameTrainingPlanCommand(uint athleteId, uint trainingPlanId, string trainingPlanName)
        {
            AthleteId = athleteId;
            TrainingPlanId = trainingPlanId;
            TrainingPlanName = trainingPlanName;
        }

    }
}
