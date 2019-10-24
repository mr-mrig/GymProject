using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class PlanWorkingSetRepetitionsCommand  : IRequest<bool>
    {


        public uint WorkoutTemplateId { get; private set; }
        public uint WorkUnitProgressiveNumber { get; private set; }
        public uint WorkingSetProgressiveNumber { get; private set; }

        public int RepetitionsValue { get; private set; }
        public int WorkTypeId { get; private set; }

        //public WSRepetitionsValue Repetitions { get; private set; }
        //public RestPeriodValue Rest { get; private set; }
        //public TrainingEffortValue Effort{ get; private set; }
        //public TUTValue Tempo{ get; private set; }
        //public IEnumerable<uint?> IntensityTechniquesIds { get; private set; }



        //public PlanWorkingSetRepetitionsCommand (uint workoutTemplateId, uint workUnitProgressiveNumber, WSRepetitionsValue repetitions, RestPeriodValue rest
        //    , TrainingEffortValue effort, TUTValue tempo, IEnumerable<uint?> intensityTechniquesIds)

        public PlanWorkingSetRepetitionsCommand(uint workoutTemplateId, uint workUnitProgressiveNumber, uint workingSetProgressiveNumber, int repetitions, int workTypeId)
        {
            WorkoutTemplateId = workoutTemplateId;
            WorkUnitProgressiveNumber = workUnitProgressiveNumber;
            WorkingSetProgressiveNumber = workingSetProgressiveNumber;
            RepetitionsValue = repetitions;
            WorkTypeId = workTypeId;
            //Rest = rest;
            //Effort = effort;
            //Tempo = tempo;
            //IntensityTechniquesIds = intensityTechniquesIds
        }




    }
}
