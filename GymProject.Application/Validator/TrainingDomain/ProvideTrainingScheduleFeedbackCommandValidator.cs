using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.Validator.StaticRule;
using GymProject.Domain.SharedKernel;
using Microsoft.Extensions.Logging;

namespace GymProject.Application.Validator.TrainingDomain
{

    public class ProvideTrainingScheduleFeedbackCommandValidator : AbstractValidator<ProvideTrainingScheduleFeedbackCommand>
    {

        public ProvideTrainingScheduleFeedbackCommandValidator(ILogger<ProvideTrainingScheduleFeedbackCommandValidator> logger)
        {
            RuleFor(x => x.Rating)
                .Must(x => TrainingDomainBasicRules.IsValidRating(x))
                .WithMessage(x => $"Invalid feedback rating: {x.Rating.ToString()} - it must be in range [{RatingValue.MinimumValue.ToString()},{RatingValue.MaximumValue.ToString()}]");

            RuleFor(x => x.Comment)
                .MaximumLength(PersonalNoteValue.DefaultMaximumLength)
                .WithMessage(x => $"Too long Feedback comment: {x.Comment.Length} - It must be shorter than {PersonalNoteValue.DefaultMaximumLength}");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

    }
}
