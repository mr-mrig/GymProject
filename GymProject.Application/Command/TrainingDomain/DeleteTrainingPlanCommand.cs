using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{
    public class DeleteTrainingPlanCommand : IRequest<bool>
    {


        public uint TrainingPlanId { get; private set; }



        public DeleteTrainingPlanCommand(uint trainingPlanId)
        {
            TrainingPlanId = trainingPlanId;
        }

    }
}
