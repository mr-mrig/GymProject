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
    public class FakeWriteTrainingPlanNoteCommandHandler : IRequestHandler<WriteTrainingPlanNoteCommand, bool>
    {

        private readonly IAthleteRepository _athleteRepository;
        private readonly ITrainingPlanNoteRepository _noteRepository;
        private readonly ILogger<FakeWriteTrainingPlanNoteCommandHandler> _logger;
        private bool _failStep1 = false;
        private bool _failStep2 = false;



        public FakeWriteTrainingPlanNoteCommandHandler(IAthleteRepository athleteRepository, ITrainingPlanNoteRepository noteRepository, ILogger<FakeWriteTrainingPlanNoteCommandHandler> logger,
            bool failStep1, bool failStep2)
        {
            _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _noteRepository = noteRepository ?? throw new ArgumentNullException(nameof(noteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _failStep1 = failStep1;
            _failStep2 = failStep2;
        }


        public async Task<bool> Handle(WriteTrainingPlanNoteCommand message, CancellationToken cancellationToken)
        {
            TrainingPlanNoteRoot note;
            _logger.LogInformation("----- Writing new note by {@UserId} for {@TrainingPlanId}", message.TrainingPlanId, message.UserId);
            try
            {
                note = TrainingPlanNoteRoot.WriteTransient(PersonalNoteValue.Write(message.NoteBody));
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
                AthleteRoot athlete = _athleteRepository.Find(message.UserId);
                athlete.AttachTrainingPlanNote(message.TrainingPlanId, note.Id);

                if (_failStep2)
                    throw new Exception();

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
