using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class DeleteTrainingPlanCommandHandler : IRequestHandler<DeleteTrainingPlanCommand, bool>
    {

        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly ITrainingProgramRepository _planRepository;
        private readonly ILogger<DeleteTrainingPlanCommandHandler> _logger;



        public DeleteTrainingPlanCommandHandler(
            IWorkoutTemplateRepository workoutTemplateRepository,
            ITrainingProgramRepository planRepository,
            ILogger<DeleteTrainingPlanCommandHandler> logger)
        {
            _workoutRepository = workoutTemplateRepository ?? throw new ArgumentNullException(nameof(workoutTemplateRepository));
            _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        public async Task<bool> Handle(DeleteTrainingPlanCommand message, CancellationToken cancellationToken)
        {
            bool result = true;

            TrainingPlanRoot plan = _planRepository.Find(message.TrainingPlanId);

            if (plan == null)
                return false;


            _logger.LogInformation("----- Deleting Training Plan - {@Plan}", plan);


            try
            {
                // Delete all the WO aggregates linked to the plan to be removed
                foreach (uint? workoutId in plan.WorkoutIds)
                {
                    WorkoutTemplateRoot workout = _workoutRepository.Find(workoutId.Value);
                    _workoutRepository.Remove(workout);
                }

                // Delete the training plan aggregate
                _planRepository.Remove(plan);

                result = await _workoutRepository.UnitOfWork.SaveAsync(cancellationToken);

                if(result)
                    result = await _planRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _workoutRepository.UnitOfWork);
                result = false;
            }

            //TrainingPlanDeleted(workout.Id);     // Delete the WO linked to it
            return result;
        }


    }
}
