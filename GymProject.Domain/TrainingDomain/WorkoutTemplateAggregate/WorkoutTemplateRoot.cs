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
    public class WorkoutTemplateRoot : Entity<uint?>, IAggregateRoot, ICloneable
    {




        /// <summary>
        /// The Workout progressive number
        /// </summary>
        public uint ProgressiveNumber { get; private set; }


        /// <summary>
        /// The Workout name
        /// </summary>
        public string Name { get; private set; } = string.Empty;


        /// <summary>
        /// The week day the Workout is scheduled to - if any
        /// </summary>
        public WeekdayEnum SpecificWeekday { get; private set; } = null;


        /// <summary>
        /// FK to the Training Plan Aggregate
        /// </summary>
        public uint? TrainingWeekId { get; private set; } = null;


        private List<WorkUnitTemplateEntity> _workUnits = new List<WorkUnitTemplateEntity>();

        /// <summary>
        /// The WUs belonging to the WorkOut
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<WorkUnitTemplateEntity> WorkUnits
        {
            get => _workUnits?.Clone().ToList().AsReadOnly()
                ?? new List<WorkUnitTemplateEntity>().AsReadOnly();
        }






        #region Ctors

        private WorkoutTemplateRoot() : base(null)
        {

        }

        private WorkoutTemplateRoot(uint? id, uint? trainingWeekId, uint progressiveNumber, IEnumerable<WorkUnitTemplateEntity> workUnits, string workoutName, WeekdayEnum weekday) 
            : base(id)
        {
            Name = workoutName ?? string.Empty;
            SpecificWeekday = weekday ?? WeekdayEnum.Generic;
            ProgressiveNumber = progressiveNumber;
            TrainingWeekId = trainingWeekId;

            _workUnits = workUnits?.Clone().ToList() ?? new List<WorkUnitTemplateEntity>();

            TestBusinessRules();

            //AddWorkoutTemplateCreatedDomainEvent();
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method - Create an empty Workout planned on the specified week
        /// </summary>
        /// <param name="trainingPlanId">The ID of the training plan</param>
        /// <param name="progressiveNumber">The Progressive Number</param>
        /// <param name="trainingWeekId">The ID of the Training Week which the Workout is planned to</param>
        /// <returns>The WorkoutTemplateRoot instance</returns>
        public static WorkoutTemplateRoot PlannedDraft(uint? trainingWeekId, uint progressiveNumber)

            //=> new WorkoutTemplateRoot();
            => PlanWorkout(null, trainingWeekId, progressiveNumber, null, null, null);


        /// <summary>
        /// Factory method - Create an empty Workout
        /// </summary>
        /// <param name="progressiveNumber">The Progressive Number</param>
        /// <returns>The WorkoutTemplateRoot instance</returns>
        public static WorkoutTemplateRoot UnplannedDraft(uint progressiveNumber)

            //=> new WorkoutTemplateRoot();
            => PlanWorkout(null, null, progressiveNumber, null, null, null);


        /// <summary>
        /// Factory method - Create a transient Workout
        /// </summary>
        /// <param name="progressiveNumber">The Progressive Number</param>
        /// <param name="weekday">The week day the Workout is scheduled to - if any</param>
        /// <param name="workoutName">The name given to the Workout - unique inside the Training Week</param>
        /// <param name="workUnits">The work unit list - cannot be empty or null</param>
        /// <returns>The WorkoutTemplateRoot instance</returns>
        public static WorkoutTemplateRoot PlanTransientWorkout(uint progressiveNumber, IEnumerable<WorkUnitTemplateEntity> workUnits, string workoutName, WeekdayEnum weekday = null)

            => PlanWorkout(null, null, progressiveNumber, workUnits, workoutName, weekday);


        /// <summary>
        /// Factory method - Load a persisted Workout
        /// </summary>
        /// <param name="id">The ID of the WU</param>
        /// <param name="progressiveNumber">The Progressive Number</param>
        /// <param name="weekday">The week day the Workout is scheduled to - if any</param>
        /// <param name="workoutName">The name given to the Workout - unique inside the Training Week</param>
        /// <param name="workUnits">The work unit list - cannot be empty or null</param>
        /// <returns>The WorkoutTemplateRoot instance</returns>
        public static WorkoutTemplateRoot PlanWorkout(uint? id, uint? trainingWeekId, uint progressiveNumber, IEnumerable<WorkUnitTemplateEntity> workUnits, string workoutName, WeekdayEnum weekday = null)

            => new WorkoutTemplateRoot(id, trainingWeekId, progressiveNumber, workUnits, workoutName, weekday);

        #endregion



        #region Public Methods

        /// <summary>
        /// Plan the Workout to the specified Training Week
        /// </summary>
        /// <param name="trainingWeekId">The ID of the Training Week</param>
        public void PlanToWeek(uint trainingWeekId)

            => TrainingWeekId = trainingWeekId;


        /// <summary>
        /// Change the Progressive Number
        /// </summary>
        /// <param name="progressiveNumber">The new name</param>
        public void MoveToNewProgressiveNumber(uint progressiveNumber)
        {
            ProgressiveNumber = progressiveNumber;
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
        /// Find the Working Unit with the progressive number specified - DEFAULT if not found
        /// </summary>
        /// <param name="workUnitProgressiveNumber">The progressive number to be found</param>
        /// <exception cref="InvalidOperationException">If more elements with the specified Progressive Number are found</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found</returns>
        public WorkUnitTemplateEntity CloneWorkUnit(uint workUnitProgressiveNumber)

            => FindWorkUnitOrDefault(workUnitProgressiveNumber)?.Clone() as WorkUnitTemplateEntity;


        /// <summary>
        /// Get a copy of the Working Set with the ID specified - DEFAULT if not found
        /// </summary>
        /// <param name="workUnitPnum">The WU Progressive Number to be found</param>
        /// <param name="workingSetPnum">The WS Progressive Number to be found</param>
        /// <exception cref="InvalidOperationException">If more elements with the specified Progressive Number are found</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found</returns>
        public WorkingSetTemplateEntity CloneWorkingSet(uint workUnitPnum, uint workingSetPnum)

            => CloneWorkUnit(workUnitPnum)?.CloneWorkingSet(workingSetPnum) as WorkingSetTemplateEntity;


        /// <summary>
        /// Get a copy of the last Working Set of the WorkUnit with the specified ID - DEFAULT if not found
        /// </summary>
        /// <param name="workUnitPnum">The WU Progressive Number to be found</param>
        /// <exception cref="InvalidOperationException">If more elements with the specified Progressive Number are found</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found</returns>
        public WorkingSetTemplateEntity CloneLastWorkingSet(uint workUnitPnum)
        {
            WorkUnitTemplateEntity workUnit = CloneWorkUnit(workUnitPnum);

            if (workUnit?.WorkingSets?.Count == 0)
                return null;

            return workUnit?.CloneWorkingSet((uint)workUnit.WorkingSets.Count - 1) as WorkingSetTemplateEntity;
        }



        /// <summary>
        /// Add the Working Unit (as a copy) to the Workout - if not already present
        /// </summary>
        /// <param name="toAdd">The Work Unit to be added</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        /// <exception cref="ArgumentException">If Work Unit already present</exception>
        public void PlanExcercise(WorkUnitTemplateEntity toAdd)
        {
            if (_workUnits.Contains(toAdd))
                throw new ArgumentException("Trying to add a duplicate Work Unit", nameof(toAdd));

            _workUnits.Add(toAdd.Clone() as WorkUnitTemplateEntity);

            TestBusinessRules();
        }


        /// <summary>
        /// Add the Working Unit to the Workout
        /// </summary>
        /// <param name="excerciseId">The ID of the excercise of the WU</param>
        /// <param name="ownerNoteId">The ID of the WU Owner's note</param>
        /// <param name="workingSets">The WS which the WU is made up of</param>
        /// <param name="linkedWorkUnitId">The ID of the Work Unit linked to the one to be planned - if any</param>
        /// <param name="linkingIntensityTechniqueId">The ID of the Intensity Technique which links the WUs - if any</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void PlanTransientExcercise(uint? excerciseId, IEnumerable<WorkingSetTemplateEntity> workingSets,
            uint? linkingIntensityTechniqueId = null, uint? ownerNoteId = null)
        {
            WorkUnitTemplateEntity toAdd = WorkUnitTemplateEntity.PlanTransientWorkUnit(
                BuildWorkUnitProgressiveNumber(),
                excerciseId,
                workingSets,
                linkingIntensityTechniqueId,
                ownerNoteId);

            _workUnits.Add(toAdd);

            TestBusinessRules();
        }


        /// <summary>
        /// Add the new excercise as a draft. Only the mandatory fields are provided
        /// </summary>
        /// <param name="excerciseId">The ID of the excercise of the WU</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void DraftExcercise(uint? excerciseId)
        {
            _workUnits.Add(
                WorkUnitTemplateEntity.NewDraft(BuildWorkUnitProgressiveNumber(), excerciseId));

            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Working Unit from the Workout
        /// </summary>
        /// <param name="toRemove">The WU to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        /// <exception cref="ArgumentException">If the Work Unit could not be found</exception>
        /// <returns>True if remove successful</returns>
        public void UnplanExcercise(WorkUnitTemplateEntity toRemove)
        {
            if (toRemove == null)
                return;

            uint workUnitPnum = toRemove.ProgressiveNumber;
            bool removed = _workUnits.Remove(toRemove);

            if (removed)
            {
                // Check for Work Units linked to the one removed
                if (workUnitPnum > 0)
                {
                    WorkUnitTemplateEntity previousWorkUnit = FindWorkUnit(workUnitPnum - 1);

                    //if (previousWorkUnit.HasLinkedUnit())
                    //{
                    //    // If nothing can be linked, then unlink
                    //    if (IsLastWorkUnit(toRemovePnum))
                    //        previousWorkUnit.Unlink();
                    //    else
                    //    {
                    //        // If the removed WU had a linked one, then link the two WUs
                    //        uint? removedLinkingIntensityTechnique = toBeRemoved.LinkingIntensityTechniqueId;

                    //        if (removedLinkingIntensityTechnique.HasValue)
                    //            previousWorkUnit.LinkTo(CloneWorkUnit(toRemovePnum).Id, removedLinkingIntensityTechnique.Value);

                    //        else
                    //            previousWorkUnit.Unlink();

                    //    }
                    //}
                    previousWorkUnit.Unlink();
                }

                ForceConsecutiveWorkUnitProgressiveNumbers(workUnitPnum);
                TestBusinessRules();
            }
        }


        /// <summary>
        /// Remove the Working Unit from the Workout
        /// </summary>
        /// <param name="toRemovePnum">The Progressive Number of the WU to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        /// <exception cref="InvalidOperationException">If no Work Units or more than one with the specified Progressive Number</exception>
        public void UnplanExcercise(uint toRemovePnum)
        {
            UnplanExcercise(FindWorkUnit(toRemovePnum));

        }


        /// <summary>
        /// Remove the Working Unit from the Workout
        /// </summary>
        /// <param name="toRemoveId">The ID of the WU to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        /// <exception cref="InvalidOperationException">If no Work Units or more than one with the specified Progressive Number</exception>
        public void UnplanExcerciseById(uint toRemoveId)
        {
            UnplanExcercise(FindWorkUnitById(toRemoveId));
        }


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

            //=> _workUnits.Count(x => x?.WorkingSets?.Count(y => y.Effort != null) > 0) == 0     // No Working Sets with Effort specified?
            //    ? TrainingEffortTypeEnum.IntensityPercentage
            //    : _workUnits.SelectMany(x => x.WorkingSets).GroupBy(x => x.Effort.EffortType).Select(x
            //         => new
            //         {
            //             Counter = x.Count(),
            //             EffortType = x.Key
            //         }).OrderByDescending(x => x.Counter).First().EffortType;

            => _workUnits.SelectMany(x => x.WorkingSets).GroupBy(x => x.ToEffort().EffortType).Select(x
                    => new
                    {
                        Counter = x.Count(),
                        EffortType = x.Key
                    })
                    .OrderByDescending(x => x.Counter).FirstOrDefault()?.EffortType
                ?? TrainingEffortTypeEnum.IntensityPercentage;


        [Obsolete("To be removed: this should be a presentation/application logic task", true)]
        /// <summary>
        /// Get the Intensity parameters computed over the specified excercises
        /// </summary>
        /// <param name="excerciseIdsList">The list of the IDs of the excercises</param>
        /// <returns>The training Intensity Parameters for the specified excercises</returns>
        public TrainingIntensityParametersValue GetIntensityByExcercises(IEnumerable<uint?> excerciseIdsList)

            => TrainingIntensityParametersValue.ComputeFromWorkingSets(
                _workUnits.Where(x => excerciseIdsList.Contains(x.ExcerciseId)).SelectMany(x => x.WorkingSets));


        #endregion


        #region Work Unit Methods


        /// <summary>
        /// Link the Work Unit to the following one with the specified Intensity Technique
        /// </summary>
        /// <param name="startingWorkUnitPNum">The Progressive Number of the Work Unit to be performed before</param>
        /// <param name="linkedWorkUnitPNum">The Progressive Number of the Work Unit to be performed after - Must be different from the first one</param>
        /// <param name="intensityTechniqueId">The ID of the intensity technique</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum, or if the PNums are the same</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void LinkWorkUnits(uint startingWorkUnitPNum, uint intensityTechniqueId)
        {
            if (IsLastWorkUnit(startingWorkUnitPNum))
                throw new InvalidOperationException($"There's no Work Unit to link to the specified one as it is the last one.");

            WorkUnitTemplateEntity startingWorkUnit = FindWorkUnit(startingWorkUnitPNum);
            //WorkUnitTemplateEntity linkedWorkUnit = FindWorkUnit(startingWorkUnitPNum + 1);

            //startingWorkUnit.LinkTo(linkedWorkUnit.Id, intensityTechniqueId);
            startingWorkUnit.LinkToNext(intensityTechniqueId);
        }


        /// <summary>
        /// Remove the link between the selected Work Unit and the one linked to it
        /// </summary>
        /// <param name="startingWorkUnitPnum">The Progressive Number of the Work Unit which starts the linked group</param>
        /// <exception cref="InvalidOperationException">Thrown if no Work Unit or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        /// <returns>True if remove successfull</returns>
        public void UnlinkWorkUnits(uint startingWorkUnitPnum)

            => FindWorkUnit(startingWorkUnitPnum).Unlink();


        /// <summary>
        /// Attach a note to the WU, or repleace it if already present
        /// </summary>
        /// <param name="workUnitPnum">The Progressive Number of the Work Unit to be modified</param>
        /// <param name="newNoteId">The ID of the note to be attached</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void AttachWorkUnitNote(uint workUnitPnum, uint? newNoteId)

            => FindWorkUnit(workUnitPnum)?.AssignNote(newNoteId);


        /// <summary>
        /// Remove the note of the WU, or repleace it if already present
        /// </summary>
        /// <param name="workUnitPnum">The Progressive Number of the Work Unit to be changed</param>
        public void DetachWorkUnitNote(uint workUnitPnum)

            => AttachWorkUnitNote(workUnitPnum, null);


        /// <summary>
        /// Assign the excercise to the WU, or repleace it if already present
        /// </summary>
        /// <param name="workUnitPnum">The Progressive Number of the Work Unit to be modified</param>
        /// <param name="newExcerciseId">The ID of the excercise to be attached</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void AssignWorkUnitExcercise(uint workUnitPnum, uint? newExcerciseId)

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
        public void AddTransientWorkingSet(uint workUnitPnum, WSRepetitionsValue repetitions,
            RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null, IEnumerable<uint?> intensityTechniqueIds = null)
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

                ManageLinkedWorkingSetsOnAdd(parentWorkUnit);
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
            WorkUnitTemplateEntity parentWorkUnit = FindWorkUnit(workUnitPnum);
            parentWorkUnit.AddWorkingSet(workingSet);

            ManageLinkedWorkingSetsOnAdd(parentWorkUnit);
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

            if (workingSet.IsTransient())
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

            parentWorkUnit.RemoveWorkingSet(workingSetPnum);

            ManageLinkedWorkingSetsOnRemove(parentWorkUnit, workingSetPnum);
        }


        ///// <summary>
        ///// Remove the Working Set from the Workout - If it could be found
        ///// </summary>
        ///// <param name="workingSetId">The ID of the WS to be removed</param>
        ///// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        ///// <exception cref="TrainingDomainInvariantViolationException"></exception>
        //public void RemoveWorkingSet(uint workingSetId)
        //{
        //    WorkingSetTemplateEntity toRemove = FindWorkingSetById(workingSetId);

        //    parentWorkUnit.RemoveWorkingSet(workingSetPnum);

        //    ManageLinkedWorkingSetsOnRemove(parentWorkUnit, workingSetPnum);

        //    TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
        //    TrainingVolume = TrainingVolume.RemoveWorkingSet(toRemove);
        //    TrainingDensity = TrainingDensity.RemoveWorkingSet(toRemove);
        //}


        /// <summary>
        /// Add the Intensity Technique to the Working Set of the Work Unit - if it could be found
        /// </summary>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which owns the WS</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be modifed</param>
        /// <param name="toAddId">The ID of the Intensity Technique to be added</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void AddWorkingSetIntensityTechnique(uint parentWorkUnitPnum, uint workingSetPnum, uint toAddId)
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
        public void RemoveWorkingSetIntensityTechnique(uint parentWorkUnitPnum, uint workingSetPnum, uint toRemoveId)
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


        public void ReviseWorkingSet(uint workingSetId, WorkingSetTemplateEntity newWorkingSet)
        {
            WorkingSetTemplateEntity workingSet = FindWorkingSetById(workingSetId);

            workingSet.ReviseRepetitions(newWorkingSet.Repetitions);
            workingSet.ReviseRestPeriod(newWorkingSet.Rest);
            workingSet.ReviseEffort(newWorkingSet.Effort);
            workingSet.ReviseLiftingTempo(newWorkingSet.Tempo);
            workingSet.AssignIntensityTechniques(newWorkingSet.IntensityTechniqueIds);
        }


        /// <summary>
        /// Change the repetitions of the WS, if it could be found
        /// </summary>
        /// <param name="newReps">The new target repetitions</param>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which owns the WS</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be modifed</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void ReviseWorkingSetRepetitions(uint parentWorkUnitPnum, uint workingSetPnum, WSRepetitionsValue newReps)
        {
            FindWorkUnit(parentWorkUnitPnum).ReviseWorkingSetRepetitions(workingSetPnum, newReps);
        }


        /// <summary>
        /// Change the effort of the WS, if it could be found
        /// </summary>
        /// <param name="newEffort">The new value</param>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which owns the WS</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be modifed</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void ReviseWorkingSetEffort(uint parentWorkUnitPnum, uint workingSetPnum, TrainingEffortValue newEffort)
        {
            FindWorkUnit(parentWorkUnitPnum).ReviseWorkingSetEffort(workingSetPnum, newEffort);
        }


        /// <summary>
        /// Change the rest period of the WS specified, if it could be found
        /// </summary>
        /// <param name="newRest">The new value</param>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which owns the WS</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be modifed</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void ReviseWorkingSetRestPeriod(uint parentWorkUnitPnum, uint workingSetPnum, RestPeriodValue newRest)
        {
            FindWorkUnit(parentWorkUnitPnum).ReviseWorkingSetRestPeriod(workingSetPnum, newRest);
        }


        /// <summary>
        /// Change the lifting tempo of the WS, if it could be found
        /// </summary>
        /// <param name="newTempo">The new value</param>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which owns the WS</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be modifed</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void ReviseWorkingSetLiftingTempo(uint parentWorkUnitPnum, uint workingSetPnum, TUTValue newTempo)
        {
            FindWorkUnit(parentWorkUnitPnum).ReviseWorkingSetLiftingTempo(workingSetPnum, newTempo);
        }

        #endregion


        #region COMMENTED - Linked Working Set Management, might be needed at a later stage

        ///// <summary>
        ///// Link the specified Working Set to the following one
        ///// </summary>
        ///// <param name="parentWorkUnitPnum">The Progressive Number of the WU which owns the WS</param>
        ///// <param name="startingWorkingSetPnum">The Progressive Number of the WS to be modifed</param>
        ///// <param name="linkingIntensityTechnique">The ID of the Intensity Technique which links the WSs</param>
        ///// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        ///// <exception cref="TrainingDomainInvariantViolationException"></exception>
        //public void LinkWorkingSet(uint parentWorkUnitPnum, uint startingWorkingSetPnum, uint linkingIntensityTechnique)
        //{
        //    FindWorkUnit(parentWorkUnitPnum).LinkWorkingSet(startingWorkingSetPnum, linkingIntensityTechnique);
        //}


        ///// <summary>
        ///// Unlink the specified working set from the following one
        ///// </summary>
        ///// <param name="parentWorkUnitPnum">The WU Progressive Number</param>
        ///// <param name="startingWorkingSetPnum">The WS Progressive Number</param>
        ///// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        ///// <exception cref="TrainingDomainInvariantViolationException"></exception>
        //public void UninkWorkingSet(uint parentWorkUnitPnum, uint startingWorkingSetPnum)
        //{
        //    WorkUnitTemplateEntity parentWorkUnit = FindWorkUnit(parentWorkUnitPnum);
        //    parentWorkUnit.UnlinkWorkingSet(startingWorkingSetPnum);
        //}


        private void ManageLinkedWorkingSetsOnAdd(WorkUnitTemplateEntity workUnit)
        {
            //uint workUnitPnum = workUnit.ProgressiveNumber;

            //// Adding a WS to a linked WU implies to link the two WSs also
            //if (workUnitPnum > 0)
            //{
            //    uint addedWorkingSetPnum = (uint)workUnit.WorkingSets.Count - 1;

            //    WorkUnitTemplateEntity previousWorkUnit = FindWorkUnit(workUnitPnum - 1);

            //    if (previousWorkUnit.HasLinkedUnit()
            //        && previousWorkUnit.WorkingSets.Count > addedWorkingSetPnum)  // Check if valid operation

            //        previousWorkUnit.LinkWorkingSet(addedWorkingSetPnum, previousWorkUnit.LinkedWorkUnit.LinkingIntensityTechniqueId.Value);
            //}
        }

        private void ManageLinkedWorkingSetsOnRemove(WorkUnitTemplateEntity parentWorkUnit, uint removedWorkingSetPnum)
        {
            //uint parentWorkUnitPnum = parentWorkUnit.ProgressiveNumber;

            //// Look for the WU which was linked to the one which has one less WS
            //if (parentWorkUnitPnum > 0)
            //{
            //    WorkUnitTemplateEntity previousWorkUnit = FindWorkUnit(parentWorkUnitPnum - 1);

            //    if (previousWorkUnit.HasLinkedUnit()
            //        && previousWorkUnit.WorkingSets.Count > removedWorkingSetPnum)    // Make sure the operation is valid

            //        previousWorkUnit.UnlinkWorkingSet(removedWorkingSetPnum);
            //}
        }
        #endregion



        #region Private Methods

        /// <summary>
        /// Build the next valid progressive number
        /// To be used before adding the WS to the list
        /// </summary>
        /// <returns>The WS Progressive Number</returns>
        private uint BuildWorkUnitProgressiveNumber() 
            
            => (uint)_workUnits.Count();


        /// <summary>
        /// Check wether the WU with the specified progressive number is the last one of the Workout
        /// </summary>
        /// <param name="workUnitProgressiveNumber">The progressive numebr of the WU to be checked</param>
        /// <returns>True if the WS is the last one</returns>
        private bool IsLastWorkUnit(uint workUnitProgressiveNumber)

            => workUnitProgressiveNumber == _workUnits.Count - 1;


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

            => _workUnits.Single(x => x.ProgressiveNumber == workUnitPnum);


        /// <summary>
        /// Find the Working Unit with the ID specified
        /// </summary>
        /// <param name="workUnitId">The ID to be found</param>
        /// <exception cref="InvalidOperationException">If no Work Units or more than one with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object/returns>
        private WorkUnitTemplateEntity FindWorkUnitById(uint workUnitId)

            => _workUnits.Single(x => x.Id == workUnitId);


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


        /// <summary>
        /// Find the Working Set according to the Progressive Numbers
        /// </summary>
        /// <param name="workingSetId">The ID of the Working Set to be found</param>
        /// <exception cref="InvalidOperationException">If no WOrk Unit or more than one with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object/returns>
        private WorkingSetTemplateEntity FindWorkingSetById(uint workingSetId)

            => _workUnits.SelectMany(x => x.WorkingSets).Single(x => x.Id == workingSetId);


        #endregion


        #region Business Rules Validation

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
            if (!WorkUnitsWithConsecutiveProgressiveNumber())
                throw new TrainingDomainInvariantViolationException($"Work Units of the same Workout must have consecutive progressive numbers.");
        }

        #endregion



        #region Domain Events

        //public void AddWorkoutTemplateCreatedDomainEvent()
        //{
        //    AddDomainEvent(new WorkoutTemplateCreatedDomainEvent(Id, 1, 1));    // TODO
        //}

        #endregion


        #region ICloneable Implementation

        public object Clone()

            => PlanWorkout(Id, TrainingWeekId, ProgressiveNumber, _workUnits, Name, SpecificWeekday);

        #endregion
    }
}
