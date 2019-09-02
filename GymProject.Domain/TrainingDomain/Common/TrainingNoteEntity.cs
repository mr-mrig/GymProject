using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Exceptions;

namespace GymProject.Domain.TrainingDomain.Common
{
    public class TrainingNoteEntity : Entity<IdTypeValue>
    {



        public PersonalNoteValue Body { get; private set; } = null;



        #region Ctors

        protected TrainingNoteEntity(IdTypeValue id, PersonalNoteValue body) : base(id)
        {
            Body = body;
            TestBusinessRules();
        }
        #endregion


        #region Factories

        ///// <summary>
        ///// Factory method for transient entities
        ///// </summary>
        ///// <param name="body">The body fo the Note</param>
        ///// <returns>The TrainingNoteEntity instance</returns>
        //public virtual TrainingNoteEntity WriteTransient(PersonalNoteValue body)

        //    => Write(null, body);

        ///// <summary>
        ///// Factory method
        ///// </summary>
        ///// <param name="body">The body fo the Note</param>
        ///// <param name="id">The Id of the Note instance</param>
        ///// <returns>The TrainingNoteEntity instance</returns>
        //public virtual TrainingNoteEntity Write(IdTypeValue id, PersonalNoteValue body)

        //    => new TrainingNoteEntity(id, body);

        #endregion


        #region Business Rules Validation


        /// <summary>
        /// The Note Body must be NON-null
        /// </summary>
        /// <returns>True if business rule is met</returns>
        public bool IsBodyNonNull() => Body != null;



        private void TestBusinessRules()
        {
            if (!IsBodyNonNull())
                throw new TrainingDomainInvariantViolationException($"The Note Body must be NON-null");
        }
        #endregion
    }
}
