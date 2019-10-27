using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Domain.TrainingDomain.Common;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace GymProject.Application.Validator.TrainingDomain
{
    public class PlanTrainingWeekCommandValidator : AbstractValidator<PlanTrainingWeekCommand>
    {

        public PlanTrainingWeekCommandValidator(ILogger<WriteTrainingPlanNoteCommand> logger)
        {
            RuleFor(x => x.WeekTypeEnumId)
                .Must(w =>  IsValidWeekType(w))
                .WithMessage("Invalid WeekTypeEnumId");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }


        private bool IsValidWeekType(uint? weekTypeEnumId)

            => weekTypeEnumId == null
                || TrainingWeekTypeEnum.List().Select(e => e.Id).Contains((int)weekTypeEnumId.Value);
    }


}
