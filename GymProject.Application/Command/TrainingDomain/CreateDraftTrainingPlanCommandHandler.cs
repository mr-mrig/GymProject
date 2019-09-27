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
    public class CreateDraftTrainingPlanCommandHandler : IRequestHandler<CreateDraftTrainingPlanCommand, bool>
    {


        private readonly ITrainingPlanRepository _trainingPlanRepository;
        private readonly ILogger<CreateDraftTrainingPlanCommandHandler> _logger;





        public CreateDraftTrainingPlanCommandHandler(
            ITrainingPlanRepository trainingPlanRepository,
            ILogger<CreateDraftTrainingPlanCommandHandler> logger
            )
        {
            _trainingPlanRepository = trainingPlanRepository ?? throw new ArgumentNullException(nameof(trainingPlanRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(CreateDraftTrainingPlanCommand message, CancellationToken cancellationToken)
        {
            TrainingPlanRoot plan = TrainingPlanRoot.NewDraft(message.OwnerId);

            _logger.LogInformation("----- Creating Training Plan - Plan: {@Plan}", plan);

            _trainingPlanRepository.Add(plan);

            return await _trainingPlanRepository.UnitOfWork.SaveAsync(cancellationToken);
        }
    }


    //// Use for Idempotency in Command process
    //public class CreateOrderIdentifiedCommandHandler : IdentifiedCommandHandler<CreateDraftTrainingPlanCommand, bool>
    //{
    //    public CreateOrderIdentifiedCommandHandler(
    //        IMediator mediator,
    //        IRequestManager requestManager,
    //        ILogger<IdentifiedCommandHandler<CreateDraftTrainingPlanCommand, bool>> logger)
    //        : base(mediator, requestManager, logger)
    //    {
    //    }


    //    protected override bool CreateResultForDuplicateRequest()
    //    {
    //        return true;                // Ignore duplicate requests for creating order.
    //    }
    //}
}
