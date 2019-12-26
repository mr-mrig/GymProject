using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

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


        /// <summary>
        /// FK to the muscle which acts as the primary mover
        /// </summary>
        public uint? PrimaryMuscleId { get; private set; } = null;

        private IList<uint?> _secondaryMusclesIds;

        /// <summary>
        /// FK to the muscles which act as secondary movers
        /// </summary>
        public IReadOnlyCollection<uint?> SecondaryMusclesIds
            => _secondaryMusclesIds?.ToList()?.AsReadOnly() ?? new List<uint?>().AsReadOnly();




        #region Ctors

        private ExcerciseRoot() : base(null, null) { }


        private ExcerciseRoot(uint? id, string name, PersonalNoteValue description, uint? primaryMuscleId, IList<uint?> secondaryMusclesIds, EntryStatusTypeEnum entryStatus) : base(id, entryStatus)
        {
            Name = name?.Trim() ?? string.Empty;
            Description = description;
            //OwnerId = ownerId;

            PrimaryMuscleId = primaryMuscleId;
            _secondaryMusclesIds = secondaryMusclesIds ?? new List<uint?>();

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
        /// <param name="entryStatus">The specified entry status - Pending if not specified</param>
        /// <param name="primaryMuscleId">The ID of the muscle which acts as the primary mover</param>
        /// <param name="primaryMuscleId">The IDs of the muscles which act as secondary movers</param>
        /// <returns>The Excercise instance</returns>
        public static ExcerciseRoot AddToExcerciseLibrary
        (
            uint? id,
            string name,
            PersonalNoteValue description,
            uint? primaryMuscleId, 
            IList<uint?> secondaryMusclesIds = null,
            EntryStatusTypeEnum entryStatus = null
        )
            => new ExcerciseRoot(id, name, description, primaryMuscleId, secondaryMusclesIds, entryStatus ?? EntryStatusTypeEnum.Pending);

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
        /// The Excercise primary muscle cannot be a secondary mover at the same time.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool PrimaryMuscleIsNotSecondaryAlso()

            => !_secondaryMusclesIds.Contains(PrimaryMuscleId);



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

            if (!PrimaryMuscleIsNotSecondaryAlso())
                throw new TrainingDomainInvariantViolationException($"The Excercise primary muscle cannot be a secondary mover at the same time.");
        }

        #endregion


    }
}