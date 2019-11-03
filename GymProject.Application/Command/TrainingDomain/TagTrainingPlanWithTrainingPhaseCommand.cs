using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class TagTrainingPlanWithTrainingPhaseCommand : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }
        public uint PhaseId { get; private set; }




        public TagTrainingPlanWithTrainingPhaseCommand(uint trainingPlanId, uint phaseId)
        {
            TrainingPlanId = trainingPlanId;
            PhaseId = phaseId;
        }

    }
}
