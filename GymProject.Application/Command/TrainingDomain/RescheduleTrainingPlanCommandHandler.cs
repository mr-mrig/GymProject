using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    public class RescheduleTrainingPlanCommandHandler : IRequestHandler<RescheduleTrainingPlanCommand, bool>
    {


        //private readonly IAthleteRepository _athleteRepository;
        private readonly ITrainingScheduleRepository _scheduleRepository;
        private readonly ILogger<RescheduleTrainingPlanCommandHandler> _logger;





        public RescheduleTrainingPlanCommandHandler(ITrainingScheduleRepository scheduleRepository, ILogger<RescheduleTrainingPlanCommandHandler> logger)
        {
            _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(RescheduleTrainingPlanCommand message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Recheduling {@ScheduleId} starting from {@StartDate}", message.TrainingScheduleId, message.StartDate);

            try
            {
                TrainingScheduleRoot schedule = _scheduleRepository.Find(message.TrainingScheduleId);
                schedule.Reschedule(message.StartDate);
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
