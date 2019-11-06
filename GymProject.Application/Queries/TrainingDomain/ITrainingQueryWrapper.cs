using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymProject.Application.Queries.TrainingDomain
{
    public interface ITrainingQueryWrapper
    {

        /// <summary>
        /// Get the Summary for all the Training Plans belonging to the specified owner
        /// </summary>
        /// <param name="ownerId">The owner of the Training Plans to be found</param>
        /// <returns>The Task for the Training Plan Summaries</returns>
        Task<IEnumerable<TrainingPlanSummaryDto>> GetTraininPlansSummaries(uint ownerId);

    }
}
