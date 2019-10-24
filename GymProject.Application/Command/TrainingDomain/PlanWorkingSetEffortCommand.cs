using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class PlanWorkingSetEffortCommand  : IRequest<bool>
    {


        public uint WorkoutTemplateId { get; private set; }
        public uint WorkUnitProgressiveNumber { get; private set; }
        public uint WorkingSetProgressiveNumber { get; private set; }

        public int? EffortValue { get; private set; }
        public int? EffortTypeId { get; private set; }




        public PlanWorkingSetEffortCommand(uint workoutTemplateId, uint workUnitProgressiveNumber, uint workingSetProgressiveNumber, int? effortValue, int? effortTypeId)
        {
            WorkoutTemplateId = workoutTemplateId;
            WorkUnitProgressiveNumber = workUnitProgressiveNumber;
            WorkingSetProgressiveNumber = workingSetProgressiveNumber;
            EffortValue = effortValue;
            EffortTypeId = effortTypeId;
        }


    }
}
