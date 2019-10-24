using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class PlanDraftWorkoutCommand : IRequest<bool>
    {



        public uint TrainingPlanId { get; private set; }
        public uint TrainingWeekId { get; private set; }
        public uint TrainingWeekProgressiveNumber { get; private set; }      // Redundant, might be computed later by adding the proper function to the domain AR




        public PlanDraftWorkoutCommand(uint trainingPlanId, uint trainingWeekId, uint trainingWeekProgressiveNumber)
        {
            TrainingPlanId = trainingPlanId;
            TrainingWeekId = trainingWeekId;
            TrainingWeekProgressiveNumber = trainingWeekProgressiveNumber;
        }



    }
}
