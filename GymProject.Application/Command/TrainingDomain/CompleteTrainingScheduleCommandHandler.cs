using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    public class CompleteTrainingScheduleCommandHandler : IRequestHandler<CompleteTrainingScheduleCommand, bool>
    {


        private readonly ITrainingScheduleRepository _scheduleRepository;
        private readonly ILogger<CompleteTrainingScheduleCommandHandler> _logger;





        public CompleteTrainingScheduleCommandHandler(ITrainingScheduleRepository scheduleRepository, ILogger<CompleteTrainingScheduleCommandHandler> logger)
        {
            _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(CompleteTrainingScheduleCommand message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Completing training schedule {@TrainingScheduleId} on {@EndDate}", message.TrainingScheduleId, message.EndDate);

            try
            {
                // Create the schedule
                TrainingScheduleRoot schedule = _scheduleRepository.Find(message.TrainingScheduleId);
                schedule.Complete(message.EndDate);

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
