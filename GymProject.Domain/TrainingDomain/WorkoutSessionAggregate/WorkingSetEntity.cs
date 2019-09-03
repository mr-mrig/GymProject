using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.WorkoutSessionAggregate
{
    public class WorkingSetEntity : Entity<IdTypeValue>, ITrainingLoadTrackableSet, ICloneable
    {


        /// <summary>
        /// The progressive number of the working set
        /// </summary>
        public uint ProgressiveNumber { get; private set; } = 0;


        /// <summary>
        /// The number of repetitions performed - Mandatory 
        /// </summary>
        public WSRepetitionsValue Repetitions { get; private set; } = null;


        /// <summary>
        /// The effort of the working set - if applicable according to the work type
        /// </summary>
        public WeightPlatesValue Load { get; private set; } = null;


        public IdTypeValue NoteId { get; private set; } = null;





        #region Ctors

        private WorkingSetEntity() : base(null) {   }


        private WorkingSetEntity(IdTypeValue id, uint progressiveNumber, WSRepetitionsValue repetitions, WeightPlatesValue load = null, IdTypeValue trainingNoteId = null) : base(id)
        {
            ProgressiveNumber = progressiveNumber;
            Repetitions = repetitions;
            Load = load;
            NoteId = trainingNoteId;

            TestBusinessRules();
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method - Creates a transient Working Set
        /// </summary>
        /// <param name="load">The lifted load - Might be null according to the performance type</param>
        /// <param name="progressiveNumber">The progressive number of the WS</param>
        /// <param name="repetitions">The repetitions performed</param>
        /// <param name="load">The load used in the Working Set - if applicable according to the work type</param>
        /// <param name="trainingNoteId">The ID of the Working Set Note - if any</param>
        /// <returns>The WorkingSetTemplate instance</returns>
        public static WorkingSetEntity TrackTransientWorkingSet(uint progressiveNumber, WSRepetitionsValue repetitions, WeightPlatesValue load = null, IdTypeValue trainingNoteId = null)

            => TrackWorkingSet(null, progressiveNumber, repetitions, load, trainingNoteId);

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="load">The lifted load - Might be null according to the performance type</param>
        /// <param name="progressiveNumber">The progressive number of the WS</param>
        /// <param name="repetitions">The repetitions performed</param>
        /// <param name="id">The ID of the Working Set</param>
        /// <param name="trainingNoteId">The ID of the Working Set Note</param>
        /// <returns>The WorkingSetTemplate instance</returns>
        public static WorkingSetEntity TrackWorkingSet(IdTypeValue id, uint progressiveNumber, WSRepetitionsValue repetitions, WeightPlatesValue load = null, IdTypeValue trainingNoteId = null)

            => new WorkingSetEntity(id, progressiveNumber, repetitions, load, trainingNoteId);

        #endregion



        #region Public Methods

        /// <summary>
        /// Track the load used
        /// </summary>
        /// <param name="load">The load used in the WS</param>
        public void TrackLoad(WeightPlatesValue load) => Load = load;


        /// <summary>
        /// Track the repetitions performed
        /// </summary>
        /// <param name="repetitions">The repetitions performed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void TrackRepetitions(WSRepetitionsValue repetitions)
        {
            Repetitions = repetitions;
            TestBusinessRules();
        }

        /// <summary>
        /// Write the training note
        /// </summary>
        /// <param name="noteId">The id of the note to be added</param>
        public void WriteNote(IdTypeValue noteId) => NoteId = noteId;


        /// <summary>
        /// Remove the training note 
        /// </summary>
        public void ClearNote() => NoteId = null;

        #endregion


        #region Business Rules Validation

        /// <summary>
        /// The Number of Repetitions must be specified and can't be AMRAP.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool ValidRepetitionNumber() => Repetitions != null && Repetitions.IsValueSpecified() && !Repetitions.IsAMRAP();


        /// <summary>
        /// Test that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
            if (!ValidRepetitionNumber())
                throw new TrainingDomainInvariantViolationException($"The Number of Repetitions must be specified and can't be AMRAP.");
        }

        #endregion



        #region IClonable Implementation

        public object Clone()

            => TrackWorkingSet(Id, ProgressiveNumber, Repetitions, Load, NoteId);

        #endregion


        #region ITrainingLoadTrackableSet Implementation

        public int ToRepetitions() => Repetitions.Value;

        public WeightPlatesValue ToWorkload() => WeightPlatesValue.Measure(ToRepetitions() * Load.Value, Load.Unit);
        #endregion


    }
}


