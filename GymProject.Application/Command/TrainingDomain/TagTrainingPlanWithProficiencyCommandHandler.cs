﻿using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class TagTrainingPlanWithProficiencyCommandHandler : IRequestHandler<TagTrainingPlanWithProficiencyCommand, bool>
    {


        private readonly ITrainingPlanRepository _planRepository;
        private readonly ILogger<TagTrainingPlanWithProficiencyCommandHandler> _logger;





        public TagTrainingPlanWithProficiencyCommandHandler(
            ITrainingPlanRepository planRepository,
            ILogger<TagTrainingPlanWithProficiencyCommandHandler> logger
            )
        {
            _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(TagTrainingPlanWithProficiencyCommand message, CancellationToken cancellationToken)
        {
            bool result;

            try
            {
                TrainingPlanRoot plan = _planRepository.Find(message.TrainingPlanId);

                _logger.LogInformation("----- Tagging {@TrainingPlan} with Proficiency {@ProficiencyId}", plan, message.ProficiencyId);

                plan.LinkTargetProficiency(message.ProficiencyId);
                _planRepository.Modify(plan);

                result = await _planRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch(Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _planRepository.UnitOfWork);
                result = false;
            }

            return result;
        }
    }

}
