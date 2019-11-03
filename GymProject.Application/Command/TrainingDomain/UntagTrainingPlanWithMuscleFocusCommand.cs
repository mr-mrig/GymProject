using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class UntagTrainingPlanWithMuscleFocusCommand : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }
        public uint MuscleId { get; private set; }




        public UntagTrainingPlanWithMuscleFocusCommand(uint trainingPlanId, uint muscleId)
        {
            TrainingPlanId = trainingPlanId;
            MuscleId = muscleId;
        }

    }
}
