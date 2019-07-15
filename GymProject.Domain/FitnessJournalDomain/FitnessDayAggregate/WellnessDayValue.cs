using System.Collections.Generic;
using GymProject.Domain.FitnessJournalDomain.MusAggregate;
using GymProject.Domain.Base;
using System.Linq;


namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class WellnessDayValue : ValueObject
    {


        #region Properties

        public TemperatureValue Temperature { get; private set; } = null;

        public GlycemiaValue Glycemia { get; private set; } = null;


        private ICollection<IdType> _musList = new List<IdType>();

        /// <summary>
        /// MUS list
        /// </summary>
        public IReadOnlyCollection<IdType> MusList
        {
            get => _musList.ToList().AsReadOnly();
        }
        #endregion



        #region Ctors

        private WellnessDayValue(TemperatureValue temperature = null, GlycemiaValue glycemia = null, ICollection<IdType> musList = null)
        {
            Temperature = temperature;
            Glycemia = glycemia;
            _musList = musList ?? new List<IdType>();
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory
        /// </summary>
        /// <param name="temperature">the temperature value</param>
        /// <param name="glycemia">The glycemia value</param>
        /// <returns>The new WellnessDay instance</returns>
        public static WellnessDayValue TrackWellness(TemperatureValue temperature = null, GlycemiaValue glycemia = null, ICollection<IdType> musList = null)
        {
            return new WellnessDayValue(temperature, glycemia, musList);
        }

        #endregion


        #region Business Methods

        /// <summary>
        /// Track the temperature value
        /// </summary>
        /// <param name="temperature">The temperature to be trakced</param>v
        /// <returns>A new WellnessDay ObjectValue with changed Temerature</returns>
        public WellnessDayValue ChangeTemperature(TemperatureValue temperature)
        {
            return WellnessDayValue.TrackWellness(temperature, Glycemia, _musList);
        }

        /// <summary>
        /// Track the glycemia value
        /// </summary>
        /// <param name="glycemia">The glycemia to be trakced</param>
        /// <returns>A new WellnessDay ObjectValue with changed Glycemia</returns>
        public WellnessDayValue ChangeGlycemia(GlycemiaValue glycemia)
        {
            return WellnessDayValue.TrackWellness(Temperature, glycemia, _musList);
        }

        /// <summary>
        /// Diagnose the MUSes in the specified list
        /// </summary>
        /// <param name="musList">The must List to be diagnosed</param>
        /// <returns>A new WellnessDay ObjectValue with the selected MUSes</returns>
        public WellnessDayValue SetMusList(ICollection<IdType> musList)
        {
            return WellnessDayValue.TrackWellness(Temperature, Glycemia, musList);
        }


        /// <summary>
        /// Add the selected MUS to the list of diagnosed MUSes
        /// </summary>
        /// <param name="musId">The must Id to be diagnosed</param>
        /// <returns>A new WellnessDay ObjectValue with the added MUS</returns>
        public WellnessDayValue DiagnoseMus(IdType musId)
        {
            _musList.Add(new IdType(1));
            return WellnessDayValue.TrackWellness(Temperature, Glycemia, _musList);
        }
        #endregion


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Temperature;
            yield return Glycemia;
            yield return MusList;
        }
    }
}
