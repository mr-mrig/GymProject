using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingScheduleAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class ScheduleTrainingPlanCommandHandler : IRequestHandler<ScheduleTrainingPlanCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepository;
        private readonly ITrainingScheduleRepository _scheduleRepository;
        private readonly ILogger<ScheduleTrainingPlanCommandHandler> _logger;





        public ScheduleTrainingPlanCommandHandler(IAthleteRepository athleteRepository, ITrainingScheduleRepository scheduleRepository, ILogger<ScheduleTrainingPlanCommandHandler> logger)
        {
            _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(ScheduleTrainingPlanCommand message, CancellationToken cancellationToken)
        {
            TrainingScheduleRoot schedule;

            throw new NotImplementedException("Need to change the TrainingSchedule domain!");

            //// Create the Schedule
            //try
            //{
            //    schedule = TrainingScheduleRoot.ScheduleTrainingPlan
            //}
            //catch (Exception exc)
            //{
            //    _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _scheduleRepository.UnitOfWork);
            //    return false;
            //}

            //// Assign the schedule to the plan
            //try
            //{
            //    AthleteRoot athlete = _athleteRepository.Find(message.AthleteId);

            //    _logger.LogInformation("----- Making Training Plan {@PlanId} of Athlete {@Athlete} variant of nothing", message.TrainingPlanId, athlete);

            //    athlete.MakeTrainingPlanNotVariantOfAny(message.TrainingPlanId);
            //    _athleteRepository.Modify(athlete);

            //    return await _athleteRepository.UnitOfWork.SaveAsync(cancellationToken);
            //}
            //catch (Exception exc)
            //{
            //    _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _athleteRepository.UnitOfWork);
            //    return false;
            //}
        }
    }

}
