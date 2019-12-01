using GymProject.Domain.Base.Mediator;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace GymProject.Application.DomainEventHandler
{
    //public class LinkWorkoutToTrainingPlanWhenCreatedDomainEventHandler : IDomainNotificationHandler<WorkoutTemplateCreatedDomainEvent>
    public class LinkWorkoutToTrainingPlanWhenCreatedDomainEventHandler : INotificationHandler<WorkoutTemplateCreatedDomainEvent>
    {


        private readonly ILoggerFactory _logger;
        private readonly ITrainingProgramRepository _trainingPlanRepository;



        public LinkWorkoutToTrainingPlanWhenCreatedDomainEventHandler(ILoggerFactory logger, ITrainingProgramRepository trainingPlanRepository)
        {
            _logger = logger 
                ?? throw new ArgumentNullException(nameof(logger));

            _trainingPlanRepository = trainingPlanRepository 
                ?? throw new ArgumentNullException(nameof(trainingPlanRepository));
        }


        public async Task Handle(WorkoutTemplateCreatedDomainEvent notification, CancellationToken cancellationToken = default)
        {
            uint workoutId = notification.WorkoutTemplateId.Value;
            uint planId = notification.TrainingPlanId.Value;
            uint weekProgressiveNumber = notification.TrainingWeekProgressiveNumber;

            TrainingPlanRoot plan = _trainingPlanRepository.Find(planId);

            if (plan == null)
                throw new ArgumentException($"Cannot find the trianing plan with ID {planId}");

            plan.PlanWorkout(weekProgressiveNumber, workoutId);

            await _trainingPlanRepository.UnitOfWork.SaveAsync(cancellationToken);

            _logger.CreateLogger<LinkWorkoutToTrainingPlanWhenCreatedDomainEventHandler>()
                .LogTrace($"Workout Template {workoutId.ToString()} linked to Training Plan {plan.Id.ToString()}, Week {weekProgressiveNumber.ToString()}");
        }


    }
}
