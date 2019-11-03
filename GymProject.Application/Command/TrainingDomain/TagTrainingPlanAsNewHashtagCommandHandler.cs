using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.TrainingHashtagAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
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


        private readonly ITrainingPlanRepository _trainingPlanRepository;
        private readonly ITrainingHashtagRepository _hashtagRepository;
        private readonly ILogger<TagTrainingPlanAsNewHashtagCommandHandler> _logger;





        public TagTrainingPlanAsNewHashtagCommandHandler(
            ITrainingPlanRepository trainingPlanRepository,
            ITrainingHashtagRepository hashtagRepository,
            ILogger<TagTrainingPlanAsNewHashtagCommandHandler> logger
            )
        {
            _trainingPlanRepository = trainingPlanRepository ?? throw new ArgumentNullException(nameof(trainingPlanRepository));
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
                TrainingPlanRoot plan = _trainingPlanRepository.Find(message.TrainingPlanId);
                plan.TagAs(hashtag.Id);

                _logger.LogInformation("----- Tagging {@TrainingPlan} with {@HashtagId}", plan, hashtag.Id);

                _trainingPlanRepository.Modify(plan);

                result = await _trainingPlanRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _trainingPlanRepository.UnitOfWork);
                result = false;
            }
            return result;
        }
    }

}
