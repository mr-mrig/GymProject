﻿//using GymProject.Domain.Base;
//using GymProject.Domain.SharedKernel;
//using GymProject.Domain.TrainingDomain.Common;
//using GymProject.Domain.TrainingDomain.Exceptions;
//using GymProject.Domain.Utils.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace GymProject.Domain.TrainingDomain.WorkoutSessionAggregate
//{
//    public class WorkUnitEntity : Entity<IdTypeValue>, ICloneable
//    {


//        /// <summary>
//        /// The progressive number of the work unit - Starts from 0
//        /// </summary>
//        public uint ProgressiveNumber { get; private set; } = 0;


//        /// <summary>
//        /// The rate the user gives to the Work Unit
//        /// </summary>
//        public RatingValue UserRating { get; private set; } = null;


//        /// <summary>
//        /// The training volume parameters, as the sum of the params of the single WSs
//        /// </summary>
//        public TrainingVolumeParametersValue TrainingVolume { get; private set; } = null;


//        private IList<WorkingSetEntity> _workingSets = new List<WorkingSetEntity>();

//        /// <summary>
//        /// The Working Sets belonging to the WU, sorted by Progressive Numbers.
//        /// Provides a value copy: the instance fields must be modified through the instance methods
//        /// </summary>
//        public IReadOnlyCollection<WorkingSetEntity> WorkingSets
//        {
//            get => _workingSets?.Clone().ToList().AsReadOnly()
//                ?? new List<WorkingSetEntity>().AsReadOnly();  // Objects are not referencally equal
//        }


//        /// <summary>
//        /// FK to the Excercise Aggregate
//        /// </summary>
//        public IdTypeValue ExcerciseId { get; private set; } = null;


//        /// <summary>
//        /// FK to the Work Unit Note Aggregate
//        /// </summary>
//        public IdTypeValue NoteId { get; private set; } = null;




//        #region Ctors


//        private WorkUnitEntity() : base(null) { }


//        private WorkUnitEntity(IdTypeValue id, uint progressiveNumber, IdTypeValue excerciseId, RatingValue rating,
//            IEnumerable<WorkingSetEntity> workingSets, IdTypeValue noteId = null) : base(id)
//        {
//            ProgressiveNumber = progressiveNumber;
//            NoteId = noteId;
//            ExcerciseId = excerciseId;
//            UserRating = rating;

//            _workingSets = workingSets?.Clone().ToList() ?? new List<WorkingSetEntity>();

//            TestBusinessRules();

//            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(workingSets);
//        }
//        #endregion



//        #region Factories

//        /// <summary>
//        /// Factory method - Creates a transient WU
//        /// </summary>
//        /// <param name="excerciseId">The ID of the excercise which the WU consists of</param>
//        /// <param name="progressiveNumber">The progressive number of the WS</param>
//        /// <param name="noteId">The ID of the user note</param>
//        /// <param name="workingSets">The working sets list - cannot be empty or null</param>
//        /// <param name="userRating">The rate the user gave to the Work Unit</param>
//        /// <returns>The WorkUnitEntity instance</returns>
//        public static WorkUnitEntity PlanTransientWorkUnit(uint progressiveNumber, IdTypeValue excerciseId, RatingValue userRating,
//            IEnumerable<WorkingSetEntity> workingSets, IdTypeValue noteId = null)

//            => new WorkUnitEntity(null, progressiveNumber, excerciseId, userRating, workingSets, noteId);


//        /// <summary>
//        /// Factory method - Loads a WU with the specified ID
//        /// </summary>
//        /// <param name="id">The Work Unit ID</param>
//        /// <param name="excerciseId">The ID of the excercise which the WU consists of</param>
//        /// <param name="progressiveNumber">The progressive number of the WS</param>
//        /// <param name="noteId">The ID of the user note</param>
//        /// <param name="workingSets">The working sets list - cannot be empty or null</param>
//        /// <param name="userRating">The rate the user gave to the Work Unit</param>
//        /// <returns>The WorkUnitEntity instance</returns>
//        public static WorkUnitEntity PlanWorkUnit(IdTypeValue id, uint progressiveNumber, IdTypeValue excerciseId, RatingValue userRating,
//            IEnumerable<WorkingSetEntity> workingSets, IdTypeValue noteId = null)

//            => new WorkUnitEntity(id, progressiveNumber, excerciseId, userRating, workingSets, noteId);

//        #endregion



//        #region Public Methods

//        /// <summary>
//        /// Attach a note to the WU, or repleace it if already present
//        /// </summary>
//        /// <param name="noteId">The ID of the note to be attached</param>
//        public void WriteNote(IdTypeValue noteId) => NoteId = noteId;


//        /// <summary>
//        /// Remove the Owner's note
//        /// </summary>
//        public void ClearNote() => NoteId = null;


//        /// <summary>
//        /// Assign an excercise to the Work Unit
//        /// </summary>
//        /// <param name="excerciseId">The ID of the excercise</param>
//        public void AssignExcercise(IdTypeValue excerciseId) => ExcerciseId = excerciseId;


//        /// <summary>
//        /// Get a copy of the Working Set with the progressive number specified or DEFAULT if not found
//        /// </summary>
//        /// <param name="pNum">The progressive number to be found</param>
//        /// <exception cref="InvalidOperationException">If more Working Sets with the specified Progressive Number</exception>
//        /// <returns>The WorkingSetTemplate object or DEFAULT if not found</returns>
//        public WorkingSetEntity CloneWorkingSet(uint pNum)

//            => FindWorkingSetOrDefault(pNum)?.Clone() as WorkingSetEntity;

//        #endregion


//        #region Working Sets Methods



//        /// <summary>
//        /// Add the Working Set to the Work Unit - copy not reference
//        /// </summary>
//        /// <param name="toAdd">The WS to be added</param>
//        /// <exception cref="ArgumentException">If Working Set already present</exception>
//        public void TrackWorkingSet(WorkingSetTemplateEntity toAdd)
//        {
//            WorkingSetTemplateEntity copy = toAdd.Clone() as WorkingSetTemplateEntity;

//            if (_workingSets.Contains(copy))
//                throw new ArgumentException("Trying to add a duplicate Working Set", nameof(toAdd));

//            foreach (IdTypeValue techniqueId in _intensityTechniquesIds)
//                copy.AddIntensityTechnique(techniqueId);

//            _workingSets.Add(copy);

//            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(_workingSets, GetMainEffortType());
//            TrainingVolume = TrainingVolume.AddWorkingSet(copy);
//            TrainingDensity = TrainingDensity.AddWorkingSet(copy);

//            TestBusinessRules();
//        }


//        /// <summary>
//        /// Add the Working Set to the Work Unit
//        /// </summary>
//        /// <param name="repetitions">The WS repetitions</param>
//        /// <param name="rest">The rest period between the WS and the following</param>
//        /// <param name="effort">The WS effort</param>
//        /// <param name="tempo">The WS lifting tempo</param>
//        /// <param name="intensityTechniqueIds">The ids of the WS intensity techniques</param>
//        public void TrackTransientWorkingSet(WSRepetitionsValue repetitions, RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null,
//            IEnumerable<IdTypeValue> intensityTechniqueIds = null)
//        {
//            List<IdTypeValue> localIntensityTechniqueIds = CommonUtilities.RemoveDuplicatesFrom(intensityTechniqueIds)?.ToList() ?? new List<IdTypeValue>();

//            // Apply the WU-wise intensity techniques - if any - to the added WS
//            if (_intensityTechniquesIds.Count > 0)
//                localIntensityTechniqueIds.AddRange(_intensityTechniquesIds.Select(x => x));

//            WorkingSetTemplateEntity toAdd = WorkingSetTemplateEntity.PlanTransientWorkingSet(
//                    BuildWorkingSetProgressiveNumber(),
//                    repetitions,
//                    rest,
//                    effort,
//                    tempo,
//                    localIntensityTechniqueIds
//                );

//            _workingSets.Add(toAdd);

//            //TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(_workingSets);
//            //TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(_workingSets);
//            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(_workingSets, GetMainEffortType());

//            TrainingVolume = TrainingVolume.AddWorkingSet(toAdd);
//            TrainingDensity = TrainingDensity.AddWorkingSet(toAdd);
//            //TrainingIntensity.AddWorkingSet(toAdd);

//            TestBusinessRules();
//        }


//        /// <summary>
//        /// Remove the Working Set from the Work Unit
//        /// </summary>
//        /// <param name="toRemove">The WS to be removed</param>
//        /// <exception cref="ArgumentException">If the WS could not be found</exception>
//        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
//        /// <returns>True if remove successful</returns>
//        public void UntrackWorkingSet(WorkingSetEntity toRemove)
//        {
//            if (toRemove == null)
//                return;

//            RemoveWorkingSet(toRemove.ProgressiveNumber);
//        }

//        /// <summary>
//        /// Remove the Working Set from the Work Unit
//        /// </summary>
//        /// <param name="progressiveNumber">The progressive number of the WS to be removed</param>
//        /// <exception cref="InvalidOperationException">If more Working Sets with the specified Progressive Number</exception>
//        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
//        public void UntrackWorkingSet(uint progressiveNumber)
//        {
//            WorkingSetTemplateEntity toBeRemoved = FindWorkingSet(progressiveNumber);

//            if (_workingSets.Remove(toBeRemoved))
//            {
//                TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(_workingSets, GetMainEffortType());
//                TrainingVolume = TrainingVolume.RemoveWorkingSet(toBeRemoved);
//                TrainingDensity = TrainingDensity.RemoveWorkingSet(toBeRemoved);

//                ForceConsecutiveWorkingSetsProgressiveNumbers(progressiveNumber);
//                TestBusinessRules();
//            }
//        }


//        /// <summary>
//        /// Change the repetitions
//        /// </summary>
//        /// <param name="newReps">The new target repetitions</param>
//        /// <param name="workingSetPnum">The Progressive Number of the WS to be changed</param>
//        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
//        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
//        public void TrackWorkingSetRepetitions(uint workingSetPnum, WSRepetitionsValue newReps)
//        {
//            WorkingSetEntity ws = FindWorkingSet(workingSetPnum);
//            ws.TrackRepetitions(newReps);
//        }

//        #endregion



//        #region Private Methods

//        /// <summary>
//        /// Build the next valid progressive number
//        /// To be used before adding the WS to the list
//        /// </summary>
//        /// <returns>The WS Progressive Number</returns>
//        private uint BuildWorkingSetProgressiveNumber() => (uint)_workingSets.Count();


//        /// <summary>
//        /// Find the Working Set with the progressive number specified
//        /// </summary>
//        /// <param name="pNum">The progressive number to be found</param>
//        /// <exception cref="InvalidOperationException">If more Working Sets with the specified Progressive Number</exception>
//        /// <returns>The WorkingSetTemplate object or DEFAULT if notfound</returns>
//        private WorkingSetEntity FindWorkingSetOrDefault(uint pNum)

//            => _workingSets.SingleOrDefault(x => x.ProgressiveNumber == pNum);


//        /// <summary>
//        /// Find the Working Set with the progressive number specified
//        /// </summary>
//        /// <param name="pNum">The progressive number to be found</param>
//        /// <exception cref="InvalidOperationException">If more Working Sets, or zero, with the specified Progressive Number</exception>
//        /// <returns>The WorkingSetTemplate object</returns>
//        private WorkingSetEntity FindWorkingSet(uint pNum)

//            => _workingSets.Single(x => x.ProgressiveNumber == pNum);

//        #endregion


//        #region Business Rules Validation

//        /// <summary>
//        /// The Work Unit musthave no NULL working sets.
//        /// </summary>
//        /// <returns>True if business rule is met</returns>
//        private bool NoNullWorkingSets() => _workingSets.All(x => x != null);


//        /// <summary>
//        /// The Work Unit must be linked to an excercise.
//        /// </summary>
//        /// <returns>True if business rule is met</returns>
//        private bool ExcerciseSpecified() => ExcerciseId != null;


//        /// <summary>
//        /// Cannot create a Work Unit without any WS 
//        /// </summary>
//        /// <returns>True if business rule is met</returns>
//        private bool AtLeastOneWorkingSet() => _workingSets?.Count > 0;


//        /// <summary>
//        /// Working Sets of the same Work Unit must have consecutive progressive numbers.
//        /// </summary>
//        /// <returns>True if business rule is met</returns>
//        private bool WorkingSetsWithConsecutiveProgressiveNumber()
//        {
//            if (_workingSets.Count == 0)
//                return true;

//            // Check the first element: the sequence must start from 0
//            if (_workingSets?.Count() == 1)
//            {
//                if (_workingSets.FirstOrDefault()?.ProgressiveNumber == 0)
//                    return true;
//                else
//                    return false;
//            }

//            // Look for non consecutive numbers - exclude the last one
//            foreach (int pnum in _workingSets.Where(x => x.ProgressiveNumber != _workingSets.Count() - 1)
//                .Select(x => x.ProgressiveNumber))
//            {
//                if (!_workingSets.Any(x => x.ProgressiveNumber == pnum + 1))
//                    return false;
//            }

//            return true;
//        }



//        /// <summary>
//        /// Tests that all the business rules are met and manages invalid states
//        /// </summary>
//        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
//        private void TestBusinessRules()
//        {
//            if (!NoNullWorkingSets())
//                throw new TrainingDomainInvariantViolationException($"The Work Unit must have no NULL working sets.");

//            if (!ExcerciseSpecified())
//                throw new TrainingDomainInvariantViolationException($"The Work Unit must be linked to an excercise.");

//            if (!AtLeastOneWorkingSet())
//                throw new TrainingDomainInvariantViolationException($"Cannot create a Work Unit without any WS.");

//            if (!WorkingSetsWithConsecutiveProgressiveNumber())
//                throw new TrainingDomainInvariantViolationException($"Working Sets of the same Work Unit must have consecutive progressive numbers.");
//        }

//        #endregion


//        #region IClonable Implementation

//        public object Clone()

//            => PlanWorkUnit(Id, ProgressiveNumber, ExcerciseId, UserRating, WorkingSets.ToList(), NoteId);

//        #endregion
//    }
//}