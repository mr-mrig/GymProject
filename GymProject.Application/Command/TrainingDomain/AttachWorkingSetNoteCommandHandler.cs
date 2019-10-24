﻿
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Application.Command.TrainingDomain
{
    //public class AttachWorkingSetNoteCommandHandler : IRequestHandler<AttachWorkingSetNoteCommand, bool>
    //{

    //    private readonly IWorkoutTemplateRepository _workoutRepository;
    //    private readonly ILogger<AttachWorkingSetNoteCommandHandler> _logger;



    //    public AttachWorkingSetNoteCommandHandler(IWorkoutTemplateRepository workoutRepository, ILogger<AttachWorkingSetNoteCommandHandler> logger)
    //    {
    //        _workoutRepository = workoutRepository ?? throw new ArgumentNullException(nameof(workoutRepository));
    //        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    //    }


    //    public async Task<bool> Handle(AttachWorkingSetNoteCommand message, CancellationToken cancellationToken)
    //    {
    //        bool result = false;

    //        WorkoutTemplateRoot workout = _workoutRepository.Find(message.WorkoutTemplateId);

    //        if (workout == null)
    //            return false;

    //        try
    //        {
    //            TUTValue tempo = TUTValue.PlanTUT(message.TutValue);

    //            workout.note(message.WorkUnitProgressiveNumber, message.WorkingSetProgressiveNumber, tempo);

    //            _logger.LogInformation("----- Setting WS tempo of [{@WorkingSetProgressiveNumber} - {@WorkUnitProgressiveNumber}] in {@workout.Id}"
    //                ,message.WorkingSetProgressiveNumber, message.WorkUnitProgressiveNumber, workout.Id);

    //            _workoutRepository.Modify(workout);
    //            result = await _workoutRepository.UnitOfWork.SaveAsync(cancellationToken);
    //        }
    //        catch (Exception exc)
    //        {
    //            _logger.LogError(exc, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", exc.Message, _workoutRepository.UnitOfWork);
    //            result = false;
    //        }

    //        return result;
    //    }

    //}
}