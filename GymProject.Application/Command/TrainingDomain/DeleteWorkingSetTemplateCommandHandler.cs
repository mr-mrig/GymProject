using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class DeleteWorkingSetTemplateCommandHandler : IRequestHandler<DeleteWorkingSetTemplateCommand, bool>
    {

        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly ILogger<DeleteWorkingSetTemplateCommandHandler> _logger;



        public DeleteWorkingSetTemplateCommandHandler(
            IWorkoutTemplateRepository workoutRepository,
            ILogger<DeleteWorkingSetTemplateCommandHandler> logger)
        {
            _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(DeleteWorkingSetTemplateCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            WorkoutTemplateRoot workout = _workoutRepository.Find(message.WorkoutTemplateId);

            if (workout == null)
                return false;

            try
            {
                workout.RemoveWorkingSet(message.WorkUnitProgressiveNumber, message.WorkingSetProgressiveNumber);


                _logger.LogInformation("----- Deleting Working Set [{@WorkUnitProgressiveNumber} - {@WorkingSetProgressiveNumber}] from {@WorkoutTemplateId}"
                    , message.WorkUnitProgressiveNumber, message.WorkingSetProgressiveNumber, message.WorkoutTemplateId);

                _workoutRepository.Modify(workout);
                result = await _workoutRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc,"Transaction failed");
                result = false;
            }

            return result;
        }

    }
}
