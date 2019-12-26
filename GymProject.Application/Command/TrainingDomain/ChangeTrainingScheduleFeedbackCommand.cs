using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class ChangeTrainingScheduleFeedbackCommand  : IRequest<bool>
    {


        public uint OwnerId { get; private set; }
        public uint ScheduleId { get; private set; }
        public float? Rating { get; private set; }
        public string Comment { get; private set; }




        public ChangeTrainingScheduleFeedbackCommand(uint ownerId, uint scheduleId, float? rating, string comment)
        {
            OwnerId = ownerId;
            ScheduleId = scheduleId;
            Rating = rating;
            Comment = comment;
        }



    }
}
