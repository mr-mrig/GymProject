using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;

namespace GymProject.Domain.TrainingDomain.Common
{
    public interface IWorkingSet : ITrainingWork
    {

        /// <summary>
        /// The rest between the working set and the following
        /// </summary>
        RestPeriodValue Rest { get; }

        /// <summary>
        /// The effort of the WS
        /// </summary>
        TrainingEffortValue Effort { get; }


        /// <summary>
        /// Get the number of repetitions
        /// </summary>
        /// <returns>The number of repetitions</returns>
        int ToRepetitions();


        /// <summary>
        /// Get the duration of the WS [s]
        /// </summary>
        /// <returns>The number of seconds under tension</returns>
        int ToSecondsUnderTension();


        /// <summary>
        /// Get the workload as repetitions number * weight lifted
        /// </summary>
        /// <returns>The workload [Kg/lbs]</returns>
        WeightPlatesValue ToWorkload();


        /// <summary>
        /// Get the duration of the WS in terms of TUT + rest [s]
        /// </summary>
        /// <returns>The number of seconds the WS requires</returns>
        int ToTotalSeconds();


        /// <summary>
        /// Changes the WS effort type to the one specified, with respect to the target repetitions.
        /// <param name="toEffortType">The effort type to convert to</param>
        /// </summary>
        void ToNewEffortType(TrainingEffortTypeEnum toEffortType);

    }
}
