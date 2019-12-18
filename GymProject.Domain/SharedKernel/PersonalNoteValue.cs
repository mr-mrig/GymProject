using GymProject.Domain.Base;
using System.Collections.Generic;
using GymProject.Domain.SharedKernel.Exceptions;

namespace GymProject.Domain.SharedKernel
{
    public class PersonalNoteValue : ValueObject
    {

        #region Constants

        /// <summary>
        /// Default maximum allowed length: 0 if unlimited
        /// </summary>
        public const int DefaultMaximumLength = 1000;

        #endregion



        /// <summary>
        /// The text
        /// </summary>
        public string Body { get; private set; } = string.Empty;




        #region Ctors

        private PersonalNoteValue() { }


        private PersonalNoteValue(string body)
        {
            //Body = body?.Trim() ?? string.Empty;
            Body = body?.Trim();

            TestBusinessRules();
        }

        #endregion


        #region Factories

        /// <summary>
        /// Create a new PersonalNoteValue object with the specified body
        /// </summary>
        /// <param name="body">The note text</param>
        /// <returns>The new PersonalNoteValue instance</returns>
        public static PersonalNoteValue Write(string body) => new PersonalNoteValue(body);

        #endregion



        #region Business Methods

        /// <summary>
        /// Get the length of the note body
        /// </summary>
        /// <returns>The length</returns>
        public int Length() => Body.Length;
        #endregion


        #region Business Rules Specifications

        /// <summary>
        /// The note text mustn't exceed the maximum length
        /// </summary>
        /// <returns></returns>
        private bool PersonalNoteNotExceedingMaximumLength() => Body.Length <= DefaultMaximumLength;

        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="ValueObjectInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
            if (!PersonalNoteNotExceedingMaximumLength())
                throw new ValueObjectInvariantViolationException($"The PersonalNote body must not exceed the maximum length: {DefaultMaximumLength.ToString()}");
        }
        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Body;
        }

    }
}
