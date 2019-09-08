using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.Utils;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GymProject.Domain.TrainingDomain.WorkoutSessionAggregate
{

    public class WorkoutSessionRoot : Entity<uint?>, IAggregateRoot, ICloneable
    {






        /// <summary>
        /// The date the Workout has been planned at
        /// </summary>
        public DateTime? PlannedDate { get; private set; }


        /// <summary>
        /// The Workout starting hour
        /// </summary>
        public DateTime? StartTime { get; private set; }


        /// <summary>
        /// The Workout end time
        /// </summary>
        public DateTime? EndTime { get; private set; }


        /// <summary>
        /// The training volume parameters, as the sum of the params of the single WSs
        /// </summary>
        public TrainingVolumeParametersValue TrainingVolume { get; private set; } = null;


        /// <summary>
        /// FK to the Workout Template - Optional in order to allow On-the-Fly sessions
        /// </summary>
        public uint? WorkoutTemplateId { get; private set; } = null;


        private IList<WorkUnitEntity> _workUnits = new List<WorkUnitEntity>();

        /// <summary>
        /// The WUs belonging to the Workout
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<WorkUnitEntity> WorkUnits
        {
            get => _workUnits?.ToList().AsReadOnly()
                ?? new List<WorkUnitEntity>().AsReadOnly();
        }



        #region Ctors

        private WorkoutSessionRoot(uint? id, DateTime? startTime, DateTime? endTime, DateTime? plannedDate, uint? workoutTemplateId, IEnumerable<WorkUnitEntity> workUnits) 
            : base(id)
        {
            StartTime = startTime;
            EndTime = endTime;
            PlannedDate = plannedDate;
            WorkoutTemplateId = workoutTemplateId;

            _workUnits = workUnits?.Clone().ToList() ?? new List<WorkUnitEntity>();

            TestBusinessRules();

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets());
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method Transient - Start trcking a workout
        /// </summary>
        /// <param name="startTime">The Session start time</param>
        /// <param name="workoutTemplateId">The ID of the Workout Template which the Session refers to - optional to allow on-the-fly Sessions</param>
        /// <returns>The WorkoutSessionRoot instance</returns>
        public static WorkoutSessionRoot BeginWorkout(DateTime? startTime, uint? workoutTemplateId = null)

            => TrackWorkout(null, startTime, null, null, workoutTemplateId, null);


        /// <summary>
        /// Factory method- Start trcking a workout
        /// </summary>
        /// <param name="id">The ID of the WU</param>
        /// <param name="startTime">The Session start time</param>
        /// <param name="workoutTemplateId">The ID of the Workout Template which the Session refers to - optional to allow on-the-fly Sessions</param>
        /// <returns>The WorkoutSessionRoot instance</returns>
        public static WorkoutSessionRoot BeginWorkout(uint? id, DateTime? startTime, uint? workoutTemplateId = null)

            => TrackWorkout(id, startTime, null, null, workoutTemplateId, null);


        /// <summary>
        /// Factory method - Schedule the workout
        /// </summary>
        /// <param name="id">The ID of the WU</param>
        /// <param name="plannedDate">The date which the workout has been scheduled to</param>
        /// <param name="workoutTemplateId">The ID of the Workout Template which the Session refers to - optional to allow on-the-fly Sessions</param>
        /// <returns>The WorkoutSessionRoot instance</returns>
        public static WorkoutSessionRoot ScheduleWorkout(uint? id, DateTime? plannedDate, uint? workoutTemplateId)

            => TrackWorkout(id, null, null, plannedDate, workoutTemplateId, null);


        /// <summary>
        /// Factory method Transient - Schedule the workout
        /// </summary>
        /// <param name="plannedDate">The date which the workout has been scheduled to</param>
        /// <param name="workoutTemplateId">The ID of the Workout Template which the Session refers to - optional to allow on-the-fly Sessions</param>
        /// <returns>The WorkoutSessionRoot instance</returns>
        public static WorkoutSessionRoot ScheduleWorkout(DateTime? plannedDate, uint? workoutTemplateId)

            => TrackWorkout(null, null, null, plannedDate, workoutTemplateId, null);


        /// <summary>
        /// Factory method - Load a persisted Workout
        /// </summary>
        /// <param name="id">The ID of the WU</param>
        /// <param name="startTime">The Session start time</param>
        /// <param name="endTime">The Session end time</param>
        /// <param name="plannedDate">The date which the workout has been scheduled to</param>
        /// <param name="workoutTemplateId">The ID of the Workout Template which the Session refers to - optional to allow on-the-fly Sessions</param>
        /// <param name="workUnits">The work unit list</param>
        /// <returns>The WorkoutSessionRoot instance</returns>
        public static WorkoutSessionRoot TrackWorkout(
            uint? id, DateTime? startTime, DateTime? endTime, DateTime? plannedDate, uint? workoutTemplateId = null, IEnumerable<WorkUnitEntity> workUnits = null)

            => new WorkoutSessionRoot(id, startTime, endTime, plannedDate, workoutTemplateId, workUnits);

        #endregion



        #region Public Methods

        /// <summary>
        /// Schedule the Workout to a specific day
        /// </summary>
        /// <param name="plannedDate">The date which the workout has been scheduled to - must be in the future</param>
        /// <exception cref="InvalidOperationException">If the specified date belongs to the past</exception>
        public void ScheduleToDate(DateTime plannedDate)
        {
            PlannedDate = plannedDate;

            if (PlannedDate < DateTime.Today)
                throw new InvalidOperationException("Cannot schedule a Workout Session in the past");
        }


        /// <summary>
        /// Mark the WO as 'not scheduled to specific day'
        /// </summary>
        public void Unschedule() => PlannedDate = null;


        /// <summary>
        /// Finish the workout at the specified time
        /// </summary>
        public void FinishWorkout(DateTime endTime) => EndTime = endTime;


        /// <summary>
        /// Find the Working Unit with the progressive number specified - DEFAULT if not found
        /// </summary>
        /// <param name="workingSetPnum">The progressive number to be found</param>
        /// <exception cref="ArgumentNullException">If more elements with the specified Progressive Number are found</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found</returns>
        public WorkUnitEntity CloneWorkUnit(uint workingSetPnum)

            => FindWorkUnitOrDefault(workingSetPnum)?.Clone() as WorkUnitEntity;


        /// <summary>
        /// Add the Working Unit (as a copy) to the Workout - if not already present
        /// </summary>
        /// <param name="workUnit">The Work Unit to be added</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        /// <exception cref="ArgumentException">If Work Unit already present</exception>
        public void TrackExcercise(WorkUnitEntity workUnit)
        {
            if (_workUnits.Contains(workUnit))
                throw new ArgumentException("Trying to add a duplicate Work Unit", nameof(workUnit));

            _workUnits.Add(workUnit.Clone() as WorkUnitEntity);

            TrainingVolume = TrainingVolume.AddWorkingSets(workUnit.WorkingSets);

            TestBusinessRules();
        }


        /// <summary>
        /// Start tracking the excercise - Transient
        /// </summary>
        /// <param name="excerciseId">The ID of the excercise of the WU</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void StartTrackingExcercise(uint? excerciseId)
        {
            WorkUnitEntity toAdd = WorkUnitEntity.StartExcercise(
                BuildWorkUnitProgressiveNumber(),
                excerciseId);

            _workUnits.Add(toAdd);
            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Working Unit from the Workout
        /// </summary>
        /// <param name="toRemove">The WU to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        /// <exception cref="ArgumentException">If the Work Unit could not be found</exception>
        /// <returns>True if remove successful</returns>
        public void UntrackExcercise(WorkUnitEntity toRemove)
        {
            if (toRemove == null)
                return;

            UntrackExcercise(toRemove.ProgressiveNumber);
        }


        /// <summary>
        /// Remove the Working Unit from the Workout
        /// </summary>
        /// <param name="toRemovePnum">The Progressive Number of the WU to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        /// <exception cref="InvalidOperationException">If no Work Units or more than one with the specified Progressive Number</exception>
        public void UntrackExcercise(uint toRemovePnum)
        {
            WorkUnitEntity toBeRemoved = FindWorkUnit(toRemovePnum);

            bool removed = _workUnits.Remove(toBeRemoved);

            if (removed)
            {
                TrainingVolume = TrainingVolume.RemoveWorkingSets(toBeRemoved.WorkingSets);

                ForceConsecutiveWorkUnitProgressiveNumbers(toRemovePnum);
                TestBusinessRules();
            }
        }

        /// <summary>
        /// Get a copy of the WSs of all the Work Units belonging to the Workout
        /// </summary>
        /// <returns>The list of the Working Sets</returns>
        public IEnumerable<WorkingSetEntity> CloneAllWorkingSets()

             => _workUnits.SelectMany(x => x.WorkingSets).Clone();

        #endregion


        #region Work Unit Methods

        /// <summary>
        /// Give a rating to the Performance when the Work Unit has been completed
        /// </summary>
        /// <param name="workUnitPnum">The Progressive Number of the Work Unit to be modified</param>
        /// <param name="workUnitRating">The rating</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void RatePerformance(uint workUnitPnum, RatingValue workUnitRating)

            => FindWorkUnit(workUnitPnum)?.RatePerformance(workUnitRating);


        /// <summary>
        /// Assign a new progressive number to the WU
        /// </summary>
        /// <param name="destPnum">The new progressive number - Pnums must be consecutive</param>
        /// <param name="srcPnum">The Progressive Number of the Work Unit to be moved</param>
        public void MoveWorkUnitToNewProgressiveNumber(uint srcPnum, uint destPnum)
        {
            WorkUnitEntity src = FindWorkUnit(srcPnum);
            WorkUnitEntity dest = FindWorkUnit(destPnum);

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
        /// <param name="repetitionsPerformed">The WS repetitions</param>
        /// <param name="weightLifted">The weight lifted</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void TrackWorkingSet(uint workUnitPnum, WSRepetitionsValue repetitionsPerformed, WeightPlatesValue weightLifted)
        {
            WorkUnitEntity parentWorkUnit = FindWorkUnit(workUnitPnum);

            parentWorkUnit.TrackWorkingSet(
                repetitionsPerformed,
                weightLifted
            );

            WorkingSetEntity added = WorkingSetEntity.CompleteWorkingSet(0, repetitionsPerformed, weightLifted);

            TrainingVolume = TrainingVolume.AddWorkingSet(added);
        }


        /// <summary>
        /// Add the Working Set to the Work Unit
        /// </summary>
        /// <param name="workUnitPnum">The Progressive Number of the WU which to add the WS to</param>
        /// <param name="workingSet">The WS instance</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void TrackWorkingSet(uint workUnitPnum, WorkingSetEntity workingSet)
        {
            FindWorkUnit(workUnitPnum).TrackWorkingSet(workingSet);

            TrainingVolume = TrainingVolume.AddWorkingSet(workingSet);
        }


        /// <summary>
        /// Remove the Working Set from the Workout
        /// Transient entities cannot be removed this way, as the ID is null
        /// </summary>
        /// <param name="workingSet">The WS to be removed</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        /// <exception cref="ArgumentNullException">If NULL input</exception>
        public void UntrackWorkingSet(WorkingSetEntity workingSet)
        {
            if (workingSet == null)
                throw new ArgumentNullException(nameof(workingSet), $"Cannot remove a NULL Working Set");

            if (workingSet.IsTransient())
                throw new InvalidOperationException($"Cannot remove transient Working Sets");

            WorkUnitEntity parentWorkUnit = _workUnits.Single(x => x.WorkingSets.Contains(workingSet));

            UntrackWorkingSet(parentWorkUnit.ProgressiveNumber, workingSet.ProgressiveNumber);
        }

        /// <summary>
        /// Remove the Working Set from the Workout - If it could be found
        /// </summary>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU to be removed</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be removed</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void UntrackWorkingSet(uint parentWorkUnitPnum, uint workingSetPnum)
        {
            WorkUnitEntity parentWorkUnit = FindWorkUnit(parentWorkUnitPnum);
            WorkingSetEntity toRemove = parentWorkUnit.CloneWorkingSet(workingSetPnum);

            parentWorkUnit?.UntrackWorkingSet(workingSetPnum);

            TrainingVolume = TrainingVolume.RemoveWorkingSet(toRemove);
        }


        /// <summary>
        /// Write a note about the specified Working Set
        /// </summary>
        /// <param name="parentWorkUnitPnum">The WU Progressive Number</param>
        /// <param name="workingSetPnum">The Progressive Number of the Working Set</param>
        /// <param name="noteId">The ID of the Note to be attached</param>
        public void WriteWorkingSetNote(uint parentWorkUnitPnum, uint workingSetPnum, uint? noteId)
        {
            WorkUnitEntity parentWorkUnit = FindWorkUnit(parentWorkUnitPnum);
            WorkingSetEntity toChange = parentWorkUnit.CloneWorkingSet(workingSetPnum);

            toChange.WriteNote(noteId);
        }


        /// <summary>
        /// Clear the note of the specifed Working Set
        /// </summary>
        /// <param name="parentWorkUnitPnum">The WU Progressive Number</param>
        /// <param name="workingSetPnum">The Progressive Number of the Working Set</param>
        public void WriteWorkingSetNote(uint parentWorkUnitPnum, uint workingSetPnum)
        {
            WorkUnitEntity parentWorkUnit = FindWorkUnit(parentWorkUnitPnum);
            WorkingSetEntity toChange = parentWorkUnit.CloneWorkingSet(workingSetPnum);

            toChange.ClearNote();
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
            WorkUnitEntity parentUnit = FindWorkUnit(parentWorkUnitPnum);

            TrainingVolume = TrainingVolume.RemoveWorkingSet(parentUnit.CloneWorkingSet(workingSetPnum));

            parentUnit.ReviseWorkingSetRepetitions(workingSetPnum, newReps);

            TrainingVolume = TrainingVolume.AddWorkingSet(parentUnit.CloneWorkingSet(workingSetPnum));
        }


        /// <summary>
        /// Change the weight lifted in the WS, if it could be found
        /// </summary>
        /// <param name="newWeight">The new value</param>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which owns the WS</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be modifed</param>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void ReviseWorkingSetLoad(uint parentWorkUnitPnum, uint workingSetPnum, WeightPlatesValue newWeight)
        {
            WorkUnitEntity parentUnit = FindWorkUnit(parentWorkUnitPnum);

            TrainingVolume = TrainingVolume.RemoveWorkingSet(parentUnit.CloneWorkingSet(workingSetPnum));

            parentUnit.ReviseWorkingSetLoad(workingSetPnum, newWeight);

            TrainingVolume = TrainingVolume.AddWorkingSet(parentUnit.CloneWorkingSet(workingSetPnum));
        }


        /// <summary>
        /// Get a copy of the Working Set with the ID specified - DEFAULT if not found
        /// </summary>
        /// <param name="workUnitPnum">The WU Progressive Number to be found</param>
        /// <param name="workingSetPnum">The WS Progressive Number to be found</param>
        /// <exception cref="ArgumentNullException">If more elements with the specified Progressive Number are found</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found</returns>
        public WorkingSetEntity CloneWorkingSet(uint workUnitPnum, uint workingSetPnum)

            => CloneWorkUnit(workUnitPnum)?.CloneWorkingSet(workingSetPnum) as WorkingSetEntity;

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
        /// Sort the Work Unit list wrt the WU progressive numbers
        /// </summary>
        /// <param name="wsIn">The input WU list</param>
        /// <returns>The sorted list</returns>
        private IEnumerable<WorkUnitEntity> SortWorkUnitByProgressiveNumber(IEnumerable<WorkUnitEntity> wsIn)

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
                WorkUnitEntity ws = _workUnits[iws];
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
             // Just overwrite all the progressive numbers
            for (int iws = (int)fromPnum; iws < _workUnits.Count(); iws++)
            {
                WorkUnitEntity ws = _workUnits[iws];
                ws.MoveToNewProgressiveNumber((uint)iws);
            }
        }

        /// <summary>
        /// Find the Working Unit with the progressive number specified - DEFAULT if not found
        /// </summary>
        /// <param name="workUnitPnum">The progressive number to be found</param>
        /// <exception cref="InvalidOperationException">If more Work Units with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found/returns>
        private WorkUnitEntity FindWorkUnitOrDefault(uint workUnitPnum)

            => _workUnits.SingleOrDefault(x => x.ProgressiveNumber == workUnitPnum);


        /// <summary>
        /// Find the Working Unit with the progressive number specified
        /// </summary>
        /// <param name="workUnitPnum">The progressive number to be found</param>
        /// <exception cref="InvalidOperationException">If no Work Units or more than one with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object/returns>
        private WorkUnitEntity FindWorkUnit(uint workUnitPnum)

            => _workUnits.Single(x => x.ProgressiveNumber == workUnitPnum);


        /// <summary>
        /// Find the Working Set according to the Progressive Numbers - DEFAULT if not found
        /// </summary>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which the WS belongs to</param>
        /// <param name="workingSetPnum">The Progressive Number of the Working Set to be found</param>
        /// <exception cref="InvalidOperationException">If more Work Units with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found/returns>
        private WorkingSetEntity FindWorkingSetOrDefault(uint parentWorkUnitPnum, uint workingSetPnum)
        {
            WorkUnitEntity parentWorkUnit = FindWorkUnitOrDefault(parentWorkUnitPnum);
            return parentWorkUnit?.WorkingSets.SingleOrDefault(x => x.ProgressiveNumber == workingSetPnum);
        }


        /// <summary>
        /// Find the Working Set according to the Progressive Numbers
        /// </summary>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which the WS belongs to</param>
        /// <param name="workingSetPnum">The Progressive Number of the Working Set to be found</param>
        /// <exception cref="InvalidOperationException">If no WOrk Unit or more than one with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object/returns>
        private WorkingSetEntity FindWorkingSet(uint parentWorkUnitPnum, uint workingSetPnum)
        {
            WorkUnitEntity parentWorkUnit = FindWorkUnit(parentWorkUnitPnum);
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

            => TrackWorkout(Id, StartTime, EndTime, PlannedDate, WorkoutTemplateId, _workUnits);

        #endregion
    }
}
