using System.Collections.Generic;
using System.Linq;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class DailyDietValue : ValueObject
    {


        public MacronutirentWeightValue CarbGrams { get; set; } = null;
        public MacronutirentWeightValue FatGrams { get; set; } = null;
        public MacronutirentWeightValue ProteinGrams { get; set; } = null;
        public float? SaltGrams { get; set; }
        public float? WaterLiters { get; set; }
        public bool? IsFreeMeal { get; set; } = null;

        //public DietDayType DietDayType { get; set; } = null;




        #region Ctors

        private DailyDietValue()
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
        /// <returns>The DailyDietValue instance</returns>
        public static DailyDietValue TrackActivity(
            uint? steps = null,
            uint? stairs = null,
            CalorieValue burnedKcal = null,
            SleepDurationValue sleepTime = null,
            RatingValue sleepQuality = null,
            HeartRateValue restHeartRate = null,
            HeartRateValue maxHeartRate = null)
        {
            if (steps is null && stairs is null && burnedKcal is null && sleepTime is null && sleepQuality is null && restHeartRate is null && maxHeartRate is null)
                throw new FitnessJournalDomainGenericException($"Cannot create a DailyDietValue with all NULL fields");

            return new DailyDietValue()
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
            yield return CarbGrams;
            yield return FatGrams;
            yield return ProteinGrams;
            yield return SaltGrams;
            yield return WaterLiters;
            yield return IsFreeMeal;
        }


    }
}
