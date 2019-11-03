using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.Validator.StaticRule;
using Microsoft.Extensions.Logging;

namespace GymProject.Application.Validator.TrainingDomain
{
    public class PlanWorkingSetCommandValidator : AbstractValidator<PlanWorkingSetCommand>
    {

        public PlanWorkingSetCommandValidator(ILogger<PlanWorkingSetCommandValidator> logger)
        {
            RuleFor(x => x.EffortTypeId)
                .Must(eff => TrainingDomainBasicRules.IsValidEffortType(eff))
                .WithMessage(eff => $"Invalid EffortTypeId: {eff.EffortTypeId.ToString()}");
            RuleFor(x => x.RestMeasUnitId)
                .Must(rmeas => TrainingDomainBasicRules.IsValidRestMeasUnit(rmeas))
                .WithMessage(rmeas => $"Invalid RestMeasUnit: {rmeas.RestMeasUnitId.ToString()}");
            RuleFor(x => x.WorkTypeId)
                .Must(wt => TrainingDomainBasicRules.IsValidWorkType(wt))
                .WithMessage(wt => $"Invalid WorkTypeId: {wt.WorkTypeId.ToString()}");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

    }


}
