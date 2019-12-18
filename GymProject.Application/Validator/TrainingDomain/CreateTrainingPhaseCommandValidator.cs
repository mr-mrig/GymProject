using FluentValidation;
using GymProject.Application.Command.TrainingDomain;
using GymProject.Application.Validator.StaticRule;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Application.Validator.TrainingDomain
{

    public class CreateTrainingPhaseCommandValidator : AbstractValidator<CreateTrainingPhaseCommand>
    {

        public CreateTrainingPhaseCommandValidator(ILogger<CreateTrainingPhaseCommandValidator> logger)
        {

            RuleFor(x => x.EntryStatusId)
                .Must(x => TrainingDomainBasicRules.IsValidEntryStatusType(x))
                .WithMessage(x => $"Invalid EntryStatusId: {x.EntryStatusId.ToString()}");

            //RuleFor(x => x.PhaseName)
            //    .MaximumLength(x => TrainingDomainBasicRules.IsValidEntryStatusType(x))
            //    .WithMessage(x => $"Invalid EntryStatusId: {x.EntryStatusId.ToString()}");

            //RuleFor(x => x.PhaseName)
            //    .Must(1)
            //    .WithMessage(x => $"the phase name contains invalid chars: {x.PhaseName}");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

    }
}
