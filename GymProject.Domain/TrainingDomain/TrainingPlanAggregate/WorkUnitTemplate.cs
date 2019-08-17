using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
{
    public class WorkUnitTemplate : Entity<IdType>, ICloneable
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


        private IList<WorkingSetTemplate> _workingSets = new List<WorkingSetTemplate>();

        /// <summary>
        /// The Working Sets belonging to the WU, sorted by Progressive Numbers.
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<WorkingSetTemplate> WorkingSets
        {
            get => _workingSets?.Clone().ToList().AsReadOnly() 
                ?? new List<WorkingSetTemplate>().AsReadOnly();  // Objects are not referencally equal
        }
        /// </summary>
        public IdType OwnerNoteId { get; private set; } = null;


        /// <summary>
        /// FK to the Excercise Aggregate
        /// </summary>
        public IdType ExcerciseId { get; private set; } = null;


        private ICollection<IdType> _intensityTechniquesIds = null;

        /// <summary>
        /// The list of the IDs of the WU-wise Intensity Techniques - Will be applied to all the WSs
        /// </summary>
        public IReadOnlyCollection<IdType> IntensityTechniquesIds
        {
            get => _intensityTechniquesIds?.Clone().ToList().AsReadOnly() ?? new List<IdType>().AsReadOnly();
        }



        #region Ctors

        private WorkUnitTemplate(IdType id, uint progressiveNumber, IdType excerciseId, IList<WorkingSetTemplate> workingSets, ICollection<IdType> workUnitIntensityTechniqueIds, IdType ownerNoteId = null)
        {
            Id = id;
            ProgressiveNumber = progressiveNumber;
            OwnerNoteId = ownerNoteId;
            ExcerciseId = excerciseId;

            _workingSets = workingSets?.Clone().ToList() ?? new List<WorkingSetTemplate>();
            _intensityTechniquesIds = workUnitIntensityTechniqueIds?.Clone().ToList() ?? new List<IdType>();

            // Check for duplicate intensity techniques or let the WS raise the exception?

            foreach (WorkingSetTemplate ws in _workingSets)
                foreach (IdType intTechniqueId in _intensityTechniquesIds)
                    ws.AddIntensityTechnique(intTechniqueId);

            TestBusinessRules();

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(workingSets);
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(workingSets);
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(workingSets, GetMainEffortType());
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The ID of the WU</param>
        /// <param name="excerciseId">The ID of the excercise which the WU consists of</param>
        /// <param name="progressiveNumber">The progressive number of the WS</param>
        /// <param name="ownerNoteId">The ID of the Work Unit Owner's note</param>
        /// <param name="workingSets">The working sets list - cannot be empty or null</param>
        /// <param name="workUnitIntensityTechniqueIds">The list of the IDs of the WU Intensity Techniques</param>
        /// <returns>The WorkUnitTemplate instance</returns>
        public static WorkUnitTemplate PlanWorkUnit(IdType id, uint progressiveNumber, IdType excerciseId, IList<WorkingSetTemplate> workingSets, ICollection<IdType> workUnitIntensityTechniqueIds = null, IdType ownerNoteId = null)

            => new WorkUnitTemplate(id, progressiveNumber, excerciseId, workingSets, workUnitIntensityTechniqueIds, ownerNoteId);

        #endregion



        #region Public Methods

        /// <summary>
        /// Assign a new progressive number to the WU
        /// </summary>
        /// <param name="newPnum">The new progressive number - PNums must be consecutive</param>
        public void MoveToNewProgressiveNumber(uint newPnum)
        {
            ProgressiveNumber = newPnum;
            TestBusinessRules();
        }


        /// <summary>
        /// Add the intensity technique to the Work Unit and all its WSs
        /// </summary>
        /// <param name="intensityTechniqueId">The ID of the intensity technique</param>
        public void AddWorkUnitIntensityTechnique(IdType intensityTechniqueId)
        {
            _intensityTechniquesIds.Add(intensityTechniqueId);

            foreach (WorkingSetTemplate ws in _workingSets)
                AddWorkingSetIntensityTechnique(ws.Id, intensityTechniqueId);

            TestBusinessRules();
        }


        /// <summary>
        /// Remove the intensity technique from the Work Unit and all its WSs
        /// </summary>
        /// <param name="intensityTechniqueId">The ID of the intensity technique</param>
        public void RemoveWorkUnitIntensityTechnique(IdType intensityTechniqueId)
        {
            bool removed =_intensityTechniquesIds.Remove(intensityTechniqueId);

            if(removed)
            {
                foreach (WorkingSetTemplate ws in _workingSets)
                    RemoveWorkingSetIntensityTechnique(ws.Id, intensityTechniqueId);
            }
            TestBusinessRules();
        }


        /// <summary>
        /// Attach a note to the WU, or repleace it if already present
        /// </summary>
        /// <param name="newNoteId">The ID of the note to be attached</param>
        public void AssignNote(IdType newNoteId)
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
        public void AssignExcercise(IdType excerciseId)
        {
            ExcerciseId = excerciseId;
        }


        /// <summary>
        /// Add the Working Set to the Work Unit
        /// </summary>
        /// <param name="repetitions">The WS repetitions</param>
        /// <param name="rest">The rest period between the WS and the following</param>
        /// <param name="effort">The WS effort</param>
        /// <param name="tempo">The WS lifting tempo</param>
        /// <param name="intensityTechniqueIds">The ids of the WS intensity techniques</param>
        public void AddWorkingSet(WSRepetitionValue repetitions, RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null, IList<IdType> intensityTechniqueIds = null)
        {
            List<IdType> localIntensityTechniqueIds = intensityTechniqueIds.Clone().ToList() ?? new List<IdType>();

            // Apply the WU-wise intensity techniques - if any - to the added WS
            if (_intensityTechniquesIds.Count > 0)
                localIntensityTechniqueIds.AddRange(_intensityTechniquesIds.Select(x => x));

            _workingSets.Add(
                WorkingSetTemplate.AddWorkingSet(
                    BuildWorkingSetId(),
                    BuildWorkingSetProgressiveNumber(),
                    repetitions,
                    rest,
                    effort,
                    tempo,
                    localIntensityTechniqueIds
                ));

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(_workingSets);
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(_workingSets);
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(_workingSets, GetMainEffortType());

            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Working Set from the Work Unit
        /// </summary>
        /// <param name="toRemoveId">The Id of the WS to be removed</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        public void RemoveWorkingSet(IdType toRemoveId)
        {
            WorkingSetTemplate toBeRemoved = FindWorkingSetById(toRemoveId);

            _workingSets.Remove(toBeRemoved);

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(_workingSets);
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(_workingSets);
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(_workingSets, GetMainEffortType());

            ForceConsecutiveWorkingSetsProgressiveNumbers();
            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Working Set from the Work Unit
        /// </summary>
        /// <param name="progressiveNumber">The progressive number of the WS to be removed</param>
        public void RemoveWorkingSet(int progressiveNumber)
        {
            WorkingSetTemplate toBeRemoved = FindWorkingSetByProgressiveNumber(progressiveNumber);

            _workingSets.Remove(toBeRemoved);

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(_workingSets);
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(_workingSets);
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(_workingSets, GetMainEffortType());

            ForceConsecutiveWorkingSetsProgressiveNumbers();
            TestBusinessRules();
        }


        /// <summary>
        /// Find the Working Set with the ID specified
        /// </summary>
        /// <param name="id">The Id to be found</param>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <returns>The WokringSetTemplate object/returns>
        public WorkingSetTemplate FindWorkingSetById(IdType id)
        {
            if (id == null)
                throw new ArgumentNullException($"Cannot find a WS with NULL id");

            WorkingSetTemplate ws = _workingSets.Where(x => x.Id == id).FirstOrDefault();

            if (ws == default)
                throw new ArgumentException($"Working Set with Id {id.ToString()} could not be found");

            return ws;
        }

        /// <summary>
        /// Find the Working Set with the progressive number specified
        /// </summary>
        /// <param name="pNum">The progressive number to be found</param>
        /// <exception cref="ArgumentException">If Progressive Number could not be found</exception>
        /// <returns>The WokringSetTemplate object or DEFAULT if not found/returns>
        public WorkingSetTemplate FindWorkingSetByProgressiveNumber(int pNum)
        {
            WorkingSetTemplate result  = _workingSets.Where(x => x.ProgressiveNumber == pNum).FirstOrDefault();

            if (result == default)
                throw new ArgumentException($"Working Set with Progressive Number {pNum.ToString()} could not be found");

            return result;
        }

        #endregion



        #region Working Sets Methods

        /// <summary>
        /// Assign a new progressive number to the WS
        /// </summary>
        /// <param name="newPnum">The new progressive number - PNums must be consecutive</param>
        /// <param name="wsId">The ID of the WS to be moved</param>
        public void MoveWorkingSetToNewProgressiveNumber(IdType wsId, uint newPnum)
        {
            uint oldPnum = FindWorkingSetById(wsId).ProgressiveNumber;

            // Switch sets
            FindWorkingSetByProgressiveNumber((int)newPnum).MoveToNewProgressiveNumber(oldPnum);
            FindWorkingSetById(wsId).MoveToNewProgressiveNumber(newPnum);

            //_workingSets = SortWorkingSetsByProgressiveNumber(_workingSets);
            ForceConsecutiveWorkingSetsProgressiveNumbers();
            TestBusinessRules();
        }


        /// <summary>
        /// Change the repetitions
        /// </summary>
        /// <param name="newReps">The new target repetitions</param>
        /// <param name="workingSetId">The Id of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="ArgumentException">Thrown if WS not found</exception>
        public void ChangeWorkingSetRepetitions(IdType workingSetId, WSRepetitionValue newReps)
        {
            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);
            ws.ChangeRepetitions(newReps);
        }


        /// <summary>
        /// Change the effort
        /// </summary>
        /// <param name="newEffort">The new value</param>
        /// <param name="workingSetId">The Id of the WS to be changed</param>/// <param name="workingSetId">The Id of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void ChangeWorkingSetEffort(IdType workingSetId, TrainingEffortValue newEffort)
        {
            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);
            ws.ChangeEffort(newEffort);
        }


        /// <summary>
        /// Change the rest period of the WS specified
        /// </summary>
        /// <param name="newRest">The new value</param>
        /// <param name="workingSetId">The Id of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void ChangeWorkingSetRestPeriod(IdType workingSetId, RestPeriodValue newRest)
        {
            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);
            ws.ChangeRestPeriod(newRest);
        }


        /// <summary>
        /// Change the lifting tempo
        /// </summary>
        /// <param name="newTempo">The new value</param>
        /// <param name="workingSetId">The Id of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void ChangeWorkingSetLiftingTempo(IdType workingSetId, TUTValue newTempo)
        {
            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);
            ws.ChangeLiftingTempo(newTempo);
        }


        /// <summary>
        /// Add an intensity technique - Do nothing if already present in the list
        /// </summary>
        /// <param name="workingSetId">The Id of the WS to be changed</param>
        /// <param name="toAddId">The id to be added</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void AddWorkingSetIntensityTechnique(IdType workingSetId, IdType toAddId)
        {
            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);
            ws.AddIntensityTechnique(toAddId);
        }


        /// <summary>
        /// Remove an intensity technique 
        /// </summary>
        /// <param name="workingSetId">The Id of the WS to be changed</param>
        /// <param name="toRemoveId">The id to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        public bool RemoveWorkingSetIntensityTechnique(IdType workingSetId, IdType toRemoveId)
        {
            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);

            bool ok = ws.RemoveIntensityTechnique(toRemoveId);
            TestBusinessRules();
            return ok;
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Build the next valid id
        /// </summary>
        /// <returns>The WS Id</returns>
        private IdType BuildWorkingSetId()
        {
            if (_workingSets.Count == 0)
                return new IdType(1);

            else
                return _workingSets.Last().Id + 1;
        }


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
        private IEnumerable<WorkingSetTemplate> SortWorkingSetsByProgressiveNumber(IEnumerable<WorkingSetTemplate> wsIn)

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
                WorkingSetTemplate ws = _workingSets[iws];
                ws.MoveToNewProgressiveNumber((uint)iws);
            }
        }

        /// <summary>
        /// Get the main effort type as the effort of most of the WSs of the WU 
        /// </summary>
        /// <returns>The training effort type</returns>
        private TrainingEffortTypeEnum GetMainEffortType()

            => _workingSets.Count == 0 ? TrainingEffortTypeEnum.IntensityPerc 
                : _workingSets.GroupBy(x => x.Effort.EffortType).Select(x
                     => new
                     {
                         Counter = x.Count(),
                         EffortType = x.Key
                     }).OrderByDescending(x => x.Counter).First().EffortType;

        #endregion


        #region Business Rules Validation

        /// <summary>
        /// The Work Unit musthave no NULL working sets.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullWorkingSets() => _workingSets.All(x => x != null);


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

            foreach(IdType itId in _intensityTechniquesIds)
            {
                if (_workingSets.Any(x => !x.IntensityTechniqueIds.Contains(itId)))
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
                throw new TrainingDomainInvariantViolationException($"The Work Unit musthave no NULL working sets.");

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


