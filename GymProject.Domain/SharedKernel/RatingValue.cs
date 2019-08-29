using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class RatingValue : ValueObject
    {

        #region Constants

        /// <summary>
        /// Minimum value
        /// </summary>
        public const float MinimumValue = 0;

        /// <summary>
        /// Maximum value
        /// </summary>
        public const float MaximumValue = 5;

        /// <summary>
        /// Number of default decimal places
        /// </summary>
        private const byte DecimalPlaces = 0;
        #endregion



        /// <summary>
        /// The Rating numeric Value
        /// </summary>
        public float Value { get; private set; }



        #region Ctors

        private RatingValue(float ratingValue)
        {
            Value = ratingValue;

            TestBusinessRules();
        }

        #endregion


        #region Factories

        /// <summary>
        /// Create a new Rating object with the specified value
        /// </summary>
        /// <param name="ratingValue">Rating value</param>
        /// <returns>The Rating object</returns>
        /// <exception cref="ValueObjectInvariantViolationException">If ratingValue out of boundaries</exception>
        public static RatingValue Rate(float ratingValue) => new RatingValue(FormatRating(ratingValue));

        #endregion



        #region Business Methods

        /// <summary>
        /// Creates a new RatingObject by increasing the current one by one unit
        /// </summary>
        /// <returns>The RatingObject increased by one unity</returns>
        public RatingValue Increase() => Rate(Value + 1);


        /// <summary>
        /// Creates a new RatingObject by decreasing the current one by one unit
        /// </summary>
        /// <returns>The RatingObject decreased by one unity</returns>
        public RatingValue Decrease() => Rate(Value - 1);

        #endregion


        #region Private Methods
        /// <summary>
        /// Converts the number to a rating compliant value
        /// </summary>
        /// <param name="value">The input rating value</param>
        /// <returns>The converted value</returns>
        private static float FormatRating(float value)
        {
            // Volume rounded to 1 decimal place
            return (float)Math.Round(value, (int)DecimalPlaces);
        }
        #endregion


        #region Business Rules Validation

        /// <summary>
        /// The Rating value must fall inside boundaries
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool RatingInsideBoundaries() => Value >= MinimumValue && Value <= MaximumValue;



        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="ValueObjectInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
            if (!RatingInsideBoundaries())
                throw new ValueObjectInvariantViolationException($"The Rating value must fall inside boundaries: {MinimumValue.ToString()} <= Rating <= {MaximumValue.ToString()}");
        }

        #endregion


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
