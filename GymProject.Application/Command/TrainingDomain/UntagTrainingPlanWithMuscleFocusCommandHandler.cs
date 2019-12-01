using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class UntagTrainingPlanWithMuscleFocusCommandHandler : IRequestHandler<UntagTrainingPlanWithMuscleFocusCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepo;
        private readonly ILogger<UntagTrainingPlanWithMuscleFocusCommandHandler> _logger;





        public UntagTrainingPlanWithMuscleFocusCommandHandler(
            IAthleteRepository athleteRepository,
            ILogger<UntagTrainingPlanWithMuscleFocusCommandHandler> logger
            )
        {
            _athleteRepo = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(UntagTrainingPlanWithMuscleFocusCommand message, CancellationToken cancellationToken)
        {
            bool result;

            try
            {
                AthleteRoot athlete = _athleteRepo.Find(message.UserId);

                _logger.LogInformation("----- Untagging {@UserTrainingPlanId} of {@Athlete} with Muscle Focus {@MuscleId}", message.TrainingPlanId, athlete, message.MuscleId);

                athlete.UnfocusTrainingPlanFromMuscle(message.TrainingPlanId, message.MuscleId);
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
