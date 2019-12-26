using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    public class ScheduleTrainingPlanCommandHandler : IRequestHandler<ScheduleTrainingPlanCommand, bool>
    {


        //private readonly IAthleteRepository _athleteRepository;
        private readonly ITrainingScheduleRepository _scheduleRepository;
        private readonly ILogger<ScheduleTrainingPlanCommandHandler> _logger;





        public ScheduleTrainingPlanCommandHandler(ITrainingScheduleRepository scheduleRepository, ILogger<ScheduleTrainingPlanCommandHandler> logger)
        {
            _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(ScheduleTrainingPlanCommand message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Scheduling {@TrainingPlanId} by {@AthleteId} from {@StartDate} to {@EndDate}", message.TrainingPlanId, message.AthleteId, message.StartDate, message.EndDate);

            try
            {
                TrainingScheduleRoot currentSchedule = _scheduleRepository.GetCurrentScheduleByAthleteOrDefault(message.AthleteId);

                // Create the schedule
                TrainingScheduleRoot schedule = TrainingScheduleRoot.ScheduleTrainingPlan(message.AthleteId, message.TrainingPlanId, message.StartDate, message.EndDate);
                _scheduleRepository.Add(schedule);

                // Complete the current schedule, if any
                if (currentSchedule != null)
                {
                    currentSchedule.Complete(DateTime.UtcNow.AddDays(-1).Date);
                    _scheduleRepository.Modify(currentSchedule);
                }
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
