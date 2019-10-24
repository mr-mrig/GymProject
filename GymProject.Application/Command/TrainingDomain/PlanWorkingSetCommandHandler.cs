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
                workout.AddTransientWorkingSet(message.WorkUnitProgressiveNumber, message.Repetitions, message.Rest, message.Effort, message.Tempo, message.IntensityTechniquesIds);


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

    }
}
