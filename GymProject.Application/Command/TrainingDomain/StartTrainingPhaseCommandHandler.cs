using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    //public class StartTrainingPhaseCommandHandler : IRequestHandler<StartTrainingPhaseCommand, bool>
    //{


    //    private readonly ITrainingPhaseRepository _phaseRepository;
    //    private readonly ILogger<StartTrainingPhaseCommandHandler> _logger;





    //    public StartTrainingPhaseCommandHandler(
    //        ITrainingPhaseRepository phaseRepository,
    //        ILogger<StartTrainingPhaseCommandHandler> logger
    //        )
    //    {
    //        _phaseRepository = phaseRepository ?? throw new ArgumentNullException(nameof(phaseRepository));
    //        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    //    }




    //    public async Task<bool> Handle(StartTrainingPhaseCommand message, CancellationToken cancellationToken)
    //    {
    //        bool result;

    //        try
    //        {
    //            TrainingPhaseRoot phase = TrainingPhaseRoot.CreateTrainingPhase(message.PhaseName,
    //               EntryStatusTypeEnum.From((int)message.EntryStatusId));

    //            _logger.LogInformation("----- Creating Training Phase - {@Plan}", phase);

    //            _phaseRepository.Add(phase);

    //            result = await _phaseRepository.UnitOfWork.SaveAsync(cancellationToken);
    //        }
    //        catch(Exception exc)
    //        {
    //            _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _phaseRepository.UnitOfWork);
    //            result = false;
    //        }

    //        return result;
    //    }
    //}

}
