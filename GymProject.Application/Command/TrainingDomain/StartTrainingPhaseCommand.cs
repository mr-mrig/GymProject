﻿using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class StartTrainingPhaseCommand : IRequest<bool>
    {


        public uint AthleteId { get; private set; }
        public uint TrainingPhaseId { get; private set; }
        public uint EntryStatusId { get; private set; }
        public string OwnerNote { get; private set; }



        public StartTrainingPhaseCommand(uint athleteId, uint trainingPhaseId, uint entryStatusId, string ownerNote)
        {
            TrainingPhaseId = trainingPhaseId;
            AthleteId = athleteId;
            EntryStatusId = entryStatusId;
            OwnerNote = ownerNote;
        }

    }
}
