using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;

namespace GymProject.Domain.TrainingDomain.AthleteAggregate
{
    public class UserTrainingPhaseRelation : RelationEntity, ICloneable
    {



        // <summary>
        /// The Phase period
        /// </summary>
        //public DateRangeValue Period { get; private set; } = null;


        /// <summary>
        /// The Date the Phase begins as a plain date - no time
        /// </summary>
        public DateTime StartDate { get; private set; }


        /// <summary>
        /// The Date the Phase ends as a plain date - no time
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


        /// <summary>
        /// FK to the User/Athlete
        /// </summary>
        public uint? UserId { get; private set; } = null;       // Do not rename - EF core

        /// <summary>
        /// Navigation Property to the Athlete Aggregate Root
        /// </summary>
        public AthleteRoot Athlete { get; private set; } = null;



        #region Ctors

        private UserTrainingPhaseRelation()
        {

        }

        private UserTrainingPhaseRelation(uint? phaseId, AthleteRoot athlete, DateTime startDate, DateTime? endDate, EntryStatusTypeEnum entryStatus, PersonalNoteValue note)
        {
            PhaseId = phaseId;
            Athlete = athlete;
            UserId = athlete.Id;

            StartDate = startDate.Date;
            EndDate = endDate.HasValue ? endDate.Value.Date : endDate;
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
        internal static UserTrainingPhaseRelation PlanPhase(AthleteRoot athlete, uint? phaseId,  DateTime startDate, DateTime? endDate, EntryStatusTypeEnum entryStatus, PersonalNoteValue trainerNote)

            => new UserTrainingPhaseRelation(phaseId, athlete, startDate, endDate, entryStatus, trainerNote);


        /// <summary>
        /// Factory method - INTERNAL: the caller should choose among the allowed Statuses by calling the proper factory
        /// </summary>
        /// <param name="startingFrom">The starting date</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="entryStatus">The status of the phase entry</param>
        /// <param name="trainerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        internal static UserTrainingPhaseRelation StartPhase(AthleteRoot athlete, uint? phaseId, DateTime startingFrom, EntryStatusTypeEnum entryStatus, PersonalNoteValue trainerNote)

            => new UserTrainingPhaseRelation(phaseId, athlete, startingFrom, null, entryStatus, trainerNote);


        /// <summary>
        /// Factory method for starting a public Phase
        /// </summary>
        /// <param name="startingFrom">The starting date</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserTrainingPhaseRelation StartPhasePublic(AthleteRoot athlete, uint? phaseId, DateTime startingFrom, PersonalNoteValue ownerNote = null)

            => PlanPhase(athlete, phaseId, startingFrom, null, EntryStatusTypeEnum.Pending, ownerNote);


        /// <summary>
        /// Factory method for starting a private Phase
        /// </summary>
        /// <param name="startingFrom">The starting date</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserTrainingPhaseRelation StartPhasePrivate(AthleteRoot athlete, uint? phaseId, DateTime startingFrom, PersonalNoteValue ownerNote = null)

            => PlanPhase(athlete, phaseId, startingFrom, null, EntryStatusTypeEnum.Private, ownerNote);


        /// <summary>
        /// Factory method for planning a public Phase
        /// </summary>
        /// <param name="startDate">The starting date</param>
        /// <param name="endDate">The end date</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserTrainingPhaseRelation PlanPhasePublic(AthleteRoot athlete, uint? phaseId, DateTime startDate, DateTime? endDate, PersonalNoteValue ownerNote = null)

            => PlanPhase(athlete, phaseId, startDate, endDate, EntryStatusTypeEnum.Pending, ownerNote);


        /// <summary>
        /// Factory method for planning a private Phase
        /// </summary>
        /// <param name="startDate">The starting date</param>
        /// <param name="endDate">The end date</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserTrainingPhaseRelation PlanPhasePrivate(AthleteRoot athlete, uint? phaseId, DateTime startDate, DateTime? endDate, PersonalNoteValue ownerNote = null)

            => PlanPhase(athlete, phaseId, startDate, endDate, EntryStatusTypeEnum.Private, ownerNote);

        #endregion



        #region Business Methods

        /// <summary>
        /// Close the Phase as a new one is started, by setting its finish date to the day before of the new one
        /// </summary>
        /// <param name="newPhaseStartDate">The starting datetime of the phase which has just been planned. It does not need to be a plain date.</param>
        public void Close(DateTime newPhaseStartDate)
        {
            EndDate = newPhaseStartDate.AddDays(-1).Date;       // This forces the phases to be contiguous, hence non option to leave gaps! Is this correct?
            TestBusinessRules();
        }

        /// <summary>
        /// Attach the note of the owner
        /// </summary>
        /// <param name="newNote">The Owner's note</param>
        public void WriteNote(string newNote) => OwnerNote = PersonalNoteValue.Write(newNote);


        /// <summary>
        /// Change the start date
        /// </summary>
        /// <param name="newStartDate">The new start datetime - the time part will be ignored.</param>
        public void ShiftStartDate(DateTime newStartDate)
        {
            StartDate = newStartDate.Date;
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

            => PlanPhase(Athlete, PhaseId, StartDate, EndDate, EntryStatus, OwnerNote);


        #endregion

        protected override IEnumerable<object> GetIdentifyingFields()
        {
            yield return UserId;
            yield return PhaseId;
            yield return StartDate;
        }
    }
}
