using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace GymProject.Application.Validator.TrainingDomain
{
    public class PlanWorkingSetCommandValidator : AbstractValidator<PlanWorkingSetCommand>
    {

        public PlanWorkingSetCommandValidator(ILogger<WriteTrainingPlanNoteCommand> logger)
        {
            RuleFor(x => x.EffortTypeId)
                .Must(eff => IsValidEffortType(eff))
                .WithMessage("Invalid EffortTypeId");
            RuleFor(x => x.RestMeasUnitId)
                .Must(rmeas => IsValidRestMeasUnit(rmeas))
                .WithMessage("Invalid RestMeasUnit");
            RuleFor(x => x.WorkTypeId)
                .Must(wt => IsValidWorkType(wt))
                .WithMessage("Invalid RestMeasUnit");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

        private bool IsValidWorkType(int? workTypeId)

            => workTypeId == null
                || WSWorkTypeEnum.List().Select(e => e.Id).Contains((int)workTypeId.Value);

        private bool IsValidEffortType(int? effortTypeId)

            => effortTypeId == null
                || TrainingEffortTypeEnum.List().Select(e => e.Id).Contains((int)effortTypeId.Value);


        private bool IsValidRestMeasUnit(int? effortTypeId)

            => effortTypeId == null
                || TimeMeasureUnitEnum.List().Select(e => e.Id).Contains((int)effortTypeId.Value);

    }


}
