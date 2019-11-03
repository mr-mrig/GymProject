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


        private readonly ITrainingPlanRepository _planRepository;
        private readonly ILogger<UntagTrainingPlanWithMuscleFocusCommandHandler> _logger;





        public UntagTrainingPlanWithMuscleFocusCommandHandler(
            ITrainingPlanRepository planRepository,
            ILogger<UntagTrainingPlanWithMuscleFocusCommandHandler> logger
            )
        {
            _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(UntagTrainingPlanWithMuscleFocusCommand message, CancellationToken cancellationToken)
        {
            bool result;

            try
            {
                TrainingPlanRoot plan = _planRepository.Find(message.TrainingPlanId);

                _logger.LogInformation("----- Untagging {@TrainingPlan} with Muscle Focus {@MuscleId}", plan, message.MuscleId);

                plan.UnfocusMuscle(message.MuscleId);
                _planRepository.Modify(plan);

                result = await _planRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch(Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _planRepository.UnitOfWork);
                result = false;
            }

            return result;
        }
    }

}
