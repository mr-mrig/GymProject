using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Exceptions;

namespace GymProject.Domain.TrainingDomain.Common
{
    public class TrainingNoteEntity : Entity<uint?>
    {



        public PersonalNoteValue Body { get; private set; } = null;



        #region Ctors

        protected TrainingNoteEntity(uint? id, PersonalNoteValue body) : base(id)
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
        //public virtual TrainingNoteEntity Write(uint? id, PersonalNoteValue body)

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
            // Unluckily EF will always need to laod from parameterless ctor, hence this will always throw when querying the repo
            // This reponsabilty should be moved to the DB layer - Non Null column
            //if (!IsBodyNonNull())
            //    throw new TrainingDomainInvariantViolationException($"The Note Body must be NON-null");
        }
        #endregion
    }
}
