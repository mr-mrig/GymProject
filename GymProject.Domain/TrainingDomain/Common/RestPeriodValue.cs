﻿using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
{
    public class RestPeriodValue : ValueObject
    {


        #region Const

        /// <summary>
        /// Value when no rest is specified
        /// </summary>
        private const int NotSetValue = -1;

        /// <summary>
        /// Value to be used when no rest is specified
        /// </summary>
        public const int DefaultRestValue = 90;

        /// <summary>
        /// Value when the rest is complete
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
        public int Value { get; private set; } = NotSetValue;


        /// <summary>
        /// The measure unit for the rest period
        /// </summary>
        public TimeMeasureUnitEnum MeasureUnit { get; private set; } = null;




        #region Ctors

        private RestPeriodValue(int rest, TimeMeasureUnitEnum unit)
        {
            Value = rest;
            MeasureUnit = unit;

            TestBusinessRules();
        }
        #endregion



        #region Factories


        /// <summary>
        /// Factory method - PROTECTED
        /// </summary>
        /// <param name="restValue">The value</param>
        /// <returns>The RestPeriodValue instance</returns>
        protected static RestPeriodValue SetRest(int restValue, TimeMeasureUnitEnum measUnit)

            => new RestPeriodValue((int)restValue, measUnit);

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
        public static RestPeriodValue SetRestNotSpecified()

            => SetRest(NotSetValue, TimeMeasureUnitEnum.Seconds);


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
        public bool IsRestSpecified() => Value != NotSetValue;


        public override string ToString() => Value.ToString() + MeasureUnit.Abbreviation;
        #endregion



        #region Business Rules Validation


        /// <summary>
        /// Rest period must be a non-negative number.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool IsValidNumber() => Value >= 0 || Value == NotSetValue;


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