using System;
using System.Collections.Generic;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;


namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class GlycemiaValue : ValueObject
    {





        /// <summary>
        /// Glycemia value
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// Measure of unit
        /// </summary>
        public GlycemiaMeasureUnitEnum Unit { get; private set; }




        #region Ctors

        private GlycemiaValue(float glycemia)
        {
            Value = glycemia;
            Unit = GlycemiaMeasureUnitEnum.Milligrams;
        }

        private GlycemiaValue(float glycemia, GlycemiaMeasureUnitEnum measUnit)
        {
            Value = glycemia;
            Unit = measUnit;
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory for creating a new mg/dL value
        /// </summary>
        /// <param name="glycemia">The value</param>
        /// <returns>The GlycemiaValue instance</returns>
        public static GlycemiaValue MeasureMg(float glycemia)
        {
            return new GlycemiaValue(FormatGlycemia(glycemia, GlycemiaMeasureUnitEnum.Milligrams));
        }

        ///// <summary>
        ///// Factory for creating a new mmol/L value
        ///// </summary>
        ///// <param name="glycemia"></param>
        ///// <returns>The GlycemiaValue instance</returns>
        //public static GlycemiaValue MeasureMmol(float glycemia, string measUnit)
        //{
        //    return new GlycemiaValue(glycemia, measUnit);
        //}

        /// <summary>
        /// Factory for creating a new value according to the measure unit
        /// </summary>
        /// <param name="glycemia">The value</param>
        /// <param name="measUnit">The measure unit</param>
        /// /// <returns>The GlycemiaValue instance</returns>
        public static GlycemiaValue Measure(float glycemia, GlycemiaMeasureUnitEnum measUnit)
        {
            return new GlycemiaValue(FormatGlycemia(glycemia, measUnit), measUnit);
        }
        #endregion



        #region Business Methods

        /// <summary>
        /// Creates a new GlycemiaValue which is the conversion of the current one to the selected measure unit
        /// </summary>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The new GlycemiaValue instance</returns>
        public GlycemiaValue Convert(GlycemiaMeasureUnitEnum toUnit)
        {
            return Measure(PerformConversion(Value, toUnit), toUnit);
        }
        #endregion



        #region Private Methods

        /// <summary>
        /// Converts the number to a glycemia compliant value
        /// </summary>
        /// <param name="value">The input temperature value</param>
        /// <returns>the converted value</returns>
        private static float FormatGlycemia(float value, GlycemiaMeasureUnitEnum measUnit)
        {
            byte decimalPlaces = 0;

            //if (measUnit == GlycemiaMeasureUnitEnum.Millimols)
            //    decimalPlaces = 1;

            // temperatures rounded to the first decimal
            return (float)Math.Round(value, decimalPlaces);
        }

        /// <summary>
        /// Apply the conversion formula to the target measure unit
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The converted value</returns>
        private static float PerformConversion(float value, GlycemiaMeasureUnitEnum toUnit)
        {
            return FormatGlycemia(toUnit.ApplyConversionFormula(value), toUnit);
        }
        #endregion


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
            yield return Unit;
        }
    }
}