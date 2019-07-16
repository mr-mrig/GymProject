using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class SleepDurationValue : ValueObject
    {


        #region Consts

        public const byte DecimalPlaces = 1;
        #endregion



        public float Value { get; private set; }

        public TimeMeasureUnitEnum Unit { get; private set; }



        #region Ctors

        private SleepDurationValue(float heartRate)
        {
            Value = heartRate;
            Unit = TimeMeasureUnitEnum.Minutes;
        }

        private SleepDurationValue(float heartRate, TimeMeasureUnitEnum measUnit)
        {
            Value = heartRate;
            Unit = measUnit;
        }
        #endregion


        #region Factories

        /// <summary>
        /// Factory for creating a new sleep duration value
        /// </summary>
        /// <param name="sleepMinutes">The sleep duration</param>
        /// <param name="unit">The time unit meas - [h] default</param>
        /// <returns>The SleepDurationValue instance</returns>
        public static SleepDurationValue Measure(float sleepMinutes, TimeMeasureUnitEnum unit = null)
        {
            TimeMeasureUnitEnum unitNotNull = unit ?? TimeMeasureUnitEnum.Hours;

            return new SleepDurationValue(FormatHeartRate(sleepMinutes, unitNotNull), unitNotNull);
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Converts the number to a glycemia compliant value
        /// </summary>
        /// <param name="value">The input temperature value</param>
        /// <returns>the converted value</returns>
        private static float FormatHeartRate(float value, TimeMeasureUnitEnum unit)
        {
            // Heartrate rounded to the nearest integer
            return (float)Math.Round(value, (int)GetDecimalPlaces(unit));
        }

        /// <summary>
        /// Decides how many decimal places according to the meas unit and the class const
        /// </summary>
        /// <param name="unit">The meas unit</param>
        /// <returns>The number of decimal places</returns>
        private static byte GetDecimalPlaces(TimeMeasureUnitEnum unit)
        {
            if (unit.HasDecimals)
                return DecimalPlaces;

            else
                return 0;
        }
        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
            yield return Unit;
        }

    }
}