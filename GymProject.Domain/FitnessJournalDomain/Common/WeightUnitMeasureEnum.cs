using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.FitnessJournalDomain.Common
{
    public class WeightUnitMeasureEnum : Enumeration
    {

        public static WeightUnitMeasureEnum Kilograms = new WeightUnitMeasureEnum(1, "Kilograms", "Kg");
        public static WeightUnitMeasureEnum Pounds = new WeightUnitMeasureEnum(2, "Pounds", "lbs");

        /// <summary>
        /// Meas unit abbreviation - Kg / lbs
        /// </summary>
        public string Abbreviation { get; private set; }

        /// <summary>
        /// Formula to convert a value to the specific measure unit
        /// </summary>
        public Func<float, float> ApplyConversionFormula { get; private set; }


        #region Ctors

        public WeightUnitMeasureEnum(int id, string name, string abbreviation) : base(id, name)
        {
            Abbreviation = abbreviation;

            // Convert to Kg
            if (this.Equals(Kilograms))
                ApplyConversionFormula = PoundsToKilograms;

            // Convert to lbs
            else if (this.Equals(Pounds))
                ApplyConversionFormula = KilogramsToPounds;
        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<WeightUnitMeasureEnum> List() =>
            new[] { Kilograms, Pounds };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static WeightUnitMeasureEnum FromName(string name)
        {
            WeightUnitMeasureEnum unit = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));


            return unit;
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static WeightUnitMeasureEnum From(int id)
        {
            WeightUnitMeasureEnum unit = List().SingleOrDefault(s => s.Id == id);

            return unit;
        }


        #region Converters

        private static float PoundsToKilograms(float pounds)
        {
            return pounds / 2.20462f;
        }


        private static float KilogramsToPounds(float kilos)
        {
            return kilos * 2.20462f;
        }
        #endregion

    }
}
