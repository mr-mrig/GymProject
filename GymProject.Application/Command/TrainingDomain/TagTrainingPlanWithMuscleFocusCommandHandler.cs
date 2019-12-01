using GymProject.Domain.TrainingDomain.AthleteAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class TagTrainingPlanWithMuscleFocusCommandHandler : IRequestHandler<TagTrainingPlanWithMuscleFocusCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepository;
        private readonly ILogger<TagTrainingPlanWithMuscleFocusCommandHandler> _logger;





        public TagTrainingPlanWithMuscleFocusCommandHandler(
            IAthleteRepository athleteRepository,
            ILogger<TagTrainingPlanWithMuscleFocusCommandHandler> logger
            )
        {
            _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(TagTrainingPlanWithMuscleFocusCommand message, CancellationToken cancellationToken)
        {
            bool result;

            try
            {
                AthleteRoot athlete = _athleteRepository.Find(message.UserId);

                _logger.LogInformation("----- Tagging {@UserTrainingPlanId} of {@User} with Muscle Focus {@MuscleId}", message.TrainingPlanId, athlete, message.MuscleId);

                athlete.FocusTrainingPlanOnMuscle(message.TrainingPlanId, message.MuscleId);
                _athleteRepository.Modify(athlete);

                result = await _athleteRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch(Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _athleteRepository.UnitOfWork);
                result = false;
            }

            return result;
        }
    }

}
