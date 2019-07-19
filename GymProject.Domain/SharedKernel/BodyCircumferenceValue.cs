using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class BodyMeasureValue : ValueObject
    {


        #region Consts

        public readonly List<LengthMeasureUnitEnum> SupportedMeasureUnits = new List<LengthMeasureUnitEnum>()
        {
            LengthMeasureUnitEnum.Centimeters,
            LengthMeasureUnitEnum.Inches,
        };
        #endregion


        /// <summary>
        /// The weight value
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// The measurement unit
        /// </summary>
        public LengthMeasureUnitEnum Unit { get; private set; }



        #region Ctors

        private BodyMeasureValue(float weight, LengthMeasureUnitEnum unit)
        {
            if (!SupportedMeasureUnits.Contains(unit))
                throw new UnsupportedMeasureException($"{GetType().Name} - Unsupported measure: {unit.Name}");

            Value = weight;
            Unit = unit;
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method for [cm] measures
        /// </summary>
        /// <param name="centimeters">The measure [cm]</param>
        /// <returns>A new BodyCircumferenceValue object</returns>
        public static BodyMeasureValue MeasureCentimeters(float centimeters)
        {
            return new BodyMeasureValue(FormatMeasure(centimeters, LengthMeasureUnitEnum.Centimeters), LengthMeasureUnitEnum.Centimeters);
        }


        /// <summary>
        /// Factory method for [inch] measures
        /// </summary>
        /// <param name="inches">The measure [inch]</param>
        /// <returns>A new BodyCircumferenceValue object</returns>
        public static BodyMeasureValue MeasureInches(float inches)
        {
            return new BodyMeasureValue(FormatMeasure(inches, LengthMeasureUnitEnum.Inches), LengthMeasureUnitEnum.Inches);
        }


        /// <summary>
        /// Factory method according to the unit specified - [cm] default
        /// </summary>
        /// <param name="measure">The measure</param>
        /// <returns>The BodyCircumferenceValue object</returns>
        public static BodyMeasureValue Measure(float measure, LengthMeasureUnitEnum unit = null)
        {
            LengthMeasureUnitEnum notNullUnit = unit ?? LengthMeasureUnitEnum.Centimeters;
            return new BodyMeasureValue(FormatMeasure(measure, notNullUnit), notNullUnit);
        }
        #endregion


        #region Business Methods

        /// <summary>
        /// Creates a new BodyCircumferenceValue which is the conversion of the current one to the selected measure unit
        /// </summary>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The new BodyCircumferenceValue instance</returns>
        public BodyMeasureValue Convert(LengthMeasureUnitEnum toUnit)
        {
            if (Unit.Equals(toUnit))
                return this;

            return Measure(LengthMeasureUnitEnum.ApplyConversionFormula(Value, Unit, toUnit), toUnit);
        }


        /// <summary>
        /// Converts the selected value to the specified measure unit, without loosing precision.
        /// To be used only in this assembly, when rounding might cause precision loss.
        /// </summary>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The exact result of the conversion, no rounding done</returns>
        internal float ConvertExact(LengthMeasureUnitEnum toUnit)
        {
            if (Unit.Equals(toUnit))
                return Value;

            return LengthMeasureUnitEnum.ApplyConversionFormula(Value, Unit, toUnit);
        }
        #endregion



        #region Private Methods

        /// <summary>
        /// Converts the number to a body measure compliant value
        /// </summary>
        /// <param name="value">The input weight value</param>
        /// <param name="unit">The measure unit</param>
        /// <returns>The formatted value</returns>
        private static float FormatMeasure(float value, LengthMeasureUnitEnum unit)
        {
            // No more than one decimal place
            int decimalPlaces = Math.Min((int)unit.MinimumDecimals, 1);

            // weight rounded to the first decimal
            return (float)Math.Round(value, decimalPlaces);
        }

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
            yield return Unit;
        }
    }
}
