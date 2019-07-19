using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class BodyFatValue : ValueObject
    {


        #region Consts

        public const byte DecimalPlaces = 2;
        #endregion



        /// <summary>
        /// The value of the measure
        /// </summary>
        public float Value { get; private set; }



        #region Ctors

        private BodyFatValue(float measValue)
        {
            Value = measValue;
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory for creating a new BF value [%]
        /// </summary>
        /// <param name="bodyfat">The value</param>
        /// <returns>The BodyFatValue instance</returns>
        public static BodyFatValue MeasureBodyFat(float bodyfat)
        {
            return new BodyFatValue(FormatBodyFat(bodyfat));
        }

        #endregion


        #region Business Methods

        /// <summary>
        /// Expresses the BF as a pure ratio number
        /// </summary>
        /// <returns>The ratio</returns>
        public PercentageValue AsRatio()
            => PercentageValue.MeasureRatio(Value / 100f, 2 * DecimalPlaces);       // Tune the decimal places not to loose precision

        #endregion



        #region Private Methods

        /// <summary>
        /// Formats the number as a percentage / ratio value
        /// </summary>
        /// <param name="value">The input value</param>
        /// <param name="decimalPlaces">Number of decimal places - default is 2</param>
        /// <param name="measUnit">The measure unit</param>
        /// <returns>The formatted value</returns>
        private static float FormatBodyFat(float value)
        {
            return (float)Math.Round(value, DecimalPlaces);
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        #endregion


    }
}
