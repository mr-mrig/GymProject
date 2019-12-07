using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class TagTrainingPlanAsNewHashtagCommandHandler : IRequestHandler<TagTrainingPlanAsNewHashtagCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepo;
        private readonly ITrainingHashtagRepository _hashtagRepository;
        private readonly ILogger<TagTrainingPlanAsNewHashtagCommandHandler> _logger;





        public TagTrainingPlanAsNewHashtagCommandHandler(
            IAthleteRepository athleteRepository,
            ITrainingHashtagRepository hashtagRepository,
            ILogger<TagTrainingPlanAsNewHashtagCommandHandler> logger
            )
        {
            _athleteRepo = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _hashtagRepository = hashtagRepository ?? throw new ArgumentNullException(nameof(hashtagRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(TagTrainingPlanAsNewHashtagCommand message, CancellationToken cancellationToken)
        {
            bool result = false;
            TrainingHashtagRoot hashtag = null;

            try
            {
                hashtag = TrainingHashtagRoot.TagWith(GenericHashtagValue.TagWith(message.HashtagBody));


                _logger.LogInformation("----- Creating {@TrainingHashtag}", hashtag);

                _hashtagRepository.Add(hashtag);
                result = await _hashtagRepository.UnitOfWork.SaveAsync(cancellationToken);

                if(!result)
                {
                    _logger.LogWarning("Could not create {@TrainingHashtag} - Context {@WarningContext}", hashtag, _hashtagRepository.UnitOfWork);
                    return false;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _hashtagRepository.UnitOfWork);
                result = false;
            }
            try
            {
                AthleteRoot athlete = _athleteRepo.Find(message.AthleteId);
                athlete.TagTrainingPlanAs(message.TrainingPlanId, hashtag.Id.Value);

                _logger.LogInformation("----- Tagging {@TrainingPlanId} of {@Athlete} with {@HashtagId}", message.TrainingPlanId, athlete, hashtag.Id);

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
