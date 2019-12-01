using GymProject.Domain.TrainingDomain.AthleteAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class UntagTrainingPlanWithProficiencyCommandHandler : IRequestHandler<UntagTrainingPlanWithProficiencyCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepo;
        private readonly ILogger<UntagTrainingPlanWithProficiencyCommandHandler> _logger;





        public UntagTrainingPlanWithProficiencyCommandHandler(
            IAthleteRepository athleteRepository,
            ILogger<UntagTrainingPlanWithProficiencyCommandHandler> logger
            )
        {
            _athleteRepo = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(UntagTrainingPlanWithProficiencyCommand message, CancellationToken cancellationToken)
        {
            bool result;

            try
            {
                AthleteRoot athlete = _athleteRepo.Find(message.UserId);

                _logger.LogInformation("----- Tagging {@UserTrainingPlan} of {@Athlete} with Proficiency {@ProficiencyId}", message.TrainingPlanId, athlete, message.ProficiencyId);

                athlete.UnlinkTrainingPlanTargetProficiency(message.TrainingPlanId, message.ProficiencyId);
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
