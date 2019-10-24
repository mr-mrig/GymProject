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
    public class PlanWorkingSetRestCommandHandler : IRequestHandler<PlanWorkingSetRestCommand, bool>
    {

        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly ILogger<PlanWorkingSetRestCommandHandler> _logger;



        public PlanWorkingSetRestCommandHandler(IWorkoutTemplateRepository workoutRepository, ILogger<PlanWorkingSetRestCommandHandler> logger)
        {
            _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(PlanWorkingSetRestCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            WorkoutTemplateRoot workout = _workoutRepository.Find(message.WorkoutTemplateId);

            if (workout == null)
                return false;

            try
            {
                TimeMeasureUnitEnum restUnit = TimeMeasureUnitEnum.From(message.RestMeasUnitId);    // Null if unit not found


                workout.ReviseWorkingSetRestPeriod(
                    message.WorkUnitProgressiveNumber, 
                    message.WorkingSetProgressiveNumber, 
                    RestPeriodValue.SetRest(message.RestValue, restUnit));

                _logger.LogInformation("----- Setting WS rest of [{@WorkingSetProgressiveNumber} - {@WorkUnitProgressiveNumber}] in {@workout.Id}"
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
