using System;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class PlanDraftWorkoutCommandHandler : IRequestHandler<PlanDraftWorkoutCommand, bool>
    {


        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly ITrainingProgramRepository _planRepository;
        private readonly ILogger<PlanDraftWorkoutCommandHandler> _logger;



        public PlanDraftWorkoutCommandHandler(
            IWorkoutTemplateRepository workoutTemplateRepository,
            ITrainingProgramRepository planRepository,
            ILogger<PlanDraftWorkoutCommandHandler> logger)
        {
            _workoutRepository = workoutTemplateRepository ?? throw new ArgumentNullException(nameof(workoutTemplateRepository));
            _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(PlanDraftWorkoutCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            TrainingPlanRoot plan = _planRepository.Find(message.TrainingPlanId);

            if (plan == null)
                return false;

            try
            {
                uint workoutProgressiveNumber = (uint)plan.GetNextWorkoutProgressiveNumber(message.TrainingWeekProgressiveNumber);

                WorkoutTemplateRoot workout = WorkoutTemplateRoot.PlannedDraft(message.TrainingWeekId, workoutProgressiveNumber);

                _logger.LogInformation("----- Creating Workout Template - {@Workout}", workout);

                workout = _workoutRepository.Add(workout);
                result = await _workoutRepository.UnitOfWork.SaveAsync(cancellationToken);

                try
                {
                    // Link to the workout
                    if (result)
                    {
                        plan.PlanWorkout(message.TrainingWeekProgressiveNumber, workout.Id.Value);
                        _planRepository.Modify(plan);

                        result = await _planRepository.UnitOfWork.SaveAsync(cancellationToken);
                    }
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _planRepository.UnitOfWork);
                    result = false;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _workoutRepository.UnitOfWork);
                result = false;
            }


            //PlanWorkoutCreationSuccessed(workout.Id);     // Link the Workout to the Plan by event

            return result;
        }
    }


}
