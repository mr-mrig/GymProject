using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class RemoveWorkingSetIntensityTechniqueCommandHandler : IRequestHandler<RemoveWorkingSetIntensityTechniqueCommand, bool>
    {

        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly ILogger<RemoveWorkingSetIntensityTechniqueCommandHandler> _logger;



        public RemoveWorkingSetIntensityTechniqueCommandHandler(IWorkoutTemplateRepository workoutRepository, ILogger<RemoveWorkingSetIntensityTechniqueCommandHandler> logger)
        {
            _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(RemoveWorkingSetIntensityTechniqueCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            WorkoutTemplateRoot workout = _workoutRepository.Find(message.WorkoutTemplateId);

            if (workout == null)
                return false;

            try
            {
                workout.RemoveWorkingSetIntensityTechnique(message.WorkUnitProgressiveNumber, message.WorkingSetProgressiveNumber, message.IntensityTechniqueId);

                _logger.LogInformation("----- Removing WS intensity Technique {@TechniqueId} to [{@WorkingSetProgressiveNumber} - {@WorkUnitProgressiveNumber}] in {@workout.Id}"
                    , message.IntensityTechniqueId, message.WorkingSetProgressiveNumber, message.WorkUnitProgressiveNumber, workout.Id);

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
