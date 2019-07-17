using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.SharedKernel
{
    public class VolumeValue : ValueObject
    {


        #region Consts

        /// <summary>
        /// Number of default decimal places for storing the Value
        /// </summary>
        private const byte DecimalPlaces = 1;

        /// <summary>
        /// Measures of unit allowed for the Value
        /// </summary>
        public readonly IEnumerable<VolumeUnitMeasureEnum> AllowedMeasUnits = new List<VolumeUnitMeasureEnum>()
        {
            VolumeUnitMeasureEnum.Liters,
        };
        #endregion


        /// <summary>
        /// The value of the measure
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// The measurement unit
        /// </summary>
        public VolumeUnitMeasureEnum Unit { get; private set; }



        #region Ctors

        private VolumeValue(float measValue, VolumeUnitMeasureEnum measUnit = null)
        {
            Value = measValue;
            Unit = measUnit ?? VolumeUnitMeasureEnum.Liters;

            if (!AllowedMeasUnits.Contains(Unit))
                throw new ArgumentException($"{measUnit.Abbreviation} is not allowed for {GetType().Name}", "measUnit");
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory for creating a new value [l]
        /// </summary>
        /// <param name="weight">The value</param>
        /// <returns>The VolumeValue instance</returns>
        public static VolumeValue MeasureLiters(float volume)
        {
            return new VolumeValue(FormatVolume(volume));
        }


        /// <summary>
        /// Factory for creating a new value according to the measure unit
        /// </summary>
        /// <param name="glycemia">The value</param>
        /// <param name="measUnit">The measure unit - [g] default</param>
        /// /// <returns>The VolumeValue instance</returns>
        public static VolumeValue Measure(float glycemia, VolumeUnitMeasureEnum measUnit = null)
        {
            return new VolumeValue(FormatVolume(glycemia), measUnit);
        }
        #endregion


        #region Business Methods

        ///// <summary>
        ///// Creates a new VolumeValue which is the conversion of the current one to the selected measure unit
        ///// </summary>
        ///// <param name="toUnit">The target measure unit</param>
        ///// <returns>The new VolumeValue instance</returns>
        //public VolumeValue Convert(VolumeUnitMeasureEnum toUnit)
        //{
        //    return Measure(PerformConversion(Value, toUnit), toUnit);
        //}
        #endregion



        #region Private Methods

        /// <summary>
        /// Converts the number to a volume compliant value
        /// </summary>
        /// <param name="value">The input volume value</param>
        /// <param name="measUnit">The measure unit</param>
        /// <returns>the converted value</returns>
        private static float FormatVolume(float value)
        {
            // Volume rounded to 1 decimal place
            return (float)Math.Round(value, DecimalPlaces);
        }

        ///// <summary>
        ///// Apply the conversion formula to the target measure unit
        ///// </summary>
        ///// <param name="value">The value to be converted</param>
        ///// <param name="toUnit">The target measure unit</param>
        ///// <returns>The converted value</returns>
        //private static float PerformConversion(float value, VolumeUnitMeasureEnum toUnit)
        //{
        //    return FormatVolume(toUnit.ApplyConversionFormula(value));
        //}
        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
            yield return Unit;
        }
    }
}