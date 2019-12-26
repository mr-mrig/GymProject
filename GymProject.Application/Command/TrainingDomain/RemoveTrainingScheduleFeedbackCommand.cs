using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class RemoveTrainingScheduleFeedbackCommand  : IRequest<bool>
    {


        public uint OwnerId { get; private set; }
        public uint ScheduleId { get; private set; }




        public RemoveTrainingScheduleFeedbackCommand(uint ownerId, uint scheduleId)
        {
            OwnerId = ownerId;
            ScheduleId = scheduleId;
        }



    }
}
