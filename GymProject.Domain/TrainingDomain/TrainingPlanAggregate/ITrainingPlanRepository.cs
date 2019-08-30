using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
{
    public interface ITrainingPlanRepository : IRepository<TrainingPlan>
    {


        /// <summary>
        /// Fetch the Aggregate with the specifed Id
        /// </summary>
        /// <param name="id">The ID to be fetched</param>
        /// <returns>The Aggregate</returns>
        TrainingPlan WithId(IdTypeValue id);

    }
}
