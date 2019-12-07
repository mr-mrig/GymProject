using GymProject.Domain.TrainingDomain.AthleteAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class BookmarkTrainingPlanCommandHandler : IRequestHandler<BookmarkTrainingPlanCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepo;
        private readonly ILogger<BookmarkTrainingPlanCommandHandler> _logger;





        public BookmarkTrainingPlanCommandHandler(
            IAthleteRepository athleteRepository,
            ILogger<BookmarkTrainingPlanCommandHandler> logger
            )
        {
            _athleteRepo = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(BookmarkTrainingPlanCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            try
            {
                AthleteRoot athlete = _athleteRepo.Find(message.AthleteId);
                athlete.BookmarkTrainingPlan(message.TrainingPlanId, message.MakeBookmarked);

                _logger.LogInformation("----- Bookmarking {@TrainingPlanId} of {@Athlete} with {@IsBookmarked}", message.TrainingPlanId, athlete, message.MakeBookmarked);

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
