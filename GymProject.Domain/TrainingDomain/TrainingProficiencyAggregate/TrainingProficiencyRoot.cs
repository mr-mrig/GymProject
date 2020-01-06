using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.SharedKernel;

namespace GymProject.Domain.TrainingDomain.TrainingProficiencyAggregate
{
    public class TrainingProficiencyRoot : StatusTrackingEntity<uint?>, IAggregateRoot
    {



        /// <summary>
        /// The default entry status that is used if nothing is specified
        /// </summary>
        public static readonly EntryStatusTypeEnum DefaultEntryStatus = EntryStatusTypeEnum.Pending;



        /// <summary>
        /// The identifying name
        /// </summary>
        public string Name { get; private set; } = string.Empty;


        /// <summary>
        /// The description
        /// </summary>
        public PersonalNoteValue Description { get; private set; } = null;


        ///// <summary>
        ///// The User who created the Phase entry
        ///// </summary>
        //public uint? OwnerId { get; private set; } = null;




        #region Ctors

        private TrainingProficiencyRoot() : base(null, null) { }

        private TrainingProficiencyRoot(uint? id, string name, PersonalNoteValue description, EntryStatusTypeEnum entryStatus) : base(id, entryStatus ?? DefaultEntryStatus)
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
        /// <param name="entryStatus">TThe specified entry status - if left null <see cref="DefaultEntryStatus"/> will be used</param>
        /// <returns>A new TrainingProficiency instance</returns>
        public static TrainingProficiencyRoot CreateTrainingProficiency
        (
            uint? id,
            string name,
            PersonalNoteValue description,
            EntryStatusTypeEnum entryStatus
        )
            => new TrainingProficiencyRoot(id, name, description, entryStatus);


        /// <summary>
        /// Factory method - Private
        /// </summary>
        /// <param name="name">The identifying name</param>
        /// <param name="description">The description</param>
        /// <param name="ownerId">The User which the phase was created by</param>
        /// <returns>A new TrainingProficiency instance</returns>
        public static TrainingProficiencyRoot CreatePrivateTrainingProficiency(string name, PersonalNoteValue description)

            => CreateTrainingProficiency(null, name, description, EntryStatusTypeEnum.Private);


        /// <summary>
        /// Factory method - Public as pending before approval
        /// </summary>
        /// <param name="name">The identifying name</param>
        /// <param name="description">The description</param>
        /// <param name="ownerId">The User which the phase was created by</param>
        /// <returns>A new TrainingProficiency instance</returns>
        public static TrainingProficiencyRoot CreatePublicTrainingProficiency(string name, PersonalNoteValue description)

            => CreateTrainingProficiency(null, name, description, EntryStatusTypeEnum.Pending);


        /// <summary>
        /// Factory method- Native, require the ID to be set
        /// </summary>
        /// <param name="id">The object ID</param>
        /// <param name="name">The identifying name</param>
        /// <param name="description">The description</param>
        /// <param name="ownerId">The User which the phase was created by</param>
        /// <returns>A new TrainingProficiency instance</returns>
        public static TrainingProficiencyRoot AddNativeTrainingProficiency(uint? id, string name, PersonalNoteValue description)

            => CreateTrainingProficiency(id, name, description, EntryStatusTypeEnum.Native);

        #endregion



        #region Business Methods

        /// <summary>
        /// Set the Phase name
        /// </summary>
        /// <param name="newName">The new Phase name</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is not met</exception>
        public void Rename(string newName)
        {
            Name = newName?.Trim() ?? string.Empty;

            TestBusinessRules();
        }

        #endregion




        #region Business Rules Validation

        /// <summary>
        /// The Training Phase must have a valid name.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NameIsMandatory() => !string.IsNullOrWhiteSpace(Name);



        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        protected void TestBusinessRules()
        {
            if (!NameIsMandatory())
                throw new TrainingDomainInvariantViolationException($"The Training Phase must have a valid name.");
        }
        #endregion


    }
}
