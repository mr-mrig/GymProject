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
    public class FakeWriteWorkUnitTemplateNoteCommandHandler : IRequestHandler<WriteWorkUnitTemplateNoteCommand, bool>
    {

        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly IWorkUnitTemplateNoteRepository _noteRepository;
        private readonly ILogger<FakeWriteWorkUnitTemplateNoteCommandHandler> _logger;
        bool _failStep1 = false;
        bool _failStep2 = false;



        public FakeWriteWorkUnitTemplateNoteCommandHandler(
            IWorkoutTemplateRepository workoutRepository, IWorkUnitTemplateNoteRepository workUnitNoteRepository, ILogger<FakeWriteWorkUnitTemplateNoteCommandHandler> logger,
            bool failStep1, bool failStep2)
        {
            _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
            _noteRepository = workUnitNoteRepository ?? throw new ArgumentNullException(nameof(workUnitNoteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _failStep1 = failStep1;
            _failStep2 = failStep2;
        }


        public async Task<bool> Handle(WriteWorkUnitTemplateNoteCommand message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Writing new note for {@WorkUnitProgressiveNumber} of {@WorkoutId}", message.WorkUnitProgressiveNumber, message.WorkoutTemplateId);

            WorkUnitTemplateNoteRoot note;

            try
            {
                note = WorkUnitTemplateNoteRoot.WriteTransient(PersonalNoteValue.Write(message.NoteBody));
                _noteRepository.Add(note);

                if (_failStep1)
                    throw new Exception();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _noteRepository.UnitOfWork);
                return false;
            }
            try
            {
                WorkoutTemplateRoot workout = _workoutRepository.Find(message.WorkoutTemplateId);
                workout.AttachWorkUnitNote(message.WorkUnitProgressiveNumber, note.Id);

                if (_failStep2)
                    throw new Exception();

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
