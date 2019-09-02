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
    public class WorkUnitTemplateEntity : Entity<IdTypeValue>, ICloneable
    {


        /// <summary>
        /// The progressive number of the work unit - Starts from 0
        /// </summary>
        public uint ProgressiveNumber { get; private set; } = 0;


        /// <summary>
        /// The training volume parameters, as the sum of the params of the single WSs
        /// </summary>
        public TrainingVolumeParametersValue TrainingVolume { get; private set; } = null;


        /// <summary>
        /// The training effort, as the average of the single WSs efforts
        /// </summary>
        public TrainingIntensityParametersValue TrainingIntensity { get; private set; } = null;


        /// <summary>
        /// The training density parameters, as the sum of the params of the single WSs
        /// </summary>
        public TrainingDensityParametersValue TrainingDensity { get; private set; } = null;


        private IList<WorkingSetTemplateEntity> _workingSets = new List<WorkingSetTemplateEntity>();

        /// <summary>
        /// The Working Sets belonging to the WU, sorted by Progressive Numbers.
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<WorkingSetTemplateEntity> WorkingSets
        {
            get => _workingSets?.Clone().ToList().AsReadOnly() 
                ?? new List<WorkingSetTemplateEntity>().AsReadOnly();  // Objects are not referencally equal
        }
        /// </summary>
        public IdTypeValue OwnerNoteId { get; private set; } = null;


        /// <summary>
        /// FK to the Excercise Aggregate
        /// </summary>
        public IdTypeValue ExcerciseId { get; private set; } = null;


        private ICollection<IdTypeValue> _intensityTechniquesIds = null;

        /// <summary>
        /// The list of the IDs of the WU-wise Intensity Techniques - Will be applied to all the WSs
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<IdTypeValue> IntensityTechniquesIds
        {
            get => _intensityTechniquesIds.ToList().AsReadOnly() ?? new List<IdTypeValue>().AsReadOnly();
        }



        #region Ctors

        private WorkUnitTemplateEntity(IdTypeValue id, uint progressiveNumber, IdTypeValue excerciseId, 
            IEnumerable<WorkingSetTemplateEntity> workingSets, IEnumerable<IdTypeValue> workUnitIntensityTechniqueIds, IdTypeValue ownerNoteId = null) : base(id)
        {
            ProgressiveNumber = progressiveNumber;
            OwnerNoteId = ownerNoteId;
            ExcerciseId = excerciseId;

            _workingSets = workingSets?.Clone().ToList() ?? new List<WorkingSetTemplateEntity>();

            //No duplicate intensity techniques
            //_intensityTechniquesIds = workUnitIntensityTechniqueIds?.NoDuplicatesClone().ToList() ?? new List<IdType>();
            //_intensityTechniquesIds = workUnitIntensityTechniqueIds?.NoDuplicatesClone().ToList() ?? new List<IdTypeValue>();
            _intensityTechniquesIds = CommonUtilities.RemoveDuplicatesFrom(workUnitIntensityTechniqueIds)?.ToList() ?? new List<IdTypeValue>();


            foreach (WorkingSetTemplateEntity ws in _workingSets)
                foreach (IdTypeValue intTechniqueId in _intensityTechniquesIds)
                    ws.AddIntensityTechnique(intTechniqueId);

            TestBusinessRules();

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(workingSets);
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(workingSets);
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(workingSets, GetMainEffortType());
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method - Creates a transient WU
        /// </summary>
        /// <param name="excerciseId">The ID of the excercise which the WU consists of</param>
        /// <param name="progressiveNumber">The progressive number of the WS</param>
        /// <param name="ownerNoteId">The ID of the Work Unit Owner's note</param>
        /// <param name="workingSets">The working sets list - cannot be empty or null</param>
        /// <param name="workUnitIntensityTechniqueIds">The list of the IDs of the WU Intensity Techniques</param>
        /// <returns>The WorkUnitTemplate instance</returns>
        public static WorkUnitTemplateEntity PlanTransientWorkUnit(uint progressiveNumber, IdTypeValue excerciseId, 
            IEnumerable<WorkingSetTemplateEntity> workingSets, IEnumerable<IdTypeValue> workUnitIntensityTechniqueIds = null, IdTypeValue ownerNoteId = null)

            => new WorkUnitTemplateEntity(null, progressiveNumber, excerciseId, workingSets, workUnitIntensityTechniqueIds, ownerNoteId);


        /// <summary>
        /// Factory method - Loads a WU with the specified ID
        /// </summary>
        /// <param name="id">The Work Unit ID</param>
        /// <param name="excerciseId">The ID of the excercise which the WU consists of</param>
        /// <param name="progressiveNumber">The progressive number of the WS</param>
        /// <param name="ownerNoteId">The ID of the Work Unit Owner's note</param>
        /// <param name="workingSets">The working sets list - cannot be empty or null</param>
        /// <param name="workUnitIntensityTechniqueIds">The list of the IDs of the WU Intensity Techniques</param>
        /// <returns>The WorkUnitTemplate instance</returns>
        public static WorkUnitTemplateEntity PlanWorkUnit(IdTypeValue id, uint progressiveNumber, IdTypeValue excerciseId, 
            IEnumerable<WorkingSetTemplateEntity> workingSets, IEnumerable<IdTypeValue> workUnitIntensityTechniqueIds = null, IdTypeValue ownerNoteId = null)

            => new WorkUnitTemplateEntity(id, progressiveNumber, excerciseId, workingSets, workUnitIntensityTechniqueIds, ownerNoteId);

        #endregion



        #region Public Methods

        /// <summary>
        /// Assign a new progressive number to the WU
        /// </summary>
        /// <param name="newPnum">The new progressive number - PNums must be consecutive</param>
        public void MoveToNewProgressiveNumber(uint newPnum)
        {
            ProgressiveNumber = newPnum;
            //TestBusinessRules();
        }


        /// <summary>
        /// Add the intensity technique to the Work Unit and all its WSs
        /// </summary>
        /// <param name="intensityTechniqueId">The ID of the intensity technique</param>
        /// <exception cref="ArgumentNullException">If the ID is null</exception>
        public void AddWorkUnitIntensityTechnique(IdTypeValue intensityTechniqueId)
        {
            if (intensityTechniqueId == null)
                throw new ArgumentNullException(nameof(intensityTechniqueId), $"Intensity Technique ID must be valid when adding a technique to the Work Unit");

            if (_intensityTechniquesIds.Contains(intensityTechniqueId))
                return;

            _intensityTechniquesIds.Add(intensityTechniqueId);

            foreach (WorkingSetTemplateEntity ws in _workingSets)
                ws.AddIntensityTechnique(intensityTechniqueId);

            TestBusinessRules();
        }


        /// <summary>
        /// Remove the intensity technique from the Work Unit and all its WSs
        /// </summary>
        /// <param name="intensityTechniqueId">The ID of the intensity technique</param>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        /// <returns>True if remove successfull</returns>
        public void RemoveWorkUnitIntensityTechnique(IdTypeValue intensityTechniqueId)
        {
            bool removed =_intensityTechniquesIds.Remove(intensityTechniqueId);

            if(removed)
            {
                foreach (WorkingSetTemplateEntity ws in _workingSets)
                    ws.RemoveIntensityTechnique(intensityTechniqueId);

                TestBusinessRules();
            }
        }


        /// <summary>
        /// Attach a note to the WU, or repleace it if already present
        /// </summary>
        /// <param name="newNoteId">The ID of the note to be attached</param>
        public void AssignNote(IdTypeValue newNoteId)
        {
            OwnerNoteId = newNoteId;
        }


        /// <summary>
        /// Remove the Owner's note
        /// </summary>
        public void RemoveNote()
        {
            AssignNote(null);
        }


        /// <summary>
        /// Assign an excercise to the Work Unit
        /// </summary>
        /// <param name="excerciseId">The ID of the excercise</param>
        public void AssignExcercise(IdTypeValue excerciseId)
        {
            ExcerciseId = excerciseId;
        }


        /// <summary>
        /// Add the Working Set to the Work Unit - copy not reference
        /// </summary>
        /// <param name="toAdd">The WS to be added</param>
        /// <exception cref="ArgumentException">If Working Set already present</exception>
        public void AddWorkingSet(WorkingSetTemplateEntity toAdd)
        {
            WorkingSetTemplateEntity copy = toAdd.Clone() as WorkingSetTemplateEntity;

            if (_workingSets.Contains(copy))
                throw new ArgumentException("Trying to add a duplicate Working Set", nameof(toAdd));

            foreach(IdTypeValue techniqueId in _intensityTechniquesIds)
                copy.AddIntensityTechnique(techniqueId);

            _workingSets.Add(copy);

            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(_workingSets, GetMainEffortType());
            TrainingVolume = TrainingVolume.AddWorkingSet(copy);
            TrainingDensity = TrainingDensity.AddWorkingSet(copy);

            TestBusinessRules();
        }


        /// <summary>
        /// Add the Working Set to the Work Unit
        /// </summary>
        /// <param name="repetitions">The WS repetitions</param>
        /// <param name="rest">The rest period between the WS and the following</param>
        /// <param name="effort">The WS effort</param>
        /// <param name="tempo">The WS lifting tempo</param>
        /// <param name="intensityTechniqueIds">The ids of the WS intensity techniques</param>
        public void AddTransientWorkingSet(WSRepetitionValue repetitions, RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null, 
            IEnumerable<IdTypeValue> intensityTechniqueIds = null)
        {
            List<IdTypeValue> localIntensityTechniqueIds = CommonUtilities.RemoveDuplicatesFrom(intensityTechniqueIds)?.ToList() ?? new List<IdTypeValue>();

            // Apply the WU-wise intensity techniques - if any - to the added WS
            if (_intensityTechniquesIds.Count > 0)
                localIntensityTechniqueIds.AddRange(_intensityTechniquesIds.Select(x => x));

            WorkingSetTemplateEntity toAdd = WorkingSetTemplateEntity.PlanTransientWorkingSet(
                    BuildWorkingSetProgressiveNumber(),
                    repetitions,
                    rest,
                    effort,
                    tempo,
                    localIntensityTechniqueIds
                );

            _workingSets.Add(toAdd);

            //TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(_workingSets);
            //TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(_workingSets);
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(_workingSets, GetMainEffortType());

            TrainingVolume = TrainingVolume.AddWorkingSet(toAdd);
            TrainingDensity = TrainingDensity.AddWorkingSet(toAdd);
            //TrainingIntensity.AddWorkingSet(toAdd);

            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Working Set from the Work Unit
        /// </summary>
        /// <param name="toRemove">The WS to be removed</param>
        /// <exception cref="ArgumentException">If the WS could not be found</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        /// <returns>True if remove successful</returns>
        public void RemoveWorkingSet(WorkingSetTemplateEntity toRemove)
        {
            if (toRemove == null)
                return;

            RemoveWorkingSet(toRemove.ProgressiveNumber);
        }


        ///// <summary>
        ///// Remove the Working Set from the Work Unit
        ///// </summary>
        ///// <param name="toRemoveId">The Id of the WS to be removed</param>
        ///// <exception cref="ArgumentException">If ID could not be found</exception>
        ///// <exception cref="ArgumentNullException">If ID is NULL</exception>
        //public void RemoveWorkingSet(IdTypeValue toRemoveId)
        //{
        //    WorkingSetTemplate toBeRemoved = FindWorkingSetById(toRemoveId);

        //    _workingSets.Remove(toBeRemoved);

        //    TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(_workingSets);
        //    TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(_workingSets);
        //    TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(_workingSets, GetMainEffortType());

        //    ForceConsecutiveWorkingSetsProgressiveNumbers();
        //    TestBusinessRules();
        //}


        /// <summary>
        /// Remove the Working Set from the Work Unit
        /// </summary>
        /// <param name="progressiveNumber">The progressive number of the WS to be removed</param>
        /// <exception cref="InvalidOperationException">If more Working Sets with the specified Progressive Number</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void RemoveWorkingSet(uint progressiveNumber)
        {
            WorkingSetTemplateEntity toBeRemoved = FindWorkingSet(progressiveNumber);

            if(_workingSets.Remove(toBeRemoved))
            {
                TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(_workingSets, GetMainEffortType());
                TrainingVolume = TrainingVolume.RemoveWorkingSet(toBeRemoved);
                TrainingDensity = TrainingDensity.RemoveWorkingSet(toBeRemoved);

                ForceConsecutiveWorkingSetsProgressiveNumbers(progressiveNumber);
                TestBusinessRules();
            }
        }


        ///// <summary>
        ///// Get a copy of the Working Set with the ID specified
        ///// </summary>
        ///// <param name="id">The Id to be found</param>
        ///// <exception cref="ArgumentNullException">If ID is NULL</exception>
        ///// <exception cref="ArgumentException">If ID could not be found</exception>
        ///// <returns>The WorkingSetTemplate object/returns>
        //public WorkingSetTemplate CloneWorkingSetWithId(IdTypeValue id)

        //    => FindWorkingSetById(id)?.Clone() as WorkingSetTemplate;


        /// <summary>
        /// Get a copy of the Working Set with the progressive number specified or DEFAULT if not found
        /// </summary>
        /// <param name="pNum">The progressive number to be found</param>
        /// <exception cref="InvalidOperationException">If more Working Sets with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found</returns>
        public WorkingSetTemplateEntity CloneWorkingSet(uint pNum)

            => FindWorkingSetOrDefault(pNum)?.Clone() as WorkingSetTemplateEntity;


        /// <summary>
        /// Get the main effort type as the effort of most of the WSs of the WU 
        /// </summary>
        /// <returns>The training effort type</returns>
        public TrainingEffortTypeEnum GetMainEffortType()

            => _workingSets.Where(x => x != null).Count() == 0 ? TrainingEffortTypeEnum.IntensityPerc
                : _workingSets.GroupBy(x => x.Effort.EffortType).Select(x
                     => new
                     {
                         Counter = x.Count(),
                         EffortType = x.Key
                     }).OrderByDescending(x => x.Counter).First().EffortType;

        #endregion


        #region Working Sets Methods

        /// <summary>
        /// Assign a new progressive number to the WS
        /// </summary>
        /// <param name="newPnum">The new progressive number - PNums must be consecutive</param>
        /// <param name="toMovePnum">The Progressive Number of the WS to be moved</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void MoveWorkingSetToNewProgressiveNumber(uint toMovePnum, uint newPnum)
        {
            WorkingSetTemplateEntity dest = FindWorkingSet(newPnum);
            WorkingSetTemplateEntity source = FindWorkingSet(toMovePnum);

            // Switch sets
            dest.MoveToNewProgressiveNumber(toMovePnum);
            source.MoveToNewProgressiveNumber(newPnum);

            ForceConsecutiveWorkingSetsProgressiveNumbers();
            TestBusinessRules();
        }


        /// <summary>
        /// Change the repetitions
        /// </summary>
        /// <param name="newReps">The new target repetitions</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void ChangeWorkingSetRepetitions(uint workingSetPnum, WSRepetitionValue newReps)
        {
            WorkingSetTemplateEntity ws = FindWorkingSet(workingSetPnum);
            ws.ChangeRepetitions(newReps);
        }


        /// <summary>
        /// Change the effort
        /// </summary>
        /// <param name="newEffort">The new value</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void ChangeWorkingSetEffort(uint workingSetPnum, TrainingEffortValue newEffort)
        {
            WorkingSetTemplateEntity ws = FindWorkingSet(workingSetPnum);
            ws.ChangeEffort(newEffort);
        }


        /// <summary>
        /// Change the rest period of the WS specified
        /// </summary>
        /// <param name="newRest">The new value</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void ChangeWorkingSetRestPeriod(uint workingSetPnum, RestPeriodValue newRest)
        {
            WorkingSetTemplateEntity ws = FindWorkingSet(workingSetPnum);
            ws.ChangeRestPeriod(newRest);
        }


        /// <summary>
        /// Change the lifting tempo
        /// </summary>
        /// <param name="newTempo">The new value</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void ChangeWorkingSetLiftingTempo(uint workingSetPnum, TUTValue newTempo)
        {
            WorkingSetTemplateEntity ws = FindWorkingSet(workingSetPnum);
            ws.ChangeLiftingTempo(newTempo);
        }


        /// <summary>
        /// Add an intensity technique - Do nothing if already present in the list
        /// </summary>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be changed</param>
        /// <param name="toAddId">The id to be added</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void AddWorkingSetIntensityTechnique(uint workingSetPnum, IdTypeValue toAddId)
        {
            WorkingSetTemplateEntity ws = FindWorkingSet(workingSetPnum);
            ws.AddIntensityTechnique(toAddId);
        }


        /// <summary>
        /// Remove an intensity technique 
        /// </summary>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be changed</param>
        /// <param name="toRemoveId">The id to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void RemoveWorkingSetIntensityTechnique(uint workingSetPnum, IdTypeValue toRemoveId)
        {
            WorkingSetTemplateEntity ws = FindWorkingSet(workingSetPnum);

            ws.RemoveIntensityTechnique(toRemoveId);
            TestBusinessRules();
        }
        #endregion

        #region Working Sets Methods - IDS

        ///// <summary>
        ///// Assign a new progressive number to the WS
        ///// </summary>
        ///// <param name="newPnum">The new progressive number - PNums must be consecutive</param>
        ///// <param name="wsId">The ID of the WS to be moved</param>
        //public void MoveWorkingSetToNewProgressiveNumber(IdTypeValue wsId, uint newPnum)
        //{
        //    uint oldPnum = FindWorkingSetById(wsId).ProgressiveNumber;

        //    // Switch sets
        //    FindWorkingSetByProgressiveNumber((int)newPnum).MoveToNewProgressiveNumber(oldPnum);
        //    FindWorkingSetById(wsId).MoveToNewProgressiveNumber(newPnum);

        //    //_workingSets = SortWorkingSetsByProgressiveNumber(_workingSets);
        //    ForceConsecutiveWorkingSetsProgressiveNumbers();
        //    TestBusinessRules();
        //}


        ///// <summary>
        ///// Change the repetitions
        ///// </summary>
        ///// <param name="newReps">The new target repetitions</param>
        ///// <param name="workingSetId">The Id of the WS to be changed</param>
        ///// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        ///// <exception cref="ArgumentException">Thrown if WS not found</exception>
        //public void ChangeWorkingSetRepetitions(IdTypeValue workingSetId, WSRepetitionValue newReps)
        //{
        //    WorkingSetTemplate ws = FindWorkingSetById(workingSetId);
        //    ws.ChangeRepetitions(newReps);
        //}


        ///// <summary>
        ///// Change the effort
        ///// </summary>
        ///// <param name="newEffort">The new value</param>
        ///// <param name="workingSetId">The Id of the WS to be changed</param>/// <param name="workingSetId">The Id of the WS to be changed</param>
        ///// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        //public void ChangeWorkingSetEffort(IdTypeValue workingSetId, TrainingEffortValue newEffort)
        //{
        //    WorkingSetTemplate ws = FindWorkingSetById(workingSetId);
        //    ws.ChangeEffort(newEffort);
        //}


        ///// <summary>
        ///// Change the rest period of the WS specified
        ///// </summary>
        ///// <param name="newRest">The new value</param>
        ///// <param name="workingSetId">The Id of the WS to be changed</param>
        ///// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        //public void ChangeWorkingSetRestPeriod(IdTypeValue workingSetId, RestPeriodValue newRest)
        //{
        //    WorkingSetTemplate ws = FindWorkingSetById(workingSetId);
        //    ws.ChangeRestPeriod(newRest);
        //}


        ///// <summary>
        ///// Change the lifting tempo
        ///// </summary>
        ///// <param name="newTempo">The new value</param>
        ///// <param name="workingSetId">The Id of the WS to be changed</param>
        ///// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        //public void ChangeWorkingSetLiftingTempo(IdTypeValue workingSetId, TUTValue newTempo)
        //{
        //    WorkingSetTemplate ws = FindWorkingSetById(workingSetId);
        //    ws.ChangeLiftingTempo(newTempo);
        //}


        ///// <summary>
        ///// Add an intensity technique - Do nothing if already present in the list
        ///// </summary>
        ///// <param name="workingSetId">The Id of the WS to be changed</param>
        ///// <param name="toAddId">The id to be added</param>
        ///// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        //public void AddWorkingSetIntensityTechnique(IdTypeValue workingSetId, IdTypeValue toAddId)
        //{
        //    WorkingSetTemplate ws = FindWorkingSetById(workingSetId);
        //    ws.AddIntensityTechnique(toAddId);
        //}


        ///// <summary>
        ///// Remove an intensity technique 
        ///// </summary>
        ///// <param name="workingSetId">The Id of the WS to be changed</param>
        ///// <param name="toRemoveId">The id to be removed</param>
        ///// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        ///// <exception cref="ArgumentNullException">If ID is NULL</exception>
        ///// <exception cref="ArgumentException">If ID could not be found</exception>
        //public bool RemoveWorkingSetIntensityTechnique(IdTypeValue workingSetId, IdTypeValue toRemoveId)
        //{
        //    WorkingSetTemplate ws = FindWorkingSetById(workingSetId);

        //    bool ok = ws.RemoveIntensityTechnique(toRemoveId);
        //    TestBusinessRules();
        //    return ok;
        //}

        #endregion


        #region Private Methods

        ///// <summary>
        ///// Build the next valid id
        ///// </summary>
        ///// <returns>The WS Id</returns>
        //private IdTypeValue BuildWorkingSetId()
        //{
        //    if (_workingSets.Count == 0)
        //        return IdTypeValue.Create(1);

        //    else
        //        return IdTypeValue.Create(_workingSets.Max(x => x.Id.Id) + 1);
        //}


        /// <summary>
        /// Build the next valid progressive number
        /// To be used before adding the WS to the list
        /// </summary>
        /// <returns>The WS Progressive Number</returns>
        private uint BuildWorkingSetProgressiveNumber() => (uint)_workingSets.Count();


        /// <summary>
        /// Sort the Working Sets list wrt the WS progressive numbers
        /// </summary>
        /// <param name="wsIn">The input WS list</param>
        /// <returns>The sorted list</returns>
        private IEnumerable<WorkingSetTemplateEntity> SortWorkingSetsByProgressiveNumber(IEnumerable<WorkingSetTemplateEntity> wsIn)

            => wsIn.OrderBy(x => x.ProgressiveNumber);

        /// <summary>
        /// Force the WSs to have consecutive progressive numbers
        /// It works by assuming that the WSs are added in a sorted fashion.
        /// </summary>
        private void ForceConsecutiveWorkingSetsProgressiveNumbers()
        {
            _workingSets = SortWorkingSetsByProgressiveNumber(_workingSets).ToList();

            // Just overwrite all the progressive numbers
            for (int iws = 0; iws < _workingSets.Count(); iws++)
            {
                WorkingSetTemplateEntity ws = _workingSets[iws];
                ws.MoveToNewProgressiveNumber((uint)iws);
            }
        }

        /// <summary>
        /// Force the WSs to have consecutive progressive numbers
        /// This algorithm is more efficient as it ignores the elments before pnum, provided that they are already sorted
        /// </summary>
        /// <param name="fromPnum">The Progressive number from which the order is not respected</param>
        private void ForceConsecutiveWorkingSetsProgressiveNumbers(uint fromPnum)
        {
            // Just overwrite all the progressive numbers
            for (int iws = (int)fromPnum; iws < _workingSets.Count(); iws++)
            {
                WorkingSetTemplateEntity ws = _workingSets[iws];
                ws.MoveToNewProgressiveNumber((uint)iws);
            }
        }


        ///// <summary>
        ///// Find the Working Set with the ID specified
        ///// </summary>
        ///// <param name="id">The Id to be found</param>
        ///// <exception cref="ArgumentNullException">If ID is NULL</exception>
        ///// <exception cref="ArgumentException">If ID could not be found</exception>
        ///// <returns>The WorkingSetTemplate object/returns>
        //private WorkingSetTemplate FindWorkingSetById(IdTypeValue id)
        //{
        //    if (id == null)
        //        throw new ArgumentNullException(nameof(id), $"Cannot find a WS with NULL id");

        //    WorkingSetTemplate ws = _workingSets.Where(x => x.Id == id).FirstOrDefault();

        //    if (ws == default)
        //        throw new ArgumentException($"Working Set with Id {id.ToString()} could not be found", nameof(id));

        //    return ws;
        //}

        /// <summary>
        /// Find the Working Set with the progressive number specified
        /// </summary>
        /// <param name="pNum">The progressive number to be found</param>
        /// <exception cref="InvalidOperationException">If more Working Sets with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if notfound</returns>
        private WorkingSetTemplateEntity FindWorkingSetOrDefault(uint pNum)

            => _workingSets.SingleOrDefault(x => x.ProgressiveNumber == pNum);


        /// <summary>
        /// Find the Working Set with the progressive number specified
        /// </summary>
        /// <param name="pNum">The progressive number to be found</param>
        /// <exception cref="InvalidOperationException">If more Working Sets, or zero, with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object</returns>
        private WorkingSetTemplateEntity FindWorkingSet(uint pNum)

            => _workingSets.Single(x => x.ProgressiveNumber == pNum);

        #endregion


        #region Business Rules Validation

        /// <summary>
        /// The Work Unit musthave no NULL working sets.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullWorkingSets() => _workingSets.All(x => x != null);


        /// <summary>
        /// Work Unit Intensity Techniques must be non NULL.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullIntensityTechniques() => _intensityTechniquesIds.All(x => x != null);


        /// <summary>
        /// The Work Unit must be linked to an excercise.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool ExcerciseSpecified() => ExcerciseId != null;


        /// <summary>
        /// Cannot create a Work Unit without any WS 
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool AtLeastOneWorkingSet() => _workingSets?.Count > 0;


        /// <summary>
        /// Working Sets of the same Work Unit must have consecutive progressive numbers.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool WorkingSetsWithConsecutiveProgressiveNumber()
        {
            if (_workingSets.Count == 0)
                return true;

            // Check the first element: the sequence must start from 0
            if (_workingSets?.Count() == 1)
            {
                if (_workingSets.FirstOrDefault()?.ProgressiveNumber == 0)
                    return true;
                else
                    return false;
            }

            // Look for non consecutive numbers - exclude the last one
            foreach(int pnum in _workingSets.Where(x => x.ProgressiveNumber != _workingSets.Count() - 1)
                .Select(x => x.ProgressiveNumber))
            {
                if (!_workingSets.Any(x => x.ProgressiveNumber == pnum + 1))
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Working sets must have all the work unit intensity techniques.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool AllWorkingSetsHaveWorkUnitWiseIntensityTechniques()
        {
            if (_intensityTechniquesIds.Count == 0)
                return true;

            foreach(IdTypeValue techniqueId in _intensityTechniquesIds)
            {
                if (_workingSets.Any(x => !x.IntensityTechniqueIds.Contains(techniqueId)))
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
            if (!NoNullWorkingSets())
                throw new TrainingDomainInvariantViolationException($"The Work Unit must have no NULL working sets.");

            if (!NoNullIntensityTechniques())
                throw new TrainingDomainInvariantViolationException($"Work Unit Intensity Techniques must be non NULL.");

            if (!ExcerciseSpecified())
                throw new TrainingDomainInvariantViolationException($"The Work Unit must be linked to an excercise.");

            if (!AtLeastOneWorkingSet())
                throw new TrainingDomainInvariantViolationException($"Cannot create a Work Unit without any WS.");

            if (!AllWorkingSetsHaveWorkUnitWiseIntensityTechniques())
                throw new TrainingDomainInvariantViolationException($"Working sets must have all the work unit intensity techniques.");

            if (!WorkingSetsWithConsecutiveProgressiveNumber())
                throw new TrainingDomainInvariantViolationException($"Working Sets of the same Work Unit must have consecutive progressive numbers.");
        }

        #endregion


        #region IClonable Implementation

        public object Clone()

            => PlanWorkUnit(Id, ProgressiveNumber, ExcerciseId, WorkingSets.ToList(), _intensityTechniquesIds, OwnerNoteId);

        #endregion
    }
}


