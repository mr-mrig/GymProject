using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class AddWorkingSetIntensityTechniqueCommandHandler : IRequestHandler<AddWorkingSetIntensityTechniqueCommand, bool>
    {

        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly ILogger<AddWorkingSetIntensityTechniqueCommandHandler> _logger;



        public AddWorkingSetIntensityTechniqueCommandHandler(IWorkoutTemplateRepository workoutRepository, ILogger<AddWorkingSetIntensityTechniqueCommandHandler> logger)
        {
            _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(AddWorkingSetIntensityTechniqueCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            WorkoutTemplateRoot workout = _workoutRepository.Find(message.WorkoutTemplateId);

            if (workout == null)
                return false;

            try
            {
                workout.AddWorkingSetIntensityTechnique(message.WorkUnitProgressiveNumber, message.WorkingSetProgressiveNumber, message.IntensityTechniqueId);

                _logger.LogInformation("----- Adding WS intensity Technique {@TechniqueId}  to [{@WorkingSetProgressiveNumber} - {@WorkUnitProgressiveNumber}] in {@workout.Id}"
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
