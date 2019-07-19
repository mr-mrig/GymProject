using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class CaliperSkinfoldValue : ValueObject
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

        private CaliperSkinfoldValue(float weight, LengthMeasureUnitEnum unit)
        {
            Value = weight;
            Unit = unit;
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method for [mm] measures
        /// </summary>
        /// <param name="millimeters">The measure [mm]</param>
        /// <returns>A new CaliperSkinfoldValue object</returns>
        public static CaliperSkinfoldValue MeasureMillimeters(float millimeters)
        {
            return new CaliperSkinfoldValue(FormatMeasure(millimeters, LengthMeasureUnitEnum.Millimeters), LengthMeasureUnitEnum.Millimeters);
        }


        /// <summary>
        /// Factory method for [inch] measures
        /// </summary>
        /// <param name="inches">The measure [inch]</param>
        /// <returns>A new CaliperSkinfoldValue object</returns>
        public static CaliperSkinfoldValue MeasureInches(float inches)
        {
            return new CaliperSkinfoldValue(FormatMeasure(inches, LengthMeasureUnitEnum.Inches), LengthMeasureUnitEnum.Inches);
        }


        /// <summary>
        /// Factory method according to the unit specified - [cm] default
        /// </summary>
        /// <param name="measure">The measure</param>
        /// <returns>The CaliperSkinfoldValue object</returns>
        public static CaliperSkinfoldValue Measure(float measure, LengthMeasureUnitEnum unit = null)
        {
            LengthMeasureUnitEnum notNullUnit = unit ?? LengthMeasureUnitEnum.Millimeters;
            return new CaliperSkinfoldValue(FormatMeasure(measure, notNullUnit), notNullUnit);
        }
        #endregion


        #region Business Methods

        /// <summary>
        /// Converts the selected value to the specified measure unit, without loosing precision.
        /// Only Metric Vs Imperial conversions supported
        /// To be used only in this assembly, when rounding might cause precision loss.
        /// </summary>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The new WeightValue instance</returns>
        internal float ConvertMetricVsImperialExact(LengthMeasureUnitEnum toUnit)
        {
            if (Unit.Equals(toUnit))
                return Value;

            return LengthMeasureUnitEnum.ApplyConversionFormula(Value, Unit, toUnit);
        }

        /// <summary>
        /// Creates a new WeightValue which is the conversion of the current one to the selected measure unit
        /// Only Metric Vs Imperial conversions supported
        /// </summary>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The new WeightValue instance</returns>
        public CaliperSkinfoldValue ConvertMetricVsImperial(LengthMeasureUnitEnum toUnit)
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
            // Allow more decimals
            int decimalPlaces = unit.MinimumDecimals;

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
