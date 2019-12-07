using GymProject.Domain.TrainingDomain.AthleteAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class MakeTrainingPlanVariantOfCommandHandler : IRequestHandler<MakeTrainingPlanVariantOfCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepository;
        private readonly ILogger<MakeTrainingPlanVariantOfCommandHandler> _logger;





        public MakeTrainingPlanVariantOfCommandHandler(IAthleteRepository athleteRepository,ILogger<MakeTrainingPlanVariantOfCommandHandler> logger)
        {
            _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(MakeTrainingPlanVariantOfCommand message, CancellationToken cancellationToken)
        {
            try
            {
                AthleteRoot athlete = _athleteRepository.Find(message.UserId);

                _logger.LogInformation("----- Making Training Plan {@PlanId} of Athlete {@Athlete} variant of {@ParentId}", message.TrainingPlanId, athlete, message.ParentPlanId);

                athlete.MakeTrainingPlanVariantOf(message.TrainingPlanId, message.ParentPlanId);
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
