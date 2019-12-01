
using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class DetachTrainingPlanNoteCommand  : IRequest<bool>
    {


        public uint UserId { get; private set; }
        public uint TrainingPlanId { get; private set; }




        public DetachTrainingPlanNoteCommand(uint userId, uint trainingPlanId)
        {
            UserId = userId;
            TrainingPlanId = trainingPlanId;
        }



    }
}
