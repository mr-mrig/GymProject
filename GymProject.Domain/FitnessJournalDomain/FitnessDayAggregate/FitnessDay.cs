using GymProject.Domain.Base;
using GymProject.Domain.FitnessJournalDomain.Common;
using GymProject.Domain.FitnessJournalDomain.Exceptions;
using GymProject.Domain.FitnessJournalDomain.MusAggregate;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class FitnessDay : Entity<IdTypeValue>, IAggregateRoot
    {


        #region Properties

        /// <summary>
        /// Fitness Journal date
        /// </summary>
        public DateTime DayDate { get; private set; }

        /// <summary>
        /// Rating
        /// </summary>
        public RatingValue Rating { get; private set; } = null;

        // FK -> Don't fetch any other fields, as they might slow the process a lot
        public IdTypeValue PostId { get; private set; }

        /// <summary>
        /// The daily Activity tracked
        /// </summary>
        public DailyActivityValue DailyActivity { get; private set; } = null;

        /// <summary>
        /// Diet Day tracked
        /// </summary>
        public DailyDietValue DailyDiet { get; private set; } = null;

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

            if (DayDate == null)
                throw new FitnessJournalDomainInvariantViolationException($"{GetType().Name} must have a valid Date");
        }


        private FitnessDay(IdTypeValue postId, DateTime dayDate, RatingValue rating = null)
        {
            PostId = postId;
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
        public static FitnessDay StartTrackingDay(DateTime dayDate, RatingValue rating = null)
        {
            return new FitnessDay(dayDate, rating);
        }
        #endregion



        #region Aggregate Root Methods

        /// <summary>
        /// Change the Rating of the entry
        /// </summary>
        /// <param name="newRating">The new rating</param>
        public void ChangeRating(RatingValue newRating)
        {
            Rating = newRating;
            AddDomainEvent(new FitnessDayChangedDomainEvent(this, PostId));
        }

        /// <summary>
        /// Change the DayDate of the entry
        /// </summary>
        /// <param name="newDate">The new date</param>
        /// 
        public void ChangeDate(DateTime newDate)
        {
            DayDate = newDate;

            if (DayDate == null)
                throw new FitnessJournalDomainInvariantViolationException($"{GetType().Name} must have a valid Date");


            AddDomainEvent(new FitnessDayChangedDomainEvent(this, PostId));
        }


        /// <summary>
        /// Linke the entry to a Post
        /// </summary>
        /// <param name="postId">The ID of the post to be linked</param>
        public void LinkToPost(IdTypeValue postId)
        {
            PostId = postId;
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
            AddDomainEvent(new FitnessDayChangedDomainEvent(this, PostId));
        }


        /// <summary>
        /// Attach the selected MUS diagnosis to the Day
        /// </summary>
        /// <param name="toAdd">The Id of the MUS to be diagnosed</param>
        public void DiagnoseMus(MusReference toAdd)
        {
            DailyWellness = DailyWellness.DiagnoseMus(toAdd);
            AddDomainEvent(new FitnessDayChangedDomainEvent(this, PostId));
        }


        /// <summary>
        /// Remove the selected MUS
        /// </summary>
        /// <param name="toRemove">The Id of the MUS to be removed</param>
        public void UndiagnoseMus(MusReference toRemove)
        {
            DailyWellness = DailyWellness.RemoveMus(toRemove);
            AddDomainEvent(new FitnessDayChangedDomainEvent(this, PostId));
        }


        /// <summary>
        /// Remove the selected MUS
        /// </summary>
        /// <param name="toRemoveId">The Id of the MUS to be removed</param>
        public void UndiagnoseMus(IdTypeValue toRemoveId)
        {
            DailyWellness = DailyWellness.RemoveMus(toRemoveId);
            AddDomainEvent(new FitnessDayChangedDomainEvent(this, PostId));
        }


        /// <summary>
        /// Remove the Wellness entry from the parent
        /// </summary>
        public void UntrackWellness()
        {
            DailyWellness = null;
            AddDomainEvent(new FitnessDayChangedDomainEvent(this, PostId));

            if (CheckNullEntries())
                AddDomainEvent(new FitnessHasBeenClearedDomainEvent(this, PostId));
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
            AddDomainEvent(new FitnessDayChangedDomainEvent(this, PostId));
        }

        /// <summary>
        /// Attach the daily Weight [Kg]
        /// </summary>
        /// <param name="weight">The weight [Kg] to be tracked</param>
        public void TrackWeightKilograms(float weight)
        {
            DailyWeight = BodyWeightValue.Measure(weight, WeightUnitMeasureEnum.Kilograms);
            AddDomainEvent(new FitnessDayChangedDomainEvent(this, PostId));
        }

        /// <summary>
        /// Attach the daily Weight [lbs]
        /// </summary>
        /// <param name="weight">The weight [lbs] to be tracked</param>
        public void TrackWeightPounds(float weight)
        {
            DailyWeight = BodyWeightValue.Measure(weight, WeightUnitMeasureEnum.Pounds);
            AddDomainEvent(new FitnessDayChangedDomainEvent(this, PostId));
        }

        /// <summary>
        /// Remove the Weight entry from the parent
        /// </summary>
        public void UntrackWeight()
        {
            DailyWeight = null;
            AddDomainEvent(new FitnessDayChangedDomainEvent(this, PostId));

            if (CheckNullEntries())
                AddDomainEvent(new FitnessHasBeenClearedDomainEvent(this, PostId));
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
            AddDomainEvent(new FitnessDayChangedDomainEvent(this, PostId));
        }

        /// <summary>
        /// Remove the Activity entry from the parent
        /// </summary>
        public void UntrackActivity()
        {
            DailyActivity = null;
            AddDomainEvent(new FitnessDayChangedDomainEvent(this, PostId));

            if (CheckNullEntries())
                AddDomainEvent(new FitnessHasBeenClearedDomainEvent(this, PostId));
        }
        #endregion


        #region Diet Day Metohds

        /// <summary>
        /// Track the diet day
        /// </summary>
        /// <param name="steps">Number of steps tracked</param>
        /// <param name="stairs">Number of stairs tracked</param>
        /// <param name="burnedKcal">Burned calories estimated</param>
        /// <param name="sleepTime">Sleep duration</param>
        /// <param name="sleepQuality">Sleep rating</param>
        /// <param name="restHeartRate">Rest heartrate tracked</param>
        /// <param name="maxHeartRate">Heartrate after activity tracked</param>
        public void TrackDiet(
            MacronutirentWeightValue carbs = null,
            MacronutirentWeightValue fats = null,
            MacronutirentWeightValue proteins = null,
            MicronutirentWeightValue salt = null,
            VolumeValue water = null,
            bool? isFreeMeal = null,
            DietDayTypeEnum dayType = null)
        {
            DailyDiet = DailyDietValue.TrackDiet(carbs, fats, proteins, salt, water, isFreeMeal, dayType);
            AddDomainEvent(new FitnessDayChangedDomainEvent(this, PostId));
        }


        /// <summary>
        /// Remove the Diet entry from the parent
        /// </summary>
        public void UntrackDiet()
        {
            DailyDiet = null;
            AddDomainEvent(new FitnessDayChangedDomainEvent(this, PostId));

            if (CheckNullEntries())
                AddDomainEvent(new FitnessHasBeenClearedDomainEvent(this, PostId));
        }
        #endregion



        /// <summary>
        /// Checks wether all the fitness entries are null -> invalid state
        /// </summary>
        /// <returns>True if all fitness entries are null</returns>
        private bool CheckNullEntries() => DailyActivity == null && DailyDiet == null && DailyWeight == null && DailyWellness == null;
    }
}
