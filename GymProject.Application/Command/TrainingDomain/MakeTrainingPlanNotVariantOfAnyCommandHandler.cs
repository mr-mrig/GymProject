using GymProject.Domain.TrainingDomain.AthleteAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class MakeTrainingPlanNotVariantOfAnyCommandHandler : IRequestHandler<MakeTrainingPlanNotVariantOfAnyCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepository;
        private readonly ILogger<MakeTrainingPlanNotVariantOfAnyCommandHandler> _logger;





        public MakeTrainingPlanNotVariantOfAnyCommandHandler(IAthleteRepository athleteRepository,ILogger<MakeTrainingPlanNotVariantOfAnyCommandHandler> logger)
        {
            _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(MakeTrainingPlanNotVariantOfAnyCommand message, CancellationToken cancellationToken)
        {
            try
            {
                AthleteRoot athlete = _athleteRepository.Find(message.UserId);

                _logger.LogInformation("----- Making Training Plan {@PlanId} of Athlete {@Athlete} variant of nothing", message.TrainingPlanId, athlete);

                athlete.MakeTrainingPlanNotVariantOfAny(message.TrainingPlanId);
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
