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
    public class FakePlanDraftWorkoutCommandHandler : IRequestHandler<PlanDraftWorkoutCommand, bool>
    {


        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly ITrainingProgramRepository _planRepository;
        private readonly ILogger<FakePlanDraftWorkoutCommandHandler> _logger;
        private readonly int _fakeWeekProgressiveNumber;



        public FakePlanDraftWorkoutCommandHandler(
            IWorkoutTemplateRepository workoutTemplateRepository,
            ITrainingProgramRepository planRepository,
            ILogger<FakePlanDraftWorkoutCommandHandler> logger,
            int fakeWeekProgressiveNumber)
        {
            _workoutRepository = workoutTemplateRepository ?? throw new ArgumentNullException(nameof(workoutTemplateRepository));
            _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fakeWeekProgressiveNumber = fakeWeekProgressiveNumber;
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

                // Link to the workout
                if (result)
                {
                    plan.PlanWorkout((uint)_fakeWeekProgressiveNumber, workout.Id.Value);     // Fails
                    _planRepository.Modify(plan);

                    result = await _planRepository.UnitOfWork.SaveAsync(cancellationToken);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError($"----- Transaction failed: {exc.Message}");
                result = false;
            }

            return result;
        }
    }


}
