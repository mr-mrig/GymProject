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
        /// Factory for creating a new sleep duration value - [h] default
        /// </summary>
        /// <param name="sleepTime">The sleep duration</param>
        /// <param name="unit">The time unit meas - [h] default</param>
        /// <returns>The SleepDurationValue instance</returns>
        public static SleepDurationValue Measure(float sleepTime, TimeMeasureUnitEnum unit = null)
        {
            TimeMeasureUnitEnum unitNotNull = unit ?? TimeMeasureUnitEnum.Hours;

            return new SleepDurationValue(FormatSleep(sleepTime, unitNotNull), unitNotNull);
        }

        /// <summary>
        /// Factory for creating a new sleep duration value - [h]
        /// </summary>
        /// <param name="sleepTime">The sleep duration</param>
        /// <param name="unit">The time unit meas - [h]</param>
        /// <returns>The SleepDurationValue instance</returns>
        public static SleepDurationValue MeasureHours(float sleepTime)
        {
            return new SleepDurationValue(FormatSleep(sleepTime, TimeMeasureUnitEnum.Hours), TimeMeasureUnitEnum.Hours);
        }

        /// <summary>
        /// Factory for creating a new sleep duration value - [m]
        /// </summary>
        /// <param name="sleepTime">The sleep duration</param>
        /// <param name="unit">The time unit meas - [m]</param>
        /// <returns>The SleepDurationValue instance</returns>
        public static SleepDurationValue MeasureMinutes(float sleepTime)
        {
            return new SleepDurationValue(FormatSleep(sleepTime, TimeMeasureUnitEnum.Minutes), TimeMeasureUnitEnum.Minutes);
        }

        /// <summary>
        /// Converts to minutes
        /// </summary>
        /// <param name="hours">The hours of sleep</param>
        /// <returns>The SleepDurationValue converted to minutes</returns>
        public static SleepDurationValue ToMinutes(float hours) => SleepDurationValue.MeasureMinutes(FormatSleep(hours * 60f, TimeMeasureUnitEnum.Minutes));

        /// <summary>
        /// Converts to hours
        /// </summary>
        /// <param name="minutes">The minutes of sleep</param>
        /// <returns>The SleepDurationValue converted to hours</returns>
        public static SleepDurationValue ToHours(float minutes) => SleepDurationValue.MeasureHours(FormatSleep(minutes / 60f, TimeMeasureUnitEnum.Hours));
        #endregion


        #region Private Methods

        /// <summary>
        /// Converts the number to a glycemia compliant value
        /// </summary>
        /// <param name="value">The input temperature value</param>
        /// <returns>the converted value</returns>
        private static float FormatSleep(float value, TimeMeasureUnitEnum unit)
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