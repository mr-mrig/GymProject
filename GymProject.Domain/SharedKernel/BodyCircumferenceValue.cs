using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    class BodyCircumferenceValue : ValueObject
    {


        #region Consts
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

        private BodyCircumferenceValue(float weight, LengthMeasureUnitEnum unit)
        {
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
        public static BodyCircumferenceValue MeasureCentimeters(float centimeters)
        {
            return new BodyCircumferenceValue(FormatMeasure(centimeters, LengthMeasureUnitEnum.Centimeters), LengthMeasureUnitEnum.Centimeters);
        }


        /// <summary>
        /// Factory method for [inch] measures
        /// </summary>
        /// <param name="inches">The measure [inch]</param>
        /// <returns>A new BodyCircumferenceValue object</returns>
        public static BodyCircumferenceValue MeasureInches(float inches)
        {
            return new BodyCircumferenceValue(FormatMeasure(inches, LengthMeasureUnitEnum.Inches), LengthMeasureUnitEnum.Inches);
        }


        /// <summary>
        /// Factory method according to the unit specified - [cm] default
        /// </summary>
        /// <param name="measure">The measure</param>
        /// <returns>The BodyCircumferenceValue object</returns>
        public static BodyCircumferenceValue Measure(float measure, LengthMeasureUnitEnum unit = null)
        {
            LengthMeasureUnitEnum notNullUnit = unit ?? LengthMeasureUnitEnum.Centimeters;
            return new BodyCircumferenceValue(FormatMeasure(measure, notNullUnit), notNullUnit);
        }
        #endregion


        #region Business Methods

        /// <summary>
        /// Creates a new WeightValue which is the conversion of the current one to the selected measure unit
        /// </summary>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The new WeightValue instance</returns>
        public BodyCircumferenceValue Convert(LengthMeasureUnitEnum toUnit)
        {
            if (Unit.Equals(toUnit))
                return this;

            return Measure(LengthMeasureUnitEnum.ApplyConversionFormula(Value, Unit, toUnit), toUnit);
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
