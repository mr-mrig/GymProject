using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Domain.TrainingDomain.WorkUnitTemplateNote;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class WriteWorkUnitTemplateNoteCommandHandler : IRequestHandler<WriteWorkUnitTemplateNoteCommand, bool>
    {

        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly IWorkUnitTemplateNoteRepository _noteRepository;
        private readonly ILogger<WriteWorkUnitTemplateNoteCommandHandler> _logger;



        public WriteWorkUnitTemplateNoteCommandHandler(
            IWorkoutTemplateRepository workoutRepository, IWorkUnitTemplateNoteRepository workUnitNoteRepository, ILogger<WriteWorkUnitTemplateNoteCommandHandler> logger)
        {
            _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
            _noteRepository = workUnitNoteRepository ?? throw new ArgumentNullException(nameof(workUnitNoteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(WriteWorkUnitTemplateNoteCommand message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Writing new note for {@WorkUnitProgressiveNumber} of {@WorkoutId}", message.WorkUnitProgressiveNumber, message.WorkoutTemplateId);

            WorkoutTemplateRoot workout = _workoutRepository.Find(message.WorkoutTemplateId);
            WorkUnitTemplateNoteRoot note;

            try
            {
                note = WorkUnitTemplateNoteRoot.Write(PersonalNoteValue.Write(message.NoteBody));
                _noteRepository.Add(note);

                if (!await _noteRepository.UnitOfWork.SaveAsync(cancellationToken))
                    return await Task.FromResult(false);    // Transaction
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _noteRepository.UnitOfWork);
                return false;
            }
            try
            {
                workout.AttachWorkUnitNote(message.WorkUnitProgressiveNumber, note.Id);

                _workoutRepository.Modify(workout);
                return await _workoutRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _workoutRepository.UnitOfWork);
                return false;
            }
        }

    }
}
