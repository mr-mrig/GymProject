using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.WorkoutSessionAggregate
{
    public class WorkingSetEntity : Entity<IdTypeValue>, IWorkingSet, ICloneable
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
        /// The effort of the working set - Optional
        /// </summary>
        public WeightPlatesValue Load { get; private set; } = null;


        public IdTypeValue NoteId { get; private set;  } = null;




        #region Ctors

        private WorkingSetEntity(IdTypeValue id, uint progressiveNumber, WSRepetitionsValue repetitions, WeightPlatesValue load = null, IEnumerable<IdTypeValue> trainingNotesIds = null) : base(id)
        {
            ProgressiveNumber = progressiveNumber;
            Repetitions = repetitions;
            Load = load;

            _trainingNotesIds = trainingNotesIds.ToList() ?? new List<IdTypeValue>();

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
        /// <param name="repetitions">The repetitions performed</param>
        /// <returns>The WorkingSetTemplate instance</returns>
        public static WorkingSetEntity TrackTransientWorkingSet(uint progressiveNumber, WSRepetitionsValue repetitions, WeightPlatesValue load = null, IEnumerable<IdTypeValue> trainingNotesIds = null)

            => TrackWorkingSet(null, progressiveNumber, repetitions, load, trainingNotesIds);

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="load">The lifted load - Might be null according to the performance type</param>
        /// <param name="progressiveNumber">The progressive number of the WS</param>
        /// <param name="repetitions">The repetitions performed</param>
        /// <param name="id">The ID of the Working Set</param>
        /// <returns>The WorkingSetTemplate instance</returns>
        public static WorkingSetEntity TrackWorkingSet(IdTypeValue id, uint progressiveNumber, WSRepetitionsValue repetitions, WeightPlatesValue load = null, IEnumerable<IdTypeValue> trainingNotesIds = null)

            => new WorkingSetEntity(id, progressiveNumber, repetitions, load, trainingNotesIds);

        #endregion



        #region Public Methods

        /// <summary>
        /// Check whether the WS ia an AMRAP one
        /// </summary>
        /// <returns>True if AMRAP</returns>
        public bool IsAMRAP() => Repetitions?.IsAMRAP() == true;


        /// <summary>
        /// Change the repetitions
        /// </summary>
        /// <param name="newReps">The new target repetitions</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void ChangeRepetitions(WSRepetitionsValue newReps)
        {
            Repetitions = newReps;
            TestBusinessRules();
        }


        /// <summary>
        /// Attach a personal note
        /// </summary>
        /// <param name="noteId">The id of the note to be added</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void AttachNote(IdTypeValue noteId)
        {
            bool removed = _trainingNotesIds.Add(noteId);

            if (removed)
                TestBusinessRules();
        }


        /// <summary>
        /// Remove a personal note 
        /// </summary>
        /// <param name="noteId">The id of the note to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void DetachNote(IdTypeValue noteId)
        {
            bool removed = _trainingNotesIds.Remove(noteId);

            if (removed)
                TestBusinessRules();
        }

        #endregion



        #region Conversions

        /// <summary>
        /// Perform a check on repetitions and effort specified to find for possible typos.
        /// IE: 10 reps @ 90% or 10reps @ 15RM might be an input mistake
        /// </summary>
        /// <returns>True if the working set is meaningful, false if it might have an error</returns>
        public bool IsEffortVsRepetitionsConsistent()
        {

            // If effort not specified or AMRAP WS return true
            if (Effort == null || Repetitions.IsAMRAP())
                return true;

            switch (Effort)
            {

                case var _ when Effort.IsIntensityPercentage():

                    TrainingEffortValue rmConverted = Effort.ToRm(Repetitions);   // Get maximum repetitions with that intensity
                    return ToRepetitions() <= rmConverted.Value;

                case var _ when Effort.IsRM():

                    return ToRepetitions() <= Effort.Value;


                case var _ when Effort.IsRPE():

                    return true;    // RPE always OK

                default:
                    return true;
            }
        }


        /// <summary>
        /// Changes the WS effort type to the one specified, with respect to the target repetitions.
        /// <param name="toEffortType">The effort type to convert to</param>
        /// </summary>
        public void ToNewEffortType(TrainingEffortTypeEnum toEffortType)
        {

            // Check for non-meaningful conversion
            if (Effort?.EffortType == null || toEffortType == Effort.EffortType)
                return;

            // Timed serie needs to be converted to the repetition equivalent
            WSRepetitionsValue reps;

            if (Repetitions.WorkType == WSWorkTypeEnum.TimeBasedSerie)
                reps = WSRepetitionsValue.TrackRepetitionSerie((uint)ToRepetitions());
            else
                reps = Repetitions;


            switch (toEffortType)
            {

                case var _ when toEffortType == TrainingEffortTypeEnum.IntensityPerc:

                    Effort = Effort.ToIntensityPercentage(reps);
                    break;


                case var _ when toEffortType == TrainingEffortTypeEnum.RM:

                    Effort = Effort.ToRm(reps);
                    break;

                case var _ when toEffortType == TrainingEffortTypeEnum.RPE:

                    Effort = Effort.ToRPE(reps);
                    break;
            }
        }


        /// <summary>
        /// Get the duration of the WS [s]
        /// </summary>
        /// <returns>The number of seconds under tension</returns>
        public int ToSecondsUnderTension()
        {
            // Unable to get the time
            if (!Repetitions.IsValueSpecified())
                return 0;

            // If Max reps, convert to RM to find the repetitions, then compute the time
            if (Repetitions.IsAMRAP())
            {
                if (Effort != null &&
                    (Effort.IsIntensityPercentage() || Effort.IsRM()))

                    return Tempo.ToSeconds() * (int)Effort.ToRm(Repetitions).Value;
                else
                    return 0;
            }

            // No conversion required
            if (Repetitions.IsTimedBasedSerie())
                return Repetitions.Value;

            return Tempo.ToSeconds() * Repetitions.Value;
        }


        /// <summary>
        /// Get the rest interval between the set and the following one [s]
        /// </summary>
        /// <returns>The rest period</returns>
        public int ToRest()

            => Rest.IsRestSpecified() ? Rest.Value : RestPeriodValue.DefaultRestValue;


        /// <summary>
        /// Get the duration of the WS in terms of TUT + rest [s]
        /// </summary>
        /// <returns>The number of seconds the WS requires</returns>
        public int ToTotalSeconds()

            => ToSecondsUnderTension() + ToRest();


        /// <summary>
        /// Get the number of repetitions
        /// </summary>
        /// <returns>The number of repetitions</returns>
        public int ToRepetitions()
        {
            // Unable to get the repetitions
            if (!Repetitions.IsValueSpecified())
                return 0;

            // If Max reps, convert to RM to find the repetitions, then compute the time
            if (Repetitions.IsAMRAP())
            {
                if (Effort != null && (Effort.IsIntensityPercentage() || Effort.IsRM()))
                    return (int)Effort.ToRm(Repetitions).Value;
                else
                    return 0;
            }

            // No conversion required
            if (Repetitions.IsRepetitionBasedSerie())
                return Repetitions.Value;

            return Repetitions.Value / Tempo.ToSeconds();
        }


        /// <summary>
        /// Get the duration of the WS in terms of TUT + rest [s]
        /// </summary>
        /// <returns>The number of seconds the WS requires</returns>
        public WeightPlatesValue ToWorkload()

            => WeightPlatesValue.MeasureKilograms(0);       // Not implemented for WS Templates

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

            => TrackWorkingSet(Id, ProgressiveNumber, Repetitions, Load);

        #endregion
    }
}


