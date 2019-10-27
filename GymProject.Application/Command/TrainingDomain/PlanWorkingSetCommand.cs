using GymProject.Domain.TrainingDomain.Common;
using MediatR;
using System.Collections.Generic;

namespace GymProject.Application.Command.TrainingDomain
{
    public class PlanWorkingSetCommand : IRequest<bool>
    {


        public uint WorkoutTemplateId { get; private set; }
        public uint WorkUnitProgressiveNumber { get; private set; }
        public int RepetitionsValue { get; private set; }
        public int? WorkTypeId { get; private set; }
        public int? RestValue { get; private set; }
        public int? RestMeasUnitId { get; private set; } = 0;
        public int? EffortValue { get; private set; }
        public int? EffortTypeId { get; private set; }
        public string Tempo { get; private set; }
        public IEnumerable<uint?> IntensityTechniquesIds { get; private set; }

        //public WSRepetitionsValue Repetitions { get; private set; }
        //public RestPeriodValue Rest { get; private set; }
        //public TrainingEffortValue Effort{ get; private set; }
        //public TUTValue Tempo{ get; private set; }



        //public PlanWorkingSetCommand(uint workoutTemplateId, uint workUnitProgressiveNumber, WSRepetitionsValue repetitions, RestPeriodValue rest
        //    , TrainingEffortValue effort, TUTValue tempo, IEnumerable<uint?> intensityTechniquesIds)

        public PlanWorkingSetCommand(uint workoutTemplateId, uint workUnitProgressiveNumber
            , int repetitionsValue, int? workTypeId, int? restValue, int? restMeasUnitId
            , int? effort, int? effortTypeId, string tempo, IEnumerable<uint?> intensityTechniquesIds)
        {
            WorkoutTemplateId = workoutTemplateId;
            WorkUnitProgressiveNumber = workUnitProgressiveNumber;
            RepetitionsValue = repetitionsValue;
            WorkTypeId = workTypeId;
            RestValue = restValue;
            RestMeasUnitId = restMeasUnitId;
            EffortValue = effort;
            EffortTypeId = effortTypeId;
            Tempo = tempo;
            IntensityTechniquesIds = intensityTechniquesIds;
        }




    }
}
