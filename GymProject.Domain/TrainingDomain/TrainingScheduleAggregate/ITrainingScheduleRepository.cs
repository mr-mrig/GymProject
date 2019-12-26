using GymProject.Domain.Base;

namespace GymProject.Domain.TrainingDomain.TrainingScheduleAggregate
{
    public interface ITrainingScheduleRepository : IRepository<TrainingScheduleRoot>
    {

        /// <summary>
        /// Get the schedule the specified athlete is currently on - if any.
        /// </summary>
        /// <param name="athleteId">The user ID to be query for</param>
        /// <returns>The Training Schedule the athelte is following or null</returns>
        TrainingScheduleRoot GetCurrentScheduleByAthleteOrDefault(uint athleteId);

    }
}
