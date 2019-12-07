using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class UntagTrainingPlanWithProficiencyCommand : IRequest<bool>
    {


        public uint AthleteId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public uint ProficiencyId { get; private set; }




        public UntagTrainingPlanWithProficiencyCommand(uint athleteId, uint trainingPlanId, uint proficiencyId)
        {
            AthleteId = athleteId;
            TrainingPlanId = trainingPlanId;
            ProficiencyId = proficiencyId;
        }

    }
}
