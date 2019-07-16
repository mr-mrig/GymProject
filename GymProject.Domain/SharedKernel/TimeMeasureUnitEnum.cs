using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class TimeMeasureUnitEnum : Enumeration
    {


        public static TimeMeasureUnitEnum Seconds = new TimeMeasureUnitEnum(1, "Seconds", "s", false);
        public static TimeMeasureUnitEnum Minutes = new TimeMeasureUnitEnum(2, "Minutes", "min", false);
        public static TimeMeasureUnitEnum Hours = new TimeMeasureUnitEnum(3, "Hours", "h", true);
        public static TimeMeasureUnitEnum Days = new TimeMeasureUnitEnum(4, "Days", "day", true);
        public static TimeMeasureUnitEnum Weeks = new TimeMeasureUnitEnum(5, "Weeks", "wks", true);
        public static TimeMeasureUnitEnum Months = new TimeMeasureUnitEnum(6, "Months", "mths", true);
        public static TimeMeasureUnitEnum Years = new TimeMeasureUnitEnum(7, "Years", "y", true);



        /// <summary>
        /// Meas unit abbreviation - mg/dL Vs mmol/L
        /// </summary>
        public string Abbreviation { get; private set; }

        /// <summary>
        /// Wether the unit allows decimal places or not.
        /// To be used when deciding how to display the measure
        /// </summary>
        public bool HasDecimals { get; private set; }

        /// <summary>
        /// Factor which to multiply
        /// </summary>
        public float LinearConversionMultiplierFactor { get; private set; }



        #region Ctors

        public TimeMeasureUnitEnum(int id, string name, string abbreviation, bool hasDecimals) : base(id, name)
        {
            Abbreviation = abbreviation;
            HasDecimals = hasDecimals;
        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<TimeMeasureUnitEnum> List() =>
            new[] { Seconds, Minutes, Hours, Days, Weeks, Months, Years };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static TimeMeasureUnitEnum FromName(string name)
        {
            return List().SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static TimeMeasureUnitEnum From(int id)
        {
            return List().SingleOrDefault(s => s.Id == id);
        }

    }
}
