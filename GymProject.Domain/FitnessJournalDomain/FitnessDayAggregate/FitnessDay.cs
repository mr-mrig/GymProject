using GymProject.Domain.Base;
using System;



namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class FitnessDay : Entity, IAggregateRoot
    {


        #region Properties
        public DateTime DayDate { get; private set; }
        public float? Rating { get; private set; }

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

        private WellnessDay _wellnessDay = null;
        public WellnessDay WellnessDay
        {
            get => _wellnessDay;
            set => _wellnessDay = value;
        }
        #endregion



        #region Ctors
        public FitnessDay()
        {
            
            

        }
        #endregion



        #region Public Methods

        #endregion

    }
}
