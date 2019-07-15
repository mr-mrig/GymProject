using GymProject.Domain.Base;
using GymProject.Domain.FitnessJournalDomain.MusAggregate;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class FitnessDay : Entity, IAggregateRoot
    {


        #region Properties
        public DateTime DayDate { get; private set; }
        public RatingValue Rating { get; private set; } = null;

        // FK
        public long PostId { get; private set; }

        private ActivityDay _activityDay = null;
        public ActivityDay ActivityDay
        {
            get => _activityDay;
            set => _activityDay = value;
        }


        private DietDay _dietDay = null;
        public DietDay DietDay
        {
            get => _dietDay;
            set => _dietDay = value;
        }


        private Weight _weight = null;
        public Weight Weight
        {
            get => _weight;
            set => _weight = value;
        }

        /// <summary>
        /// Wellness Day information
        /// </summary>
        private WellnessDay _wellnessDay = null;
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

        public void TrackWellnessDay(TemperatureValue temperature = null, GlycemiaValue glycemia = null, ICollection<Mus> musList = null)
        {
            _wellnessDay = WellnessDay.TrackWellness();
            _wellnessDay.TrackTemperature(temperature);
            _wellnessDay.TrackGlycemia(glycemia);
        }

        /// <summary>
        /// Attach the selected MUS diagnosis to the Day
        /// </summary>
        /// <param name="toAdd">The MUS to be diagnosed</param>
        public void DiagnoseMus(Mus toAdd)
        {
            if (_wellnessDay is null)
                _wellnessDay = WellnessDay.TrackWellness();

            _wellnessDay.DiagnoseMus(toAdd);
        }
        #endregion


        #region Getters

        /// <summary>
        /// Get the Wellness day
        /// </summary>
        /// <returns>The WellnessDay instance</returns>
        public WellnessDay GetWellnessDay()
        {
            return _wellnessDay;
        }
        #endregion

    }
}
