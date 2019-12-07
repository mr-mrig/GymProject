using GymProject.Domain.TrainingDomain.AthleteAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class RenameTrainingPlanCommandHandler : IRequestHandler<RenameTrainingPlanCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepo;
        private readonly ILogger<RenameTrainingPlanCommandHandler> _logger;





        public RenameTrainingPlanCommandHandler(
            IAthleteRepository athleteRepository,
            ILogger<RenameTrainingPlanCommandHandler> logger
            )
        {
            _athleteRepo = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(RenameTrainingPlanCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            try
            {
                AthleteRoot athlete = _athleteRepo.Find(message.AthleteId);
                athlete.RenameTrainingPlan(message.TrainingPlanId, message.TrainingPlanName);

                _logger.LogInformation("----- Renaming {@TrainingPlanId} of {@Athlete} with {@TrainingPlanName}", message.TrainingPlanId, athlete, message.TrainingPlanName);

                _athleteRepo.Modify(athlete);

                result = await _athleteRepo.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _athleteRepo.UnitOfWork);
                result = false;
            }
            return result;
        }
    }

}
