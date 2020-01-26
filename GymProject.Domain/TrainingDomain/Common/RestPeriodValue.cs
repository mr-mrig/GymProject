using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.Common
{
    public class RestPeriodValue : ValueObject
    {


        #region Const

        /// <summary>
        /// Default meas unit to be used when nothing is specified
        /// </summary>
        public static readonly TimeMeasureUnitEnum DefaultRestMeasUnit = TimeMeasureUnitEnum.Seconds;

        /// <summary>
        /// Values that identifies that an input value has not been specified
        /// </summary>
        protected const int NotSpecifiedInput = -1;


        /// <summary>
        /// Value to be used when no rest is specified. This value is used internally, but the user should see the NotSpecifiedInput one.
        /// </summary>
        public const int DefaultRestValue = 90;

        /// <summary>
        /// Value which identifies a complete rest.
        /// </summary>
        public const int FullRecoveryRestValue = 240;

        ///// <summary>
        ///// Value when two sets are linked together
        ///// </summary>
        //public const int LinkedSetRestValue = 0;
        #endregion



        /// <summary>
        /// The TUT as 4 values: concentric-stop-eccentric-stop 
        /// </summary>
        public int Value { get; private set; } = NotSpecifiedInput;


        /// <summary>
        /// The measure unit for the rest period
        /// </summary>
        public TimeMeasureUnitEnum MeasureUnit { get; private set; } = null;





        #region Ctors

        private RestPeriodValue() { }


        private RestPeriodValue(int rest, TimeMeasureUnitEnum unit) 
        {
            Value = rest;
            MeasureUnit = unit ?? TimeMeasureUnitEnum.Seconds;

            TestBusinessRules();
        }
        #endregion



        #region Factories


        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="restValue">The value</param>
        /// <returns>The RestPeriodValue instance</returns>
        public static RestPeriodValue SetRest(int restValue, TimeMeasureUnitEnum measUnit)

            => new RestPeriodValue(restValue, measUnit);

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="restSeconds">The value</param>
        /// <returns>The RestPeriodValue instance</returns>
        public static RestPeriodValue SetRestSeconds(uint restSeconds)

            => SetRest((int)restSeconds, TimeMeasureUnitEnum.Seconds);


        /// <summary>
        /// Factory method - When the rest is not specified - Default value is assumed in computations
        /// </summary>
        /// <returns>The RestPeriodValue instance</returns>
        public static RestPeriodValue SetNotSpecifiedRest()

            => SetRest(NotSpecifiedInput, TimeMeasureUnitEnum.Seconds);


        /// <summary>
        /// Factory method - When the rest is complete
        /// </summary>
        /// <returns>The RestPeriodValue instance</returns>
        public static RestPeriodValue SetFullRecoveryRest()

            => SetRest(FullRecoveryRestValue, TimeMeasureUnitEnum.Seconds);

        #endregion


        #region Business Methods

        /// <summary>
        /// Check whether the rest time has been specified or it has been left not set
        /// </summary>
        /// <returns>True if a valid rest time has been specified</returns>
        public bool IsRestSpecified() => Value != NotSpecifiedInput;


        public override string ToString() => Value.ToString() + MeasureUnit?.Abbreviation ?? "";
        #endregion



        #region Business Rules Validation


        /// <summary>
        /// Rest period must be a non-negative number.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool IsValidNumber() => Value >= 0 || Value == NotSpecifiedInput;


        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
            if (!IsValidNumber())
                throw new TrainingDomainInvariantViolationException($"Rest period must be a non-negative number.");
        }
        #endregion



        #region Operators

        public static RestPeriodValue operator +(RestPeriodValue left, int right)
        {
            if (left == null)
                return null;

            // If special rest values, treat them as 0
            // Adding special values might require different treatment here
            if (!left.IsRestSpecified())
                left.Value = 0;

            return RestPeriodValue.SetRestSeconds((uint)(left.Value + right));
        }


        public static RestPeriodValue operator -(RestPeriodValue left, int right)
        {
            if (left == null)
                return null;

            return RestPeriodValue.SetRestSeconds((uint)(left.Value - right));
        }
        #endregion




        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
            yield return MeasureUnit;
        }
    }
}