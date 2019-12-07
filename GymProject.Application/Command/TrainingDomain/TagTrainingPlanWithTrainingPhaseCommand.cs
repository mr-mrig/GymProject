using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class TagTrainingPlanWithTrainingPhaseCommand : IRequest<bool>
    {


        public uint AthleteId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public uint PhaseId { get; private set; }




        public TagTrainingPlanWithTrainingPhaseCommand(uint athleteId, uint trainingPlanId, uint phaseId)
        {
            AthleteId = athleteId;
            TrainingPlanId = trainingPlanId;
            PhaseId = phaseId;
        }

    }
}
