using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class RemoveTrainingScheduleFeedbackCommandHandler : IRequestHandler<RemoveTrainingScheduleFeedbackCommand, bool>
    {

        private readonly ITrainingScheduleRepository _scheduleRepository;
        private readonly ILogger<RemoveTrainingScheduleFeedbackCommandHandler> _logger;



        public RemoveTrainingScheduleFeedbackCommandHandler(ITrainingScheduleRepository scheduleRepository, ILogger<RemoveTrainingScheduleFeedbackCommandHandler> logger)
        {
            _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(RemoveTrainingScheduleFeedbackCommand message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Removing feedback for {@ScheduleId} by {@OwnerId}", message.ScheduleId, message.OwnerId);
            try
            {
                TrainingScheduleRoot schedule = _scheduleRepository.Find(message.ScheduleId);
                schedule.RemoveFeedback(message.OwnerId);

                _scheduleRepository.Modify(schedule);

                return await _scheduleRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _scheduleRepository.UnitOfWork);
                return false;
            }            
        }

    }
}
