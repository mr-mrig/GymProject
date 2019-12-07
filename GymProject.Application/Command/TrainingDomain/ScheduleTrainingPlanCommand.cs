using MediatR;
using System;

namespace GymProject.Application.Command.TrainingDomain
{

    public class ScheduleTrainingPlanCommand : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }
        public uint AthleteId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }



        public ScheduleTrainingPlanCommand(uint trainingPlanId, uint athleteId, DateTime startDate, DateTime? endDate)
        {
            TrainingPlanId = trainingPlanId;
            AthleteId = athleteId;
            StartDate = startDate;
            EndDate = endDate;
        }

    }
}
