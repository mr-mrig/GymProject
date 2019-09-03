using GymProject.Domain.TrainingDomain.TrainingPlanAggregate;

namespace GymProject.Domain.TrainingDomain.Common
{
    public interface IFullWorkingSet : ITrainingLoadTrackableSet
    {



        /// <summary>
        /// The effort of the WS
        /// </summary>
        TrainingEffortValue Effort { get; }


        /// <summary>
        /// Get the duration of the WS [s]
        /// </summary>
        /// <returns>The number of seconds under tension</returns>
        int ToSecondsUnderTension();


        /// <summary>
        /// Get the duration of the WS in terms of TUT + rest [s]
        /// </summary>
        /// <returns>The number of seconds the WS requires</returns>
        int ToTotalSeconds();


        /// <summary>
        /// Get the rest interval between the set and the following one [s]
        /// </summary>
        /// <returns>The rest period</returns>
        int ToRest();


        /// <summary>
        /// Changes the WS effort type to the one specified, with respect to the target repetitions.
        /// <param name="toEffortType">The effort type to convert to</param>
        /// </summary>
        void ToNewEffortType(TrainingEffortTypeEnum toEffortType);

    }
}
