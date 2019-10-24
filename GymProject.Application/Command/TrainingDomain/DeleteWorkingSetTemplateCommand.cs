using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class DeleteWorkingSetTemplateCommand : IRequest<bool>
    {


        public uint WorkoutTemplateId { get; private set; }
        public uint WorkUnitProgressiveNumber { get; private set; }
        public uint WorkingSetProgressiveNumber { get; private set; }



        public DeleteWorkingSetTemplateCommand(uint workoutTemplateId, uint workUnitProgressiveNumber, uint workingSetProgressiveNumber)
        {
            WorkoutTemplateId = workoutTemplateId;
            WorkUnitProgressiveNumber = workUnitProgressiveNumber;
            WorkingSetProgressiveNumber = workingSetProgressiveNumber;
        }


    }
}
