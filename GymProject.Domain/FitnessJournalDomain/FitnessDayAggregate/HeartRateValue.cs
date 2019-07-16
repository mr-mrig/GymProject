using System;
using System.Collections.Generic;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class HeartRateValue : ValueObject
    {


        #region Consts

        public const byte DecimalPlaces = 0;
        #endregion



        public float Value { get; private set; }

        public HeartRateMeasureUnitEnum Unit { get; private set; }



        #region Ctors

        private HeartRateValue(float heartRate)
        {
            Value = heartRate;
            Unit = HeartRateMeasureUnitEnum.Pulses;
        }

        private HeartRateValue(float heartRate, HeartRateMeasureUnitEnum measUnit)
        {
            Value = heartRate;
            Unit = measUnit;
        }
        #endregion


        #region Factories

        /// <summary>
        /// Factory for creating a new heart rate measure [bpm]
        /// </summary>
        /// <param name="heartRate">The value</param>
        /// <returns>The HeartRateValue instance</returns>
        public static HeartRateValue Measure(float heartRate, HeartRateMeasureUnitEnum unit = null)
        {
            return new HeartRateValue(FormatHeartRate(heartRate), unit ?? HeartRateMeasureUnitEnum.Pulses);
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Converts the number to a glycemia compliant value
        /// </summary>
        /// <param name="value">The input temperature value</param>
        /// <returns>the converted value</returns>
        private static float FormatHeartRate(float value)
        {
            // Heartrate rounded to the nearest integer
            return (float)Math.Round(value, (int)DecimalPlaces);
        }
        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            throw new System.NotImplementedException();
        }
    }
}