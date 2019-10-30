using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.Validator.StaticRule;
using Microsoft.Extensions.Logging;

namespace GymProject.Application.Validator.TrainingDomain
{
    public class PlanWorkingSetRepetitionsCommandValidator : AbstractValidator<PlanWorkingSetRepetitionsCommand>
    {

        public PlanWorkingSetRepetitionsCommandValidator(ILogger<PlanWorkingSetRepetitionsCommandValidator> logger)
        {
            RuleFor(x => x.WorkTypeId)
                .Must(wt => TrainingDomainBasicRules.IsValidWorkType(wt))
                .WithMessage($"Invalid WorkTypeId");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

    }


}
