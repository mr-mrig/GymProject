using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class ExcludeTrainingPlanFromUserLibraryCommand : IRequest<bool>
    {


        
        public uint TrainingPlanId { get; private set; }
        public uint UserId { get; private set; }




        public ExcludeTrainingPlanFromUserLibraryCommand(uint trainingPlanId, uint userId)
        {
            TrainingPlanId = trainingPlanId;
            UserId = userId;
        }

    }
}
