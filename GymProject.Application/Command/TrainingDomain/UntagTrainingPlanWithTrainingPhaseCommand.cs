using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class UntagTrainingPlanWithTrainingPhaseCommand : IRequest<bool>
    {


        public uint UserId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public uint PhaseId { get; private set; }




        public UntagTrainingPlanWithTrainingPhaseCommand(uint userId, uint trainingPlanId, uint phaseId)
        {
            UserId = userId;
            TrainingPlanId = trainingPlanId;
            PhaseId = phaseId;
        }

    }
}
