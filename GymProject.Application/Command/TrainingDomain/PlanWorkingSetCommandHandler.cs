using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class PlanWorkingSetCommandHandler : IRequestHandler<PlanWorkingSetCommand, bool>
    {

        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly ILogger<PlanWorkingSetCommandHandler> _logger;



        public PlanWorkingSetCommandHandler(
            IWorkoutTemplateRepository workoutRepository,
            ILogger<PlanWorkingSetCommandHandler> logger)
        {
            _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(PlanWorkingSetCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            WorkoutTemplateRoot workout = _workoutRepository.Find(message.WorkoutTemplateId);

            if (workout == null)
                return false;

            try
            {
                WSRepetitionsValue repetitions = ParseRepetitions(message.RepetitionsValue, message.WorkTypeId);
                RestPeriodValue rest = ParseRest(message.RestValue, message.RestMeasUnitId);
                TrainingEffortValue effort = ParseEffort(message.EffortValue, message.EffortTypeId);
                TUTValue tempo = ParseTempo(message.Tempo);

                workout.AddTransientWorkingSet(message.WorkUnitProgressiveNumber, repetitions, rest, effort, tempo, message.IntensityTechniquesIds);


                _logger.LogInformation("----- Creating Working Set {@WorkUnitProgressiveNumber} in {@workout.Id}", message.WorkUnitProgressiveNumber, workout.Id);

                _workoutRepository.Modify(workout);
                result = await _workoutRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _workoutRepository.UnitOfWork);
                result = false;
            }

            return result;
        }


        private WSRepetitionsValue ParseRepetitions(int repetitionsValue, int? workTypeId)
        {
            //if (!repetitionsValue.HasValue)
            //    return null;

            WSWorkTypeEnum workType = workTypeId.HasValue
                ? WSWorkTypeEnum.From(workTypeId.Value)
                : WSWorkTypeEnum.RepetitionBasedSerie;

            WSRepetitionsValue repetitions;

            if (repetitionsValue == WSRepetitionsValue.AMRAPValue && workType == WSWorkTypeEnum.RepetitionBasedSerie)
                repetitions = WSRepetitionsValue.TrackAMRAP();
            else
                repetitions = WSRepetitionsValue.TrackWork(repetitionsValue, workType);

            return repetitions;
        }


        private RestPeriodValue ParseRest(int? restValue, int? restMeasUnitId)
        {
            if (!restValue.HasValue)
                return null;


            TimeMeasureUnitEnum restUnit = restMeasUnitId.HasValue
                ? TimeMeasureUnitEnum.From(restMeasUnitId.Value)    // Null if unit not found
                : TimeMeasureUnitEnum.Seconds;

            return RestPeriodValue.SetRest(restValue.Value, restUnit);
        }

        private TrainingEffortValue ParseEffort(int? effortValue, int? effortTypeId)
        {
            if(!effortValue.HasValue)
                return null;

            TrainingEffortTypeEnum effortType = effortTypeId.HasValue
                ? TrainingEffortTypeEnum.From(effortTypeId.Value)
                : TrainingEffortTypeEnum.IntensityPercentage;


            return TrainingEffortValue.FromEffort(effortValue.Value, effortType);
        }


        private TUTValue ParseTempo(string tempo)
        {
            TUTValue ret;

            if (string.IsNullOrWhiteSpace(tempo))
                ret = null;
            else
                ret = TUTValue.PlanTUT(tempo);

            return ret;
        }


    }
}
