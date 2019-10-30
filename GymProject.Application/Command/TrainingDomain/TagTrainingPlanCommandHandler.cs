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
    public class TagTrainingPlanCommandHandler : IRequestHandler<TagTrainingPlanCommand, bool>
    {


        private readonly ITrainingPlanRepository _trainingPlanRepository;
        private readonly ILogger<TagTrainingPlanCommandHandler> _logger;





        public TagTrainingPlanCommandHandler(
            ITrainingPlanRepository trainingPlanRepository,
            ILogger<TagTrainingPlanCommandHandler> logger
            )
        {
            _trainingPlanRepository = trainingPlanRepository ?? throw new ArgumentNullException(nameof(trainingPlanRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(TagTrainingPlanCommand message, CancellationToken cancellationToken)
        {
            TrainingPlanRoot plan = _trainingPlanRepository.Find(message.TrainingPlanId);
            plan.TagAs(message.TrainingHashtagId);

            _logger.LogInformation("----- Tagging {@TrainingPlan} with {@HashtagId}", plan, message.TrainingHashtagId);

            _trainingPlanRepository.Modify(plan);

            return await _trainingPlanRepository.UnitOfWork.SaveAsync(cancellationToken);
        }
    }

}
