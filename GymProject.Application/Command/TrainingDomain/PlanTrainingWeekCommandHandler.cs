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


        private readonly ITrainingPlanRepository _planRepository;
        private readonly ILogger<PlanTrainingWeekCommandHandler> _logger;



        public PlanTrainingWeekCommandHandler(
            ITrainingPlanRepository planRepository,
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
                plan.PlanTrainingWeek();


                _logger.LogInformation("----- Creating Training Week - {@TrainingPlan}", plan);

                _planRepository.Modify(plan);
                result = await _planRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError($"----- Transaction failed: {exc.Message}");
                result = false;
            }

            //PlanWorkoutCreationSuccessed(workout.Id);     // Link the Workout to the Plan by event

            return result;
        }
    }
    }
