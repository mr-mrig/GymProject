using GymProject.Domain.TrainingDomain.Common;
using MediatR;
using System.Collections.Generic;

namespace GymProject.Application.Command.TrainingDomain
{
    public class PlanWorkingSetCommand : IRequest<bool>
    {


        public uint WorkoutTemplateId { get; private set; }
        public uint WorkUnitProgressiveNumber { get; private set; }
        public WSRepetitionsValue Repetitions { get; private set; }
        public RestPeriodValue Rest { get; private set; }
        public TrainingEffortValue Effort{ get; private set; }
        public TUTValue Tempo{ get; private set; }
        public IEnumerable<uint?> IntensityTechniquesIds { get; private set; }



        public PlanWorkingSetCommand(uint workoutTemplateId, uint workUnitProgressiveNumber, WSRepetitionsValue repetitions, RestPeriodValue rest
            , TrainingEffortValue effort, TUTValue tempo, IEnumerable<uint?> intensityTechniquesIds)
        {
            WorkoutTemplateId = workoutTemplateId;
            WorkUnitProgressiveNumber = workUnitProgressiveNumber;
            Repetitions = repetitions;
            Rest = rest;
            Effort = effort;
            Tempo = tempo;
            IntensityTechniquesIds = intensityTechniquesIds;
        }




    }
}
