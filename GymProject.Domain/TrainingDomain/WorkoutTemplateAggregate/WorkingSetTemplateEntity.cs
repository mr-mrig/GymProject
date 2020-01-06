using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate
{
    public class WorkingSetTemplateEntity : Entity<uint?>, IFullWorkingSet, ICloneable
    {


        /// <summary>
        /// The progressive number of the working set
        /// </summary>
        public uint ProgressiveNumber { get; private set; } = 0;


        /// <summary>
        /// The repetitions number which the working set is planned of - Mandatory value
        /// </summary>
        public WSRepetitionsValue Repetitions { get; private set; } = null;


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


        ///// <summary>
        ///// The Working Set linked to this one, if any
        ///// </summary>
        //public LinkedWorkValue LinkedWorkingSet =>
        //    FindLinkingRelationOrDefault()?.GetLinkedOrDefault();


        private List<WorkingSetIntensityTechniqueRelation> _intensityTechniquesRelations = new List<WorkingSetIntensityTechniqueRelation>();

        /// <summary>
        /// FK to the Intensity Techniques
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<uint?> IntensityTechniqueIds
        {
            get => _intensityTechniquesRelations?.Select(x => x.IntensityTechniqueId).ToList().AsReadOnly() 
                ?? new List<uint?>().AsReadOnly();
        }


        //public IReadOnlyCollection<uint?> NonLinkingIntensityTechniqueIds

        //    => _intensityTechniquesRelations
        //            .Where(x => x.LinkedWorkingSetId == null)?.Select(x => x.IntensityTechniqueId).ToList().AsReadOnly()
        //        ?? new List<uint?>().AsReadOnly();




        #region Ctors

        private WorkingSetTemplateEntity() : base(null)
        {

        }


        private WorkingSetTemplateEntity(uint? id, uint progressiveNumber, WSRepetitionsValue repetitions, RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null, 
            IEnumerable<uint?> intensityTechniqueIds = null) : base(id)
        {
            ProgressiveNumber = progressiveNumber;
            Repetitions = repetitions;
            Rest = rest;// ?? RestPeriodValue.SetNotSpecifiedRest();
            Tempo = tempo;// ?? TUTValue.SetGenericTUT();
            Effort = effort; //?? TrainingEffortValue.DefaultEffort;

            _intensityTechniquesRelations = new List<WorkingSetIntensityTechniqueRelation>();

            foreach (uint? techniqueId in intensityTechniqueIds ?? new List<uint?>())
                _intensityTechniquesRelations.Add(WorkingSetIntensityTechniqueRelation.BuildLink(this, techniqueId));

            //_intensityTechniquesRelations.Add(WorkingSetIntensityTechniqueRelation.BuildLink(this, linkedWorkingSet.LinkingIntensityTechniqueId, linkedWorkingSet.LinkedWorkId));

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
        /// <returns>The WorkingSetTemplateEntity instance</returns>
        public static WorkingSetTemplateEntity PlanTransientWorkingSet(uint progressiveNumber, WSRepetitionsValue repetitions, RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null, 
            IEnumerable<uint?> intensityTechniqueIds = null)

            => PlanWorkingSet(null, progressiveNumber, repetitions, rest, effort, tempo, intensityTechniqueIds);

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
        /// <returns>The WorkingSetTemplateEntity instance</returns>
        public static WorkingSetTemplateEntity PlanWorkingSet(uint? id, uint progressiveNumber, WSRepetitionsValue repetitions, RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null, 
            IEnumerable<uint?> intensityTechniqueIds = null)

            => new WorkingSetTemplateEntity(id, progressiveNumber, repetitions, rest, effort, tempo, intensityTechniqueIds);

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
        public void ReviseRepetitions(WSRepetitionsValue newReps)
        {
            Repetitions = newReps;
            TestBusinessRules();
        }


        /// <summary>
        /// Change the effort
        /// </summary>
        /// <param name="newEffort">The new value</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void ReviseEffort(TrainingEffortValue newEffort)
        {
            Effort = newEffort ?? TrainingEffortValue.DefaultEffort;
            TestBusinessRules();
        }


        /// <summary>
        /// Change the rest period
        /// </summary>
        /// <param name="newRest">The new value</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void ReviseRestPeriod(RestPeriodValue newRest)
        {
            Rest = newRest;
            TestBusinessRules();
        }


        /// <summary>
        /// Change the lifting tempo
        /// </summary>
        /// <param name="newReps">The new value</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void ReviseLiftingTempo(TUTValue newTempo)
        {
            Tempo = newTempo;
            TestBusinessRules();
        }


        /// <summary>
        /// Reset the Intensity Technique IDs and assign the specified ones
        /// </summary>
        /// <param name="intensityTechniqueIds">The list of the intensity techinques IDs</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void AssignIntensityTechniques(IEnumerable<uint?> intensityTechniqueIds)
        {
            _intensityTechniquesRelations.Clear();

            foreach (uint? techniqueId in intensityTechniqueIds)
                _intensityTechniquesRelations.Add(
                    WorkingSetIntensityTechniqueRelation.BuildLink(this, techniqueId));

            TestBusinessRules();
        }


        /// <summary>
        /// Add an intensity technique - Do nothing if already present in the list
        /// </summary>
        /// <param name="intensityTechniqueId">The id to be added</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void AddIntensityTechnique(uint intensityTechniqueId)
        {
            if (HasIntensityTechnique(intensityTechniqueId))
                return;

            _intensityTechniquesRelations.Add(
                WorkingSetIntensityTechniqueRelation.BuildLink(this, intensityTechniqueId));

            TestBusinessRules();
        }


        /// <summary>
        /// Remove an intensity technique 
        /// </summary>
        /// <param name="intensityTechniqueId">The ID to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="ArgumentException">If the Working Set is not associated to the Intensity Technique </exception>
        public void RemoveIntensityTechnique(uint intensityTechniqueId)
        {
            if (!HasIntensityTechnique(intensityTechniqueId))
                return;

            if(_intensityTechniquesRelations.Remove(FindNonLinkingRelationOrDefault(intensityTechniqueId)))
                TestBusinessRules();
        }


        ///// <summary>
        ///// Link the Working Set to another one
        ///// </summary>
        ///// <param name="intensityTechniqueId">The ID of the Linking Intensity Technique</param>
        ///// <param name="linkedWorkingSetId">The ID of the Working Set to be linked</param>
        ///// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        ///// <exception cref="ArgumentException">If intensity technique already present</exception>
        ///// <exception cref="InvalidOperationException">If another link is present</exception>
        //public void LinkTo(uint linkedWorkingSetId, uint intensityTechniqueId)
        //{
        //    if(LinkedWorkingSet != null)
        //        throw new InvalidOperationException($"A working set can be linked to only another one.");

        //    if (HasIntensityTechnique(intensityTechniqueId))
        //        throw new ArgumentException($"The Intensity Technique has already been added", nameof(intensityTechniqueId));

        //    _intensityTechniquesRelations.Add(
        //        WorkingSetIntensityTechniqueRelation.BuildLink(this, intensityTechniqueId, linkedWorkingSetId));

        //    TestBusinessRules();
        //}


        ///// <summary>
        ///// Unlink the Working Set
        ///// </summary>
        ///// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        ///// <exception cref="ArgumentException">If intensity technique already present</exception>
        ///// <exception cref="InvalidOperationException">If another link is present</exception>
        //public void Unlink()
        //{
        //    WorkingSetIntensityTechniqueRelation linkingRelation = FindLinkingRelationOrDefault();

        //    if(_intensityTechniquesRelations.Remove(linkingRelation))
        //        TestBusinessRules();
        //}


        /// <summary>
        /// Change the progressive number
        /// </summary>
        /// <param name="newNumber">The new value - PNums must be consecutive</param>
        public void MoveToNewProgressiveNumber(uint newNumber)
        {
            ProgressiveNumber = newNumber;
        }


        /// <summary>
        /// Check whether the Working Set has the specified Intensity Technique associated
        /// </summary>
        /// <param name="intensityTechniqueId">The ID of the Intensity Technique to be checked for</param>
        /// <returns>True if the WS has the Intensity Technique</returns>
        public bool HasIntensityTechnique(uint intensityTechniqueId)

            => IntensityTechniqueIds.Count(x => x.Value == intensityTechniqueId) > 0;

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

                case var _ when toEffortType == TrainingEffortTypeEnum.IntensityPercentage:

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
        /// Get the effort of the WS. If no effort is specified, assume it is a 8RPE, converted as RM
        /// </summary>
        /// <returns>The effort</returns>
        public TrainingEffortValue ToEffort()

            => Effort != null ? Effort : TrainingEffortValue.AsRM(
                ToRepetitions() + 2);   // 2 reps in reserve


        /// <summary>
        /// Get the duration of the WS [s]
        /// </summary>
        /// <returns>The number of seconds under tension</returns>
        public int ToSecondsUnderTension()
        {
            // Unable to get the time
            if (!Repetitions.IsValueSpecified())
                return 0;

            //// If Max reps, convert to RM to find the repetitions, then compute the time
            //if(Repetitions.IsAMRAP())
            //{
            //    if (Effort != null && 
            //        (Effort.IsIntensityPercentage() || Effort.IsRM()))

            //        return (Tempo?.ToSeconds() ?? TUTValue.SetGenericTUT().ToSeconds()) * ToRepetitions();
            //    else
            //        return 0;
            //}

            // No conversion required
            if (Repetitions.IsTimedBasedSerie())
                return Repetitions.Value;

            return (Tempo?.ToSeconds() ?? TUTValue.SetGenericTUT().ToSeconds()) 
                * ToRepetitions();
        }


        /// <summary>
        /// Get the rest interval between the set and the following one [s]
        /// </summary>
        /// <returns>The rest period</returns>
        public int ToRest()

            => Rest != null && Rest.IsRestSpecified() ? Rest.Value : RestPeriodValue.DefaultRestValue;


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

            return Repetitions.Value / (Tempo?.ToSeconds()
                ?? TUTValue.SetGenericTUT().ToSeconds());
        }


        /// <summary>
        /// Get the duration of the WS in terms of TUT + rest [s]
        /// </summary>
        /// <returns>The number of seconds the WS requires</returns>
        public WeightPlatesValue ToWorkload()

            => WeightPlatesValue.MeasureKilograms(0);       // Not implemented for WS Templates

        #endregion


        #region Private Functions

        /// <summary>
        /// Get a reference to the Intensity Technique relation which represent a Link between to WSs
        /// </summary>
        /// <returns></returns>
        private WorkingSetIntensityTechniqueRelation FindLinkingRelationOrDefault()

            => _intensityTechniquesRelations.SingleOrDefault(x => x.LinkedWorkingSetId != null);


        /// <summary>
        /// Get a reference to the Intensity Technique relation which is not a linking one
        /// </summary>
        /// <param name="intensityTechniqueId">The ID of the Intensity Technique to be found</param>
        /// <returns></returns>
        private WorkingSetIntensityTechniqueRelation FindNonLinkingRelationOrDefault(uint intensityTechniqueId)

            => _intensityTechniquesRelations.SingleOrDefault(x => x.LinkedWorkingSetId == null && x.IntensityTechniqueId == intensityTechniqueId);

        #endregion


        #region Business Rules Validation

        /// <summary>
        /// Working Set Intensity Techniques must be non NULL.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullIntensityTechniqueRelations() => _intensityTechniquesRelations.All(x => x != null);


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

            => /*Repetitions == null ||*/
                !Repetitions.IsAMRAP() ||
                (Effort != null && 
                    (Effort.IsRM() || Effort.IsIntensityPercentage()));


        /// <summary>
        /// No duplicate intensity techniques are allowed.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoDuplicateIntensityTechniques()
        {
            if (_intensityTechniquesRelations.Count < 2)
                return true;

            for(int idIndex = 0; idIndex < _intensityTechniquesRelations.Count; idIndex++)
            {
                if (_intensityTechniquesRelations.SkipWhile((x, i) => i <= idIndex).Contains(_intensityTechniquesRelations[idIndex]))
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
            if (!NoNullIntensityTechniqueRelations())
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

            => PlanWorkingSet(Id, ProgressiveNumber, Repetitions, Rest, Effort, Tempo, IntensityTechniqueIds);

        #endregion
    }
}


