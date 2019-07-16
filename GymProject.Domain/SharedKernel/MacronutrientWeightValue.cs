using System.Collections.Generic;
using GymProject.Domain.Base;

namespace GymProject.Domain.SharedKernel
{
    public class MacronutirentWeightValue : ValueObject
    {


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
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory for creating a new value [g]
        /// </summary>
        /// <param name="weight">The value</param>
        /// <returns>The MacronutirentWeightValue instance</returns>
        public static MacronutirentWeightValue MeasureGrams(float weight)
        {
            return new MacronutirentWeightValue(FormatGlycemia(weight, WeightUnitMeasureEnum.Grams));
        }


        /// <summary>
        /// Factory for creating a new value according to the measure unit
        /// </summary>
        /// <param name="glycemia">The value</param>
        /// <param name="measUnit">The measure unit - [g] default</param>
        /// /// <returns>The MacronutirentWeightValue instance</returns>
        public static MacronutirentWeightValue Measure(float glycemia, WeightUnitMeasureEnum measUnit = null)
        {
            return new MacronutirentWeightValue(FormatGlycemia(glycemia, measUnit), measUnit);
        }
        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            throw new System.NotImplementedException();
        }
    }
}