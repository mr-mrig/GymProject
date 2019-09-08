using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using System.Collections.Generic;

namespace GymProject.Domain.TrainingDomain.TrainingPhaseAggregate
{
    public class TrainingPhaseRoot : StatusTrackingEntity<uint?>, IAggregateRoot
    {


        #region Consts
        #endregion



        /// <summary>
        /// The Diet Plan name
        /// </summary>
        public string Name { get; private set; } = null;


        ///// <summary>
        ///// The User who created the Phase entry
        ///// </summary>
        //public Owner Owner { get; private set; } = null;




        #region Ctors

        private TrainingPhaseRoot(string name, EntryStatusTypeEnum entryStatus) 
            : base(null, entryStatus ?? EntryStatusTypeEnum.NotSet)
        {
            Name = name?.Trim() ?? string.Empty;

            TestBusinessRules();
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method - PROTECTED
        /// </summary>
        /// <param name="name">The name of the phase</param>
        /// <param name="entryStatus">TThe specified entry status</param>
        /// <returns>A new TrainingPhase instance</returns>
        protected static TrainingPhaseRoot CreateTrainingPhase
        (
            string name,
            EntryStatusTypeEnum entryStatus
        )
            => new TrainingPhaseRoot(name, entryStatus);


        /// <summary>
        /// Factory method fro private Phases
        /// </summary>
        /// <param name="name">The name of the phase</param>
        /// <returns>A new TrainingPhase instance</returns>
        public static TrainingPhaseRoot CreatePrivateTrainingPhase(string name)

            => CreateTrainingPhase(name, EntryStatusTypeEnum.Private);


        /// <summary>
        /// Factory method fro native Phases
        /// </summary>
        /// <param name="name">The name of the phase</param>
        /// <returns>A new TrainingPhase instance</returns>
        public static TrainingPhaseRoot CreateNativeTrainingPhase(string name)

            => CreateTrainingPhase(name, EntryStatusTypeEnum.Native);


        /// <summary>
        /// Factory method fro public Phases as pending before approval
        /// </summary>
        /// <param name="name">The name of the phase</param>
        /// <returns>A new TrainingPhase instance</returns>
        public static TrainingPhaseRoot CreatePublicTrainingPhase(string name)

            => CreateTrainingPhase(name, EntryStatusTypeEnum.Pending);

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
        /// The Training Phase requires the Entry Status to be set.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool ValidEntryStatus() => EntryStatus != EntryStatusTypeEnum.NotSet;




        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        protected void TestBusinessRules()
        {
            if (!NameIsMandatory())
                throw new TrainingDomainInvariantViolationException($"The Training Phase must have a valid name.");

            if (!ValidEntryStatus())
                throw new TrainingDomainInvariantViolationException($"The Training Phase requires the Entry Status to be set.");

        }
        #endregion


    }
}
