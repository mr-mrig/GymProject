using GymProject.Domain.Base;

namespace GymProject.Domain.TrainingDomain.AthleteAggregate
{
    public interface IAthleteRepository : IRepository<AthleteRoot>
    {


        /// <summary>
        /// Count the athletes having the specified Training Plan in their training plan library
        /// </summary>
        /// <param name="trainingPlanId">The ID of the Training Plan</param>
        /// <returns>The counter</returns>
        int CountAthletesWithTrainingPlanInLibrary(uint trainingPlanId);

    }
}
