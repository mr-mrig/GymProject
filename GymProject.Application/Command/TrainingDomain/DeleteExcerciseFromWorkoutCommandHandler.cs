using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class DeleteExcerciseFromWorkoutCommandHandler : IRequestHandler<DeleteExcerciseFromWorkoutCommand, bool>
    {

        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly ILogger<DeleteExcerciseFromWorkoutCommandHandler> _logger;



        public DeleteExcerciseFromWorkoutCommandHandler(
            IWorkoutTemplateRepository workoutRepository,
            ILogger<DeleteExcerciseFromWorkoutCommandHandler> logger)
        {
            _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(DeleteExcerciseFromWorkoutCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            WorkoutTemplateRoot workout = _workoutRepository.Find(message.WorkoutTemplateId);

            if (workout == null)
                return false;

            try
            {
                workout.UnplanExcercise(message.WorkUnitProgressiveNumber);


                _logger.LogInformation("----- Deleting Work Unit - {@WorkUnit}", message.WorkUnitProgressiveNumber);

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
