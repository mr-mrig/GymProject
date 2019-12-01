using GymProject.Domain.TrainingDomain.AthleteAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class UntagTrainingPlanAsHashtagCommandHandler : IRequestHandler<UntagTrainingPlanAsHashtagCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepo;
        private readonly ILogger<UntagTrainingPlanAsHashtagCommandHandler> _logger;





        public UntagTrainingPlanAsHashtagCommandHandler(
            IAthleteRepository athleteRepository,
            ILogger<UntagTrainingPlanAsHashtagCommandHandler> logger
            )
        {
            _athleteRepo = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(UntagTrainingPlanAsHashtagCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            try
            {
                AthleteRoot athlete = _athleteRepo.Find(message.UserId);
                athlete.UntagTrainingPlan(message.TrainingPlanId, message.TrainingHashtagId);

                _logger.LogInformation("----- Unagging {@UserTrainingPlanId} of {@Athlete} with {@HashtagId}", message.TrainingPlanId, athlete, message.TrainingHashtagId);

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
