using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.Validator.StaticRule;
using GymProject.Domain.SharedKernel;
using Microsoft.Extensions.Logging;

namespace GymProject.Application.Validator.TrainingDomain
{
    public class TagTrainingPlanAsNewHashtagCommandValidator : AbstractValidator<TagTrainingPlanAsNewHashtagCommand>
    {



        public TagTrainingPlanAsNewHashtagCommandValidator(ILogger<TagTrainingPlanAsNewHashtagCommandValidator> logger)
        {
            RuleFor(x => x.HashtagBody).Must(h => GenericHashtagRule.IsValidHashtag(h))
                .WithMessage(h => "Invalid Hashtag format: " + h.HashtagBody);

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

    }
}
