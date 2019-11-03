using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class UntagTrainingPlanWithProficiencyCommand : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }
        public uint ProficiencyId { get; private set; }




        public UntagTrainingPlanWithProficiencyCommand(uint trainingPlanId, uint proficiencyId)
        {
            TrainingPlanId = trainingPlanId;
            ProficiencyId = proficiencyId;
        }

    }
}
