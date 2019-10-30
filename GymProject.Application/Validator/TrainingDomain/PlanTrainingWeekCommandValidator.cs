using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.Validator.StaticRule;
using Microsoft.Extensions.Logging;

namespace GymProject.Application.Validator.TrainingDomain
{
    public class PlanTrainingWeekCommandValidator : AbstractValidator<PlanTrainingWeekCommand>
    {

        public PlanTrainingWeekCommandValidator(ILogger<PlanTrainingWeekCommandValidator> logger)
        {
            RuleFor(x => x.WeekTypeEnumId)
                .Must(w => TrainingDomainBasicRules.IsValidWeekType(w))
                .WithMessage("Invalid WeekTypeEnumId");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

    }


}
