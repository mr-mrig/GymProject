
using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class DetachTrainingPlanNoteCommand  : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }




        public DetachTrainingPlanNoteCommand(uint trainingPlanId)
        {
            TrainingPlanId = trainingPlanId;
        }



    }
}
