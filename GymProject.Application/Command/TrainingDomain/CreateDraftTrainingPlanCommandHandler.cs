using GymProject.Domain.Base;
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
    public class CreateDraftTrainingPlanCommandHandler : IRequestHandler<CreateDraftTrainingPlanCommand, bool>
    {


        private readonly ITrainingPlanRepository _trainingPlanRepository;
        private readonly IAthleteRepository _athleteRepository;
        private readonly ILogger<CreateDraftTrainingPlanCommandHandler> _logger;





        public CreateDraftTrainingPlanCommandHandler(
            ITrainingPlanRepository trainingPlanRepository,
            IAthleteRepository athleteRepository,
            ILogger<CreateDraftTrainingPlanCommandHandler> logger
            )
        {
            _trainingPlanRepository = trainingPlanRepository ?? throw new ArgumentNullException(nameof(trainingPlanRepository));
            _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(CreateDraftTrainingPlanCommand message, CancellationToken cancellationToken)
        {
            TrainingPlanRoot plan;

            try
            {
                plan = TrainingPlanRoot.NewDraft(message.OwnerId);
                _logger.LogInformation("----- Creating Training Plan - Plan: {@Plan}", plan);

                _trainingPlanRepository.Add(plan);
                await _trainingPlanRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _trainingPlanRepository.UnitOfWork);
                return false;
            }

            try
            {
                AthleteRoot athlete = _athleteRepository.Find(message.OwnerId);
                _logger.LogInformation("----- Adding Training Plan {@Plan} to User Library {@Athlete}", plan, athlete);

                athlete.AddTrainingPlanToLibrary(plan.Id.Value);

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
