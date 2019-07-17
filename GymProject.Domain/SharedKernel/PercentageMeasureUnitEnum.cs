using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class PercentageMeasureUnitEnum : Enumeration
    {


        #region Consts

        private const float RatioToPercentageMultiplierFactor = 100f;
        #endregion



        public static PercentageMeasureUnitEnum Percentage = new PercentageMeasureUnitEnum(1, "Percentage", "%", RatioToPercentage);
        public static PercentageMeasureUnitEnum Ratio = new PercentageMeasureUnitEnum(2, "Ratio", "", PercentageToRatio);



        /// <summary>
        /// Meas unit abbreviation - mg/dL Vs mmol/L
        /// </summary>
        public string Abbreviation { get; private set; }

        /// <summary>
        /// Formula to convert a value to the specific measure unit
        /// </summary>
        public Func<float, float> ApplyConversionFormula { get; private set; }


        #region Ctors

        public PercentageMeasureUnitEnum(int id, string name, string abbreviation, Func<float, float>  conversionFormula) : base(id, name)
        {
            Abbreviation = abbreviation;
            ApplyConversionFormula = conversionFormula;
        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<PercentageMeasureUnitEnum> List() =>
            new[] { Percentage, Ratio };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static PercentageMeasureUnitEnum FromName(string name)
        {
            return List().SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static PercentageMeasureUnitEnum From(int id)
        {
            return List().SingleOrDefault(s => s.Id == id);
        }


        private static float RatioToPercentage(float ratio) => ratio * RatioToPercentageMultiplierFactor;


        private static float PercentageToRatio(float percentage) =>  percentage / RatioToPercentageMultiplierFactor;
    }
}
