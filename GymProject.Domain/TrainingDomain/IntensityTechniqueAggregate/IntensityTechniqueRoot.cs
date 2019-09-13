using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.SharedKernel;

namespace GymProject.Domain.TrainingDomain.IntensityTechniqueAggregate
{
    public class IntensityTechniqueRoot : StatusTrackingEntity<uint?>, IAggregateRoot
    {




        /// <summary>
        /// The Intensity Technique full name
        /// </summary>
        public string Name { get; private set; } = null;


        /// <summary>
        /// The Intensity Technique abbreviation
        /// </summary>
        public string Abbreviation { get; private set; } = null;


        /// <summary>
        /// The Intensity Technique extensive description
        /// </summary>
        public PersonalNoteValue Description { get; private set; } = null;


        /// <summary>
        /// Whether the Intensity Technique links two or more Work Components
        /// </summary>
        public bool IsLinkingTechnique { get; private set; }


        /// <summary>
        /// FK to the User who created the Intensity Technique entry
        /// </summary>
        public uint? OwnerId { get; private set; }





        #region Ctors

        private IntensityTechniqueRoot() : base(null, null)
        {
                
        }

        private IntensityTechniqueRoot(uint? id, uint? ownerId, string name, string abbreviation, PersonalNoteValue description, bool isLinkingTechnique, EntryStatusTypeEnum entryStatus) 
            : base(id, entryStatus)
        {
            Name = name?.Trim() ?? string.Empty;
            OwnerId = ownerId;
            Abbreviation = abbreviation.Trim();
            Description = description;
            IsLinkingTechnique = isLinkingTechnique;

            TestBusinessRules();
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method - PROTECTED
        /// </summary>
        /// <param name="name">The Intensity Technique name</param>
        /// <param name="abbreviation">The Intensity Technique abbreviation</param>
        /// <param name="description">The Intensity Technique description</param>
        /// <param name="ownerId">The ID of the User which is adding the entry</param>
        /// <param name="isLinkingTechnique">Wether the Intensity Technique links two or more work components</param>
        /// <param name="id">The ID of the Intensity Technique</param>
        /// <param name="entryStatus">The status of the entry</param>
        /// <returns>A new IntensityTechnique instance</returns>
        protected static IntensityTechniqueRoot CreateIntensityTechnique(uint? id, uint? ownerId, string name, string abbreviation, PersonalNoteValue description, bool isLinkingTechnique, EntryStatusTypeEnum entryStatus)

            => new IntensityTechniqueRoot(id, ownerId, name, abbreviation, description, isLinkingTechnique, entryStatus);


        /// <summary>
        /// Factory method for private Intensity Technqiue entries as pending before approval
        /// </summary>
        /// <param name="name">The Intensity Technique name</param>
        /// <param name="abbreviation">The Intensity Technique abbreviation</param>
        /// <param name="description">The Intensity Technique description</param>
        /// <param name="ownerId">The ID of the User which is adding the entry</param>
        /// <param name="isLinkingTechnique">Wether the Intensity Technique links two or more work components</param>
        /// <param name="id">The ID of the Intensity Technique</param>
        /// <returns>A new IntensityTechnique instance</returns>
        public static IntensityTechniqueRoot CreatePrivateIntensityTechnique(uint? id, uint? ownerId, string name, string abbreviation, PersonalNoteValue description, bool isLinkingTechnique)

            => CreateIntensityTechnique(id, ownerId, name, abbreviation, description, isLinkingTechnique, EntryStatusTypeEnum.Private);


        /// <summary>
        /// Factory method for native Intensity Technqiue entries as pending before approval
        /// </summary>
        /// <param name="name">The Intensity Technique name</param>
        /// <param name="abbreviation">The Intensity Technique abbreviation</param>
        /// <param name="description">The Intensity Technique description</param>
        /// <param name="ownerId">The ID of the User which is adding the entry</param>
        /// <param name="isLinkingTechnique">Wether the Intensity Technique links two or more work components</param>
        /// <param name="id">The ID of the Intensity Technique</param>
        /// <returns>A new IntensityTechnique instance</returns>
        public static IntensityTechniqueRoot CreateNativeIntensityTechnique(uint? id, string name, string abbreviation, PersonalNoteValue description, bool isLinkingTechnique)

            => CreateIntensityTechnique(id, null, name, abbreviation, description, isLinkingTechnique, EntryStatusTypeEnum.Native);


        /// <summary>
        /// Factory method for public Intensity Technqiue entries as pending before approval
        /// </summary>
        /// <param name="name">The Intensity Technique name</param>
        /// <param name="abbreviation">The Intensity Technique abbreviation</param>
        /// <param name="description">The Intensity Technique description</param>
        /// <param name="ownerId">The ID of the User which is adding the entry</param>
        /// <param name="isLinkingTechnique">Wether the Intensity Technique links two or more work components</param>
        /// <param name="id">The ID of the Intensity Technique</param>
        /// <returns>A new IntensityTechnique instance</returns>
        public static IntensityTechniqueRoot CreatePublicIntensityTechnique(uint? id, uint? ownerId, string name, string abbreviation, PersonalNoteValue description, bool isLinkingTechnique)

            => CreateIntensityTechnique(id, ownerId, name, abbreviation, description, isLinkingTechnique, EntryStatusTypeEnum.Pending);

        #endregion



        #region Business Methods


        //public void Rename(string newName)
        //{
        //    Name = newName?.Trim() ?? string.Empty;
        //    TestBusinessRules();
        //}

        /// <summary>
        /// Attach a description to the Intensity Technique
        /// </summary>
        /// <param name="description"></param>
        public void AttachDescription(PersonalNoteValue description)

            => Description = description;



        #endregion


        #region Business Rules Validation

        /// <summary>
        /// The Intensity Technique must have a valid name.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NameIsMandatory() => !string.IsNullOrWhiteSpace(Name);


        /// <summary>
        /// The Intensity Technique must have a valid abbreviation.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool AbbreviationIsMandatory() => !string.IsNullOrWhiteSpace(Abbreviation);


        /// <summary>
        /// The Intensity Technique must be linked to its Owner.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool OwnerIsMandatory() => OwnerId != null;



        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        protected void TestBusinessRules()
        {
            if (!NameIsMandatory())
                throw new TrainingDomainInvariantViolationException($"The Intensity Technique must have a valid name.");

            if (!AbbreviationIsMandatory())
                throw new TrainingDomainInvariantViolationException($"The Intensity Technique must have a valid abbreviation.");

            if (!OwnerIsMandatory())
                throw new TrainingDomainInvariantViolationException($"The Intensity Technique must be linked to its Owner.");

        }
        #endregion

    }
}
