using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using GymProject.Domain.TrainingDomain.TrainingPhaseAggregate;
using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    // Regular CommandHandler
    public class FakeExcludeTrainingPlanFromUserLibraryCommandHandler : IRequestHandler<ExcludeTrainingPlanFromUserLibraryCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepository;
        private readonly ITrainingPlanRepository _planRepository;
        private readonly ILogger<FakeExcludeTrainingPlanFromUserLibraryCommandHandler> _logger;





        public FakeExcludeTrainingPlanFromUserLibraryCommandHandler(IAthleteRepository athleteRepository, ITrainingPlanRepository planRepository, ILogger<FakeExcludeTrainingPlanFromUserLibraryCommandHandler> logger)
        {
            _planRepository = planRepository ?? throw new ArgumentNullException(nameof(planRepository));
            _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(ExcludeTrainingPlanFromUserLibraryCommand message, CancellationToken cancellationToken)
        {
            try
            {
                AthleteRoot athlete = _athleteRepository.Find(message.UserId);

                _logger.LogInformation("----- Removing Training Plan {@PlanId} from User Training Plan Library {@Athlete}", message.TrainingPlanId, athlete);

                athlete.RemoveTrainingPlanFromLibrary(message.TrainingPlanId);      // Domain event is raised
                _athleteRepository.Modify(athlete);
            }
            catch(Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _athleteRepository.UnitOfWork);
                return false;
            }

            // Check if the Training Plan should be deleted as well as it is the last one
            try
            {
                if (_athleteRepository.CountAthletesWithTrainingPlanInLibrary(message.TrainingPlanId) == 1)   // Data read might be stale - Should we do this here or maybe run a schedudled script for orphans removing?
                {
                    TrainingPlanRoot plan = _planRepository.Find(message.TrainingPlanId);

                    _logger.LogInformation("----- Removing Training Plan {@TrainingPlan} ", plan);
                    _planRepository.Remove(plan);

                    throw new Exception();
                }
                else
                    await _athleteRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _planRepository.UnitOfWork);
                return false;
            }
            return true;
        }
    }

}
