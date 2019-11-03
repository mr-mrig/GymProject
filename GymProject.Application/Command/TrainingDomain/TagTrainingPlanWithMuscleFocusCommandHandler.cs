using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
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


        private readonly ITrainingPlanRepository _planRepository;
        private readonly ILogger<TagTrainingPlanWithMuscleFocusCommandHandler> _logger;





        public TagTrainingPlanWithMuscleFocusCommandHandler(
            ITrainingPlanRepository planRepository,
            ILogger<TagTrainingPlanWithMuscleFocusCommandHandler> logger
            )
        {
            _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(TagTrainingPlanWithMuscleFocusCommand message, CancellationToken cancellationToken)
        {
            bool result;

            try
            {
                TrainingPlanRoot plan = _planRepository.Find(message.TrainingPlanId);

                _logger.LogInformation("----- Tagging {@TrainingPlan} with Muscle Focus {@MuscleId}", plan, message.MuscleId);

                plan.FocusOnMuscle(message.MuscleId);
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
