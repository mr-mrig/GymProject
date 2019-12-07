using MediatR;
using System;

namespace GymProject.Application.Command.TrainingDomain
{

    public class PlanTrainingPhaseCommand : IRequest<bool>
    {


        public uint AthleteId { get; private set; }
        public uint TrainingPhaseId { get; private set; }
        public uint EntryStatusId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public string OwnerNote { get; private set; }



        public PlanTrainingPhaseCommand(uint athleteId, uint trainingPhaseId, uint entryStatusId, string ownerNote, DateTime startDate, DateTime? endDate)
        {
            TrainingPhaseId = trainingPhaseId;
            AthleteId = athleteId;
            EntryStatusId = entryStatusId;
            OwnerNote = ownerNote;
            StartDate = startDate;
            EndDate = endDate;
        }

    }
}
