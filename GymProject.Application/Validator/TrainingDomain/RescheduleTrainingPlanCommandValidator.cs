using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using Microsoft.Extensions.Logging;
using System;

namespace GymProject.Application.Validator.TrainingDomain
{

    public class RescheduleTrainingPlanCommandValidator : AbstractValidator<RescheduleTrainingPlanCommand>
    {

        public RescheduleTrainingPlanCommandValidator(ILogger<RescheduleTrainingPlanCommandValidator> logger)
        {
            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage(x => $"Cannot reschedule in the past, StartDate is {x.StartDate.ToShortDateString()}");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

    }
}
