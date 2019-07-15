using System.Collections.Generic;
using GymProject.Domain.FitnessJournalDomain.MusAggregate;
using GymProject.Domain.Base;
using System.Linq;


namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class WellnessDay : Entity
    {


        #region Properties

        public TemperatureValue Temperature { get; private set; } = null;

        public GlycemiaValue Glycemia { get; private set; } = null;


        private ICollection<Mus> _musList;

        /// <summary>
        /// MUS list
        /// </summary>
        public IReadOnlyCollection<Mus> MusList
        {
            get => _musList.ToList().AsReadOnly();
        }
        #endregion



        #region Ctors

        private WellnessDay()
        {
            _musList = new List<Mus>();
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory
        /// </summary>
        /// <param name="temperature">the temperature value</param>
        /// <param name="glycemia">The glycemia value</param>
        /// <returns>The new WellnessDay instance</returns>
        public static WellnessDay TrackWellness(TemperatureValue temperature = null, GlycemiaValue glycemia = null)
        {
            return new WellnessDay()
            {
                Temperature = temperature,
                Glycemia = glycemia,
            };
        }

        #endregion


        #region Business Methods

        /// <summary>
        /// Attach the selected MUS to the day
        /// </summary>
        /// <param name="mus">The MUS to be added</param>
        internal void DiagnoseMus(Mus mus)
        {
            _musList.Add(mus);
        }

        /// <summary>
        /// Track the temperature value
        /// </summary>
        /// <param name="temperature">The temperature to be trakced</param>
        internal void TrackTemperature(TemperatureValue temperature)
        {
            Temperature = temperature;
        }

        /// <summary>
        /// Track the glycemia value
        /// </summary>
        /// <param name="glycemia">The glycemia to be trakced</param>
        internal void TrackGlycemia(GlycemiaValue glycemia)
        {
            Glycemia = glycemia;
        }
        #endregion

    }
}
