using GymProject.Domain.Base;
using GymProject.Domain.FitnessJournalDomain.Common;
using GymProject.Domain.FitnessJournalDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using System.Collections.Generic;
using System.Linq;


namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class DailyActivityValue : ValueObject
    {

        /// <summary>
        ///  The number of steps
        /// </summary>
        public uint? Steps { get; private set; }


        /// <summary>
        /// The number of stairs
        /// </summary>
        public uint? Stairs { get; private set; }

        /// <summary>
        /// The estimated burned calories
        /// </summary>
        public CalorieValue BurnedCalories { get; private set; } = null;

        /// <summary>
        /// The sleep duration [h]
        /// </summary>
        public SleepDurationValue SleepTime { get; private set; } = null;

        /// <summary>
        /// Sleep quality as rated by the user
        /// </summary>
        public RatingValue SleepQuality { get; private set; } = null;

        /// <summary>
        /// Resting heart rate
        /// </summary>
        public HeartRateValue HeartRateRest { get; private set; } = null;

        /// <summary>
        /// Heart rate after activity
        /// </summary>
        public HeartRateValue HeartRateMax { get; private set; } = null;



        #region Ctors

        private DailyActivityValue()
        {

        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="steps">Number of steps tracked</param>
        /// <param name="stairs">Number of stairs tracked</param>
        /// <param name="burnedKcal">Burned calories estimated</param>
        /// <param name="sleepTime">Sleep duration</param>
        /// <param name="sleepQuality">Sleep rating</param>
        /// <param name="restHeartRate">Rest heartrate tracked</param>
        /// <param name="maxHeartRate">Heartrate after activity tracked</param>
        /// <returns>The DailyActivityValue instance</returns>
        public static DailyActivityValue TrackActivity(
            uint? steps = null, 
            uint? stairs = null, 
            CalorieValue burnedKcal = null, 
            SleepDurationValue sleepTime = null, 
            RatingValue sleepQuality = null, 
            HeartRateValue restHeartRate = null, 
            HeartRateValue maxHeartRate = null)
        {
            if (steps is null && stairs is null && burnedKcal is null && sleepTime is null && sleepQuality is null && restHeartRate is null && maxHeartRate is null)
                throw new FitnessJournalDomainGenericException($"Cannot create a DailyActivityValue with all NULL fields");

            return new DailyActivityValue()
            {
                Steps = steps,
                Stairs = stairs,
                BurnedCalories = burnedKcal,
                SleepTime = sleepTime,
                SleepQuality = sleepQuality,
                HeartRateRest = restHeartRate,
                HeartRateMax = maxHeartRate,
            };
        }
        #endregion



        #region Business Methods


        /// <summary>
        /// Checks whether all the properties are null
        /// </summary>
        /// <returns>True if no there are no non-null properties</returns>
        public bool CheckNullState()
        {
            return GetAtomicValues().All(x => x is null);
        }
        #endregion


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Steps;
            yield return BurnedCalories;
            yield return Stairs;
            yield return SleepTime;
            yield return SleepQuality;
            yield return HeartRateRest;
            yield return HeartRateMax;
        }
    }
}
