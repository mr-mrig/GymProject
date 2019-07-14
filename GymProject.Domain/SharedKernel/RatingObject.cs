using GymProject.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public class RatingObject : ValueObject
    {

        #region Constants

        /// <summary>
        /// NULL value
        /// </summary>
        public const float NullValue = -1;

        /// <summary>
        /// Default value
        /// </summary>
        public const float DefaultValue = NullValue;

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

        private RatingObject(float ratingValue = DefaultValue)
        {
            if (ratingValue != NullValue 
                && (ratingValue < MinimumValue || ratingValue > MaximumValue))

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
        public static RatingObject Rate(float ratingValue)
        {
            return new RatingObject(ratingValue);
        }

        /// <summary>
        /// Create a new Rating object with the default value
        /// </summary>
        /// <returns>The Rating object</returns>
        public static RatingObject Rate()
        {
            return new RatingObject();
        }
        #endregion



        #region Business Methods

        /// <summary>
        /// Creates a new RatingObject by increasing the current one by one unit
        /// </summary>
        /// <returns>The RatingObject increased by one unity</returns>
        public RatingObject Increase()
        {
            return new RatingObject(Value + 1);
        }

        /// <summary>
        /// Creates a new RatingObject by decreasing the current one by one unit
        /// </summary>
        /// <returns>The RatingObject decreased by one unity</returns>
        public RatingObject Decrease()
        {
            return new RatingObject(Value - 1);
        }

        /// <summary>
        /// Checks if the RatingObject is NULL
        /// </summary>
        /// <returns>True or false</returns>
        public bool IsNull()
        {
            return Value == NullValue;
        }
        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
