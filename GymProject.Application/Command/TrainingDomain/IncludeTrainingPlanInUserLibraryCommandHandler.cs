using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class IncludeTrainingPlanInUserLibraryCommandHandler : IRequestHandler<IncludeTrainingPlanInUserLibraryCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepository;
        private readonly ILogger<IncludeTrainingPlanInUserLibraryCommandHandler> _logger;





        public IncludeTrainingPlanInUserLibraryCommandHandler(IAthleteRepository athleteRepository,ILogger<IncludeTrainingPlanInUserLibraryCommandHandler> logger)
        {
            _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(IncludeTrainingPlanInUserLibraryCommand message, CancellationToken cancellationToken)
        {
            try
            {
                AthleteRoot athlete = _athleteRepository.Find(message.UserId);

                _logger.LogInformation("----- Adding Training Plan {@PlanId} to User Training Plan Library {@Athlete}", message.TrainingPlanId, athlete);

                athlete.AddTrainingPlanToLibrary(message.TrainingPlanId);
                _athleteRepository.Modify(athlete);

                return await _athleteRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch(Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _athleteRepository.UnitOfWork);
                return false;
            }

            return true;
        }
    }

}
