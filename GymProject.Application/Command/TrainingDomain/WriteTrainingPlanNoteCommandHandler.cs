using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    public class WriteTrainingPlanNoteCommandHandler : IRequestHandler<WriteTrainingPlanNoteCommand, bool>
    {

        private readonly ITrainingProgramRepository _trainingPlanRepository;
        private readonly ILogger<WriteTrainingPlanNoteCommandHandler> _logger;



        public WriteTrainingPlanNoteCommandHandler(ITrainingProgramRepository trainingPlanRepository, ILogger<WriteTrainingPlanNoteCommandHandler> logger)
        {
            _trainingPlanRepository = trainingPlanRepository ?? throw new ArgumentNullException(nameof(trainingPlanRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<bool> Handle(WriteTrainingPlanNoteCommand message, CancellationToken cancellationToken)
        {
            bool result = false;

            TrainingPlanRoot plan = _trainingPlanRepository.Find(message.TrainingPlanId);

            if (plan == null)
                return false;

            try
            {
                throw new NotImplementedException("How do we deal with transactions?");




                //plan.WriteNote(message.NoteBody);

                //_logger.LogInformation("----- Attaching {@NoteId} to {@TrainingPlanId}", message.TrainingPlanNoteId, message.TrainingPlanId);

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
