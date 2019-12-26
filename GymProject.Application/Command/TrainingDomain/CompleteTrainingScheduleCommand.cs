using MediatR;
using System;

namespace GymProject.Application.Command.TrainingDomain
{

    public class CompleteTrainingScheduleCommand : IRequest<bool>
    {


        public uint TrainingScheduleId { get; private set; }
        public DateTime EndDate { get; private set; }



        public CompleteTrainingScheduleCommand(uint trainingScheduleId, DateTime endDate)
        {
            TrainingScheduleId = trainingScheduleId;
            EndDate = endDate;
        }

    }
}
