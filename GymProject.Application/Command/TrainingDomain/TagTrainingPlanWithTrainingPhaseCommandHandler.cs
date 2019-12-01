using GymProject.Domain.TrainingDomain.AthleteAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class TagTrainingPlanWithTrainingPhaseCommandHandler : IRequestHandler<TagTrainingPlanWithTrainingPhaseCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepo;
        private readonly ILogger<TagTrainingPlanWithTrainingPhaseCommandHandler> _logger;





        public TagTrainingPlanWithTrainingPhaseCommandHandler(
            IAthleteRepository athleteRepository,
            ILogger<TagTrainingPlanWithTrainingPhaseCommandHandler> logger
            )
        {
            _athleteRepo = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(TagTrainingPlanWithTrainingPhaseCommand message, CancellationToken cancellationToken)
        {
            bool result;

            try
            {
                AthleteRoot athlete = _athleteRepo.Find(message.UserId);

                _logger.LogInformation("----- Tagging {@UserTrainingPlanId} of {@Athlete} with Phase {@PhaseId}", message.TrainingPlanId, athlete, message.PhaseId);

                athlete.TagTrainingPlanWithPhase(message.TrainingPlanId, message.PhaseId);
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
