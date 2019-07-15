using GymProject.Domain.Base;
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

        #endregion




        public float Value { get; private set; }



        #region Ctors

        private RatingValue(float ratingValue)
        {
            if (ratingValue < MinimumValue || ratingValue > MaximumValue)

                throw new ArgumentException($"Trying to create an invalid Rating object: {ratingValue.ToString()} is not between {MinimumValue.ToString()} and {MaximumValue.ToString()}");

            Value = ratingValue;
        }

        #endregion


        #region Factories

        /// <summary>
        /// Create a new Rating object with the specified value
        /// </summary>
        /// <param name="ratingValue">Rating value</param>
        /// <returns>The Rating object</returns>
        public static RatingValue Rate(float ratingValue)
        {
            return new RatingValue(ratingValue);
        }
        #endregion



        #region Business Methods

        /// <summary>
        /// Creates a new RatingObject by increasing the current one by one unit
        /// </summary>
        /// <returns>The RatingObject increased by one unity</returns>
        public RatingValue Increase()
        {
            return new RatingValue(Value + 1);
        }

        /// <summary>
        /// Creates a new RatingObject by decreasing the current one by one unit
        /// </summary>
        /// <returns>The RatingObject decreased by one unity</returns>
        public RatingValue Decrease()
        {
            return new RatingValue(Value - 1);
        }

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
