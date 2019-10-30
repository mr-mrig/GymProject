using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.Validator.StaticRule;
using Microsoft.Extensions.Logging;

namespace GymProject.Application.Validator.TrainingDomain
{
    public class PlanWorkingSetEffortCommandValidator : AbstractValidator<PlanWorkingSetEffortCommand>
    {

        public PlanWorkingSetEffortCommandValidator(ILogger<PlanWorkingSetEffortCommandValidator> logger)
        {
            RuleFor(x => x.EffortTypeId)
                .Must(eff => TrainingDomainBasicRules.IsValidEffortType(eff))
                .WithMessage("Invalid EffortTypeId");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

    }


}
