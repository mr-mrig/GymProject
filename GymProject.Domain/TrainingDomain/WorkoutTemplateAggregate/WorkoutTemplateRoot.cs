using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate
{
    public class WorkoutTemplateRoot : Entity<IdTypeValue>, IAggregateRoot, ICloneable
    {


        ///// <summary>
        ///// The progressive number of the Workout - Starts from 0
        ///// </summary>
        //public uint ProgressiveNumber { get; private set; } = 0;


        /// <summary>
        /// The Workout name - Unique among the WOs of the Training Week
        /// </summary>
        public string Name { get; private set; } = string.Empty;


        /// <summary>
        /// The week day the Workout is scheduled to - if any
        /// </summary>
        public WeekdayEnum SpecificWeekday { get; private set; } = null;


        /// <summary>
        /// The training volume parameters, as the sum of the params of the single WUs
        /// </summary>
        public TrainingVolumeParametersValue TrainingVolume { get; private set; } = null;


        /// <summary>
        /// The training effort, as the average of the single WUs efforts
        /// </summary>
        public TrainingIntensityParametersValue TrainingIntensity { get; private set; } = null;


        /// <summary>
        /// The training density parameters, as the sum of the params of the single WUs
        /// </summary>
        public TrainingDensityParametersValue TrainingDensity { get; private set; } = null;


        private IList<WorkUnitTemplateEntity> _workUnits = new List<WorkUnitTemplateEntity>();

        /// <summary>
        /// The WUs belonging to the WorkOut
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<WorkUnitTemplateEntity> WorkUnits
        {
            get => _workUnits?.Clone().ToList().AsReadOnly() ?? new List<WorkUnitTemplateEntity>().AsReadOnly();
        }




        #region Ctors

        private WorkoutTemplateRoot(IdTypeValue id, IEnumerable<WorkUnitTemplateEntity> workUnits, string workoutName, WeekdayEnum weekday) : base(id)
        {
            Name = workoutName ?? string.Empty;
            SpecificWeekday = weekday ?? WeekdayEnum.Generic;

            _workUnits = workUnits?.Clone().ToList() ?? new List<WorkUnitTemplateEntity>();

            TestBusinessRules();

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets());
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets());
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method - Create a transient Workout
        /// </summary>
        /// <param name="weekday">The week day the Workout is scheduled to - if any</param>
        /// <param name="workoutName">The name given to the Workout - unique inside the Training Week</param>
        /// <param name="workUnits">The work unit list - cannot be empty or null</param>
        /// <returns>The WorkUnitTemplate instance</returns>
        public static WorkoutTemplateRoot PlanTransientWorkout(IEnumerable<WorkUnitTemplateEntity> workUnits, string workoutName, WeekdayEnum weekday = null)

            => new WorkoutTemplateRoot(null, workUnits, workoutName, weekday);


        /// <summary>
        /// Factory method - Load a persisted Workout
        /// </summary>
        /// <param name="id">The ID of the WU</param>
        /// <param name="weekday">The week day the Workout is scheduled to - if any</param>
        /// <param name="workoutName">The name given to the Workout - unique inside the Training Week</param>
        /// <param name="workUnits">The work unit list - cannot be empty or null</param>
        /// <returns>The WorkUnitTemplate instance</returns>
        public static WorkoutTemplateRoot PlanWorkout(IdTypeValue id, IEnumerable<WorkUnitTemplateEntity> workUnits, string workoutName, WeekdayEnum weekday = null)

            => new WorkoutTemplateRoot(id, workUnits, workoutName, weekday);

        #endregion



        #region Public Methods

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


        ///// <summary>
        ///// Find the WorkUnit with the ID specified
        ///// </summary>
        ///// <param name="id">The Id to be found</param>
        ///// <exception cref="ArgumentException">If ID could not be found</exception>
        ///// <exception cref="ArgumentNullException">If ID is NULL</exception>
        ///// <returns>The WorkingSetTemplate object/returns>
        //public WorkUnitTemplate FindWorkUnitById(IdTypeValue id)
        //{
        //    if (id == null)
        //        throw new ArgumentNullException($"Cannot find a WS with NULL id");

        //    WorkUnitTemplate ws = _workUnits.Where(x => x.Id == id).FirstOrDefault();

        //    if (ws == default)
        //        throw new ArgumentException($"Work Unit with Id {id.ToString()} could not be found");

        //    return ws;
        //}


        ///// <summary>
        ///// Find the Working Unit which the WS belongs
        ///// </summary>
        ///// <param name="wsId">The WS ID to be found</param>
        ///// <exception cref="ArgumentException">If ID could not be found</exception>
        ///// <exception cref="ArgumentNullException">If ID is NULL</exception>
        ///// <returns>The WorkUnitTemplate object/returns>
        //public WorkUnitTemplate FindWorkUnitByWorkingSetId(IdTypeValue wsId)
        //{
        //    if (wsId == null)
        //        throw new ArgumentNullException($"Cannot find a WS with NULL id");

        //    WorkUnitTemplate result = _workUnits.SingleOrDefault(wu => wu.WorkingSets.SingleOrDefault(ws => ws.Id == wsId) != default);

        //    if (result == default)
        //        throw new ArgumentException($"Working Set with Id {wsId.ToString()} could not be found");

        //    return result;
        //}


        ///// <summary>
        ///// Find the Working Set with the ID specified
        ///// </summary>
        ///// <param name="id">The Id to be found</param>
        ///// <exception cref="ArgumentException">If ID could not be found</exception>
        ///// <exception cref="ArgumentNullException">If ID is NULL</exception>
        ///// <returns>The WorkingSetTemplate object/returns>
        //public WorkingSetTemplate FindWorkingSetById(IdTypeValue id)
        //{
        //    if (id == null)
        //        throw new ArgumentNullException($"Cannot find a WS with NULL id");

        //    WorkingSetTemplate result = CloneAllWorkingSets().Where(x => x.Id == id).FirstOrDefault();

        //    if (result == default)
        //        throw new ArgumentException($"Working Set with Id {id.ToString()} could not be found");

        //    return result;
        //}



        /// <summary>
        /// Find the Working Unit with the progressive number specified - DEFAULT if not found
        /// </summary>
        /// <param name="workingSetPnum">The progressive number to be found</param>
        /// <exception cref="ArgumentNullException">If more elements with the specified Progressive Number are found</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found</returns>
        public WorkUnitTemplateEntity CloneWorkUnit(uint workingSetPnum)

            => FindWorkUnitOrDefault(workingSetPnum)?.Clone() as WorkUnitTemplateEntity;


        /// <summary>
        /// Get a copy of the Working Set with the ID specified - DEFAULT if not found
        /// </summary>
        /// <param name="workUnitPnum">The WU Progressive Number to be found</param>
        /// <param name="workingSetPnum">The WS Progressive Number to be found</param>
        /// <exception cref="ArgumentNullException">If more elements with the specified Progressive Number are found</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found</returns>
        public WorkingSetTemplateEntity CloneWorkingSet(uint workUnitPnum, uint workingSetPnum)

            => CloneWorkUnit(workUnitPnum)?.CloneWorkingSet(workingSetPnum) as WorkingSetTemplateEntity;


        /// <summary>
        /// Add the Working Unit (as a copy) to the Workout - if not already present
        /// </summary>
        /// <param name="toAdd">The Work Unit to be added</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        /// <exception cref="ArgumentException">If Work Unit already present</exception>
        public void AddWorkUnit(WorkUnitTemplateEntity toAdd)
        {
            if (_workUnits.Contains(toAdd))
                throw new ArgumentException("Trying to add a duplicate Work Unit", nameof(toAdd));

            _workUnits.Add(toAdd.Clone() as WorkUnitTemplateEntity);

            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
            TrainingVolume = TrainingVolume.AddWorkingSets(toAdd.WorkingSets);
            TrainingDensity = TrainingDensity.AddWorkingSets(toAdd.WorkingSets);
  
            TestBusinessRules();
        }


        /// <summary>
        /// Add the Working Unit to the Workout
        /// </summary>
        /// <param name="excerciseId">The ID of the excercise of the WU</param>
        /// <param name="ownerNoteId">The ID of the WU Owner's note</param>
        /// <param name="workingSets">The WS which the WU is made up of</param>
        /// <param name="workUnitIntensityTechniquesIds">The IDs of the WS intensity techniques</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void AddTransientWorkUnit(IdTypeValue excerciseId, IEnumerable<WorkingSetTemplateEntity> workingSets, IEnumerable<IdTypeValue> workUnitIntensityTechniquesIds = null, IdTypeValue ownerNoteId = null)
        {
            WorkUnitTemplateEntity toAdd = WorkUnitTemplateEntity.PlanTransientWorkUnit(
                BuildWorkUnitProgressiveNumber(),
                excerciseId,
                workingSets,
                workUnitIntensityTechniquesIds,
                ownerNoteId);

            _workUnits.Add(toAdd);

            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
            TrainingVolume = TrainingVolume.AddWorkingSets(toAdd.WorkingSets);
            TrainingDensity = TrainingDensity.AddWorkingSets(toAdd.WorkingSets);

            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Working Unit from the Workout
        /// </summary>
        /// <param name="toRemove">The WU to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        /// <exception cref="ArgumentException">If the Work Unit could not be found</exception>
        /// <returns>True if remove successful</returns>
        public void RemoveWorkUnit(WorkUnitTemplateEntity toRemove)
        {
            if (toRemove == null)
                return;

            RemoveWorkUnit(toRemove.ProgressiveNumber);
        }



        /// <summary>
        /// Remove the Working Unit from the Workout
        /// </summary>
        /// <param name="toRemovePnum">The Progressive Number of the WU to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        /// <exception cref="InvalidOperationException">If no Work Units or more than one with the specified Progressive Number</exception>
        public void RemoveWorkUnit(uint toRemovePnum)
        {
            WorkUnitTemplateEntity toBeRemoved = FindWorkUnit(toRemovePnum);

            bool removed = _workUnits.Remove(toBeRemoved);

            if(removed)
            {
                TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
                TrainingVolume = TrainingVolume.RemoveWorkingSets(toBeRemoved.WorkingSets);
                TrainingDensity = TrainingDensity.RemoveWorkingSets(toBeRemoved.WorkingSets);

                ForceConsecutiveWorkUnitProgressiveNumbers(toRemovePnum);
                TestBusinessRules();
            }
        }


        ///// <summary>
        ///// Remove the Working Unit from the Workout
        ///// </summary>
        ///// <param name="toRemoveId">The Id of the WU to be removed</param>
        ///// <exception cref="ArgumentException">If ID could not be found</exception>
        ///// <exception cref="ArgumentNullException">If ID is NULL</exception>
        //public void RemoveWorkUnit(IdTypeValue toRemoveId)
        //{
        //    WorkUnitTemplate toBeRemoved = FindWorkUnitById(toRemoveId);

        //    _workUnits.Remove(toBeRemoved);

        //    TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets());
        //    TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets());
        //    TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());

        //    ForceConsecutiveWorkUnitProgressiveNumbers();
        //    TestBusinessRules();
        //}


        /// <summary>
        /// Get a copy of the WSs of all the Work Units belonging to the Workout
        /// </summary>
        /// <returns>The list of the Working Sets</returns>
        public IEnumerable<WorkingSetTemplateEntity> CloneAllWorkingSets()

             => _workUnits.SelectMany(x => x.WorkingSets).Clone();


        /// <summary>
        /// Get the main effort type as the effort of most of the WSs of the WU 
        /// </summary>
        /// <returns>The training effort type</returns>
        public TrainingEffortTypeEnum GetMainEffortType()

            => _workUnits.Where(x => x != null).Count() == 0 ? TrainingEffortTypeEnum.IntensityPerc
                : _workUnits.SelectMany(x => x.WorkingSets).GroupBy(x => x.Effort.EffortType).Select(x
                     => new
                     {
                         Counter = x.Count(),
                         EffortType = x.Key
                     }).OrderByDescending(x => x.Counter).First().EffortType;


        [Obsolete("To be removed: this should be a presentation/application logic task",true)]
        /// <summary>
        /// Get the Intensity parameters computed over the specified excercises
        /// </summary>
        /// <param name="excerciseIdsList">The list of the IDs of the excercises</param>
        /// <returns>The training Intensity Parameters for the specified excercises</returns>
        public TrainingIntensityParametersValue GetIntensityByExcercises(IEnumerable<IdTypeValue> excerciseIdsList)

            => TrainingIntensityParametersValue.ComputeFromWorkingSets(
                _workUnits.Where(x => excerciseIdsList.Contains(x.ExcerciseId)).SelectMany(x => x.WorkingSets));


        #endregion


        #region Work Unit Methods


        /// <summary>
        /// Add the intensity technique to the selected Work Unit and all its WSs
        /// </summary>
        /// <param name="workUnitPnum">The Progressive Number of the Work Unit to be modified</param>
        /// <param name="intensityTechniqueId">The ID of the intensity technique</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void AddWorkUnitIntensityTechnique(uint workUnitPnum, IdTypeValue intensityTechniqueId)
        {
            WorkUnitTemplateEntity toBeModified = FindWorkUnit(workUnitPnum);

            toBeModified?.AddWorkUnitIntensityTechnique(intensityTechniqueId);
        }


        /// <summary>
        /// Remove the intensity technique from the selected Work Unit and all its WSs
        /// </summary>
        /// <param name="workUnitPnum">The Progressive Number of the Work Unit to be modified</param>
        /// <param name="intensityTechniqueId">The ID of the intensity technique</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        /// <returns>True if remove successfull</returns>
        public void RemoveWorkUnitIntensityTechnique(uint workUnitPnum, IdTypeValue intensityTechniqueId)
        {
            WorkUnitTemplateEntity toBeModified = FindWorkUnit(workUnitPnum);

            toBeModified.RemoveWorkUnitIntensityTechnique(intensityTechniqueId);
        }


        /// <summary>
        /// Attach a note to the WU, or repleace it if already present
        /// </summary>
        /// <param name="workUnitPnum">The Progressive Number of the Work Unit to be modified</param>
        /// <param name="newNoteId">The ID of the note to be attached</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void AssignWorkUnitNote(uint workUnitPnum, IdTypeValue newNoteId)

            => FindWorkUnit(workUnitPnum)?.AssignNote(newNoteId);


        /// <summary>
        /// Remove the note of the WU, or repleace it if already present
        /// </summary>
        /// <param name="workUnitPnum">The Progressive Number of the Work Unit to be changed</param>
        public void RemoveWorkUnitNote(uint workUnitPnum)

            => AssignWorkUnitNote(workUnitPnum, null);


        /// <summary>
        /// Assign the excercise to the WU, or repleace it if already present
        /// </summary>
        /// <param name="workUnitPnum">The Progressive Number of the Work Unit to be modified</param>
        /// <param name="newExcerciseId">The ID of the excercise to be attached</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void AssignWorkUnitExcercise(uint workUnitPnum, IdTypeValue newExcerciseId)

            => FindWorkUnit(workUnitPnum)?.AssignExcercise(newExcerciseId);


        /// <summary>
        /// Assign a new progressive number to the WU
        /// </summary>
        /// <param name="destPnum">The new progressive number - Pnums must be consecutive</param>
        /// <param name="srcPnum">The Progressive Number of the Work Unit to be moved</param>
        public void MoveWorkUnitToNewProgressiveNumber(uint srcPnum, uint destPnum)
        {
            WorkUnitTemplateEntity src = FindWorkUnit(srcPnum);
            WorkUnitTemplateEntity dest = FindWorkUnit(destPnum);


            // Switch sets
            src.MoveToNewProgressiveNumber(destPnum);
            dest.MoveToNewProgressiveNumber(srcPnum);

            ForceConsecutiveWorkUnitProgressiveNumbers();
            TestBusinessRules();
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



        #region Working Sets Method

        /// <summary>
        /// Add the Working Set to the Work Unit
        /// </summary>
        /// <param name="workUnitPnum">The Progressive Number of the WU which to add the WS to</param>
        /// <param name="repetitions">The WS repetitions</param>
        /// <param name="rest">The rest period between the WS and the following</param>
        /// <param name="effort">The WS effort</param>
        /// <param name="tempo">The WS lifting tempo</param>
        /// <param name="intensityTechniqueIds">The ids of the WS intensity techniques</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void AddTransientWorkingSet(uint workUnitPnum, WSRepetitionValue repetitions, 
            RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null, IList<IdTypeValue> intensityTechniqueIds = null)
        {
            WorkUnitTemplateEntity parentWorkUnit = FindWorkUnit(workUnitPnum);

            if (parentWorkUnit != default)
            {
                parentWorkUnit.AddTransientWorkingSet(
                    repetitions,
                    rest,
                    effort,
                    tempo,
                    intensityTechniqueIds
                );


                TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());

                WorkingSetTemplateEntity added = WorkingSetTemplateEntity.
                    PlanTransientWorkingSet(0, repetitions, rest, effort, tempo, intensityTechniqueIds);

                TrainingVolume = TrainingVolume.AddWorkingSet(added);
                TrainingDensity = TrainingDensity.AddWorkingSet(added);
            }
        }


        /// <summary>
        /// Add the Working Set to the Work Unit
        /// </summary>
        /// <param name="workUnitPnum">The Progressive Number of the WU which to add the WS to</param>
        /// <param name="workingSet">The WS instance</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void AddWorkingSet(uint workUnitPnum, WorkingSetTemplateEntity workingSet)
        {
            FindWorkUnit(workUnitPnum).AddWorkingSet(workingSet);

            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
            TrainingVolume = TrainingVolume.AddWorkingSet(workingSet);
            TrainingDensity = TrainingDensity.AddWorkingSet(workingSet);
        }


        /// <summary>
        /// Remove the Working Set from the Workout
        /// Transient entities cannot be removed this way, as the ID is null
        /// </summary>
        /// <param name="workingSet">The WS to be removed</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void RemoveWorkingSet(WorkingSetTemplateEntity workingSet)
        {
            // Transient entities
            if (workingSet == null)
                return;

            if(workingSet.IsTransient())
                throw new InvalidOperationException($"Cannot remove transient Working Sets");

            WorkUnitTemplateEntity parentWorkUnit = _workUnits.Single(x => x.WorkingSets.Contains(workingSet));

            RemoveWorkingSet(parentWorkUnit.ProgressiveNumber, workingSet.ProgressiveNumber);
        }

        /// <summary>
        /// Remove the Working Set from the Workout - If it could be found
        /// </summary>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU to be removed</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be removed</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void RemoveWorkingSet(uint parentWorkUnitPnum, uint workingSetPnum)
        {
            WorkUnitTemplateEntity parentWorkUnit = FindWorkUnit(parentWorkUnitPnum);
            WorkingSetTemplateEntity toRemove = parentWorkUnit.CloneWorkingSet(workingSetPnum);

            parentWorkUnit?.RemoveWorkingSet(workingSetPnum);

            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
            TrainingVolume = TrainingVolume.RemoveWorkingSet(toRemove);
            TrainingDensity = TrainingDensity.RemoveWorkingSet(toRemove);
        }


        /// <summary>
        /// Add the Intensity Technique to the Working Set of the Work Unit - if it could be found
        /// </summary>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which owns the WS</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be modifed</param>
        /// <param name="toAddId">The ID of the Intensity Technique to be added</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void AddWorkingSetIntensityTechnique(uint parentWorkUnitPnum, uint workingSetPnum, IdTypeValue toAddId)
        {
            WorkUnitTemplateEntity parentWorkUnit = FindWorkUnit(parentWorkUnitPnum);
            parentWorkUnit.AddWorkingSetIntensityTechnique(workingSetPnum, toAddId);
        }


        /// <summary>
        /// Remove an intensity technique from the WS, if it could be found
        /// </summary>
        /// <param name="parentWorkUnitPnum">The WU Progressive Number</param>
        /// <param name="workingSetPnum">The WS Progressive Number</param>
        /// <param name="toRemoveId">The id to be removed</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void RemoveWorkingSetIntensityTechnique(uint parentWorkUnitPnum, uint workingSetPnum, IdTypeValue toRemoveId)
        {
            WorkUnitTemplateEntity parentWorkUnit = FindWorkUnit(parentWorkUnitPnum);

            parentWorkUnit?.RemoveWorkingSetIntensityTechnique(workingSetPnum, toRemoveId);
        }


        /// <summary>
        /// Assign a new progressive number to the WS
        /// </summary>
        /// <param name="parentWorkUnitPnum">The WU Progressive Number</param>
        /// <param name="destPnum">The new progressive number - Pnums must be consecutive</param>
        /// <param name="srcPnum">The Progressive Number of the Working Set to be moved</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void MoveWorkingSetToNewProgressiveNumber(uint parentWorkUnitPnum, uint srcPnum, uint destPnum)
        {
            WorkUnitTemplateEntity parent = FindWorkUnit(parentWorkUnitPnum);

            parent.MoveWorkingSetToNewProgressiveNumber(srcPnum, destPnum);
   
        }


        /// <summary>
        /// Change the repetitions of the WS, if it could be found
        /// </summary>
        /// <param name="newReps">The new target repetitions</param>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which owns the WS</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be modifed</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void ChangeWorkingSetRepetitions(uint parentWorkUnitPnum, uint workingSetPnum, WSRepetitionValue newReps)
        {
            WorkingSetTemplateEntity ws = FindWorkingSet(parentWorkUnitPnum, workingSetPnum);

            ws?.ChangeRepetitions(newReps);
        }


        /// <summary>
        /// Change the effort of the WS, if it could be found
        /// </summary>
        /// <param name="newEffort">The new value</param>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which owns the WS</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be modifed</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void ChangeWorkingSetEffort(uint parentWorkUnitPnum, uint workingSetPnum, TrainingEffortValue newEffort)
        {
            WorkingSetTemplateEntity ws = FindWorkingSet(parentWorkUnitPnum, workingSetPnum);

            ws?.ChangeEffort(newEffort);
        }


        /// <summary>
        /// Change the rest period of the WS specified, if it could be found
        /// </summary>
        /// <param name="newRest">The new value</param>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which owns the WS</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be modifed</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void ChangeWorkingSetRestPeriod(uint parentWorkUnitPnum, uint workingSetPnum, RestPeriodValue newRest)
        {
            WorkingSetTemplateEntity ws = FindWorkingSet(parentWorkUnitPnum, workingSetPnum);

            ws?.ChangeRestPeriod(newRest);
        }


        /// <summary>
        /// Change the lifting tempo of the WS, if it could be found
        /// </summary>
        /// <param name="newTempo">The new value</param>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which owns the WS</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be modifed</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void ChangeWorkingSetLiftingTempo(uint parentWorkUnitPnum, uint workingSetPnum, TUTValue newTempo)
        {
            WorkingSetTemplateEntity ws = FindWorkingSet(parentWorkUnitPnum, workingSetPnum);

            ws?.ChangeLiftingTempo(newTempo);
        }

        #endregion


        #region Private Methods

        ///// <summary>
        ///// Build the next valid id
        ///// </summary>
        ///// <returns>The WU Id</returns>
        //private IdTypeValue BuildWorkUnitId()
        //{
        //    if (_workUnits.Count == 0)
        //        return IdTypeValue.Create(1);

        //    else
        //        return IdTypeValue.Create(_workUnits.Max(x => x.Id.Id) + 1);
        //}


        /// <summary>
        /// Build the next valid progressive number
        /// To be used before adding the WS to the list
        /// </summary>
        /// <returns>The WS Progressive Number</returns>
        private uint BuildWorkUnitProgressiveNumber() 
            
            => (uint)_workUnits.Count();


        /// <summary>
        /// Sort the Work Unit list wrt the WU progressive numbers
        /// </summary>
        /// <param name="wsIn">The input WU list</param>
        /// <returns>The sorted list</returns>
        private IEnumerable<WorkUnitTemplateEntity> SortWorkUnitByProgressiveNumber(IEnumerable<WorkUnitTemplateEntity> wsIn)

            => wsIn.OrderBy(x => x.ProgressiveNumber);


        /// <summary>
        /// Force the WUs to have consecutive progressive numbers
        /// It works by assuming that the WSs are added in a sorted fashion.
        /// </summary>
        private void ForceConsecutiveWorkUnitProgressiveNumbers()
        {
            _workUnits = SortWorkUnitByProgressiveNumber(_workUnits).ToList();

            // Just overwrite all the progressive numbers
            for (int iws = 0; iws < _workUnits.Count(); iws++)
            {
                WorkUnitTemplateEntity ws = _workUnits[iws];
                ws.MoveToNewProgressiveNumber((uint)iws);
            }
        }


        /// <summary>
        /// Force the WUs to have consecutive progressive numbers
        /// This algorithm is more efficient as it ignores the elments before pnum, provided that they are already sorted
        /// </summary>
        /// <param name="fromPnum">The Progressive number from which the order is not respected</param>
        private void ForceConsecutiveWorkUnitProgressiveNumbers(uint fromPnum)
        {
            //_workUnits = SortWorkUnitByProgressiveNumber(_workUnits).ToList();

            // Just overwrite all the progressive numbers
            for (int iws = (int)fromPnum; iws < _workUnits.Count(); iws++)
            {
                WorkUnitTemplateEntity ws = _workUnits[iws];
                ws.MoveToNewProgressiveNumber((uint)iws);
            }
        }

        /// <summary>
        /// Find the Working Unit with the progressive number specified - DEFAULT if not found
        /// </summary>
        /// <param name="workUnitPnum">The progressive number to be found</param>
        /// <exception cref="InvalidOperationException">If more Work Units with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found/returns>
        private WorkUnitTemplateEntity FindWorkUnitOrDefault(uint workUnitPnum)
        
            => _workUnits.SingleOrDefault(x => x.ProgressiveNumber == workUnitPnum);


        /// <summary>
        /// Find the Working Unit with the progressive number specified
        /// </summary>
        /// <param name="workUnitPnum">The progressive number to be found</param>
        /// <exception cref="InvalidOperationException">If no Work Units or more than one with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object/returns>
        private WorkUnitTemplateEntity FindWorkUnit(uint workUnitPnum)

            => _workUnits.SingleOrDefault(x => x.ProgressiveNumber == workUnitPnum);


        /// <summary>
        /// Find the Working Set according to the Progressive Numbers - DEFAULT if not found
        /// </summary>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which the WS belongs to</param>
        /// <param name="workingSetPnum">The Progressive Number of the Working Set to be found</param>
        /// <exception cref="InvalidOperationException">If more Work Units with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found/returns>
        private WorkingSetTemplateEntity FindWorkingSetOrDefault(uint parentWorkUnitPnum, uint workingSetPnum)
        {
            WorkUnitTemplateEntity parentWorkUnit = FindWorkUnitOrDefault(parentWorkUnitPnum);
            return parentWorkUnit?.WorkingSets.SingleOrDefault(x => x.ProgressiveNumber == workingSetPnum);
        }


        /// <summary>
        /// Find the Working Set according to the Progressive Numbers
        /// </summary>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which the WS belongs to</param>
        /// <param name="workingSetPnum">The Progressive Number of the Working Set to be found</param>
        /// <exception cref="InvalidOperationException">If no WOrk Unit or more than one with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object/returns>
        private WorkingSetTemplateEntity FindWorkingSet(uint parentWorkUnitPnum, uint workingSetPnum)
        {
            WorkUnitTemplateEntity parentWorkUnit = FindWorkUnit(parentWorkUnitPnum);
            return parentWorkUnit.WorkingSets.Single(x => x.ProgressiveNumber == workingSetPnum);
        }

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

            => PlanWorkout(Id, _workUnits, Name, SpecificWeekday);

        #endregion
    }
}
