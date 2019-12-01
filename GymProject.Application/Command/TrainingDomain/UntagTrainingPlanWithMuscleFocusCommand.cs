using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class UntagTrainingPlanWithMuscleFocusCommand : IRequest<bool>
    {


        public uint UserId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public uint MuscleId { get; private set; }




        public UntagTrainingPlanWithMuscleFocusCommand(uint userId, uint trainingPlanId, uint muscleId)
        {
            UserId = userId;
            TrainingPlanId = trainingPlanId;
            MuscleId = muscleId;
        }

    }
}
