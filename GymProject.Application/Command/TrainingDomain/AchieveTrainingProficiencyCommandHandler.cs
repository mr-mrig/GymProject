using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    public class AchieveTrainingProficiencyCommandHandler : IRequestHandler<AchieveTrainingProficiencyCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepository;
        private readonly ILogger<AchieveTrainingProficiencyCommandHandler> _logger;





        public AchieveTrainingProficiencyCommandHandler(IAthleteRepository athleteRepository,ILogger<AchieveTrainingProficiencyCommandHandler> logger)
        {
            _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(AchieveTrainingProficiencyCommand message, CancellationToken cancellationToken)
        {
            try
            {
                AthleteRoot athlete = _athleteRepository.Find(message.AthleteId);

                _logger.LogInformation("----- Achieving Training Proficiency {@ProficiencyId} for Athlete {@Athlete}", message.TrainingProficiencyId, athlete);

                // If another proficiency has been started today, the first of the two is a mistake -> remove it
                // This application logic, not domain
                var currentProficiency = athlete.CurrentTrainingProficiency;
                if (currentProficiency != null && currentProficiency.StartDate.Date == DateTime.UtcNow.Date)
                    athlete.RemoveTrainingProficiency(currentProficiency);

                athlete.AchieveTrainingProficiency(message.TrainingProficiencyId);
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
