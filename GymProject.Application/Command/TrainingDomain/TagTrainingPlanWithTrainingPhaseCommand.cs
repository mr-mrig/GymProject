using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class TagTrainingPlanWithTrainingPhaseCommand : IRequest<bool>
    {


        public uint UserId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public uint PhaseId { get; private set; }




        public TagTrainingPlanWithTrainingPhaseCommand(uint userId, uint trainingPlanId, uint phaseId)
        {
            UserId = userId;
            TrainingPlanId = trainingPlanId;
            PhaseId = phaseId;
        }

    }
}
