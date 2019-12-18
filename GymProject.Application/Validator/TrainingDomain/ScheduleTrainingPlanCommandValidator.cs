using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using Microsoft.Extensions.Logging;

namespace GymProject.Application.Validator.TrainingDomain
{

    public class ScheduleTrainingPlanCommandValidator : AbstractValidator<ScheduleTrainingPlanCommand>
    {

        public ScheduleTrainingPlanCommandValidator(ILogger<ScheduleTrainingPlanCommandValidator> logger)
        {
            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate)
                .When(x => x.EndDate != null)
                .WithMessage(x => $"Start Date must be before End Date, while {x.StartDate.ToShortDateString()} >= {x.EndDate.Value.ToShortDateString()}");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

    }
}
