using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class PlanDraftExcerciseCommand : IRequest<bool>
    {


        public uint WorkoutTemplateId { get; private set; }
        public uint ExcerciseId { get; private set; }



        public PlanDraftExcerciseCommand(uint workoutTemplateId, uint excerciseId)
        {
            WorkoutTemplateId = workoutTemplateId;
            ExcerciseId = excerciseId;
        }




    }
}
