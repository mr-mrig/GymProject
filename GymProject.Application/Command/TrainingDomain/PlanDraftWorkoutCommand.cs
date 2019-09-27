using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class PlanDraftWorkoutCommand : IRequest<bool>
    {



        public uint TrainingPlanId;
        public uint TrainingWeekId; 
        public uint TrainingWeekProgressiveNumber;      // Redundant, might be computed later by adding the proper function to the domain AR




        public PlanDraftWorkoutCommand(uint trainingPlanId, uint trainingWeekId, uint trainingWeekProgressiveNumber)
        {
            TrainingPlanId = trainingPlanId;
            TrainingWeekId = trainingWeekId;
            TrainingWeekProgressiveNumber = trainingWeekProgressiveNumber;
        }



    }
}
