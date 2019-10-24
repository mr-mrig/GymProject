using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class PlanWorkingSetRestCommand  : IRequest<bool>
    {


        public uint WorkoutTemplateId { get; private set; }
        public uint WorkUnitProgressiveNumber { get; private set; }
        public uint WorkingSetProgressiveNumber { get; private set; }
        public int RestValue { get; private set; }
        public int RestMeasUnitId { get; private set; } = 0;



        public PlanWorkingSetRestCommand(uint workoutTemplateId, uint workUnitProgressiveNumber, uint workingSetProgressiveNumber, int restValue, int restMeasUnitId) 
            : this(workoutTemplateId, workUnitProgressiveNumber, workingSetProgressiveNumber, restValue)
        {
            RestMeasUnitId = restMeasUnitId;
        }


        /// <summary>
        /// Update the WS Rest with the specified value and using the dafault measure unit
        /// </summary>
        /// <param name="workoutTemplateId"></param>
        /// <param name="workUnitProgressiveNumber"></param>
        /// <param name="workingSetProgressiveNumber"></param>
        /// <param name="restValue"></param>
        public PlanWorkingSetRestCommand(uint workoutTemplateId, uint workUnitProgressiveNumber, uint workingSetProgressiveNumber, int restValue)
        {
            WorkoutTemplateId = workoutTemplateId;
            WorkUnitProgressiveNumber = workUnitProgressiveNumber;
            WorkingSetProgressiveNumber = workingSetProgressiveNumber;
            RestValue = restValue;
        }



    }
}
