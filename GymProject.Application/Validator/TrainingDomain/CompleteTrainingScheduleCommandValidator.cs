using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using Microsoft.Extensions.Logging;
using System;

namespace GymProject.Application.Validator.TrainingDomain
{

    public class CompleteTrainingScheduleCommandValidator : AbstractValidator<CompleteTrainingScheduleCommand>
    {

        public CompleteTrainingScheduleCommandValidator(ILogger<CompleteTrainingScheduleCommandValidator> logger)
        {
            RuleFor(x => x.EndDate)
                .LessThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage(x => $"The schedule cannot be completed in a future date, while End Date is {x.EndDate.ToShortDateString()}");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

    }
}
