using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;

namespace GymProject.Domain.TrainingDomain.AthleteAggregate
{
    public class UserTrainingPhaseRelation : ValueObject, ICloneable
    {



        // <summary>
        /// The Phase period
        /// </summary>
        //public DateRangeValue Period { get; private set; } = null;


        /// <summary>
        /// The Date the Phase begins
        /// </summary>
        public DateTime StartDate { get; private set; }


        /// <summary>
        /// The Date the Phase ends
        /// </summary>
        public DateTime? EndDate { get; private set; }


        /// <summary>
        /// The Owner's note
        /// </summary>
        public PersonalNoteValue OwnerNote { get; private set; } = null;


        /// <summary>
        /// The Entry visibility
        /// </summary>
        public EntryStatusTypeEnum EntryStatus { get; private set; } = null;


        /// <summary>
        /// FK to the Phase entry
        /// </summary>
        public uint? PhaseId{ get; private set; } = null;




        #region Ctors

        private UserTrainingPhaseRelation()
        {

        }

        private UserTrainingPhaseRelation(uint? phaseId, DateTime startDate, DateTime? endDate, EntryStatusTypeEnum entryStatus, PersonalNoteValue note) 
        {
            PhaseId = phaseId;

            StartDate = startDate;
            EndDate = endDate;
            OwnerNote = note;
            EntryStatus = entryStatus;


            if (PhaseId == null)
                throw new TrainingDomainInvariantViolationException($"Cannot create a UserPhase object without a Phase linked to it");

            TestBusinessRules();
        }

        #endregion



        #region Factories


        /// <summary>
        /// Factory method - INTERNAL: the caller should choose among the allowed Statuses by calling the proper factory
        /// </summary>
        /// <param name="startDate">The starting date</param>
        /// <param name="endDate">The end date</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="entryStatus">The status of the phase entry</param>
        /// <param name="trainerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        internal static UserTrainingPhaseRelation PlanPhase(uint? phaseId, DateTime startDate, DateTime? endDate, EntryStatusTypeEnum entryStatus, PersonalNoteValue trainerNote)

            => new UserTrainingPhaseRelation(phaseId, startDate, endDate, entryStatus, trainerNote);


        /// <summary>
        /// Factory method - INTERNAL: the caller should choose among the allowed Statuses by calling the proper factory
        /// </summary>
        /// <param name="startingFrom">The starting date</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="entryStatus">The status of the phase entry</param>
        /// <param name="trainerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        internal static UserTrainingPhaseRelation StartPhase(uint? phaseId, DateTime startingFrom, EntryStatusTypeEnum entryStatus, PersonalNoteValue trainerNote)

            => new UserTrainingPhaseRelation(phaseId, startingFrom, null, entryStatus, trainerNote);


        /// <summary>
        /// Factory method for starting a public Phase
        /// </summary>
        /// <param name="startingFrom">The starting date</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserTrainingPhaseRelation StartPhasePublic(uint? phaseId, DateTime startingFrom, PersonalNoteValue ownerNote = null)

            => PlanPhase(phaseId, startingFrom, null, EntryStatusTypeEnum.Pending, ownerNote);


        /// <summary>
        /// Factory method for starting a private Phase
        /// </summary>
        /// <param name="startingFrom">The starting date</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserTrainingPhaseRelation StartPhasePrivate(uint? phaseId, DateTime startingFrom, PersonalNoteValue ownerNote = null)

            => PlanPhase(phaseId, startingFrom, null, EntryStatusTypeEnum.Private, ownerNote);


        /// <summary>
        /// Factory method for planning a public Phase
        /// </summary>
        /// <param name="startDate">The starting date</param>
        /// <param name="endDate">The end date</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserTrainingPhaseRelation PlanPhasePublic(uint? phaseId, DateTime startDate, DateTime? endDate, PersonalNoteValue ownerNote = null)

            => PlanPhase(phaseId, startDate, endDate, EntryStatusTypeEnum.Pending, ownerNote);


        /// <summary>
        /// Factory method for planning a private Phase
        /// </summary>
        /// <param name="startDate">The starting date</param>
        /// <param name="endDate">The end date</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserTrainingPhaseRelation PlanPhasePrivate(uint? phaseId, DateTime startDate, DateTime? endDate, PersonalNoteValue ownerNote = null)

            => PlanPhase(phaseId, startDate, endDate, EntryStatusTypeEnum.Private, ownerNote);

        #endregion



        #region Business Methods

        /// <summary>
        /// Close the Phase as a new one is started.
        /// The previous Phase level finishes the day before the current one
        /// </summary>
        public void Close() => EndDate = DateTime.UtcNow.AddDays(-1);


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
            StartDate = newStartDate;
            TestBusinessRules();
        }


        /// <summary>
        /// Change the status of this entry
        /// </summary>
        /// <param name="newStartDate">The new start date</param>
        public void ChangeStatus(EntryStatusTypeEnum status)

            => EntryStatus = status;

        #endregion


        #region Business Rules Validation

        private bool StartDateBeforeEndDate()

            => EndDate == null || StartDate < EndDate;


        private void TestBusinessRules()
        {
            if (!StartDateBeforeEndDate())
                throw new TrainingDomainInvariantViolationException($"Invalid chronological order: start date must preceed end date");
        }

        #endregion



        #region IClonable Implementation

        public object Clone()

            => PlanPhase(PhaseId, StartDate, EndDate, EntryStatus, OwnerNote);


        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return StartDate;
            yield return EndDate;
            yield return OwnerNote;
            yield return EntryStatus;
            yield return PhaseId;
        }
    }
}
