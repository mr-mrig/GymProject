using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class TagTrainingPlanWithTrainingPhaseCommandHandler : IRequestHandler<TagTrainingPlanWithTrainingPhaseCommand, bool>
    {


        private readonly ITrainingPlanRepository _planRepository;
        private readonly ILogger<TagTrainingPlanWithTrainingPhaseCommandHandler> _logger;





        public TagTrainingPlanWithTrainingPhaseCommandHandler(
            ITrainingPlanRepository planRepository,
            ILogger<TagTrainingPlanWithTrainingPhaseCommandHandler> logger
            )
        {
            _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(TagTrainingPlanWithTrainingPhaseCommand message, CancellationToken cancellationToken)
        {
            bool result;

            try
            {
                TrainingPlanRoot plan = _planRepository.Find(message.TrainingPlanId);

                _logger.LogInformation("----- Tagging {@TrainingPlan} with Phase {@PhaseId}", plan, message.PhaseId);

                plan.TagPhase(message.PhaseId);
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
