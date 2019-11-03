using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class UntagTrainingPlanWithTrainingPhaseCommand : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }
        public uint PhaseId { get; private set; }




        public UntagTrainingPlanWithTrainingPhaseCommand(uint trainingPlanId, uint phaseId)
        {
            TrainingPlanId = trainingPlanId;
            PhaseId = phaseId;
        }

    }
}
