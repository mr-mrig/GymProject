using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class PlanTrainingWeekCommand : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }
        public uint WeekTypeEnumId { get; private set; }



        public PlanTrainingWeekCommand(uint trainingPlanId, uint weekTypeEnumId)
        {
            TrainingPlanId = trainingPlanId;
            WeekTypeEnumId = weekTypeEnumId;
        }
    }
}
