using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using Microsoft.Extensions.Logging;

namespace GymProject.Application.Validator.TrainingDomain
{


    public class RenameTrainingPlanCommandValidator : AbstractValidator<RenameTrainingPlanCommand>
    {

        public RenameTrainingPlanCommandValidator(ILogger<RenameTrainingPlanCommand> logger)
        {

            //RuleFor(x => x.TrainingPlanName)
            //    .MaximumLength(1)
            //    .WithMessage(x => $"Training plan name is too long: {x.TrainingPlanName.Length} > {}");

            //RuleFor(x => x.TrainingPlanName)
            //    .Must(1)
            //    .WithMessage(x => $"The training plan name contains invalid chars: {x.TrainingPlanName}");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

    }
}
