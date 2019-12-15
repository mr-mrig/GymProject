using GymProject.Application.Command.TrainingDomain;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Test.FakeCommands
{
    public class FakeDeleteTrainingPlanCommandHandler : IRequestHandler<DeleteTrainingPlanCommand, bool>
    {

        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly ITrainingPlanRepository _planRepository;
        private readonly ILogger<FakeDeleteTrainingPlanCommandHandler> _logger;



        public FakeDeleteTrainingPlanCommandHandler(
            IWorkoutTemplateRepository workoutTemplateRepository,
            ITrainingPlanRepository planRepository,
            ILogger<FakeDeleteTrainingPlanCommandHandler> logger)
        {
            _workoutRepository = workoutTemplateRepository ?? throw new ArgumentNullException(nameof(workoutTemplateRepository));
            _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        public async Task<bool> Handle(DeleteTrainingPlanCommand message, CancellationToken cancellationToken)
        {
            TrainingPlanRoot plan = _planRepository.Find(message.TrainingPlanId);

            _logger.LogInformation("----- Deleting Training Plan - {@Plan}", plan);

            try
            {
                // Delete all the WO aggregates linked to the plan to be removed
                foreach (uint? workoutId in plan.WorkoutIds)
                {
                    WorkoutTemplateRoot workout = _workoutRepository.Find(workoutId.Value);
                    _workoutRepository.Remove(workout);
                }

                throw new Exception("Testing transactional failure");

                // Delete the training plan aggregate
                _planRepository.Remove(plan);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _workoutRepository.UnitOfWork);
                return false;
            }
        }


    }
}
