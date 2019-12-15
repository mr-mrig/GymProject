using GymProject.Application.Command.TrainingDomain;
using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace GymProject.Application.Test.FakeCommands
{

    // Regular CommandHandler
    public class FakeCreateDraftTrainingPlanCommandHandler : IRequestHandler<CreateDraftTrainingPlanCommand, bool>
    {


        private readonly ITrainingPlanRepository _trainingPlanRepository;
        private readonly IAthleteRepository _athleteRepository;
        private readonly ILogger<FakeCreateDraftTrainingPlanCommandHandler> _logger;





        public FakeCreateDraftTrainingPlanCommandHandler(
            ITrainingPlanRepository trainingPlanRepository,
            IAthleteRepository athleteRepository,
            ILogger<FakeCreateDraftTrainingPlanCommandHandler> logger
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
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _trainingPlanRepository.UnitOfWork);
                return false;
            }

            try
            {
                throw new Exception("Simulating a failure in the second operation");
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _athleteRepository.UnitOfWork);
                return false;
            }
        }
    }

}
