using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class PlanWorkingSetRepetitionsCommandHandler : IRequestHandler<PlanWorkingSetRepetitionsCommand, bool>
    {

        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly ILogger<PlanWorkingSetRepetitionsCommandHandler> _logger;



        public PlanWorkingSetRepetitionsCommandHandler(
            IWorkoutTemplateRepository workoutRepository,
            ILogger<PlanWorkingSetRepetitionsCommandHandler> logger)
        {
            _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(PlanWorkingSetRepetitionsCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            WorkoutTemplateRoot workout = _workoutRepository.Find(message.WorkoutTemplateId);

            if (workout == null)
                return false;

            try
            {
                WSWorkTypeEnum workType = WSWorkTypeEnum.From(message.WorkTypeId);
                WSRepetitionsValue repetitions;

                //switch(workType)
                //{
                //    case _ when workType == WSWorkTypeEnum.RepetitionBasedSerie:

                //        if (message.RepetitionsValue == WSRepetitionsValue.AMRAPValue)
                //            repetitions = WSRepetitionsValue.TrackAMRAP();

                //        break;

                //    case _ when workType == WSWorkTypeEnum.TimeBasedSerie:

                //        break;

                //    default:

                //        break;
                //}

                if (message.RepetitionsValue == WSRepetitionsValue.AMRAPValue && workType == WSWorkTypeEnum.RepetitionBasedSerie)
                    repetitions = WSRepetitionsValue.TrackAMRAP();
                else
                    repetitions = WSRepetitionsValue.TrackWork(message.RepetitionsValue, workType);


                workout.ReviseWorkingSetRepetitions(message.WorkUnitProgressiveNumber, message.WorkingSetProgressiveNumber, repetitions);


                _logger.LogInformation("----- Setting repetitions for Working Set [{@WorkingSetProgressiveNumber} of {@WorkUnitProgressiveNumber}] in {@workout.Id}"
                    ,message.WorkingSetProgressiveNumber, message.WorkUnitProgressiveNumber, workout.Id);

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

    }
}
