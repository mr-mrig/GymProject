using System;
using System.Collections.Generic;
using GymProject.Domain.Base;



namespace GymProject.Domain.SharedKernel
{

    public class BodyWeightValue : ValueObject
    {


        #region Consts

        private const byte DecimalPlaces = 1;
        #endregion


        /// <summary>
        /// The weight value
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// The measurement unit
        /// </summary>
        public WeightUnitMeasureEnum Unit { get; private set; }



        #region Ctors

        private BodyWeightValue(float weight, WeightUnitMeasureEnum unit)
        {
            Value = weight;
            Unit = unit;
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method for Kilograms
        /// </summary>
        /// <param name="kilograms">The weight</param>
        /// <returns>A new Weight object</returns>
        public static BodyWeightValue MeasureKilograms(float kilograms)
        {
            return new BodyWeightValue(FormatWeight(kilograms), WeightUnitMeasureEnum.Kilograms);
        }


        /// <summary>
        /// Factory method for Pounds
        /// </summary>
        /// <param name="pounds">The weight</param>
        /// <returns>A new Weight object</returns>
        public static BodyWeightValue MeasurePounds(float pounds)
        {
            return new BodyWeightValue(FormatWeight(pounds), WeightUnitMeasureEnum.Pounds);
        }


        /// <summary>
        /// Factory method according to the unit specified
        /// </summary>
        /// <param name="weight">The temperature</param>
        /// <returns>The Weight object</returns>
        public static BodyWeightValue Measure(float weight, WeightUnitMeasureEnum unit = null)
        {
            WeightUnitMeasureEnum notNullUnit = unit ?? WeightUnitMeasureEnum.Kilograms;
            return new BodyWeightValue(FormatWeight(weight), notNullUnit);
        }
        #endregion


        #region Business Methods

        /// <summary>
        /// Creates a new WeightValue which is the conversion of the current one to the selected measure unit
        /// </summary>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The new WeightValue instance</returns>
        public BodyWeightValue Convert(WeightUnitMeasureEnum toUnit)
        {
            if (Unit.Equals(toUnit))
                return this;

            return Measure(PerformConversion(Value, toUnit), toUnit);
        }
        #endregion



        #region Private Methods

        /// <summary>
        /// Converts the number to a weight compliant value
        /// </summary>
        /// <param name="value">The input weight value</param>
        /// <returns>The converted value</returns>
        private static float FormatWeight(float value)
        {
            // weight rounded to the first decimal
            return (float)Math.Round(value, DecimalPlaces);
        }

        /// <summary>
        /// Apply the conversion formula to the target measure unit
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns></returns>
        private static float PerformConversion(float value, WeightUnitMeasureEnum toUnit)
        {
            return FormatWeight(toUnit.ApplyConversionFormula(value));
        }
        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
            yield return Unit;
        }
    }


}
