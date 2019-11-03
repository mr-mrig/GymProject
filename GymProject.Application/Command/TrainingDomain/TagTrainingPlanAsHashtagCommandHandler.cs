using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class TagTrainingPlanAsHashtagCommandHandler : IRequestHandler<TagTrainingPlanAsHashtagCommand, bool>
    {


        private readonly ITrainingPlanRepository _trainingPlanRepository;
        private readonly ILogger<TagTrainingPlanAsHashtagCommandHandler> _logger;





        public TagTrainingPlanAsHashtagCommandHandler(
            ITrainingPlanRepository trainingPlanRepository,
            ILogger<TagTrainingPlanAsHashtagCommandHandler> logger
            )
        {
            _trainingPlanRepository = trainingPlanRepository ?? throw new ArgumentNullException(nameof(trainingPlanRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(TagTrainingPlanAsHashtagCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            try
            {
                TrainingPlanRoot plan = _trainingPlanRepository.Find(message.TrainingPlanId);
                plan.TagAs(message.TrainingHashtagId);

                _logger.LogInformation("----- Tagging {@TrainingPlan} with {@HashtagId}", plan, message.TrainingHashtagId);

                _trainingPlanRepository.Modify(plan);

                result = await _trainingPlanRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _trainingPlanRepository.UnitOfWork);
                result = false;
            }
            return result;
        }
    }

}
