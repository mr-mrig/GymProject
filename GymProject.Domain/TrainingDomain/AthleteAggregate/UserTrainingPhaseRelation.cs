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
        public DateRangeValue Period { get; private set; } = null;


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

        private UserTrainingPhaseRelation(uint? phaseId, DateRangeValue period, EntryStatusTypeEnum entryStatus, PersonalNoteValue note) 
        {
            PhaseId = phaseId;

            Period = period;
            OwnerNote = note;
            EntryStatus = entryStatus;


            if (PhaseId == null)
                throw new TrainingDomainInvariantViolationException($"Cannot create a UserPhase object without a Phase linked to it");

            if (Period == null || !Period.IsLeftBounded())
                throw new TrainingDomainInvariantViolationException($"Cannot create a UserPhase object with an invalid period");
        }

        #endregion



        #region Factories


        /// <summary>
        /// Factory method - INTERNAL: the caller should choose among the allowed Statuses by calling the proper factory
        /// </summary>
        /// <param name="period">The phase period</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="athlete">The Athlete which the phase refers to</param>
        /// <param name="entryStatus">The status of the phase entry</param>
        /// <param name="trainerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        internal static UserTrainingPhaseRelation PlanPhase(uint? phaseId, DateRangeValue period, EntryStatusTypeEnum entryStatus, PersonalNoteValue trainerNote)

            => new UserTrainingPhaseRelation(phaseId, period, entryStatus, trainerNote);


        /// <summary>
        /// Factory method - INTERNAL: the caller should choose among the allowed Statuses by calling the proper factory
        /// </summary>
        /// <param name="startingFrom">The starting date</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="trainer">The one who set the Phase</param>
        /// <param name="entryStatus">The status of the phase entry</param>
        /// <param name="trainerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        internal static UserTrainingPhaseRelation StartPhase(uint? phaseId, DateTime startingFrom, EntryStatusTypeEnum entryStatus, PersonalNoteValue trainerNote)

            => new UserTrainingPhaseRelation(phaseId, DateRangeValue.RangeStartingFrom(startingFrom), entryStatus, trainerNote);


        /// <summary>
        /// Factory method for starting a public Phase
        /// </summary>
        /// <param name="startingFrom">The starting date</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="owner">The one who set the Phase</param>
        /// <param name="athlete">The Athlete which the phase refers to</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserTrainingPhaseRelation StartPhasePublic(uint? phaseId, DateTime startingFrom, PersonalNoteValue ownerNote = null)

            => PlanPhase(phaseId, DateRangeValue.RangeStartingFrom(startingFrom), EntryStatusTypeEnum.Pending, ownerNote);


        /// <summary>
        /// Factory method for starting a private Phase
        /// </summary>
        /// <param name="startingFrom">The starting date</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="owner">The one who set the Phase</param>
        /// <param name="athlete">The Athlete which the phase refers to</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserTrainingPhaseRelation StartPhasePrivate(uint? phaseId, DateTime startingFrom, PersonalNoteValue ownerNote = null)

            => PlanPhase(phaseId, DateRangeValue.RangeStartingFrom(startingFrom), EntryStatusTypeEnum.Private, ownerNote);


        /// <summary>
        /// Factory method for planning a public Phase
        /// </summary>
        /// <param name="phasePeriod">The phase planned period</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="owner">The one who set the Phase</param>
        /// <param name="athlete">The Athlete which the phase refers to</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserTrainingPhaseRelation PlanPhasePublic(uint? phaseId, DateRangeValue phasePeriod, PersonalNoteValue ownerNote = null)

            => PlanPhase(phaseId, phasePeriod, EntryStatusTypeEnum.Pending, ownerNote);


        /// <summary>
        /// Factory method for planning a private Phase
        /// </summary>
        /// <param name="phasePeriod">The Phase planned period</param>
        /// <param name="phaseId">The ID of the phase to be started</param>
        /// <param name="owner">The one who set the Phase</param>
        /// <param name="athlete">The Athlete which the phase refers to</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserTrainingPhaseRelation PlanPhasePrivate(uint? phaseId, DateRangeValue phasePeriod, PersonalNoteValue ownerNote = null)

            => PlanPhase(phaseId, phasePeriod, EntryStatusTypeEnum.Private, ownerNote);

        #endregion



        #region Business Methods

        /// <summary>
        /// Close the Phase as a new one is started.
        /// The previous Phase level finishes the day before the current one
        /// </summary>
        public void Close() => Period = DateRangeValue.RangeBetween(Period.Start, DateTime.UtcNow.AddDays(-1));


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


        /// <summary>
        /// Change the status of this entry
        /// </summary>
        /// <param name="newStartDate">The new start date</param>
        public void ChangeStatus(EntryStatusTypeEnum status)

            => EntryStatus = status;

        #endregion



        #region IClonable Implementation

        public object Clone()

            => PlanPhase(PhaseId, Period, EntryStatus, OwnerNote);


        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Period;
            yield return OwnerNote;
            yield return EntryStatus;
            yield return PhaseId;
        }
    }
}
