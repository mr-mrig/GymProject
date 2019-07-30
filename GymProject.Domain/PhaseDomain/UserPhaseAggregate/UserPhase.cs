using GymProject.Domain.Base;
using GymProject.Domain.PhaseDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using System;

namespace GymProject.Domain.PhaseDomain.UserPhaseAggregate
{
    public class UserPhase : StatusTrackingEntity<IdType>, IAggregateRoot
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
        public IdType PhaseId{ get; private set; } = null;




        #region Ctors

        private UserPhase(IdType phaseId, Owner owner, DateRangeValue period, PersonalNoteValue note)
        {
            PhaseId = phaseId;
            Owner = owner;
            Period = period;
            OwnerNote = note;

            CreatedOn = DateTime.UtcNow;

            if (PhaseId == null)
                throw new PhaseDomainInvariantViolationException($"Cannot create a UserPhase object without a Phase linked to it");

            if (Owner == null)
                throw new PhaseDomainInvariantViolationException($"Cannot create a UserPhase object without an owner");

            if (Period == null || !Period.IsLeftBounded())
                throw new PhaseDomainInvariantViolationException($"Cannot create a UserPhase object with an invalid period");
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method fro public Phases as pending before approval
        /// </summary>
        /// <param name="name">The name of the phase</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserPhase CreatePublicUserPhase(string name)

            => CreateUserPhase(name, EntryStatusTypeEnum.Pending);

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
