using GymProject.Domain.Base;
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

        private ActivityDayValue _activityDay = null;
        public ActivityDayValue ActivityDay
        {
            get => _activityDay;
            set => _activityDay = value;
        }


        private DietDayValue _dietDay = null;
        public DietDayValue DietDay
        {
            get => _dietDay;
            set => _dietDay = value;
        }


        private WeightValue _weight = null;
        public WeightValue Weight
        {
            get => _weight;
            set => _weight = value;
        }

        /// <summary>
        /// Wellness Day information
        /// </summary>
        public WellnessDayValue WellnessDay { get; private set; } = null;
        #endregion



        #region Ctors

        private FitnessDay(DateTime dayDate) : this(dayDate, null)
        {
        }

        private FitnessDay(DateTime dayDate, RatingValue rating)
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
        public static FitnessDay TrackDay(DateTime dayDate, RatingValue rating)
        {
            return new FitnessDay(dayDate, rating);
        }
        #endregion


        #region Business Methods

        /// <summary>
        /// Attach the Wellness Day information
        /// </summary>
        /// <param name="temperature">The temperature to be tracked</param>
        /// <param name="glycemia">The glycemia to be tracked</param>
        /// <param name="musIdList">The Ids of the MUSes diagnosed</param>
        public void TrackWellnessDay(TemperatureValue temperature = null, GlycemiaValue glycemia = null, ICollection<IdType> musIdList = null)
        {
            WellnessDay = WellnessDayValue.TrackWellness(temperature, glycemia, musIdList);
        }

        /// <summary>
        /// Attach the selected MUS diagnosis to the Day
        /// </summary>
        /// <param name="toAdd">The Id of the MUS to be diagnosed</param>
        public void DiagnoseMus(IdType toAdd)
        {
            WellnessDay = WellnessDay.DiagnoseMus(toAdd);
        }
        #endregion



    }
}
