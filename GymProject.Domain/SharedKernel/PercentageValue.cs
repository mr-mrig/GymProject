using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{

    public class PercentageValue : ValueObject
    {


        #region Consts

        public const byte DefaultDecimalPlaces = 1;
        #endregion


        /// <summary>
        /// The value of the measure
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// The measurement unit
        /// </summary>
        public PercentageMeasureUnitEnum Unit { get; private set; }

        /// <summary>
        /// The decimal places
        /// </summary>
        public byte DecimalPlaces { get; private set; }


        #region Ctors

        private PercentageValue(float measValue, byte decimalPlaces = DefaultDecimalPlaces, PercentageMeasureUnitEnum measUnit = null)
        {
            Value = measValue;
            Unit = measUnit ?? PercentageMeasureUnitEnum.Percentage;
            DecimalPlaces = decimalPlaces;
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory for creating a new value [%]
        /// </summary>
        /// <param name="percentage">The value</param>
        /// <param name="decimalPlaces">Number of decimal places - default is 2</param>
        /// <returns>The PercentageValue instance</returns>
        public static PercentageValue MeasurePercentage(float percentage, byte decimalPlaces = DefaultDecimalPlaces)
        {
            return new PercentageValue(FormatStatic(percentage, decimalPlaces));
        }

        /// <summary>
        /// Factory for creating a new value as a pure ratio
        /// </summary>
        /// <param name="ratio">The value</param>
        /// <param name="decimalPlaces">Number of decimal places - default is 2</param>
        /// <returns>The PercentageValue instance</returns>
        public static PercentageValue MeasureRatio(float ratio, byte decimalPlaces = DefaultDecimalPlaces)
        {
            return new PercentageValue(FormatStatic(ratio, decimalPlaces));
        }

        /// <summary>
        /// Factory for creating a new value according to the measure unit
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="decimalPlaces">Number of decimal places - default is 2</param>
        /// <param name="measUnit">The measure unit - [%] default</param>
        /// /// <returns>The PercentageValue instance</returns>
        public static PercentageValue Measure(float value, byte decimalPlaces = DefaultDecimalPlaces, PercentageMeasureUnitEnum measUnit = null)
        {
            return new PercentageValue(FormatStatic(value, decimalPlaces), decimalPlaces, measUnit);
        }
        #endregion


        #region Business Methods

        /// <summary>
        /// Creates a new PercentageValue which is the conversion of the current one to the selected measure unit
        /// </summary>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The new PercentageValue instance</returns>
        public PercentageValue Convert(PercentageMeasureUnitEnum toUnit)
        {
            if (Unit.Equals(toUnit))
                return this;

            return Measure(Format(toUnit.ApplyConversionFormula(Value)), DecimalPlaces, toUnit);
        }
        #endregion



        #region Private Methods

        /// <summary>
        /// Formats the number as a percentage / ratio value
        /// </summary>
        /// <param name="value">The input value</param>
        /// <param name="decimalPlaces">Number of decimal places - default is 2</param>
        /// <param name="measUnit">The measure unit</param>
        /// <returns>The formatted value</returns>
        private static float FormatStatic(float value, byte decimalPlaces)
        {
            return (float)Math.Round(value, decimalPlaces);
        }

        /// <summary>
        /// Formats the number as a percentage / ratio value
        /// </summary>
        /// <param name="value">The input value</param>
        /// <param name="decimalPlaces">Number of decimal places - default is 2</param>
        /// <param name="measUnit">The measure unit</param>
        /// <returns>The formatted value</returns>
        private float Format(float value)
        {
            // Weight rounded accorded to the meas unit
            return (float)Math.Round(value, DecimalPlaces);
        }
        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
            yield return Unit;
            yield return DecimalPlaces;
        }
    }
}
