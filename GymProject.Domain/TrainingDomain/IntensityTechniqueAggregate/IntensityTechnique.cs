using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.SharedKernel;

namespace GymProject.Domain.TrainingDomain.IntensityTechniqueAggregate
{
    public class IntensityTechnique : StatusTrackingEntity<IdTypeValue>, IAggregateRoot
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


        public 





        #region Ctors

        private IntensityTechnique(string name, EntryStatusTypeEnum entryStatus) : base(null)
        {
            Name = name ?? string.Empty;
            EntryStatusType = entryStatus;


            if (string.IsNullOrWhiteSpace(Name))
                throw new TrainingDomainInvariantViolationException($"Training Phase must have a tag name.");

            if (EntryStatusType == null || EntryStatusType.Equals(EntryStatusTypeEnum.NotSet))
                throw new TrainingDomainInvariantViolationException($"Cannot create a IntensityTechnique object with an invalid entry status");
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method - PROTECTED
        /// </summary>
        /// <param name="name">The name of the phase</param>
        /// <param name="entryStatus">TThe specified entry status</param>
        /// <returns>A new IntensityTechnique instance</returns>
        protected static IntensityTechnique CreateIntensityTechnique
        (
            string name,
            EntryStatusTypeEnum entryStatus
        )
            => new IntensityTechnique(name, entryStatus);


        /// <summary>
        /// Factory method fro private Phases
        /// </summary>
        /// <param name="name">The name of the phase</param>
        /// <returns>A new IntensityTechnique instance</returns>
        public static IntensityTechnique CreatePrivateIntensityTechnique(string name)

            => CreateIntensityTechnique(name, EntryStatusTypeEnum.Private);


        /// <summary>
        /// Factory method fro native Phases
        /// </summary>
        /// <param name="name">The name of the phase</param>
        /// <returns>A new IntensityTechnique instance</returns>
        public static IntensityTechnique CreateNativeIntensityTechnique(string name)

            => CreateIntensityTechnique(name, EntryStatusTypeEnum.Native);


        /// <summary>
        /// Factory method fro public Phases as pending before approval
        /// </summary>
        /// <param name="name">The name of the phase</param>
        /// <returns>A new IntensityTechnique instance</returns>
        public static IntensityTechnique CreatePublicIntensityTechnique(string name)

            => CreateIntensityTechnique(name, EntryStatusTypeEnum.Pending);

        #endregion



        #region Business Methods

        /// <summary>
        /// Set the Phase name
        /// </summary>
        /// <param name="newName">The new Phase name</param>
        public void Rename(string newName) => Name = newName ?? string.Empty;

        #endregion


        #region Private Methods

        ///// <summary>
        ///// Builds the DescriptiveNameValue from the name text
        ///// </summary>
        ///// <param name="nameText">The name text</param>
        ///// <returns>The DescriptiveNameValue object</returns>
        //private DescriptiveNameValue BuildName(string nameText) => DescriptiveNameValue.Write(nameText, TagMinimumLength, TagMaximumLength);
        #endregion

    }
}
