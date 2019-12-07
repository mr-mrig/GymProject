using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;

namespace GymProject.Application.DomainEventHandler
{
    public class TrainingPlanRemovedFromLibraryDomainEventHandler : INotificationHandler<TrainingPlanRemovedFromLibraryDomainEvent>
    {


        private readonly ILogger<TrainingPlanRemovedFromLibraryDomainEventHandler> _logger;
        private readonly ITrainingPlanRepository _trainingPlanRepository;
        private readonly IWorkoutTemplateRepository _workoutRepository;
        private readonly IAthleteRepository _athleteRepository;



        public TrainingPlanRemovedFromLibraryDomainEventHandler(ILogger<TrainingPlanRemovedFromLibraryDomainEventHandler> logger, 
            ITrainingPlanRepository trainingPlanRepository, IAthleteRepository athleteRepository, IWorkoutTemplateRepository workoutRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
            _trainingPlanRepository = trainingPlanRepository ?? throw new ArgumentNullException(nameof(trainingPlanRepository));
        }


        public async Task Handle(TrainingPlanRemovedFromLibraryDomainEvent notification, CancellationToken cancellationToken = default)
        {
            int athletesWithPlan;

            // Check if the only athlete having the plan in its library is the one who is removing it
            try
            {
                _logger.LogInformation("----- Checking if Training Plan {@TrainingPlanId} can be deleted", notification.TrainingPlanId);
                athletesWithPlan = _athleteRepository.CountAthletesWithTrainingPlanInLibrary(notification.TrainingPlanId); // Data read might be stale - Should we do this here or maybe run a schedudled script for orphans removing?
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _athleteRepository.UnitOfWork);
                return;
            }
            try
            {
                // If only one, then the entry must be deleted
                if (athletesWithPlan == 1)
                {
                    TrainingPlanRoot plan = _trainingPlanRepository.Find(notification.TrainingPlanId);

                    _logger.LogInformation("----- Deleting Workouts for {@TrainingPlan} ", plan);

                    // Delete all the WO aggregates linked to the plan to be removed
                    foreach (uint? workoutId in plan.WorkoutIds)
                    {
                        WorkoutTemplateRoot workout = _workoutRepository.Find(workoutId.Value);
                        _workoutRepository.Remove(workout);
                    }

                    _logger.LogInformation("----- Deleting Training Plan {@TrainingPlan} ", plan);
                    _trainingPlanRepository.Remove(plan);

                    await _trainingPlanRepository.UnitOfWork.SaveAsync(cancellationToken);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _trainingPlanRepository.UnitOfWork);
            }
        }


    }
}
