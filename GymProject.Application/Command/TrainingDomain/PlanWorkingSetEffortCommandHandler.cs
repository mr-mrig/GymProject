using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class PlanWorkingSetEffortCommandHandler : IRequestHandler<PlanWorkingSetEffortCommand, bool>
    {

        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly ILogger<PlanWorkingSetEffortCommandHandler> _logger;



        public PlanWorkingSetEffortCommandHandler(
            IWorkoutTemplateRepository workoutRepository,
            ILogger<PlanWorkingSetEffortCommandHandler> logger)
        {
            _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(PlanWorkingSetEffortCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            WorkoutTemplateRoot workout = _workoutRepository.Find(message.WorkoutTemplateId);

            if (workout == null)
                return false;

            try
            {
                TrainingEffortValue effort = ParseEffort(message.EffortValue, message.EffortTypeId);


                workout.ReviseWorkingSetEffort(message.WorkUnitProgressiveNumber, message.WorkingSetProgressiveNumber, effort);

                _logger.LogInformation("----- Setting effort for Working Set [{@WorkingSetProgressiveNumber} of {@WorkUnitProgressiveNumber}] in {@workout.Id}"
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

        private TrainingEffortValue ParseEffort(int? effortValue, int? effortTypeId)
        {
            if (!effortValue.HasValue)
                return null;

            TrainingEffortTypeEnum effortType = effortTypeId.HasValue
                ? TrainingEffortTypeEnum.From(effortTypeId.Value)
                : TrainingEffortTypeEnum.IntensityPercentage;


            return TrainingEffortValue.FromEffort(effortValue.Value, effortType);
        }
    }
}
