using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanNoteAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class WriteTrainingPlanNoteCommandHandler : IRequestHandler<WriteTrainingPlanNoteCommand, bool>
    {

        private readonly IAthleteRepository _athleteRepository;
        private readonly ITrainingPlanNoteRepository _noteRepository;
        private readonly ILogger<WriteTrainingPlanNoteCommandHandler> _logger;



        public WriteTrainingPlanNoteCommandHandler(IAthleteRepository athleteRepository, ITrainingPlanNoteRepository noteRepository, ILogger<WriteTrainingPlanNoteCommandHandler> logger)
        {
            _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _noteRepository = noteRepository ?? throw new ArgumentNullException(nameof(noteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(WriteTrainingPlanNoteCommand message, CancellationToken cancellationToken)
        {
            TrainingPlanNoteRoot note;
            _logger.LogInformation("----- Writing new note by {@UserId} for {@TrainingPlanId}", message.TrainingPlanId, message.UserId);
            try
            {
                note = TrainingPlanNoteRoot.WriteTransient(PersonalNoteValue.Write(message.NoteBody));
                _noteRepository.Add(note);
                await _noteRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _noteRepository.UnitOfWork);
                return false;
            }            
            try
            {
                AthleteRoot athlete = _athleteRepository.Find(message.UserId);
                athlete.AttachTrainingPlanNote(message.TrainingPlanId, note.Id);

                _athleteRepository.Modify(athlete);
                return await _athleteRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _athleteRepository.UnitOfWork);
                return false;
            }
        }

    }
}
