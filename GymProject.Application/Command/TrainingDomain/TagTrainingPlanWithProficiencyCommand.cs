using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class TagTrainingPlanWithProficiencyCommand : IRequest<bool>
    {


        public uint UserId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public uint ProficiencyId { get; private set; }




        public TagTrainingPlanWithProficiencyCommand(uint userId, uint trainingPlanId, uint proficiencyId)
        {
            UserId = userId;
            TrainingPlanId = trainingPlanId;
            ProficiencyId = proficiencyId;
        }

    }
}
