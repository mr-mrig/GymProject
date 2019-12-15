using GymProject.Application.Command.TrainingDomain;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Test.FakeCommands
{


    // Regular CommandHandler
    public class FakeTagTrainingPlanAsNewHashtagCommandHandler : IRequestHandler<TagTrainingPlanAsNewHashtagCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepo;
        private readonly ITrainingHashtagRepository _hashtagRepository;
        private readonly ILogger<FakeTagTrainingPlanAsNewHashtagCommandHandler> _logger;
        private bool _failStep1;
        private bool _failStep2;




        public FakeTagTrainingPlanAsNewHashtagCommandHandler(
            IAthleteRepository athleteRepository,
            ITrainingHashtagRepository hashtagRepository,
            ILogger<FakeTagTrainingPlanAsNewHashtagCommandHandler> logger,
            bool failStep1, bool failStep2
            )
        {
            _athleteRepo = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _hashtagRepository = hashtagRepository ?? throw new ArgumentNullException(nameof(hashtagRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _failStep2 = failStep2;
            _failStep1 = failStep1;
        }




        public async Task<bool> Handle(TagTrainingPlanAsNewHashtagCommand message, CancellationToken cancellationToken)
        {
            TrainingHashtagRoot hashtag = null;

            try
            {
                hashtag = TrainingHashtagRoot.TagWith(GenericHashtagValue.TagWith(message.HashtagBody));

                _logger.LogInformation("----- Creating {@TrainingHashtag}", hashtag);
                _hashtagRepository.Add(hashtag);

                if (_failStep1)
                    throw new Exception();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _hashtagRepository.UnitOfWork);
                return false;
            }
            try
            {
                AthleteRoot athlete = _athleteRepo.Find(message.AthleteId);
                athlete.TagTrainingPlanAs(message.TrainingPlanId, hashtag.Id.Value);

                _logger.LogInformation("----- Tagging {@TrainingPlanId} of {@Athlete} with {@HashtagId}", message.TrainingPlanId, athlete, hashtag.Id);

                _athleteRepo.Modify(athlete);

                if (_failStep2)
                    throw new Exception("Testing transaction failure");

                return await _athleteRepo.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _athleteRepo.UnitOfWork);
                return false;
            }
        }
    }

}
