using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GymProject.Domain.SharedKernel
{
    public class TemperatureMeasureUnitEnum : Enumeration
    {


        public static TemperatureMeasureUnitEnum Celsius = new TemperatureMeasureUnitEnum(1, "Celsius", "°C", FahrenheitToCelsius);
        public static TemperatureMeasureUnitEnum Fahrenheit = new TemperatureMeasureUnitEnum(2, "Fahrenheit", "°F", CelsiusToFahrenheit);

        /// <summary>
        /// Meas unit abbreviation - °C / °F
        /// </summary>
        public string Abbreviation { get; private set; }

        /// <summary>
        /// Formula to convert a value to the specific measure unit
        /// </summary>
        public Func<float, float> ApplyConversionFormula { get; private set; }


        #region Ctors



        public TemperatureMeasureUnitEnum(int id, string name, string abbreviation, Func<float, float> conversionFormula) : base(id, name)
        {
            Abbreviation = abbreviation;
            ApplyConversionFormula = conversionFormula;
        }
        #endregion



        /// <summary>
        /// Get the enumeration list
        /// </summary>
        /// <returns>The list storing the enumeration</returns>
        public static IEnumerable<TemperatureMeasureUnitEnum> List() =>
            new[] { Celsius, Fahrenheit };


        /// <summary>
        /// Creates a PictureType object with the selected name
        /// </summary>
        /// <param name="name">Enumeration name</param>
        /// <returns>The PictureType object instance</returns>
        public static TemperatureMeasureUnitEnum FromName(string name)
        {
            TemperatureMeasureUnitEnum unit = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));


            return unit;
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static TemperatureMeasureUnitEnum From(int id)
        {
            TemperatureMeasureUnitEnum unit = List().SingleOrDefault(s => s.Id == id);

            return unit;
        }


        private static float CelsiusToFahrenheit(float celsius)
        {
            return (celsius * 9 / 5) + 32;
        }


        private static float FahrenheitToCelsius(float fahrenheit)
        {
            return (fahrenheit - 32) * 5 / 9;
        }
    }
}
