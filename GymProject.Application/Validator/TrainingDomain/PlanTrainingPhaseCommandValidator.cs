using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.Validator.StaticRule;
using Microsoft.Extensions.Logging;

namespace GymProject.Application.Validator.TrainingDomain
{

    public class PlanTrainingPhaseCommandValidator : AbstractValidator<PlanTrainingPhaseCommand>
    {

        public PlanTrainingPhaseCommandValidator(ILogger<PlanTrainingPhaseCommandValidator> logger)
        {
            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate)
                .When(x => x.EndDate != null)
                .WithMessage(x => $"Start Date must be before End Date, while {x.StartDate.ToShortDateString()} >= {x.EndDate.Value.ToShortDateString()}");

            RuleFor(x => x.EntryStatusId)
                .Must(x => TrainingDomainBasicRules.IsValidEntryStatusType((uint?)x))
                .WithMessage(x => $"Invalid EntryStatusId: {x.EntryStatusId.ToString()}");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

    }
}
