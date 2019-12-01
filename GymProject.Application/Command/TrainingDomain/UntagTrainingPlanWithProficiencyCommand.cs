using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class UntagTrainingPlanWithProficiencyCommand : IRequest<bool>
    {


        public uint UserId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public uint ProficiencyId { get; private set; }




        public UntagTrainingPlanWithProficiencyCommand(uint userId, uint trainingPlanId, uint proficiencyId)
        {
            UserId = userId;
            TrainingPlanId = trainingPlanId;
            ProficiencyId = proficiencyId;
        }

    }
}
