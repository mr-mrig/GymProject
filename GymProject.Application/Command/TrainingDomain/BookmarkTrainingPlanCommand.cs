using MediatR;

namespace GymProject.Application.Command.TrainingDomain
{

    public class BookmarkTrainingPlanCommand : IRequest<bool>
    {


        public uint AthleteId { get; private set; }
        public uint TrainingPlanId { get; private set; }
        public bool MakeBookmarked { get; private set; }




        public BookmarkTrainingPlanCommand(uint athleteId, uint trainingPlanId, bool makeBookmarked)
        {
            AthleteId = athleteId;
            TrainingPlanId = trainingPlanId;
            MakeBookmarked = makeBookmarked;
        }

    }
}
