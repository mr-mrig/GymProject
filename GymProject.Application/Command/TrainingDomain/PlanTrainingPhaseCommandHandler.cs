using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    public class PlanTrainingPhaseCommandHandler : IRequestHandler<PlanTrainingPhaseCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepository;
        private readonly ILogger<PlanTrainingPhaseCommandHandler> _logger;





        public PlanTrainingPhaseCommandHandler(IAthleteRepository athleteRepository,ILogger<PlanTrainingPhaseCommandHandler> logger)
        {
            _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(PlanTrainingPhaseCommand message, CancellationToken cancellationToken)
        {
            bool result;

            try
            {
                AthleteRoot athlete = _athleteRepository.Find(message.AthleteId);

                _logger.LogInformation("----- Planning Training Phase {@PhaseId} for Athlete {@Athlete} from {@StartDate} to {@EndDate}", message.TrainingPhaseId, athlete, message.StartDate, message.EndDate);

                athlete.StartTrainingPhase(message.TrainingPhaseId, 
                    EntryStatusTypeEnum.From((int)message.EntryStatusId), 
                    message.StartDate, 
                    message.EndDate,
                    message.OwnerNote == null ? null : PersonalNoteValue.Write(message.OwnerNote));
                _athleteRepository.Modify(athlete);

                result = await _athleteRepository.UnitOfWork.SaveAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _athleteRepository.UnitOfWork);
                result = false;
            }

            return result;
        }
    }

}
