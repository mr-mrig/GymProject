using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class TagTrainingPlanWithMuscleFocusCommand : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }
        public uint MuscleId { get; private set; }




        public TagTrainingPlanWithMuscleFocusCommand(uint trainingPlanId, uint muscleId)
        {
            TrainingPlanId = trainingPlanId;
            MuscleId = muscleId;
        }

    }
}
