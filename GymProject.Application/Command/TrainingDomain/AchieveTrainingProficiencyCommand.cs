using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class AchieveTrainingProficiencyCommand : IRequest<bool>
    {


        public uint AthleteId { get; private set; }
        public uint TrainingProficiencyId { get; private set; }



        public AchieveTrainingProficiencyCommand(uint athleteId, uint trainingProficiencyId)
        {
            TrainingProficiencyId = trainingProficiencyId;
            AthleteId = athleteId;
        }

    }
}
