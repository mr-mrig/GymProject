using GymProject.Domain.Base;
using System.Collections.Generic;
using GymProject.Domain.SharedKernel.Exceptions;
using System.Text.RegularExpressions;

namespace GymProject.Domain.SharedKernel
{
    public class DescriptiveNameValue : ValueObject
    {

        #region Constants

        /// <summary>
        /// Default maximum allowed length: 0 if unlimited
        /// </summary>
        public const int DefaultMaximumLength = 25;


        /// <summary>
        /// Default minimum allowed length
        /// </summary>
        public const int DefaultMinimumLength = 3;

        #endregion



        private int _minimumLength = DefaultMinimumLength;

        private int _maximumLength = DefaultMaximumLength;




        /// <summary>
        /// The text
        /// </summary>
        public string Name { get; private set; } = string.Empty;




        #region Ctors

        private DescriptiveNameValue(string name, int maximumLength, int minimumLength)
        {
            Name = name ?? string.Empty;
            //_maximumLength = maximumLength <= 0 ? DefaultMaximumLength : maximumLength;
            //_minimumLength = minimumLength <= 0 ? DefaultMinimumLength : minimumLength;

            _maximumLength = maximumLength;
            _minimumLength = minimumLength;

            TestBusinessRules();
        }

        #endregion


        #region Factories

        /// <summary>
        /// Create a new DescriptiveNameValue object with the specified body
        /// There are some naming rules for the object: only alphanumeric, spaces, hyphens and underscors are allowed.
        /// </summary>
        /// <param name="name">The descriptive name text</param>
        /// <param name="maximumLength">The name maximum allowed length</param>
        /// <param name="minimumLength">The name minimum allowed length</param>
        /// <returns>The new DescriptiveNameValue instance</returns>
        public static DescriptiveNameValue Write(string name, int minimumLength, int maximumLength) => new DescriptiveNameValue(name, maximumLength, minimumLength);

        #endregion



        #region Business Methods

        /// <summary>
        /// Get the length of the Name
        /// </summary>
        /// <returns>The length of the name</returns>
        public int Length() => Name.Length;
        #endregion


        #region Business Rules Specifications

        /// <summary>
        /// The note text mustn't exceed the maximum length
        /// </summary>
        /// <returns></returns>
        private bool NameBelowMaximumLength() => Length() <= _maximumLength;


        /// <summary>
        /// The note text mustn't exceed the minimum length
        /// </summary>
        /// <returns></returns>
        private bool NameAboveMinimumLength() => Length() >= _minimumLength;


        /// <summary>
        /// The note text must be made up of allowed chars only
        /// </summary>
        /// <returns></returns>
        private bool NameWithAllowedChars() => !Regex.IsMatch(Name, "[^0-9a-zA-Z_\\-\\s]+");


        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="ValueObjectInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
            if (!NameBelowMaximumLength())
                throw new ValueObjectInvariantViolationException($"The DescriptiveName must not exceed the maximum length: {_maximumLength.ToString()}");

            if (!NameAboveMinimumLength())
                throw new ValueObjectInvariantViolationException($"The DescriptiveName must not exceed the minimum length: {_minimumLength.ToString()}");

            if (!NameWithAllowedChars())
                throw new ValueObjectInvariantViolationException($"The DescriptiveName has not allowed chars");
        }
        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Name;
        }

    }
}

