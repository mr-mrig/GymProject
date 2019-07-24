using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
using System.Collections.Generic;

namespace GymProject.Domain.SharedKernel
{
    public class WeeklyOccuranceValue : ValueObject
    {



        #region Consts

        /// <summary>
        /// Maximum value
        /// </summary>
        public const int MaximumTimes = 7;
        #endregion


        /// <summary>
        /// The number of occurences inside the week
        /// </summary>
        public int Value { get; private set; } = 0;





        #region Ctors

        private WeeklyOccuranceValue(int value)
        {
            Value = value;

            TestBusinessRules();
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory for creating a new WeeklyOccuranceValue
        /// </summary>
        /// <param name="value">The number of times per week</param>
        /// /// <returns>The WeeklyOccuranceValue instance</returns>
        public static WeeklyOccuranceValue TrackOccurance(int value) => new WeeklyOccuranceValue(value);


        #endregion



        #region Business Methods

        /// <summary>
        /// Checks if the occurance means never
        /// </summary>
        /// <returns>True if never inside the week</returns>
        public bool Never() => Value == 0;
        #endregion


        #region Business Rule sSpecifications

        /// <summary>
        /// The occurence number cannot exceed the number of weekdays
        /// </summary>
        /// <returns>True if the business rule is met</returns>
        private bool WeeklyOccuranceNotExceedingWeekdays() => Value <= MaximumTimes;


        /// <summary>
        /// The occurence number cannot be negative
        /// </summary>
        /// <returns>True if the business rule is met</returns>
        private bool WeeklyOccuranceNotNegative() => Value >= 0;


        /// <summary>
        /// Apply the Business Rules and manage invalid states
        /// </summary>
        /// <exception cref="ValueObjectInvariantViolationException">Thrown if business rules are not met</exception>
        private void TestBusinessRules()
        {
            if (!WeeklyOccuranceNotExceedingWeekdays())
                throw new ValueObjectInvariantViolationException($"The occurence number cannot exceed the number of weekdays: {Value.ToString()}");

            if (!WeeklyOccuranceNotNegative())
                throw new ValueObjectInvariantViolationException($"The occurence number cannot be negative: {Value.ToString()}");
        }
        #endregion


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

    }
}
