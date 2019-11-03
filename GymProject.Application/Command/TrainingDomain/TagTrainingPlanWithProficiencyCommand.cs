using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class TagTrainingPlanWithProficiencyCommand : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }
        public uint ProficiencyId { get; private set; }




        public TagTrainingPlanWithProficiencyCommand(uint trainingPlanId, uint proficiencyId)
        {
            TrainingPlanId = trainingPlanId;
            ProficiencyId = proficiencyId;
        }

    }
}
