using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class PlanDraftExcerciseCommandHandler : IRequestHandler<PlanDraftExcerciseCommand, bool>
    {

        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly ILogger<PlanDraftExcerciseCommandHandler> _logger;



        public PlanDraftExcerciseCommandHandler(
            IWorkoutTemplateRepository workoutRepository,
            ILogger<PlanDraftExcerciseCommandHandler> logger)
        {
            _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(PlanDraftExcerciseCommand message, CancellationToken cancellationToken)
        {
            WorkoutTemplateRoot workout = _workoutRepository.Find(message.WorkoutTemplateId);

            if (workout == null)
                return false;

            try
            {
                workout.DraftExcercise(message.ExcerciseId);


                _logger.LogInformation("----- Creating Work Unit in - {@Workout}", workout.Id);

                _workoutRepository.Modify(workout);
                return await _workoutRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _workoutRepository.UnitOfWork);
                return false;
            }
        }

    }
}
