﻿using GymProject.Domain.TrainingDomain.AthleteAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class TagTrainingPlanAsHashtagCommandHandler : IRequestHandler<TagTrainingPlanAsHashtagCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepo;
        private readonly ILogger<TagTrainingPlanAsHashtagCommandHandler> _logger;





        public TagTrainingPlanAsHashtagCommandHandler(
            IAthleteRepository athleteRepository,
            ILogger<TagTrainingPlanAsHashtagCommandHandler> logger
            )
        {
            _athleteRepo = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(TagTrainingPlanAsHashtagCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            try
            {
                AthleteRoot athlete = _athleteRepo.Find(message.UserId);
                athlete.TagTrainingPlanAs(message.TrainingPlanId, message.TrainingHashtagId);

                _logger.LogInformation("----- Tagging {@UserTrainingPlanId} of {@Athlete} with {@HashtagId}", message.TrainingPlanId, athlete, message.TrainingHashtagId);

                _athleteRepo.Modify(athlete);

                result = await _athleteRepo.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _athleteRepo.UnitOfWork);
                result = false;
            }
            return result;
        }
    }

}