using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class PlanTrainingWeekCommandHandler : IRequestHandler<PlanTrainingWeekCommand, bool>
    {


        private readonly ITrainingProgramRepository _planRepository;
        private readonly ILogger<PlanTrainingWeekCommandHandler> _logger;



        public PlanTrainingWeekCommandHandler(
            ITrainingProgramRepository planRepository,
            ILogger<PlanTrainingWeekCommandHandler> logger)
        {
            _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(PlanTrainingWeekCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            TrainingPlanRoot plan = _planRepository.Find(message.TrainingPlanId);

            if (plan == null)
                return false;

            try
            {

                TrainingWeekTypeEnum weekType = message.WeekTypeEnumId == null 
                    ? null
                    : TrainingWeekTypeEnum.From((int)message.WeekTypeEnumId);

                // Validator should handle this
                //if(weekType == null)
                //{
                //    _logger.LogWarning("ERROR: {@WeekTypeId} not parsable", message.WeekTypeEnumId);
                //    return false;
                //}


                plan.PlanDraftTrainingWeek(weekType);


                _logger.LogInformation("----- Creating Training Week - {@TrainingPlan}", plan);

                _planRepository.Modify(plan);
                result = await _planRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _planRepository.UnitOfWork);
                result = false;
            }

            //PlanWorkoutCreationSuccessed(workout.Id);     // Link the Workout to the Plan by event

            return result;
        }
    }
}
