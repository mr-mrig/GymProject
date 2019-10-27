using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class DetachTrainingPlanNoteCommandHandler : IRequestHandler<DetachTrainingPlanNoteCommand, bool>
    {

        private readonly ITrainingPlanRepository _trainingPlanRepository;
        private readonly ILogger<DetachTrainingPlanNoteCommandHandler> _logger;



        public DetachTrainingPlanNoteCommandHandler(ITrainingPlanRepository trainingPlanRepository, ILogger<DetachTrainingPlanNoteCommandHandler> logger)
        {
            _trainingPlanRepository = trainingPlanRepository ?? throw new ArgumentNullException(nameof(trainingPlanRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(DetachTrainingPlanNoteCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            TrainingPlanRoot plan = _trainingPlanRepository.Find(message.TrainingPlanId);

            if (plan == null)
                return false;

            try
            {
                plan.CleanNote();

                _logger.LogInformation("----- Detaching TrainingPlanNote to {@TrainingPlanId}", message.TrainingPlanId);

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
