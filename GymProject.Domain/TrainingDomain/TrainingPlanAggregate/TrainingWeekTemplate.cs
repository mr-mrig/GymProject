using GymProject.Domain.Base;
using GymProject.Domain.Base.Mediator;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
{
    public class TrainingWeekTemplate : Entity<IdTypeValue>, ICloneable
    {


        /// <summary>
        /// The progressive number of the training week - Starts from 0
        /// </summary>
        public uint ProgressiveNumber { get; private set; } = 0;


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


        private IList<WorkoutTemplateReferenceValue> _workouts = new List<WorkoutTemplateReferenceValue>();

        /// <summary>
        /// The IDs of the Workouts belonging to the TW
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<WorkoutTemplateReferenceValue> Workouts
        {
            get => _workouts?.ToList().AsReadOnly() ?? new List<WorkoutTemplateReferenceValue>().AsReadOnly();
        }


        /// <summary>
        /// The type of the training week
        /// </summary>
        public TrainingWeekTypeEnum TrainingWeekType { get; private set; } = null;




        #region Ctors

        private TrainingWeekTemplate(IdTypeValue id, uint progressiveNumber, IEnumerable<WorkoutTemplateReferenceValue> workouts = null, TrainingWeekTypeEnum weekType = null) : base(id)
        {
            ProgressiveNumber = progressiveNumber;
            TrainingWeekType = weekType ?? TrainingWeekTypeEnum.Generic;

            _workouts = workouts?.ToList() ?? new List<WorkoutTemplateReferenceValue>();

            TestBusinessRules();

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets());
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets());
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The ID of the Training Week</param>
        /// <param name="workouts">The reference to the Workouts belonging to the Training Week</param>
        /// <param name="progressiveNumber">The progressive number of the Training Week</param>
        /// <param name="weekType">The type of the Training Week - optional</param>
        /// <returns>The TrainingWeekTemplate instance</returns>
        public static TrainingWeekTemplate PlanTrainingWeek(IdTypeValue id, uint progressiveNumber, IEnumerable<WorkoutTemplateReferenceValue> workouts = null, TrainingWeekTypeEnum weekType = null)

            => new TrainingWeekTemplate(id, progressiveNumber, workouts, weekType);


        /// <summary>
        /// Factory method for transient entities
        /// </summary>
        /// <param name="workouts">The reference to the Workouts belonging to the Training Week</param>
        /// <param name="progressiveNumber">The progressive number of the Training Week</param>
        /// <param name="weekType">The type of the Training Week - optional</param>
        /// <returns>The TrainingWeekTemplate instance</returns>
        public static TrainingWeekTemplate PlanTransientTrainingWeek(uint progressiveNumber, IEnumerable<WorkoutTemplateReferenceValue> workouts = null, TrainingWeekTypeEnum weekType = null)

            => new TrainingWeekTemplate(null, progressiveNumber, workouts, weekType);


        /// <summary>
        /// Factory method for full rest weeks
        /// </summary>
        /// <param name="progressiveNumber">The progressive number of the Training Week</param>
        /// <returns>The TrainingWeekTemplate instance</returns>
        public static TrainingWeekTemplate PlanTransientFullRestWeek(uint progressiveNumber)

            => new TrainingWeekTemplate(null, progressiveNumber, new List<WorkoutTemplateReferenceValue>(), TrainingWeekTypeEnum.FullRest);


        /// <summary>
        /// Factory method for full rest weeks
        /// </summary>
        /// <param name="id">The ID of the Training Week</param>
        /// <param name="progressiveNumber">The progressive number of the Training Week</param>
        /// <returns>The TrainingWeekTemplate instance</returns>
        public static TrainingWeekTemplate PlanFullRestWeek(IdTypeValue id, uint progressiveNumber)

            => new TrainingWeekTemplate(id, progressiveNumber, new List<WorkoutTemplateReferenceValue>(), TrainingWeekTypeEnum.FullRest);

        #endregion



        #region Public Methods

        /// <summary>
        /// Tell whether the Training Week is a Full Rest one
        /// </summary>
        /// <returns>True if Full Rest week</returns>
        public bool IsFullRestWeek()

            => TrainingWeekType == TrainingWeekTypeEnum.FullRest;


        /// <summary>
        /// Assign a new progressive number to the Training Week
        /// </summary>
        /// <param name="newPnum">The new progressive number - PNums must be consecutive</param>
        public void MoveToNewProgressiveNumber(uint newPnum)
        {
            ProgressiveNumber = newPnum;
        }


        /// <summary>
        /// Assign the specified week type to the Training Week
        /// Be careful when switching to/from Full Rest as business rule might be violated
        /// </summary>
        /// <param name="weekType">The training week type</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void AssignSpecificWeekType(TrainingWeekTypeEnum weekType)
        {
            TrainingWeekType = weekType;
            TestBusinessRules();
        }


        /// <summary>
        /// Make the week a full rest one. 
        /// This function also clears all the workouts scheduled, in order to meet the business rule.
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void MarkAsFullRestWeek()
        {
            _workouts.Clear();
            TrainingWeekType = TrainingWeekTypeEnum.FullRest;

            TrainingVolume = TrainingVolumeParametersValue.InitEmpty();
            TrainingDensity = TrainingDensityParametersValue.InitEmpty();
            TrainingIntensity = TrainingIntensityParametersValue.InitEmpty();

            TestBusinessRules();
        }


        /// <summary>
        /// Get the main effort type as the effort of most of the WSs of the WU 
        /// </summary>
        /// <returns>The training effort type</returns>
        public TrainingEffortTypeEnum GetMainEffortType()

            => _workouts.Sum(x => x?.WorkingSets?.Count()) == 0 
                ? TrainingEffortTypeEnum.IntensityPerc
                : CloneAllWorkingSets().GroupBy(x => x.Effort.EffortType).Select(x
                     => new
                     {
                         Counter = x.Count(),
                         EffortType = x.Key
                     }).OrderByDescending(x => x.Counter).First().EffortType;

        #endregion


        #region Training Parameters Aggregates

        /// <summary>
        /// Get the Training Volume Parameters for the selected Workout
        /// </summary>
        /// <param name="progressiveNumber">The Workout progressive number</param>
        /// <returns>The Training Volume Parameters</returns>
        /// <exception cref="ArgumentOutOfRangeException">If Workout not found</exception>
        public TrainingVolumeParametersValue GetWorkoutTrainingVolume(uint progressiveNumber)

            => TrainingVolumeParametersValue.ComputeFromWorkingSets(FindWorkout(progressiveNumber).WorkingSets);


        /// <summary>
        /// Get the Training Density Parameters for the selected Workout
        /// </summary>
        /// <param name="progressiveNumber">The Workout progressive number</param>
        /// <returns>The Training Volume Parameters</returns>
        /// <exception cref="ArgumentOutOfRangeException">If Workout not found</exception>
        public TrainingDensityParametersValue GetWorkoutTrainingDensity(uint progressiveNumber)

            => TrainingDensityParametersValue.ComputeFromWorkingSets(FindWorkout(progressiveNumber).WorkingSets);


        /// <summary>
        /// Get the Training Intensity Parameters for the selected Workout
        /// </summary>
        /// <param name="progressiveNumber">The Workout progressive number</param>
        /// <returns>The Training Volume Parameters</returns>
        /// <exception cref="ArgumentOutOfRangeException">If Workout not found</exception>
        public TrainingIntensityParametersValue GetWorkoutTrainingIntensity(uint progressiveNumber)
        {
            WorkoutTemplateReferenceValue workout = FindWorkout(progressiveNumber);

            return TrainingIntensityParametersValue.ComputeFromWorkingSets(workout.WorkingSets, workout.GetMainEffortType());
        }


        #endregion


        #region Workout Methods

        /// <summary>
        /// Add the Workout to the Training Week
        /// </summary>
        /// <param name="workingSets">The list of the WSs which the WO is made up of</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void PlanWorkout(IEnumerable<WorkingSetTemplate> workingSets)
        {
            _workouts.Add(WorkoutTemplateReferenceValue.BuildLinkToWorkout(
                BuildWorkoutProgressiveNumber(),
                workingSets));


            TrainingVolume = TrainingVolume.AddWorkingSets(workingSets);
            TrainingDensity = TrainingDensity.AddWorkingSets(workingSets);
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());

            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Workout from the Training Week
        /// </summary>
        /// <param name="workoutPnum">The Progressive Number of the WO to be removed</param>
        /// <exception cref="InvalidOperationException">If no workouts or more than one found</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void UnplanWorkout(uint workoutPnum)
        {
            IEnumerable<WorkingSetTemplate> removedWorkingSets = 
                _workouts.ElementAt((int)workoutPnum).WorkingSets;

            _workouts.RemoveAt((int)workoutPnum);

            TrainingVolume = TrainingVolume.RemoveWorkingSets(removedWorkingSets);
            TrainingDensity = TrainingDensity.RemoveWorkingSets(removedWorkingSets);
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());

            ForceConsecutiveWorkoutProgressiveNumbers(workoutPnum);
            TestBusinessRules();
        }


        /// <summary>
        /// Modify the Workout by adding the selected Working Sets
        /// </summary>
        /// <param name="workingSets">The list of the WSs to add to the WO</param>
        /// <param name="workoutPnum">The Progressive Number of the Workout to be modified</param>
        /// <exception cref="ArgumentOutOfRangeException">If Workout not found</exception>
        public void AddWorkingSets(uint workoutPnum, IEnumerable<WorkingSetTemplate> workingSets)
        {
            WorkoutTemplateReferenceValue workout =  FindWorkout(workoutPnum);

            _workouts[(int)workoutPnum] = workout.AddWorkingSets(workingSets);

            TrainingVolume = TrainingVolume.AddWorkingSets(workingSets);
            TrainingDensity = TrainingDensity.AddWorkingSets(workingSets);
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
        }


        /// <summary>
        /// Modify the Workout by removing the selected Working Sets
        /// </summary>
        /// <param name="workingSets">The list of the WSs to remove from the WO</param>
        /// <param name="workoutPnum">The Progressive Number of the Workout to be modified</param>
        /// <exception cref="ArgumentOutOfRangeException">If Workout not found</exception>
        /// <exception cref="InvalidOperationException">If trying to remove transient Working Sets</exception>
        public void RemoveWorkingSets(uint workoutPnum, IEnumerable<WorkingSetTemplate> workingSets)
        {
            WorkoutTemplateReferenceValue workout = FindWorkout(workoutPnum);

            _workouts[(int)workoutPnum] = workout.RemoveWorkingSets(workingSets);

            TrainingVolume = TrainingVolume.AddWorkingSets(workingSets);
            TrainingDensity = TrainingDensity.AddWorkingSets(workingSets);
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
        }


        /// <summary>
        /// Assign a new progressive number to the WO
        /// </summary>
        /// <param name="destPnum">The new Progressive Number - PNums must be consecutive</param>
        /// <param name="srcPnum">The Progressive Number of the WO to be moved</param>
        /// <exception cref="ArgumentOutOfRangeException">If Workout not found</exception>
        public void MoveWorkoutToNewProgressiveNumber(uint srcPnum, uint destPnum)
        {
            WorkoutTemplateReferenceValue src = FindWorkout(srcPnum);
            WorkoutTemplateReferenceValue dest = FindWorkout(destPnum);

            _workouts[(int)srcPnum] = dest.MoveToNewProgressiveNumber(srcPnum);
            _workouts[(int)destPnum] = src.MoveToNewProgressiveNumber(destPnum);
        }


        /// <summary>
        /// Get the WSs of the specified Workout
        /// </summary>
        /// <param name="workoutPnum">The Progressive Number of the Workout</param>
        /// <returns>The list of the Working Sets</returns>
        /// <exception cref="ArgumentOutOfRangeException">If Workout not found</exception>
        public IEnumerable<WorkingSetTemplate> CloneWorkoutWorkingSets(uint workoutPnum)

             => FindWorkout(workoutPnum)?.WorkingSets;


        /// <summary>
        /// Clone the Workout Reference with the specified Progressive Number
        /// </summary>
        /// <param name="workoutPnum">The Progressive Number of the Workout</param>
        /// <returns>The list of the Working Sets</returns>
        /// <exception cref="ArgumentOutOfRangeException">If Workout not found</exception>
        public WorkoutTemplateReferenceValue CloneWorkout(uint workoutPnum)

            => FindWorkout(workoutPnum);

        /// <summary>
        /// Get the WSs of all the Workouts belonging to the Training Week
        /// </summary>
        /// <returns>The list of the Working Sets</returns>
        public IEnumerable<WorkingSetTemplate> CloneAllWorkingSets()

             => _workouts.Where(x => x != null).SelectMany(x => x.WorkingSets);

        #endregion


        #region Private Methods

        /// <summary>
        /// Get a reference to the Workout with the specified Progressive Number
        /// </summary>
        /// <param name="progressiveNumber"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">If Workout not found</exception>
        private WorkoutTemplateReferenceValue FindWorkout(uint progressiveNumber)

            => _workouts[(int)progressiveNumber];


        /// <summary>
        /// Build the next valid progressive number
        /// To be used before adding the WS to the list
        /// </summary>
        /// <returns>The WS Progressive Number</returns>
        private uint BuildWorkoutProgressiveNumber()

            => (uint)_workouts.Count();


        /// <summary>
        /// Force the WOs to have consecutive progressive numbers
        /// It works by assuming that the WSs are added in a sorted fashion.
        /// </summary>
        private void ForceConsecutiveWorkoutProgressiveNumbers()
        {
            IList<WorkoutTemplateReferenceValue> sortedCopy = new List<WorkoutTemplateReferenceValue>();

            // Just overwrite all the progressive numbers
            for (int iwo = 0; iwo < _workouts.Count(); iwo++)
            {
                WorkoutTemplateReferenceValue srcWorkout = _workouts.ElementAt(iwo);

                sortedCopy.Add(
                    srcWorkout.MoveToNewProgressiveNumber((uint)iwo));
            }

            // Copy back
            _workouts = sortedCopy;
        }


        /// <summary>
        /// Force the WOs to have consecutive progressive numbers
        /// It works by assuming that the WSs are added in a sorted fashion.
        /// This algorithm is more efficient as it ignores the elments before pnum, provided that they are already sorted
        /// </summary>
        /// <param name="fromPnum">The Progressive number from which the order is not respected</param>
        private void ForceConsecutiveWorkoutProgressiveNumbers(uint fromPnum)
        {
            // Just overwrite all the progressive numbers
            for (int iwo = (int)fromPnum; iwo < _workouts.Count(); iwo++)
                _workouts[iwo] = _workouts[iwo].MoveToNewProgressiveNumber((uint)iwo);
        }

        #endregion


        #region Business Rules Validation

        /// <summary>
        /// The Training Week must have no NULL workouts.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullWorkouts() => _workouts.All(x => x != null);


        /// <summary>
        /// Cannot create a Training Week without any Workout unless it is a Full Rest one.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool AtLeastOneWorkout() => _workouts?.Count > 0 || TrainingWeekType == TrainingWeekTypeEnum.FullRest;


        /// <summary>
        /// Full Rest week must have no scheduled Workouts.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool FullRestWeekHasNoWorkouts() => _workouts?.Count == 0 || TrainingWeekType != TrainingWeekTypeEnum.FullRest;


        /// <summary>
        /// Workouts of the same Training Week must have consecutive progressive numbers.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool WorkoutWithConsecutiveProgressiveNumber()
        {
            if (_workouts.Count == 0)
                return true;

            // Check the first element: the sequence must start from 0
            if (_workouts?.Count() == 1)
            {
                if (_workouts.FirstOrDefault()?.ProgressiveNumber == 0)
                    return true;
                else
                    return false;
            }

            // Look for non consecutive numbers - exclude the last one
            foreach (int pnum in _workouts.Where(x => x.ProgressiveNumber != _workouts.Count() - 1)
                .Select(x => x.ProgressiveNumber))
            {
                if (!_workouts.Any(x => x.ProgressiveNumber == pnum + 1))
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
            if (!NoNullWorkouts())
                throw new TrainingDomainInvariantViolationException($"The Training Week must have no NULL workouts.");

            //if (!AtLeastOneWorkout())
            //    throw new TrainingDomainInvariantViolationException($"Cannot create a Training Week without any Workout unless it is a Full Rest one.");

            if (!FullRestWeekHasNoWorkouts())
                throw new TrainingDomainInvariantViolationException($"Full Rest week must have no scheduled Workouts.");

            if (!WorkoutWithConsecutiveProgressiveNumber())
                throw new TrainingDomainInvariantViolationException($"Workouts of the same Training Week must have consecutive progressive numbers.");
        }

        #endregion


        #region IClonable Interface

        public object Clone()

            => PlanTrainingWeek(Id, ProgressiveNumber, Workouts.ToList(), TrainingWeekType);

        #endregion
    }
}
