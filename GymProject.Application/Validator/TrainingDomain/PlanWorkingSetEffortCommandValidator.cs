using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace GymProject.Application.Validator.TrainingDomain
{
    public class PlanWorkingSetEffortCommandValidator : AbstractValidator<PlanWorkingSetEffortCommand>
    {

        public PlanWorkingSetEffortCommandValidator(ILogger<WriteTrainingPlanNoteCommand> logger)
        {
            RuleFor(x => x.EffortTypeId)
                .Must(eff => IsValidEffortType(eff))
                .WithMessage("Invalid EffortTypeId");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

        private bool IsValidEffortType(int? effortTypeId)

            => effortTypeId == null
                || TrainingEffortTypeEnum.List().Select(e => e.Id).Contains((int)effortTypeId.Value);
    }


}
