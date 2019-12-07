using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class IncludeTrainingPlanInUserLibraryCommand : IRequest<bool>
    {


        
        public uint TrainingPlanId { get; private set; }
        public uint UserId { get; private set; }




        public IncludeTrainingPlanInUserLibraryCommand(uint trainingPlanId, uint userId)
        {
            TrainingPlanId = trainingPlanId;
            UserId = userId;
        }

    }
}
