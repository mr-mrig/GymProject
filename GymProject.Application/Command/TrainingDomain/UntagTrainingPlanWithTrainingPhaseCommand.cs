using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class UntagTrainingPlanWithTrainingPhaseCommand : IRequest<bool>
    {


        public uint AthleteId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public uint PhaseId { get; private set; }




        public UntagTrainingPlanWithTrainingPhaseCommand(uint athleteId, uint trainingPlanId, uint phaseId)
        {
            AthleteId = athleteId;
            TrainingPlanId = trainingPlanId;
            PhaseId = phaseId;
        }

    }
}
