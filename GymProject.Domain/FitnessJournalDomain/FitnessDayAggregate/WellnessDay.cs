using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using GymProject.Domain.FitnessJournalDomain.MusAggregate;


namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class WellnessDay
    {


        #region Properties

        private float _temperature;
        public float Temperature
        {
            get => _temperature;
            set => _temperature = value;
        }

        private float _glycemia;
        public float Glycemia
        {
            get => _glycemia;
            set => _glycemia = value;
        }

        public IReadOnlyCollection<Mus> MusList { get; private set; }
        #endregion

    }
}
