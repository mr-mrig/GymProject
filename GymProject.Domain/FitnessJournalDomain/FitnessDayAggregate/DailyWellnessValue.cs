using System.Collections.Generic;
using GymProject.Domain.FitnessJournalDomain.MusAggregate;
using GymProject.Domain.Base;
using System.Linq;
using GymProject.Domain.FitnessJournalDomain.Exceptions;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class DailyWellnessValue : ValueObject
    {


        #region Properties

        public TemperatureValue Temperature { get; private set; } = null;

        public GlycemiaValue Glycemia { get; private set; } = null;


        private ICollection<MusReference> _musList = new List<MusReference>();

        /// <summary>
        /// MUS list
        /// </summary>
        public IReadOnlyCollection<MusReference> MusList
        {
            get => _musList?.ToList().AsReadOnly();
        }
        #endregion



        #region Ctors

        private DailyWellnessValue(TemperatureValue temperature = null, GlycemiaValue glycemia = null, ICollection<MusReference> musList = null)
        {
            Temperature = temperature;
            Glycemia = glycemia;
            _musList = musList ?? new List<MusReference>();
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory
        /// </summary>
        /// <param name="temperature">the temperature value</param>
        /// <param name="glycemia">The glycemia value</param>
        /// <returns>The new WellnessDay instance</returns>
        public static DailyWellnessValue TrackWellness(TemperatureValue temperature = null, GlycemiaValue glycemia = null, ICollection<MusReference> musList = null)
        {
            if (temperature is null && glycemia is null && (musList is null || musList.Count == 0))
                throw new FitnessJournalDomainGenericException($"Cannot create a DailyWellnessValue with all NULL fields");

            return new DailyWellnessValue(temperature, glycemia, musList);
        }

        #endregion


        #region Business Methods

        /// <summary>
        /// Track the temperature value
        /// </summary>
        /// <param name="temperature">The temperature to be trakced</param>v
        /// <returns>A new WellnessDay ObjectValue with changed Temerature</returns>
        public DailyWellnessValue ChangeTemperature(TemperatureValue temperature)
        {
            return DailyWellnessValue.TrackWellness(temperature, Glycemia, _musList);
        }

        /// <summary>
        /// Track the glycemia value
        /// </summary>
        /// <param name="glycemia">The glycemia to be trakced</param>
        /// <returns>A new WellnessDay ObjectValue with changed Glycemia</returns>
        public DailyWellnessValue ChangeGlycemia(GlycemiaValue glycemia)
        {
            return DailyWellnessValue.TrackWellness(Temperature, glycemia, _musList);
        }

        /// <summary>
        /// Diagnose the MUSes in the specified list
        /// </summary>
        /// <param name="musList">The MUS List to be diagnosed</param>
        /// <returns>A new WellnessDay ObjectValue with the selected MUSes</returns>
        public DailyWellnessValue SetMusList(ICollection<MusReference> musList)
        {
            return DailyWellnessValue.TrackWellness(Temperature, Glycemia, musList);
        }


        /// <summary>
        /// Add the selected MUS to the list of diagnosed MUSes
        /// </summary>
        /// <param name="mus">The MUS to be diagnosed</param>
        /// <returns>A new WellnessDay ObjectValue with the added MUS</returns>
        public DailyWellnessValue DiagnoseMus(MusReference mus)
        {
            _musList.Add(mus);
            return DailyWellnessValue.TrackWellness(Temperature, Glycemia, _musList);
        }


        /// <summary>
        /// Remove the selected MUS to the list of diagnosed MUSes
        /// </summary>
        /// <param name="mus">The MUS to be diagnosed</param>
        /// <returns>A new WellnessDay ObjectValue with the updated MUS list</returns>
        public DailyWellnessValue RemoveMus(MusReference mus)
        {
            _musList.Remove(mus);
            return DailyWellnessValue.TrackWellness(Temperature, Glycemia, _musList);
        }


        /// <summary>
        /// Remove the selected MUS to the list of diagnosed MUSes
        /// </summary>
        /// <param name="musId">The ID of the MUS to be diagnosed</param>
        /// <returns>A new WellnessDay ObjectValue with the updated MUS list</returns>
        public DailyWellnessValue RemoveMus(IdType musId)
        {
            MusReference toBeRemoved = _musList.Where(x => x.Id == musId).FirstOrDefault();

            _musList.Remove(toBeRemoved);
            return DailyWellnessValue.TrackWellness(Temperature, Glycemia, _musList);
        }
        #endregion



        /// <summary>
        /// Checks whether all the properties are null
        /// </summary>
        /// <returns>True if no there are no non-null properties</returns>
        public bool CheckNullState()
        {
            return GetAtomicValues().All(x => x is null);
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Temperature;
            yield return Glycemia;
            yield return MusList;
        }
    }
}
