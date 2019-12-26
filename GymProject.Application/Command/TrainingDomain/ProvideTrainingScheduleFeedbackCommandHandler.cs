using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class ProvideTrainingScheduleFeedbackCommandHandler : IRequestHandler<ProvideTrainingScheduleFeedbackCommand, bool>
    {

        private readonly ITrainingScheduleRepository _scheduleRepository;
        private readonly ILogger<ProvideTrainingScheduleFeedbackCommandHandler> _logger;



        public ProvideTrainingScheduleFeedbackCommandHandler(ITrainingScheduleRepository scheduleRepository, ILogger<ProvideTrainingScheduleFeedbackCommandHandler> logger)
        {
            _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(ProvideTrainingScheduleFeedbackCommand message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Providing feedback for {@ScheduleId} by {@OwnerId}", message.ScheduleId, message.OwnerId);
            try
            {
                TrainingScheduleRoot schedule = _scheduleRepository.Find(message.ScheduleId);
                RatingValue rating = message.Rating.HasValue ? RatingValue.Rate(message.Rating.Value) : null;
                RatingValue comment = message.Rating.HasValue ? RatingValue.Rate(message.Rating.Value) : null;

                schedule.ProvideFeedback(TrainingScheduleFeedbackEntity.ProvideTransientFeedback(
                    message.OwnerId, rating, PersonalNoteValue.Write(message.Comment)));

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
