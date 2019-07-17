using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class CaloriesMeasureUnitEnum : Enumeration
    {

        public static CaloriesMeasureUnitEnum Kilocals = new CaloriesMeasureUnitEnum(1, "Kilocal", "kcal", JoulesToKilocals);
        public static CaloriesMeasureUnitEnum KiloJoules = new CaloriesMeasureUnitEnum(2, "KiloJoules", "kJ", KilocalsToJoules);



        /// <summary>
        /// Meas unit abbreviation - Kg / lbs
        /// </summary>
        public string Abbreviation { get; private set; }

        /// <summary>
        /// Formula to convert a value to the specific measure unit
        /// </summary>
        public Func<float, float> ApplyConversionFormula { get; private set; }



        #region Ctors

        public CaloriesMeasureUnitEnum(int id, string name, string abbreviation, Func<float, float> conversionFormula) : base(id, name)
        {
            Abbreviation = abbreviation;
            ApplyConversionFormula = conversionFormula;
        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<CaloriesMeasureUnitEnum> List() =>
            new[] { Kilocals, KiloJoules };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static CaloriesMeasureUnitEnum FromName(string name)
        {
            CaloriesMeasureUnitEnum unit = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));


            return unit;
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static CaloriesMeasureUnitEnum From(int id)
        {
            CaloriesMeasureUnitEnum unit = List().SingleOrDefault(s => s.Id == id);

            return unit;
        }


        #region Converters

        private static float JoulesToKilocals(float kj)
        {
            return kj / 4.184f;
        }


        private static float KilocalsToJoules(float kcal)
        {
            return kcal * 4.184f;
        }
        #endregion

    }
}
