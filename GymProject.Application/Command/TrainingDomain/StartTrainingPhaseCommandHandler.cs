using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.AthleteAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{


    public class StartTrainingPhaseCommandHandler : IRequestHandler<StartTrainingPhaseCommand, bool>
    {


        private readonly IAthleteRepository _athleteRepository;
        private readonly ILogger<StartTrainingPhaseCommandHandler> _logger;





        public StartTrainingPhaseCommandHandler(IAthleteRepository athleteRepository,ILogger<StartTrainingPhaseCommandHandler> logger)
        {
            _athleteRepository = athleteRepository ?? throw new ArgumentNullException(nameof(athleteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }




        public async Task<bool> Handle(StartTrainingPhaseCommand message, CancellationToken cancellationToken)
        {
            bool result;

            try
            {
                PersonalNoteValue note = null;
                DateTime startingDate = DateTime.UtcNow.Date;
                AthleteRoot athlete = _athleteRepository.Find(message.AthleteId);

                _logger.LogInformation("----- Starting Training Phase {@PhaseId} for Athlete {@Athlete}", message.TrainingPhaseId, athlete);

                // If another phase is starting today, the first of the two is a mistake -> remove it
                // This application logic, not domain
                var ongoingPhase = athlete.ClonePhaseStartingFrom(startingDate);
                if (ongoingPhase != null)
                {
                    athlete.RemoveTrainingPhase(ongoingPhase);
                    result = await _athleteRepository.UnitOfWork.SaveAsync(cancellationToken).ConfigureAwait(false);
                }

                if (message.OwnerNote != null)
                    note = PersonalNoteValue.Write(message.OwnerNote);

                athlete.StartTrainingPhase(message.TrainingPhaseId, EntryStatusTypeEnum.From((int)message.EntryStatusId), ownerNote: note);
                _athleteRepository.Modify(athlete);

                result = await _athleteRepository.UnitOfWork.SaveAsync(cancellationToken).ConfigureAwait(false);
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
