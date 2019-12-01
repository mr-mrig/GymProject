using GymProject.Domain.TrainingDomain.AthleteAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class UntagTrainingPlanWithTrainingPhaseCommandHandler : IRequestHandler<UntagTrainingPlanWithTrainingPhaseCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepo;
        private readonly ILogger<UntagTrainingPlanWithTrainingPhaseCommandHandler> _logger;





        public UntagTrainingPlanWithTrainingPhaseCommandHandler(
            IAthleteRepository athleteRepository,
            ILogger<UntagTrainingPlanWithTrainingPhaseCommandHandler> logger
            )
        {
            _athleteRepo = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(UntagTrainingPlanWithTrainingPhaseCommand message, CancellationToken cancellationToken)
        {
            bool result;

            try
            {
                AthleteRoot athlete = _athleteRepo.Find(message.UserId);

                _logger.LogInformation("----- Unagging {@UserTrainingPlanId} of {@Athlete} with Phase {@PhaseId}", message.TrainingPlanId, athlete, message.PhaseId);

                athlete.UntagTrainingPlanWithPhase(message.TrainingPlanId, message.PhaseId);
                _athleteRepo.Modify(athlete);

                result = await _athleteRepo.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch(Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _athleteRepo.UnitOfWork);
                result = false;
            }

            return result;
        }
    }

}
