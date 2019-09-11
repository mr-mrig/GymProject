using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Exceptions;

namespace GymProject.Domain.TrainingDomain.ExcerciseAggregate
{
    public class ExcerciseRoot : StatusTrackingEntity<uint?>, IAggregateRoot
    {




        /// <summary>
        /// The identifying name
        /// </summary>
        public string Name { get; private set; } = string.Empty;


        /// <summary>
        /// The description
        /// </summary>
        public PersonalNoteValue Description { get; private set; } = null;





        #region Ctors

        private ExcerciseRoot() : base(null, null) { }


        private ExcerciseRoot(uint? id, string name, PersonalNoteValue description, EntryStatusTypeEnum entryStatus) : base(id, entryStatus)
        {
            Name = name?.Trim() ?? string.Empty;
            Description = description;
            //OwnerId = ownerId;

            TestBusinessRules();
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method - For adding new objects
        /// </summary>
        /// <param name="id">The object ID</param>
        /// <param name="name">The identifying name</param>
        /// <param name="description">The description</param>
        /// <param name="ownerId">The User which the phase was created by</param>
        /// <param name="entryStatus">TThe specified entry status</param>
        /// <returns>The Excercise instance</returns>
        public static ExcerciseRoot AddToExcerciseLibrary
        (
            uint? id,
            string name,
            PersonalNoteValue description,
            EntryStatusTypeEnum entryStatus
        )
            => new ExcerciseRoot(id, name, description, entryStatus);

        #endregion



        #region Business Methods
        #endregion




        #region Business Rules Validation

        /// <summary>
        /// The Training Phase must have a valid name.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NameIsMandatory() => !string.IsNullOrWhiteSpace(Name);


        /// <summary>
        /// The Training Phase requires the Entry Status to be set.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool ValidEntryStatus() => EntryStatus != null;




        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        protected void TestBusinessRules()
        {
            if (!NameIsMandatory())
                throw new TrainingDomainInvariantViolationException($"The Excercise must have a valid name.");

            if (!ValidEntryStatus())
                throw new TrainingDomainInvariantViolationException($"The Excercise requires the Entry Status to be set.");

        }
        #endregion


    }
}