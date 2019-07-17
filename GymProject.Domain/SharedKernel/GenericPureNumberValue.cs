using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class GenericPureNumberValue : ValueObject
    {


        #region Consts

        public const byte DefaultDecimalPlaces = 2;
        #endregion


        /// <summary>
        /// The weight value
        /// </summary>
        public float Value { get; private set; }




        #region Ctors

        private GenericPureNumberValue(float weight)
        {
            Value = weight;
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method according - 2 decimal places default
        /// </summary>
        /// <param name="measure">The measure</param>
        /// <param name="decimalPlaces">The number of decimal places - default is 2</param>
        /// <returns>The GenericPureNumberValue object</returns>
        public static GenericPureNumberValue Measure(float measure, byte decimalPlaces = DefaultDecimalPlaces)
        {
            return new GenericPureNumberValue(FormatNumber(measure, decimalPlaces));
        }
        #endregion


        #region Business Methods
        #endregion



        #region Private Methods

        /// <summary>
        /// Formats the number according to the decimal places
        /// </summary>
        /// <param name="value">The input value</param>
        /// <param name="decimalPlaces">The number of decimal places</param>
        /// <returns>The formatted value</returns>
        private static float FormatNumber(float value, byte decimalPlaces)
        {
            // weight rounded to the first decimal
            return (float)Math.Round(value, decimalPlaces);
        }

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
