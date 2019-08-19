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
    public class WorkoutTemplate : Entity<IdType>, ICloneable
    {


        /// <summary>
        /// The progressive number of the Workout - Starts from 0
        /// </summary>
        public uint ProgressiveNumber { get; private set; } = 0;


        /// <summary>
        /// The Workout name - Unique among the WOs of the Training Week
        /// </summary>
        public string Name { get; private set; } = string.Empty;


        /// <summary>
        /// The week day the Workout is scheduled to - if any
        /// </summary>
        public WeekdayEnum SpecificWeekday { get; private set; } = null;


        /// <summary>
        /// The training volume parameters, as the sum of the params of the single WOs
        /// </summary>
        public TrainingVolumeParametersValue TrainingVolume { get; private set; } = null;


        /// <summary>
        /// The training effort, as the average of the single WOs efforts
        /// </summary>
        public TrainingIntensityParametersValue TrainingIntensity { get; private set; } = null;


        /// <summary>
        /// The training density parameters, as the sum of the params of the single WOs
        /// </summary>
        public TrainingDensityParametersValue TrainingDensity { get; private set; } = null;


        private IList<WorkUnitTemplate> _workUnits = new List<WorkUnitTemplate>();

        /// <summary>
        /// The WUs belonging to the WorkOut
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<WorkUnitTemplate> WorkUnits
        {
            get => _workUnits?.Clone().ToList().AsReadOnly() ?? new List<WorkUnitTemplate>().AsReadOnly();
        }




        #region Ctors

        private WorkoutTemplate(IdType id, uint progressiveNumber, IList<WorkUnitTemplate> workUnits, string workoutName, WeekdayEnum weekday)
        {
            Id = id;
            ProgressiveNumber = progressiveNumber;
            Name = workoutName ?? string.Empty;
            SpecificWeekday = weekday ?? WeekdayEnum.Generic;

            _workUnits = workUnits?.Clone().ToList() ?? new List<WorkUnitTemplate>();

            TestBusinessRules();

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets(), GetMainEffortType());
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The ID of the WU</param>
        /// <param name="weekday">The week day the Workout is scheduled to - if any</param>
        /// <param name="progressiveNumber">The progressive number of the WS</param>
        /// <param name="workoutName">The name given to the Workout - unique inside the Training Week</param>
        /// <param name="workUnits">The work unit list - cannot be empty or null</param>
        /// <returns>The WorkUnitTemplate instance</returns>
        public static WorkoutTemplate PlanWorkout(IdType id, uint progressiveNumber, IList<WorkUnitTemplate> workUnits, string workoutName, WeekdayEnum weekday = null)

            => new WorkoutTemplate(id, progressiveNumber, workUnits, workoutName, weekday);

        #endregion



        #region Public Methods

        /// <summary>
        /// Assign a new progressive number to the WO
        /// </summary>
        /// <param name="newPnum">The new progressive number - PNums must be consecutive</param>
        public void MoveToNewProgressiveNumber(uint newPnum)
        {
            ProgressiveNumber = newPnum;
            TestBusinessRules();
        }


        /// <summary>
        /// Change the name of the WO
        /// </summary>
        /// <param name="newName">The new name</param>
        public void GiveName(string newName)
        {
            Name = newName;
        }


        /// <summary>
        /// Assign a specific day to the WO
        /// </summary>
        /// <param name="newDay">The specific day to schedule the workout</param>
        public void ScheduleToSpecificDay(WeekdayEnum newDay)
        {
            SpecificWeekday = newDay ?? WeekdayEnum.Generic;
        }


        /// <summary>
        /// Mark the WO as 'not scheduled to specific day'
        /// </summary>
        public void UnscheduleSpecificDay()
        {
            ScheduleToSpecificDay(WeekdayEnum.Generic);
        }


        /// <summary>
        /// Find the WorkUnit with the ID specified
        /// </summary>
        /// <param name="id">The Id to be found</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        /// <returns>The WokringSetTemplate object/returns>
        public WorkUnitTemplate FindWorkUnitById(IdType id)
        {
            if (id == null)
                throw new ArgumentNullException($"Cannot find a WS with NULL id");

            WorkUnitTemplate ws = _workUnits.Where(x => x.Id == id).FirstOrDefault();

            if (ws == default)
                throw new ArgumentException($"Work Unit with Id {id.ToString()} could not be found");

            return ws;
        }


        /// <summary>
        /// Find the Working Unit with the progressive number specified
        /// </summary>
        /// <param name="pNum">The progressive number to be found</param>
        /// <exception cref="ArgumentException">If Progressive Number could not be found</exception>
        /// <returns>The WokringSetTemplate object or DEFAULT if not found/returns>
        public WorkUnitTemplate FindWorkUnitByProgressiveNumber(int pNum)
        {
            WorkUnitTemplate result = _workUnits.Where(x => x.ProgressiveNumber == pNum).FirstOrDefault();

            if (result == default)
                throw new ArgumentException($"Work Unit with Progressive Number {pNum.ToString()} could not be found");

            return result;
        }


        /// <summary>
        /// Find the Working Unit which the WS belongs
        /// </summary>
        /// <param name="wsId">The WS ID to be found</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        /// <returns>The WorkUnitTemplate object/returns>
        public WorkUnitTemplate FindWorkUnitByWorkingSetId(IdType wsId)
        {
            if (wsId == null)
                throw new ArgumentNullException($"Cannot find a WS with NULL id");

            WorkUnitTemplate result = _workUnits.SingleOrDefault(wu => wu.WorkingSets.SingleOrDefault(ws => ws.Id == wsId) != default);

            if (result == default)
                throw new ArgumentException($"Working Set with Id {wsId.ToString()} could not be found");

            return result;
        }


        /// <summary>
        /// Find the Working Set with the ID specified
        /// </summary>
        /// <param name="id">The Id to be found</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        /// <returns>The WorkingSetTemplate object/returns>
        public WorkingSetTemplate FindWorkingSetById(IdType id)
        {
            if (id == null)
                throw new ArgumentNullException($"Cannot find a WS with NULL id");

            WorkingSetTemplate result = GetAllWorkingSets().Where(x => x.Id == id).FirstOrDefault();

            if (result == default)
                throw new ArgumentException($"Working Set with Id {id.ToString()} could not be found");

            return result;
        }


        /// <summary>
        /// Find the Working Set with the ID specified
        /// </summary>
        /// <param name="id">The Id to be found</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <returns>The WorkingSetTemplate object/returns>
        public WorkingSetTemplate FindWorkingSetByProgressiveNumber(int workUnitPnum, int workingSetPnum)
        {
            WorkingSetTemplate result = FindWorkUnitByProgressiveNumber(workUnitPnum).FindWorkingSetByProgressiveNumber(workingSetPnum);

            if (result == default)
                throw new ArgumentException($"Work Unit + Working Set with Progressive Numbers {workUnitPnum.ToString()} + {workingSetPnum.ToString()} could not be found.");

            return result;
        }


        /// <summary>
        /// Add the Working Unit to the Workout
        /// </summary>
        /// <param name="excerciseId">The ID of the excercise of the WU</param>
        /// <param name="ownerNoteId">The ID of the WU Owner's note</param>
        /// <param name="workingSets">The WS which the WU is made up of</param>
        public void AddWorkUnit(IdType excerciseId, IList<WorkingSetTemplate> workingSets, ICollection<IdType> workUnitIntensityTechniquesIds = null, IdType ownerNoteId = null)
        {
            WorkUnitTemplate toAdd = WorkUnitTemplate.PlanWorkUnit(
                BuildWorkUnitId(),
                BuildWorkUnitProgressiveNumber(),
                excerciseId,
                workingSets,
                workUnitIntensityTechniquesIds,
                ownerNoteId);

            _workUnits.Add(toAdd);

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets(), GetMainEffortType());

            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Working Unit from the Workout
        /// </summary>
        /// <param name="toRemoveId">The Id of the WU to be removed</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        public void RemoveWorkUnit(IdType toRemoveId)
        {
            WorkUnitTemplate toBeRemoved = FindWorkUnitById(toRemoveId);

            _workUnits.Remove(toBeRemoved);

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets(), GetMainEffortType());

            ForceConsecutiveWorkUnitProgressiveNumbers();
            TestBusinessRules();
        }


        /// <summary>
        /// Get the WSs of all the Work Units belonging to the Workout
        /// </summary>
        /// <returns>The list of the Working Sets</returns>
        public IEnumerable<WorkingSetTemplate> GetAllWorkingSets()

             => _workUnits.SelectMany(x => x.WorkingSets);


        #endregion


        #region Work Unit Methods


        /// <summary>
        /// Add the intensity technique to the selected Work Unit and all its WSs
        /// </summary>
        /// <param name="workUnitId">The ID of the Work Unit to be modified</param>
        /// <param name="intensityTechniqueId">The ID of the intensity technique</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        public void AddWorkUnitIntensityTechnique(IdType workUnitId, IdType intensityTechniqueId)
        {
            WorkUnitTemplate toBeModified = FindWorkUnitById(workUnitId);

            toBeModified.AddWorkUnitIntensityTechnique(intensityTechniqueId);
        }


        /// <summary>
        /// Remove the intensity technique from the selected Work Unit and all its WSs
        /// </summary>
        /// <param name="workUnitId">The ID of the Work Unit to be modified</param>
        /// <param name="intensityTechniqueId">The ID of the intensity technique</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        public void RemoveWorkUnitIntensityTechnique(IdType workUnitId, IdType intensityTechniqueId)
        {
            WorkUnitTemplate toBeModified = FindWorkUnitById(workUnitId);

            toBeModified.RemoveWorkUnitIntensityTechnique(intensityTechniqueId);
        }


        /// <summary>
        /// Attach a note to the WU, or repleace it if already present
        /// </summary>
        /// <param name="workUnitId">The ID of the Work Unit to be changed</param>
        /// <param name="newNoteId">The ID of the note to be attached</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        public void AssignWorkUnitNote(IdType workUnitId, IdType newNoteId)

            => FindWorkUnitById(workUnitId).AssignNote(newNoteId);


        /// <summary>
        /// Remove the note of the WU, or repleace it if already present
        /// </summary>
        /// <param name="workUnitId">The ID of the Work Unit to be changed</param>
        public void RemoveWorkUnitNote(IdType workUnitId)

            => AssignWorkUnitNote(workUnitId, null);


        /// <summary>
        /// Assign the excercise to the WU, or repleace it if already present
        /// </summary>
        /// <param name="workUnitId">The ID of the Work Unit to be changed</param>
        /// <param name="newExcerciseId">The ID of the excercise to be attached</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        public void AssignWorkUnitExcercise(IdType workUnitId, IdType newExcerciseId)

            => FindWorkUnitById(workUnitId).AssignExcercise(newExcerciseId);


        /// <summary>
        /// Assign a new progressive number to the WS
        /// </summary>
        /// <param name="newPnum">The new progressive number - PNums must be consecutive</param>
        /// <param name="wuId">The ID of the WS to be moved</param>
        public void MoveWorkUnitToNewProgressiveNumber(IdType wuId, uint newPnum)
        {
            uint oldPnum = FindWorkUnitById(wuId).ProgressiveNumber;

            // Switch sets
            FindWorkUnitByProgressiveNumber((int)newPnum).MoveToNewProgressiveNumber(oldPnum);
            FindWorkUnitById(wuId).MoveToNewProgressiveNumber(newPnum);

            ForceConsecutiveWorkUnitProgressiveNumbers();
            TestBusinessRules();
        }


        /// <summary>
        /// Add the Working Set to the Work Unit
        /// </summary>
        /// <param name="workUnitId">The ID of the WU which to add the WS to</param>
        /// <param name="repetitions">The WS repetitions</param>
        /// <param name="rest">The rest period between the WS and the following</param>
        /// <param name="effort">The WS effort</param>
        /// <param name="tempo">The WS lifting tempo</param>
        /// <param name="intensityTechniqueIds">The ids of the WS intensity techniques</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        public void AddWorkingSet(IdType workUnitId, WSRepetitionValue repetitions, RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null, IList<IdType> intensityTechniqueIds = null)
        {
            FindWorkUnitById(workUnitId).AddWorkingSet(
                    repetitions,
                    rest,
                    effort,
                    tempo,
                    intensityTechniqueIds
                );

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets(), GetMainEffortType());
        }


        /// <summary>
        /// Remove the Working Set from the Workout - The ID is unique in the Workout
        /// </summary>
        /// <param name="toRemoveId">The Id of the WS to be removed</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        public void RemoveWorkingSet(IdType toRemoveId)
        {
            WorkUnitTemplate toBeRemoved = FindWorkUnitByWorkingSetId(toRemoveId);

            toBeRemoved.RemoveWorkingSet(toRemoveId);

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets(), GetMainEffortType());
        }


        ///// <summary>
        ///// Remove the Working Set from the Work Unit
        ///// </summary>
        ///// <param name="wsPnum">The progressive number of the WS to be removed</param>
        //public void RemoveWorkingSet(int wuPnum, int wsPnum)
        //{
        //    WorkingSetTemplate toBeRemoved = FindWorkingSetByProgressiveNumber(wsPnum);

        //    _workUnits.Remove(toBeRemoved);

        //    TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(_workUnits);
        //    TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(_workUnits);
        //    TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(_workUnits, GetMainEffortType());

        //    ForceConsecutiveWorkUnitProgressiveNumbers();
        //    TestBusinessRules();
        //}
        #endregion



        #region Working Sets Methods

        /// <summary>
        /// Change the repetitions
        /// </summary>
        /// <param name="newReps">The new target repetitions</param>
        /// <param name="workingSetId">The Id of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If business rules not met</exception>
        /// <exception cref="ArgumentException">If WS not found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
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
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
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
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
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
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
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
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
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
        public bool RemoveWorkingSetIntensityTechnique(IdType workingSetId, IdType toRemoveId)
        {
            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);

            return ws.RemoveIntensityTechnique(toRemoveId);
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Build the next valid id
        /// </summary>
        /// <returns>The WU Id</returns>
        private IdType BuildWorkUnitId()
        {
            if (_workUnits.Count == 0)
                return new IdType(1);

            else
                return new IdType(_workUnits.Max(x => x.Id.Id) + 1);
        }


        /// <summary>
        /// Build the next valid progressive number
        /// To be used before adding the WS to the list
        /// </summary>
        /// <returns>The WS Progressive Number</returns>
        private uint BuildWorkUnitProgressiveNumber() => (uint)_workUnits.Count();


        /// <summary>
        /// Sort the Work Unit list wrt the WU progressive numbers
        /// </summary>
        /// <param name="wsIn">The input WU list</param>
        /// <returns>The sorted list</returns>
        private IEnumerable<WorkUnitTemplate> SortWorkUnitByProgressiveNumber(IEnumerable<WorkUnitTemplate> wsIn)

            => wsIn.OrderBy(x => x.ProgressiveNumber);


        /// <summary>
        /// Force the WSs to have consecutive progressive numbers
        /// It works by assuming that the WSs are added in a sorted fashion.
        /// </summary>
        private void ForceConsecutiveWorkUnitProgressiveNumbers()
        {
            _workUnits = SortWorkUnitByProgressiveNumber(_workUnits).ToList();

            // Just overwrite all the progressive numbers
            for (int iws = 0; iws < _workUnits.Count(); iws++)
            {
                WorkUnitTemplate ws = _workUnits[iws];
                ws.MoveToNewProgressiveNumber((uint)iws);
            }
        }


        /// <summary>
        /// Get the main effort type as the effort of most of the WSs of the WU 
        /// </summary>
        /// <returns>The training effort type</returns>
        private TrainingEffortTypeEnum GetMainEffortType()

            => _workUnits.Count == 0 ? TrainingEffortTypeEnum.IntensityPerc
                : _workUnits.SelectMany(x => x.WorkingSets).GroupBy(x => x.Effort.EffortType).Select(x
                     => new
                     {
                         Counter = x.Count(),
                         EffortType = x.Key
                     }).OrderByDescending(x => x.Counter).First().EffortType;


        #endregion


        #region Business Rules Validation

        /// <summary>
        /// The Workout must have no NULL Work Units.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullWorkunits() => _workUnits.All(x => x != null);


        /// <summary>
        /// Work Units of the same Workout must have consecutive progressive numbers.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool WorkUnitsWithConsecutiveProgressiveNumber()
        {
            if (_workUnits.Count == 0)
                return true;

            // Check the first element: the sequence must start from 0
            if (_workUnits?.Count() == 1)
            {
                if (_workUnits.FirstOrDefault()?.ProgressiveNumber == 0)
                    return true;
                else
                    return false;
            }

            // Look for non consecutive numbers - exclude the last one
            foreach (int pnum in _workUnits.Where(x => x.ProgressiveNumber != _workUnits.Count() - 1)
                .Select(x => x.ProgressiveNumber))
            {
                if (!_workUnits.Any(x => x.ProgressiveNumber == pnum + 1))
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
            if (!NoNullWorkunits())
                throw new TrainingDomainInvariantViolationException($"The Workout must have no NULL Work Units.");

            if (!WorkUnitsWithConsecutiveProgressiveNumber())
                throw new TrainingDomainInvariantViolationException($"Work Units of the same Workout must have consecutive progressive numbers.");
        }

        #endregion


        #region IClonable Implementation

        public object Clone()

            => PlanWorkout(Id, ProgressiveNumber, _workUnits, Name, SpecificWeekday);

        #endregion
    }
}
