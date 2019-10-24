using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class PlanWorkingSetTempoCommand  : IRequest<bool>
    {


        public uint WorkoutTemplateId { get; private set; }
        public uint WorkUnitProgressiveNumber { get; private set; }
        public uint WorkingSetProgressiveNumber { get; private set; }
        public string TutValue { get; private set; }




        public PlanWorkingSetTempoCommand(uint workoutTemplateId, uint workUnitProgressiveNumber, uint workingSetProgressiveNumber, string tutValue)
        {
            WorkoutTemplateId = workoutTemplateId;
            WorkUnitProgressiveNumber = workUnitProgressiveNumber;
            WorkingSetProgressiveNumber = workingSetProgressiveNumber;
            TutValue = TutValue;
        }



    }
}
