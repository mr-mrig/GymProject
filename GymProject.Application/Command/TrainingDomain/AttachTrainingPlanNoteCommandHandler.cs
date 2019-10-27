using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class AttachTrainingPlanNoteCommandHandler : IRequestHandler<AttachTrainingPlanNoteCommand, bool>
    {

        private readonly ITrainingPlanRepository _trainingPlanRepository;
        private readonly ILogger<AttachTrainingPlanNoteCommandHandler> _logger;



        public AttachTrainingPlanNoteCommandHandler(ITrainingPlanRepository trainingPlanRepository, ILogger<AttachTrainingPlanNoteCommandHandler> logger)
        {
            _trainingPlanRepository = trainingPlanRepository ?? throw new ArgumentNullException(nameof(trainingPlanRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(AttachTrainingPlanNoteCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            TrainingPlanRoot plan = _trainingPlanRepository.Find(message.TrainingPlanId);

            if (plan == null)
                return false;

            try
            {
                plan.WriteNote(message.TrainingPlanNoteId);

                _logger.LogInformation("----- Attaching {@NoteId} to {@TrainingPlanId}", message.TrainingPlanNoteId, message.TrainingPlanId);

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
