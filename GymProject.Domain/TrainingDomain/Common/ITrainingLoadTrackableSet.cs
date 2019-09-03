using System;

namespace GymProject.Domain.TrainingDomain.Common
{
    public interface ITrainingLoadTrackableSet : ICloneable
    {



        /// <summary>
        /// Get the number of repetitions
        /// </summary>
        /// <returns>The number of repetitions</returns>
        int ToRepetitions();


        /// <summary>
        /// Get the workload as repetitions number * weight lifted
        /// </summary>
        /// <returns>The workload [Kg/lbs]</returns>
        WeightPlatesValue ToWorkload();


    }
}
