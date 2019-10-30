using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Domain.SharedKernel;
using Microsoft.Extensions.Logging;

namespace GymProject.Application.Validator.TrainingDomain
{
    public class WriteWorkUnitTemplateNoteCommandValidator  : AbstractValidator<WriteWorkUnitTemplateNoteCommand>
    {



        public WriteWorkUnitTemplateNoteCommandValidator (ILogger<WriteWorkUnitTemplateNoteCommandValidator> logger)
        {
            RuleFor(x => x.WorkoutTemplateId).NotEmpty();
            RuleFor(x => x.WorkUnitProgressiveNumber).NotEmpty();
            RuleFor(x => x.NoteBody).MaximumLength(PersonalNoteValue.DefaultMaximumLength);

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

    }
}
