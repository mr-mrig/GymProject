using System;
using System.Collections.Generic;
using System.Linq;
using GymProject.Domain.Base;

namespace GymProject.Domain.SharedKernel
{
    public class MacronutirentWeightValue : ValueObject
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

        private MacronutirentWeightValue(float measValue, WeightUnitMeasureEnum measUnit = null)
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
        /// <param name="grams">The value</param>
        /// <returns>The MacronutirentWeightValue instance</returns>
        public static MacronutirentWeightValue MeasureGrams(float grams)
        {
            return new MacronutirentWeightValue(FormatWeight(grams, WeightUnitMeasureEnum.Grams));
        }

        /// <summary>
        /// Factory for creating a new value [oz]
        /// </summary>
        /// <param name="ounces">The value</param>
        /// <returns>The MacronutirentWeightValue instance</returns>
        public static MacronutirentWeightValue MeasureOunces(float ounces)
        {
            return new MacronutirentWeightValue(FormatWeight(ounces, WeightUnitMeasureEnum.Ounces), WeightUnitMeasureEnum.Ounces);
        }

        /// <summary>
        /// Factory for creating a new value according to the measure unit
        /// </summary>
        /// <param name="weight">The value</param>
        /// <param name="measUnit">The measure unit - [g] default</param>
        /// /// <returns>The MacronutirentWeightValue instance</returns>
        public static MacronutirentWeightValue Measure(float weight, WeightUnitMeasureEnum measUnit = null)
        {
            return new MacronutirentWeightValue(FormatWeight(weight, measUnit), measUnit);
        }
        #endregion


        #region Business Methods

        /// <summary>
        /// Creates a new MacronutirentWeightValue which is the conversion of the current one to the selected measure unit
        /// </summary>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The new MacronutirentWeightValue instance</returns>
        public MacronutirentWeightValue Convert(WeightUnitMeasureEnum toUnit)
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
            // Don't keep decimals, unless Ounces
            byte decimalPlaces = (byte)(measUnit == WeightUnitMeasureEnum.Ounces ? 1 : 0);

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


        #region Operators

        public static MacronutirentWeightValue operator +(MacronutirentWeightValue left, MacronutirentWeightValue right)
        {
            if (left.Unit != right.Unit)
                throw new UnsupportedMeasureException($"When summing the operands must have the same measure unit");

            return Measure(left.Value + right.Value, left.Unit);
        }

        public static MacronutirentWeightValue operator -(MacronutirentWeightValue left, MacronutirentWeightValue right)
        {
            if (left.Unit != right.Unit)
                throw new UnsupportedMeasureException($"When summing the operands must have the same measure unit");

            return Measure(left.Value - right.Value, left.Unit);
        }
        #endregion


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
            yield return Unit;
        }
    }
}