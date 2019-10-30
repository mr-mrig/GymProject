using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Domain.SharedKernel;
using Microsoft.Extensions.Logging;

namespace GymProject.Application.Validator.TrainingDomain
{
    public class WriteTrainingPlanNoteCommandValidator : AbstractValidator<WriteTrainingPlanNoteCommand>
    {



        public WriteTrainingPlanNoteCommandValidator(ILogger<WriteTrainingPlanNoteCommandValidator> logger)
        {
            RuleFor(x => x.TrainingPlanId).NotEmpty();
            RuleFor(x => x.NoteBody).MaximumLength(PersonalNoteValue.DefaultMaximumLength);

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

    }
}
