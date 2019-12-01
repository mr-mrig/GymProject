using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class DetachTrainingPlanNoteCommandHandler : IRequestHandler<DetachTrainingPlanNoteCommand, bool>
    {

        private readonly IAthleteRepository _athleteRepository;
        private readonly ILogger<DetachTrainingPlanNoteCommandHandler> _logger;



        public DetachTrainingPlanNoteCommandHandler(IAthleteRepository athleteRepository, ILogger<DetachTrainingPlanNoteCommandHandler> logger)
        {
            _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(DetachTrainingPlanNoteCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            AthleteRoot athlete = _athleteRepository.Find(message.UserId);

            if (athlete == null)
                return false;

            try
            {
                athlete.CleanTrainingPlanNote(message.TrainingPlanId);

                _logger.LogInformation("----- Detaching TrainingPlanNote from {@UserTrainingPlanId} of {@UserId}", message.TrainingPlanId, message.UserId);

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
