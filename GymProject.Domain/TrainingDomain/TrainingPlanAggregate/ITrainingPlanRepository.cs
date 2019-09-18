using GymProject.Domain.Base;

namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
{
    public interface ITrainingPlanRepository : IRepository<TrainingPlanRoot>
    {


        TrainingPlanRoot Find(uint id);

    }
}
