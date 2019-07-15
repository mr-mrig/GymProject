using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GymProject.Domain.SharedKernel
{
    public class TemperatureMeasureUnitEnum : Enumeration
    {


        public static TemperatureMeasureUnitEnum Celsius = new TemperatureMeasureUnitEnum(1, "Celsius", "°C");
        public static TemperatureMeasureUnitEnum Fahrenheit = new TemperatureMeasureUnitEnum(2, "Fahrenheit", "°F");

        /// <summary>
        /// Meas unit abbreviation - °C / °F
        /// </summary>
        public string Abbreviation { get; private set; }

        /// <summary>
        /// Formula to convert a value to the specific measure unit
        /// </summary>
        public Func<float, float> ApplyConversionFormula { get; private set; }


        #region Ctors

        public TemperatureMeasureUnitEnum(int id, string name, string abbreviation) : base(id, name)
        {
            Abbreviation = abbreviation;

            // Convert to °C
            if (this.Equals(Celsius))
                ApplyConversionFormula = FahrenheitToCelsius;

            // Convert to °F
            else if (this.Equals(Fahrenheit))
                ApplyConversionFormula = CelsiusToFahrenheit;
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
            TemperatureMeasureUnitEnum picType = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));


            return picType;
        }


        /// <summary>
        /// Creates a PictureType object with the selected id
        /// </summary>
        /// <param name="name">Enumeration id</param>
        /// <returns>The PictureType object instance</returns>
        public static TemperatureMeasureUnitEnum From(int id)
        {
            TemperatureMeasureUnitEnum picType = List().SingleOrDefault(s => s.Id == id);

            return picType;
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
