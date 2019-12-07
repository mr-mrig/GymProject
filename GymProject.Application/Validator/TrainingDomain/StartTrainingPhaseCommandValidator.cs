using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.Validator.StaticRule;
using GymProject.Domain.SharedKernel;
using Microsoft.Extensions.Logging;

namespace GymProject.Application.Validator.TrainingDomain
{

    public class StartTrainingPhaseCommandValidator : AbstractValidator<StartTrainingPhaseCommand>
    {

        public StartTrainingPhaseCommandValidator(ILogger<StartTrainingPhaseCommand> logger)
        {
            RuleFor(x => x.EntryStatusId)
                .Must(x => TrainingDomainBasicRules.IsValidEntryStatusType(x))
                .WithMessage(x => $"Invalid EntryStatusId: {x.EntryStatusId.ToString()}");

            RuleFor(x => x.OwnerNote).MaximumLength(PersonalNoteValue.DefaultMaximumLength);

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

    }
}
