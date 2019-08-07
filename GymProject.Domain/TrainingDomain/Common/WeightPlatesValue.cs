using System;
using System.Collections.Generic;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Exceptions;

namespace GymProject.Domain.TrainingDomain.Common
{

    public class WeightPlatesValue : ValueObject
    {


        #region Consts

        public readonly List<WeightUnitMeasureEnum> SupportedMeasureUnits = new List<WeightUnitMeasureEnum>()
        {
            WeightUnitMeasureEnum.Kilograms,
            WeightUnitMeasureEnum.Pounds,
        };

        private const byte DecimalPlaces = 2;
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

        private WeightPlatesValue(float weight, WeightUnitMeasureEnum unit)
        {
            Value = weight;
            Unit = unit;

            TestBusinessRules();
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method for Kilograms
        /// </summary>
        /// <param name="kilograms">The weight</param>
        /// <returns>A new Weight object</returns>
        public static WeightPlatesValue MeasureKilograms(float kilograms)
        {
            return new WeightPlatesValue(FormatWeight(kilograms), WeightUnitMeasureEnum.Kilograms);
        }


        /// <summary>
        /// Factory method for Pounds
        /// </summary>
        /// <param name="pounds">The weight</param>
        /// <returns>A new Weight object</returns>
        public static WeightPlatesValue MeasurePounds(float pounds)
        {
            return new WeightPlatesValue(FormatWeight(pounds), WeightUnitMeasureEnum.Pounds);
        }


        /// <summary>
        /// Factory method according to the unit specified
        /// </summary>
        /// <param name="weight">The temperature</param>
        /// <returns>The Weight object</returns>
        public static WeightPlatesValue Measure(float weight, WeightUnitMeasureEnum unit = null)
        {
            WeightUnitMeasureEnum notNullUnit = unit ?? WeightUnitMeasureEnum.Kilograms;
            return new WeightPlatesValue(FormatWeight(weight), notNullUnit);
        }
        #endregion


        #region Business Methods

        /// <summary>
        /// Creates a new WeightValue which is the conversion of the current one to the selected measure unit
        /// </summary>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The new WeightValue instance</returns>
        public WeightPlatesValue Convert(WeightUnitMeasureEnum toUnit)
        {
            if (Unit.Equals(toUnit))
                return this;

            return Measure(PerformConversion(Value, toUnit), toUnit);
        }


        /// <summary>
        /// Converts the selected value to the specified measure unit, without loosing precision.
        /// To be used only in this assembly, when rounding might cause precision loss.
        /// </summary>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The exact result of the conversion, no rounding done</returns>
        internal float ConvertExact(WeightUnitMeasureEnum toUnit)
        {
            if (Unit.Equals(toUnit))
                return Value;

            return PerformConversion(Value, toUnit);
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



        #region Business Rules Validation

        /// <summary>
        /// The WeightPlates must have a supported meas unit.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool MeasureUnitIsSupported() => SupportedMeasureUnits.Contains(Unit);


        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
            if (!MeasureUnitIsSupported())
                throw new UnsupportedMeasureException($"The WeightPlates must have a supported meas unit.");
        }

        #endregion


        #region Operators

        public static WeightPlatesValue operator +(WeightPlatesValue left, WeightPlatesValue right)
        {
            if (left.Unit != right.Unit)
                throw new UnsupportedMeasureException($"When summing the operands must have the same measure unit");

            return Measure(left.Value + right.Value, left.Unit);
        }

        public static WeightPlatesValue operator -(WeightPlatesValue left, WeightPlatesValue right)
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
