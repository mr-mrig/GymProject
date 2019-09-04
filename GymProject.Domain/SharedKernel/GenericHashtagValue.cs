using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel.Exceptions;
using System;
using System.Collections.Generic;

namespace GymProject.Domain.SharedKernel
{
    public class GenericHashtagValue : ValueObject
    {


        #region Consts

        /// <summary>
        ///  The delimeter which identifies the Hashtag content
        /// </summary>
        public const string HashtagDelimiter = "#";

        /// <summary>
        /// The maximum allowed length
        /// </summary>
        public const int DefaultMaximumLength = 100;

        /// <summary>
        /// The minimum allowed length
        /// </summary>
        public const int DefaultMinimumLength = 2;
        #endregion



        /// <summary>
        /// The Hashtag body
        /// </summary>
        public string Body { get; private set; }




        #region Ctors

        private GenericHashtagValue() {}

        private GenericHashtagValue(string body)
        {
            Body = body.Trim();
            TestBusinessRules();
        }

        #endregion



        #region Factories


        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="body">The body of the Hashtag</param>
        /// <returns>The GenericHashtagValue instance</returns>
        public static GenericHashtagValue TagWith(string body)

            => new GenericHashtagValue(body);

        #endregion



        #region Business Methods

        /// <summary>
        /// Get the length of the Hashtag body, delimiter excluded
        /// </summary>
        /// <returns>The length of the body</returns>
        public int Length() => Body.Length;


        /// <summary>
        /// Get the length of the Hashtag body, delimiter included
        /// </summary>
        /// <returns>The full length</returns>
        public int FullLength() => Body.Length + HashtagDelimiter.Length;


        /// <summary>
        /// Build the hashtag string appending the body to the delimiter
        /// </summary>
        /// <returns></returns>
        public string ToFullHashtag()
        
            => HashtagDelimiter + Body;

        #endregion


        #region Business Validation

        /// <summary>
        /// A Hashtag cannot contain spaces
        /// </summary>
        /// <returns>True if business rule is met</returns>
        public bool HasNoSpaces() => !Body.Contains(" ");


        /// <summary>
        /// A Hashtag cannot contain hashtag delimiters in the middle
        /// </summary>
        /// <returns>True if business rule is met</returns>
        public bool ContainsNoDelimiters() => !Body.Contains(HashtagDelimiter);



        /// <summary>
        /// The Hashtag length must not exceed the maximum length
        /// </summary>
        /// <returns></returns>
        private bool IsBelowMaximumLength() => Length() <= DefaultMaximumLength;


        /// <summary>
        /// The Hashtag length must exceed the minimum length
        /// </summary>
        /// <returns></returns>
        private bool IsAboveMinimumLength() => Length() >= DefaultMinimumLength;



        public void TestBusinessRules()
        {
            if (!IsBelowMaximumLength())
                throw new ValueObjectInvariantViolationException($"The Hashtag length must not exceed the maximum length.");

            if (!IsAboveMinimumLength())
                throw new ValueObjectInvariantViolationException($"The Hashtag length must exceed the minimum length.");

            if (!ContainsNoDelimiters())
                throw new ValueObjectInvariantViolationException($"A Hashtag cannot contain hashtag delimiters in the middle.");

            if (!HasNoSpaces())
                throw new ValueObjectInvariantViolationException($"A Hashtag cannot contain spaces.");
        }

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Body;
        }


    }
}
