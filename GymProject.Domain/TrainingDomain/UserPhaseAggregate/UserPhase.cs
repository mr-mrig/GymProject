using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using System;

namespace GymProject.Domain.TrainingDomain.UserPhaseAggregate
{
    public class UserPhase : StatusTrackingEntity<IdTypeValue>, IAggregateRoot
    {


 
        // <summary>
        /// The Phase period
        /// </summary>
        public DateRangeValue Period { get; private set; } = null;


        /// <summary>
        /// The Owner's note
        /// </summary>
        public PersonalNoteValue OwnerNote { get; private set; } = null;


        /// <summary>
        /// The entry creation date
        /// </summary>
        public DateTime CreatedOn{ get; private set; } = DateTime.MinValue;


        /// <summary>
        /// The Author of the Phase
        /// </summary>
        public Owner Owner { get; private set; } = null;


        /// <summary>
        /// FK to the Phase entry
        /// </summary>
        public IdTypeValue PhaseId{ get; private set; } = null;




        #region Ctors

        private UserPhase(IdTypeValue phaseId, Owner owner, DateRangeValue period, EntryStatusTypeEnum status, PersonalNoteValue note) : base(null)
        {
            PhaseId = phaseId;
            Owner = owner;
            Period = period;
            OwnerNote = note;
            EntryStatusType = status;

            CreatedOn = DateTime.UtcNow;

            if (PhaseId == null)
                throw new TrainingDomainInvariantViolationException($"Cannot create a UserPhase object without a Phase linked to it");

            if (Owner == null)
                throw new TrainingDomainInvariantViolationException($"Cannot create a UserPhase object without an owner");

            if (Period == null || !Period.IsLeftBounded())
                throw new TrainingDomainInvariantViolationException($"Cannot create a UserPhase object with an invalid period");
        }

        #endregion



        #region Factories


        /// <summary>
        /// Factory method - PROTECTED: the caller should choose among the allowed Statuses by calling the proper factory
        /// </summary>
        /// <param name="period">The phase period</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="trainer">The one who set the Phase</param>
        /// <param name="entryStatus">The status of the phase entry</param>
        /// <param name="trainerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        protected static UserPhase PlanPhase(IdTypeValue phaseId, Owner trainer, DateRangeValue period, EntryStatusTypeEnum entryStatus, PersonalNoteValue trainerNote)

            => new UserPhase(phaseId, trainer, period, entryStatus, trainerNote);


        /// <summary>
        /// Factory method for starting a public Phase
        /// </summary>
        /// <param name="startingFrom">The starting date</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="owner">The one who set the Phase</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserPhase StartPhasePublic(IdTypeValue phaseId, Owner owner, DateTime startingFrom, PersonalNoteValue ownerNote = null)

            => PlanPhase(phaseId, owner, DateRangeValue.RangeStartingFrom(startingFrom), EntryStatusTypeEnum.Pending, ownerNote);


        /// <summary>
        /// Factory method for starting a private Phase
        /// </summary>
        /// <param name="startingFrom">The starting date</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="owner">The one who set the Phase</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserPhase StartPhasePrivate(IdTypeValue phaseId, Owner owner, DateTime startingFrom, PersonalNoteValue ownerNote = null)

            => PlanPhase(phaseId, owner, DateRangeValue.RangeStartingFrom(startingFrom), EntryStatusTypeEnum.Private, ownerNote);


        /// <summary>
        /// Factory method for planning a public Phase
        /// </summary>
        /// <param name="phasePeriod">The phase planned period</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="owner">The one who set the Phase</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserPhase PlanPhasePublic(IdTypeValue phaseId, Owner owner, DateRangeValue phasePeriod, PersonalNoteValue ownerNote = null)

            => PlanPhase(phaseId, owner, phasePeriod, EntryStatusTypeEnum.Pending, ownerNote);


        /// <summary>
        /// Factory method for planning a private Phase
        /// </summary>
        /// <param name="phasePeriod">The Phase planned period</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="owner">The one who set the Phase</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserPhase PlanPhasePrivate(IdTypeValue phaseId, Owner owner, DateRangeValue phasePeriod, PersonalNoteValue ownerNote = null)

            => PlanPhase(phaseId, owner, phasePeriod, EntryStatusTypeEnum.Private, ownerNote);

        #endregion



        #region Business Methods

        /// <summary>
        /// Close the Phase as a new one is started.
        /// </summary>
        public void Close() => Period = DateRangeValue.RangeBetween(Period.Start, DateTime.Now);


        /// <summary>
        /// Attach the note of the owner
        /// </summary>
        /// <param name="newNote">The Owner's note</param>
        public void WriteNote(string newNote) => OwnerNote = PersonalNoteValue.Write(newNote);


        /// <summary>
        /// Change the start date
        /// </summary>
        /// <param name="newStartDate">The new start date</param>
        public void ShiftStartDate(DateTime newStartDate)
        {
            if (Period.IsRightBounded())
                Period = DateRangeValue.RangeBetween(newStartDate, Period.End);

            else
                Period = DateRangeValue.RangeStartingFrom(newStartDate);
        }

        #endregion

    }
}
