using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GymProject.Domain.SharedKernel
{
    public class MicronutirentWeightValue : ValueObject
    {


        #region Consts

        public readonly IEnumerable<WeightUnitMeasureEnum> AllowedMeasUnits = new List<WeightUnitMeasureEnum>()
        {
            WeightUnitMeasureEnum.Grams,
            WeightUnitMeasureEnum.Ounces,
        };
        #endregion


        /// <summary>
        /// The value of the measure
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// The measurement unit
        /// </summary>
        public WeightUnitMeasureEnum Unit { get; private set; }



        #region Ctors

        private MicronutirentWeightValue(float measValue, WeightUnitMeasureEnum measUnit = null)
        {
            Value = measValue;
            Unit = measUnit ?? WeightUnitMeasureEnum.Grams;

            if (!AllowedMeasUnits.Contains(Unit))
                throw new ArgumentException($"{Unit.Abbreviation} is not allowed for {GetType().Name}", "measUnit");
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory for creating a new value [g]
        /// </summary>
        /// <param name="weight">The value</param>
        /// <returns>The MicronutirentWeightValue instance</returns>
        public static MicronutirentWeightValue MeasureGrams(float weight)
        {
            return new MicronutirentWeightValue(FormatWeight(weight, WeightUnitMeasureEnum.Grams));
        }

        /// <summary>
        /// Factory for creating a new value [oz]
        /// </summary>
        /// <param name="weight">The value</param>
        /// <returns>The MacronutirentWeightValue instance</returns>
        public static MicronutirentWeightValue MeasureOunces(float weight)
        {
            return new MicronutirentWeightValue(FormatWeight(weight, WeightUnitMeasureEnum.Ounces), WeightUnitMeasureEnum.Ounces);
        }

        /// <summary>
        /// Factory for creating a new value according to the measure unit
        /// </summary>
        /// <param name="glycemia">The value</param>
        /// <param name="measUnit">The measure unit - [g] default</param>
        /// /// <returns>The MicronutirentWeightValue instance</returns>
        public static MicronutirentWeightValue Measure(float glycemia, WeightUnitMeasureEnum measUnit = null)
        {
            WeightUnitMeasureEnum unit = measUnit ?? WeightUnitMeasureEnum.Grams;
            return new MicronutirentWeightValue(FormatWeight(glycemia, measUnit), unit);
        }
        #endregion


        #region Business Methods

        /// <summary>
        /// Creates a new MicronutirentWeightValue which is the conversion of the current one to the selected measure unit
        /// </summary>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The new MicronutirentWeightValue instance</returns>
        public MicronutirentWeightValue Convert(WeightUnitMeasureEnum toUnit)
        {
            if (Unit.Equals(toUnit))
                return this;

            return Measure(PerformConversion(Value, toUnit), toUnit);
        }
        #endregion



        #region Private Methods

        /// <summary>
        /// Converts the number to a macronutrient weight compliant value
        /// </summary>
        /// <param name="value">The input weight value</param>
        /// <param name="measUnit">The measure unit</param>
        /// <returns>the converted value</returns>
        private static float FormatWeight(float value, WeightUnitMeasureEnum measUnit)
        {
            // One decimal place, unless Ounces
            byte decimalPlaces = (byte)(measUnit == WeightUnitMeasureEnum.Ounces ? 2 : 1);

            // Weight rounded accorded to the meas unit
            return (float)Math.Round(value, decimalPlaces);
        }

        /// <summary>
        /// Apply the conversion formula to the target measure unit
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The converted value</returns>
        private static float PerformConversion(float value, WeightUnitMeasureEnum toUnit)
        {
            return FormatWeight(toUnit.ApplyConversionFormula(value), toUnit);
        }
        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
            yield return Unit;
        }
    }
}