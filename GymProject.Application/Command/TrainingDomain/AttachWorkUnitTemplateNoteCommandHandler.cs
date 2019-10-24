using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class AttachWorkUnitTemplateNoteCommandHandler : IRequestHandler<AttachWorkUnitTemplateNoteCommand, bool>
    {

        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly ILogger<AttachWorkUnitTemplateNoteCommandHandler> _logger;



        public AttachWorkUnitTemplateNoteCommandHandler(IWorkoutTemplateRepository workoutRepository, ILogger<AttachWorkUnitTemplateNoteCommandHandler> logger)
        {
            _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(AttachWorkUnitTemplateNoteCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            WorkoutTemplateRoot workout = _workoutRepository.Find(message.WorkoutTemplateId);

            if (workout == null)
                return false;

            try
            {
                workout.AttachWorkUnitNote(message.WorkUnitProgressiveNumber, message.WorkUnitNoteId);

                _logger.LogInformation("----- Attaching {@NoteId} to {@WorkUnitProgressiveNumber} in {@workout.Id}"
                    , message.WorkUnitNoteId, message.WorkUnitProgressiveNumber, workout.Id);

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
