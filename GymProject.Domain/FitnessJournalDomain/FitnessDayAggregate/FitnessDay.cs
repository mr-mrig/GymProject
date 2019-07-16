using GymProject.Domain.Base;
using GymProject.Domain.FitnessJournalDomain.Common;
using GymProject.Domain.FitnessJournalDomain.MusAggregate;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class FitnessDay : Entity<IdType>, IAggregateRoot
    {


        #region Properties
        public DateTime DayDate { get; private set; }
        public RatingValue Rating { get; private set; } = null;

        // FK
        public long PostId { get; private set; }

        /// <summary>
        /// The daily Activity tracked
        /// </summary>
        public DailyActivityValue DailyActivity { get; private set; } = null;


        private DailyDietValue _dietDay = null;
        public DailyDietValue DietDay
        {
            get => _dietDay;
            set => _dietDay = value;
        }

        /// <summary>
        /// The daily Weight measure
        /// </summary>
        public BodyWeightValue DailyWeight { get; private set; } = null;

        /// <summary>
        /// The daily Wellness parameters
        /// </summary>
        public DailyWellnessValue DailyWellness { get; private set; } = null;
        #endregion



        #region Ctors


        private FitnessDay(DateTime dayDate, RatingValue rating = null)
        {
            DayDate = dayDate;
            Rating = rating;
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory
        /// </summary>
        /// <param name="dayDate">The date of the day to be tracked</param>
        /// <param name="rating">The rating</param>
        /// <returns>The new FitnessDay instance</returns>
        public static FitnessDay TrackDay(DateTime dayDate, RatingValue rating = null)
        {
            return new FitnessDay(dayDate, rating);
        }
        #endregion



        #region Aggregate Methods

        /// <summary>
        /// Change the Rating of the entry
        /// </summary>
        /// <param name="newRating">The new rating</param>
        public void ChangeRating(RatingValue newRating)
        {
            Rating = newRating;
        }

        /// <summary>
        /// Change the DayDate of the entry
        /// </summary>
        /// <param name="newDate">The new date</param>
        /// 
        public void ChangeDate(DateTime newDate)
        {
            DayDate = newDate;
        }
        #endregion


        #region Daily Wellness Methods

        /// <summary>
        /// Attach the Wellness Day information
        /// </summary>
        /// <param name="temperature">The temperature to be tracked</param>
        /// <param name="glycemia">The glycemia to be tracked</param>
        /// <param name="musList">The  MUSes diagnosed</param>
        public void TrackWellnessDay(TemperatureValue temperature = null, GlycemiaValue glycemia = null, ICollection<MusReference> musList = null)
        {
            DailyWellness = DailyWellnessValue.TrackWellness(temperature, glycemia, musList);
        }

        /// <summary>
        /// Attach the selected MUS diagnosis to the Day
        /// </summary>
        /// <param name="toAdd">The Id of the MUS to be diagnosed</param>
        public void DiagnoseMus(MusReference toAdd)
        {
            DailyWellness = DailyWellness.DiagnoseMus(toAdd);
        }

        #endregion


        #region Daily Weight Methods

        /// <summary>
        /// Attach the daily Weight
        /// </summary>
        /// <param name="weight">The weight to be tracked</param>
        /// <param name="measUnit">The meassure unit</param>
        public void TrackWeight(float weight, WeightUnitMeasureEnum measUnit)
        {
            DailyWeight = BodyWeightValue.Measure(weight, measUnit);
        }

        /// <summary>
        /// Attach the daily Weight [Kg]
        /// </summary>
        /// <param name="weight">The weight [Kg] to be tracked</param>
        public void TrackWeightKilograms(float weight)
        {
            DailyWeight = BodyWeightValue.Measure(weight, WeightUnitMeasureEnum.Kilograms);
        }

        /// <summary>
        /// Attach the daily Weight [lbs]
        /// </summary>
        /// <param name="weight">The weight [lbs] to be tracked</param>
        public void TrackWeightPounds(float weight)
        {
            DailyWeight = BodyWeightValue.Measure(weight, WeightUnitMeasureEnum.Pounds);
        }

        #endregion


        #region Daily Activity Metohds

        /// <summary>
        /// Track the daily activity
        /// </summary>
        /// <param name="steps">Number of steps tracked</param>
        /// <param name="stairs">Number of stairs tracked</param>
        /// <param name="burnedKcal">Burned calories estimated</param>
        /// <param name="sleepTime">Sleep duration</param>
        /// <param name="sleepQuality">Sleep rating</param>
        /// <param name="restHeartRate">Rest heartrate tracked</param>
        /// <param name="maxHeartRate">Heartrate after activity tracked</param>
        public void TrackActivity(
            uint? steps = null,
            uint? stairs = null,
            CalorieValue burnedKcal = null,
            SleepDurationValue sleepTime = null,
            RatingValue sleepQuality = null,
            HeartRateValue restHeartRate = null,
            HeartRateValue maxHeartRate = null)
        {
            DailyActivity = DailyActivityValue.TrackActivity(steps, stairs, burnedKcal, sleepTime, sleepQuality, restHeartRate, maxHeartRate);
        }
        #endregion
    }
}
