using System;
using System.Collections.Generic;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class TemperatureValue : ValueObject
    {

        #region Consts

        /// <summary>
        ///  Number of decimal places allowed for this type
        /// </summary>
        private const byte DecimalPlaces = 1;
        #endregion


        /// <summary>
        /// Temperature value
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// Measure of unit
        /// </summary>
        public TemperatureMeasureUnitEnum Unit { get; private set; }


        #region Ctors

        private TemperatureValue(float temperature, TemperatureMeasureUnitEnum unit)
        {
            Value = temperature;
            Unit = unit;
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method for Celsius temperatures
        /// </summary>
        /// <param name="celsius">The temperature</param>
        /// <returns>The TemperatureValue object</returns>
        public static TemperatureValue MeasureCelsius(float celsius)
        {
            return new TemperatureValue(FormatTemperature(celsius), TemperatureMeasureUnitEnum.Celsius);
        }


        /// <summary>
        /// Factory method for Fahrenheit temperatures
        /// </summary>
        /// <param name="fahrenheit">The temperature</param>
        /// <returns>The TemperatureValue object</returns>
        public static TemperatureValue MeasureFahrenheit(float fahrenheit)
        {
            return new TemperatureValue(FormatTemperature(fahrenheit), TemperatureMeasureUnitEnum.Fahrenheit);
        }


        /// <summary>
        /// Factory method according to the unit specified
        /// </summary>
        /// <param name="temperature">The temperature</param>
        /// <returns>The TemperatureValue object</returns>
        public static TemperatureValue Measure(float temperature, TemperatureMeasureUnitEnum unit = null)
        {
            TemperatureMeasureUnitEnum notNullUnit = unit ?? TemperatureMeasureUnitEnum.Celsius;
            return new TemperatureValue(FormatTemperature(temperature), notNullUnit);
        }
        #endregion



        #region Business Methods

        /// <summary>
        /// Creates a new TemperatureValue which is the conversion of the current one to the selected measure unit
        /// </summary>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The new TemperatureValue instance</returns>
        public TemperatureValue Convert(TemperatureMeasureUnitEnum toUnit)
        {
            if (Unit.Equals(toUnit))
                return this;

            return Measure(PerformConversion(Value, toUnit), toUnit);
        }
        #endregion



        #region Private Methods

        /// <summary>
        /// Converts the number to a temperature compliant value
        /// </summary>
        /// <param name="value">The input temperature value</param>
        /// <returns>the converted value</returns>
        private static float FormatTemperature(float value)
        {
            // temperatures rounded to the first decimal
            return (float)Math.Round(value, DecimalPlaces);
        }

        /// <summary>
        /// Apply the conversion formula to the target measure unit
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns></returns>
        private static float PerformConversion(float value, TemperatureMeasureUnitEnum toUnit)
        {
            return FormatTemperature(toUnit.ApplyConversionFormula(value));
        }
        #endregion


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
            yield return Unit;
        }
    }
}