using System;
using System.Collections.Generic;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;

namespace GymProject.Domain.SharedKernel
{
    public class CalorieValue : ValueObject
    {



        #region Consts

        public const byte DecimalPlaces = 0;
        #endregion



        /// <summary>
        /// Calories value
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// Measure of unit - [kcal] / [kJ]
        /// </summary>
        public CaloriesMeasureUnitEnum Unit { get; private set; }




        #region Ctors

        private CalorieValue(float calories)
        {
            Value = calories;
            Unit = CaloriesMeasureUnitEnum.Kilocals;
        }

        private CalorieValue(float calories, CaloriesMeasureUnitEnum measUnit)
        {
            Value = calories;
            Unit = measUnit;
        }
        #endregion



        #region Factories


        /// <summary>
        /// Factory for creating a new value according to the measure unit - [kcal] default
        /// </summary>
        /// <param name="calories">The value</param>
        /// <param name="measUnit">The measure unit - [kcal] default </param>
        /// /// <returns>The CalorieValue instance</returns>
        public static CalorieValue Measure(float calories, CaloriesMeasureUnitEnum measUnit = null)
        {
            CaloriesMeasureUnitEnum unit = measUnit ?? CaloriesMeasureUnitEnum.Kilocals;
            return new CalorieValue(FormatCalories(calories), unit);
        }


        /// <summary>
        /// Factory for creating a new value - [kcal]
        /// </summary>
        /// <param name="kcals">The value</param>
        /// /// <returns>The CalorieValue instance</returns>
        public static CalorieValue MeasureKcal(float kcals)
        {
            return new CalorieValue(FormatCalories(kcals), CaloriesMeasureUnitEnum.Kilocals);
        }


        /// <summary>
        /// Factory for creating a new value - [kJ]
        /// </summary>
        /// <param name="kJoules">The value</param>
        /// /// <returns>The CalorieValue instance</returns>
        public static CalorieValue MeasureKJoules(float kJoules)
        {
            return new CalorieValue(FormatCalories(kJoules), CaloriesMeasureUnitEnum.KiloJoules);
        }
        #endregion



        #region Business Methods

        /// <summary>
        /// Creates a new CalorieValue which is the conversion of the current one to the selected measure unit
        /// </summary>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The new CalorieValue instance</returns>
        public CalorieValue Convert(CaloriesMeasureUnitEnum toUnit)
        {
            return Measure(PerformConversion(Value, toUnit), toUnit);
        }
        #endregion



        #region Private Methods

        /// <summary>
        /// Converts the number to a calories compliant value
        /// </summary>
        /// <param name="value">The input calories value</param>
        /// <returns>the converted value</returns>
        private static float FormatCalories(float value)
        {
            // Glycemia rounded to the first decimal
            return (float)Math.Round(value, (int)DecimalPlaces);
        }

        /// <summary>
        /// Apply the conversion formula to the target measure unit
        /// </summary>
        /// <param name="value">The value to be converted</param>
        /// <param name="toUnit">The target measure unit</param>
        /// <returns>The converted value</returns>
        private static float PerformConversion(float value, CaloriesMeasureUnitEnum toUnit)
        {
            return FormatCalories(toUnit.ApplyConversionFormula(value));
        }
        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
            yield return Unit;
        }


    }
}