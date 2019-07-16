using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class LengthMeasureUnitEnum : Enumeration
    {



        #region Consts

        /// <summary>
        /// Factor for converting between meas units
        /// </summary>
        public const float LinearConversionFactor = 10f;
        #endregion


        public static LengthMeasureUnitEnum Millimeters = new LengthMeasureUnitEnum(1, "Millimeters", "mm", false);
        public static LengthMeasureUnitEnum Centimeters = new LengthMeasureUnitEnum(2, "Centimeters", "cm", true);
        public static LengthMeasureUnitEnum Meters = new LengthMeasureUnitEnum(4, "Meters", "m", true);
        public static LengthMeasureUnitEnum Kilometers = new LengthMeasureUnitEnum(7, "Kilometers", "Km", true);




        /// <summary>
        /// Meas unit abbreviation - mg/dL Vs mmol/L
        /// </summary>
        public string Abbreviation { get; private set; }

        /// <summary>
        /// Wether the unit allows decimal places or not.
        /// To be used when deciding how to display the measure
        /// </summary>
        public bool HasDecimals { get; private set; }



        #region Ctors

        public LengthMeasureUnitEnum(int id, string name, string abbreviation, bool hasDecimals) : base(id, name)
        {
            Abbreviation = abbreviation;
            HasDecimals = hasDecimals;
        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<LengthMeasureUnitEnum> List() =>
            new[] { Millimeters, Centimeters, Meters, Kilometers };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static LengthMeasureUnitEnum FromName(string name)
        {
            return List().SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static LengthMeasureUnitEnum From(int id)
        {
            return List().SingleOrDefault(s => s.Id == id);
        }



        /// <summary>
        /// Convert the value from one measure unit to the other, assuming a linear conversion
        /// </summary>
        /// <param name="value">The value to be converteed</param>
        /// <param name="fromUnit">The original measure unit</param>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The converted value</returns>
        public static float ConverToUnit(float value, LengthMeasureUnitEnum fromUnit, LengthMeasureUnitEnum toUnit)
        {
            int idDifference = fromUnit.Id - toUnit.Id;

            // No conversion needed
            if (idDifference == 0)
                return value;

            // Convert backward
            else if (idDifference > 0)
                return value * LinearConversionFactor * idDifference;

            // Convert forward
            else
                return value / LinearConversionFactor / idDifference;

        }
    }
}
