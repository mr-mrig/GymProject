using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.Utils;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate
{
    public class WorkingSetTemplate : Entity<IdTypeValue>, IWorkingSet, ICloneable
    {


        /// <summary>
        /// The progressive number of the working set
        /// </summary>
        public uint ProgressiveNumber { get; private set; } = 0;


        /// <summary>
        /// The repetitions number which the working set is planned of - Mandatory value
        /// </summary>
        public WSRepetitionValue Repetitions { get; private set; } = null;


        /// <summary>
        /// The rest between the working set and the following - Optional
        /// </summary>
        public RestPeriodValue Rest { get; private set; } = null;


        /// <summary>
        /// The cadence of the working set - Optional
        /// </summary>
        public TUTValue Tempo { get; private set; } = null;


        /// <summary>
        /// The effort of the working set - Optional
        /// </summary>
        public TrainingEffortValue Effort { get; private set; } = null;


        private IList<IdTypeValue> _intensityTechniquesIds = new List<IdTypeValue>();

        /// <summary>
        /// FK to the Intensity Techniques - Optional
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<IdTypeValue> IntensityTechniqueIds
        {
            get => _intensityTechniquesIds?.ToList().AsReadOnly() ?? new List<IdTypeValue>().AsReadOnly();
        }





        #region Ctors

        private WorkingSetTemplate(IdTypeValue id, uint progressiveNumber, WSRepetitionValue repetitions, RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null, IList<IdTypeValue> intensityTechniqueIds = null) : base(id)
        {
            ProgressiveNumber = progressiveNumber;
            Repetitions = repetitions;
            Rest = rest ?? RestPeriodValue.SetRestNotSpecified();
            Tempo = tempo ?? TUTValue.SetGenericTUT();
            Effort = effort?? TrainingEffortValue.DefaultEffort;

            //_intensityTechniqueIds = intensityTechniqueIds?.NoDuplicatesClone().ToList() ?? new List<IdTypeValue>();
            _intensityTechniquesIds = CommonUtilities.RemoveDuplicatesFrom(intensityTechniqueIds)?.ToList() ?? new List<IdTypeValue>();

            TestBusinessRules();
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method - Creates a transient WS
        /// </summary>
        /// <param name="effort">The effort of the WS</param>
        /// <param name="progressiveNumber">The progressive number of the WS</param>
        /// <param name="repetitions">The target repetitions</param>
        /// <param name="rest">The rest period before the next WS</param>
        /// <param name="tempo">The lifting tempo of the WS</param>
        /// <param name="intensityTechniqueIds">The list of the intensity techniques to be applied</param>
        /// <returns>The WorkingSetTemplate instance</returns>
        public static WorkingSetTemplate PlanTransientWorkingSet(uint progressiveNumber, WSRepetitionValue repetitions, RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null, IList<IdTypeValue> intensityTechniqueIds = null)

            => new WorkingSetTemplate(null, progressiveNumber, repetitions, rest, effort, tempo, intensityTechniqueIds);

        /// <summary>
        /// Factory method - Loads a WS with the specified ID
        /// </summary>
        /// <param name="id">The WorkingSet ID</param>
        /// <param name="effort">The effort of the WS</param>
        /// <param name="progressiveNumber">The progressive number of the WS</param>
        /// <param name="repetitions">The target repetitions</param>
        /// <param name="rest">The rest period before the next WS</param>
        /// <param name="tempo">The lifting tempo of the WS</param>
        /// <param name="intensityTechniqueIds">The list of the intensity techniques to be applied</param>
        /// <returns>The WorkingSetTemplate instance</returns>
        public static WorkingSetTemplate PlanWorkingSet(IdTypeValue id, uint progressiveNumber, WSRepetitionValue repetitions, RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null, IList<IdTypeValue> intensityTechniqueIds = null)

            => new WorkingSetTemplate(id, progressiveNumber, repetitions, rest, effort, tempo, intensityTechniqueIds);

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
        public void ChangeRepetitions(WSRepetitionValue newReps)
        {
            Repetitions = newReps;
            TestBusinessRules();
        }


        /// <summary>
        /// Change the effort
        /// </summary>
        /// <param name="newEffort">The new value</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void ChangeEffort(TrainingEffortValue newEffort)
        {
            Effort = newEffort ?? TrainingEffortValue.DefaultEffort;
            TestBusinessRules();
        }


        /// <summary>
        /// Change the rest period
        /// </summary>
        /// <param name="newRest">The new value</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void ChangeRestPeriod(RestPeriodValue newRest)
        {
            Rest = newRest;
            TestBusinessRules();
        }


        /// <summary>
        /// Change the lifting tempo
        /// </summary>
        /// <param name="newReps">The new value</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void ChangeLiftingTempo(TUTValue newTempo)
        {
            Tempo = newTempo;
            TestBusinessRules();
        }


        /// <summary>
        /// Add an intensity technique - Do nothing if already present in the list
        /// </summary>
        /// <param name="toAddId">The id to be added</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="ArgumentNullException">If the ID is null</exception>
        public void AddIntensityTechnique(IdTypeValue toAddId)
        {
            if (toAddId == null)
                throw new ArgumentNullException($"Intensity Technique ID must be valid when adding a technique to the Working Set", nameof(toAddId));

            if (_intensityTechniquesIds.Contains(toAddId))
                return;

            _intensityTechniquesIds.Add(toAddId);
            TestBusinessRules();
        }


        /// <summary>
        /// Remove an intensity technique 
        /// </summary>
        /// <param name="toRemoveId">The id to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void RemoveIntensityTechnique(IdTypeValue toRemoveId)
        {
            bool removed = _intensityTechniquesIds.Remove(toRemoveId);

            if(removed)
                TestBusinessRules();
        }


        /// <summary>
        /// Change the progressive number
        /// </summary>
        /// <param name="newNumber">The new value - PNums must be consecutive</param>
        public void MoveToNewProgressiveNumber(uint newNumber)
        {
            ProgressiveNumber = newNumber;
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
            WSRepetitionValue reps;

            if (Repetitions.WorkType == WSWorkTypeEnum.TimeBasedSerie)
                reps = WSRepetitionValue.TrackRepetitionSerie((uint)ToRepetitions());
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
            if(Repetitions.IsAMRAP())
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
        /// Get the duration of the WS in terms of TUT + rest [s]
        /// </summary>
        /// <returns>The number of seconds the WS requires</returns>
        public int ToTotalSeconds()
        {
            if(Rest.IsRestSpecified())
                return ToSecondsUnderTension() + Rest.Value;

            else
                return ToSecondsUnderTension() + RestPeriodValue.DefaultRestValue;
        }


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
        /// Working Set Intensity Techniques must be non NULL.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullIntensityTechniques() => _intensityTechniquesIds.All(x => x != null);


        /// <summary>
        /// The Target Repetitions must be specified.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool ValidRepetitionNumber() => Repetitions != null && Repetitions.IsValueSpecified();


        /// <summary>
        /// AMRAP requires the effort to be specified as Intensity Percentage or RM
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool ValidEffortWhenAMRAP()

            => !Repetitions.IsAMRAP() ||
                (Effort != null && 
                    (Effort.IsRM() || Effort.IsIntensityPercentage()));


        /// <summary>
        /// No duplicate intensity techniques are allowed.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoDuplicateIntensityTechniques()
        {
            if (_intensityTechniquesIds.Count < 2)
                return true;

            for(int idIndex = 0; idIndex < _intensityTechniquesIds.Count; idIndex++)
            {
                if (_intensityTechniquesIds.SkipWhile((x, i) => i <= idIndex).Contains(_intensityTechniquesIds[idIndex]))
                    return false;
            }
            return true;
        }
           //=> _intensityTechniqueIds.Count < 1 || _intensityTechniqueIds.Any(first => _intensityTechniqueIds.SkipWhile(second => second != first).Contains(first));


        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
            if (!NoNullIntensityTechniques())
                throw new TrainingDomainInvariantViolationException($"Working Set Intensity Techniques must be non NULL.");

            if (!ValidRepetitionNumber())
                throw new TrainingDomainInvariantViolationException($"The Target Repetitions must be specified.");

            if (!ValidEffortWhenAMRAP())
                throw new TrainingDomainInvariantViolationException($"AMRAP requires the effort to be specified as Intensity Percentage or RM.");

            if (!NoDuplicateIntensityTechniques())
                throw new TrainingDomainInvariantViolationException($"No duplicate intensity techniques are allowed.");
        }

        #endregion



        #region IClonable Implementation

        public object Clone()

            => PlanWorkingSet(Id, ProgressiveNumber, Repetitions, Rest, Effort, Tempo, _intensityTechniquesIds);

        #endregion
    }
}


