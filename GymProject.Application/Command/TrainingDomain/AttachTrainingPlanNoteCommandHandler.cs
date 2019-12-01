using GymProject.Domain.TrainingDomain.AthleteAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class AttachTrainingPlanNoteCommandHandler : IRequestHandler<AttachTrainingPlanNoteCommand, bool>
    {

        private readonly IAthleteRepository _athleteRepository;
        private readonly ILogger<AttachTrainingPlanNoteCommandHandler> _logger;



        public AttachTrainingPlanNoteCommandHandler(IAthleteRepository athleteRepository, ILogger<AttachTrainingPlanNoteCommandHandler> logger)
        {
            _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(AttachTrainingPlanNoteCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            AthleteRoot athlete = _athleteRepository.Find(message.UserId);

            if (athlete == null)
                return false;

            try
            {
                athlete.AttachTrainingPlanNote(message.TrainingPlanId, message.TrainingPlanNoteId);

                _logger.LogInformation("----- Attaching {@NoteId} to {@TrainingPlanId} of {@UserId}", message.TrainingPlanNoteId, message.TrainingPlanId, message.UserId);

                _athleteRepository.Modify(athlete);
                result = await _athleteRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _athleteRepository.UnitOfWork);
                result = false;
            }

            return result;
        }

    }
}
