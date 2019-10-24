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
        private readonly IWorkUnitTemplateNoteRepository _workUnitNoteRepository;
        private readonly ILogger<WriteWorkUnitTemplateNoteCommandHandler> _logger;



        public WriteWorkUnitTemplateNoteCommandHandler(
            IWorkoutTemplateRepository workoutRepository, IWorkUnitTemplateNoteRepository workUnitNoteRepository, ILogger<WriteWorkUnitTemplateNoteCommandHandler> logger)
        {
            _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
            _workUnitNoteRepository = workUnitNoteRepository ?? throw new ArgumentNullException(nameof(workUnitNoteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(WriteWorkUnitTemplateNoteCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            WorkoutTemplateRoot workout = _workoutRepository.Find(message.WorkoutTemplateId);
            WorkUnitTemplateNoteRoot note;

            if (workout == null)
                return false;

            try
            {
                note = WorkUnitTemplateNoteRoot.Write(PersonalNoteValue.Write(message.NoteBody));
                _workUnitNoteRepository.Add(note);

                _logger.LogInformation("----- Creating WorkUnitTemplateNote");

                result = await _workUnitNoteRepository.UnitOfWork.SaveAsync(cancellationToken);

                if (!result)
                {
                    _logger.LogError("ERROR handling message: Creating WorkUnitTemplateNote failed - Context: {@ExceptionContext}", _workoutRepository.UnitOfWork);
                    return false;
                }

                try
                {
                    workout.AttachWorkUnitNote(message.WorkUnitProgressiveNumber, note.Id);

                    _logger.LogInformation("----- Attaching {@NoteId} to {@WorkUnitProgressiveNumber} in {@workout.Id}"
                        , note.Id, message.WorkUnitProgressiveNumber, workout.Id);

                    _workoutRepository.Modify(workout);
                    result = await _workoutRepository.UnitOfWork.SaveAsync(cancellationToken);
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _workoutRepository.UnitOfWork);
                    result = false;
                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _workoutRepository.UnitOfWork);
                result = false;
            }


            //WorkUnitTemplateNoteCreationSuccessed(workout.Id);     // Link the Note to the WorkUnit by event

            return result;
        }

    }
}
