using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.SharedKernel;

namespace GymProject.Domain.TrainingDomain.TrainingPhaseAggregate
{
    public class TrainingPhase : StatusTrackingEntity<IdType>, IAggregateRoot
    {


        #region Consts
        #endregion



        /// <summary>
        /// The Diet Plan name
        /// </summary>
        public string Name { get; private set; } = null;


        ///// <summary>
        ///// The author of the diet plan
        ///// </summary>
        //public Owner Owner { get; private set; } = null;




        #region Ctors

        private TrainingPhase(string name, EntryStatusTypeEnum entryStatus)
        {
            Name = name ?? string.Empty;
            EntryStatusType = entryStatus;


            if(string.IsNullOrWhiteSpace(Name))
                throw new TrainingDomainInvariantViolationException($"Training Phase must have a tag name.");

            if (EntryStatusType == null || EntryStatusType.Equals(EntryStatusTypeEnum.NotSet))
                throw new TrainingDomainInvariantViolationException($"Cannot create a TrainingPhase object with an invalid entry status");
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method - PROTECTED
        /// </summary>
        /// <param name="name">The name of the phase</param>
        /// <param name="entryStatus">TThe specified entry status</param>
        /// <returns>A new TrainingPhase instance</returns>
        protected static TrainingPhase CreateTrainingPhase
        (
            string name,
            EntryStatusTypeEnum entryStatus
        )
            => new TrainingPhase(name, entryStatus);


        /// <summary>
        /// Factory method fro private Phases
        /// </summary>
        /// <param name="name">The name of the phase</param>
        /// <returns>A new TrainingPhase instance</returns>
        public static TrainingPhase CreatePrivateTrainingPhase(string name)

            => CreateTrainingPhase(name, EntryStatusTypeEnum.Private);


        /// <summary>
        /// Factory method fro native Phases
        /// </summary>
        /// <param name="name">The name of the phase</param>
        /// <returns>A new TrainingPhase instance</returns>
        public static TrainingPhase CreateNativeTrainingPhase(string name)

            => CreateTrainingPhase(name, EntryStatusTypeEnum.Native);


        /// <summary>
        /// Factory method fro public Phases as pending before approval
        /// </summary>
        /// <param name="name">The name of the phase</param>
        /// <returns>A new TrainingPhase instance</returns>
        public static TrainingPhase CreatePublicTrainingPhase(string name)

            => CreateTrainingPhase(name, EntryStatusTypeEnum.Pending);

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
