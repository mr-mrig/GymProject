using MediatR;
using System;

namespace GymProject.Application.Command.TrainingDomain
{

    public class RescheduleTrainingPlanCommand : IRequest<bool>
    {


        public uint TrainingScheduleId { get; private set; }
        public DateTime StartDate { get; private set; }



        public RescheduleTrainingPlanCommand(uint trainingScheduleId, DateTime startDate)
        {
            TrainingScheduleId = trainingScheduleId;
            StartDate = startDate;
        }

    }
}
