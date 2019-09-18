using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
{
    public class TrainingWeekEntity : Entity<uint?>, ICloneable
    {


        /// <summary>
        /// The progressive number of the training week - Starts from 0
        /// </summary>
        public uint ProgressiveNumber { get; private set; } = 0;


        ///// <summary>
        ///// The training volume parameters, as the sum of the params of the single WOs
        ///// </summary>
        //public TrainingVolumeParametersValue TrainingVolume { get; private set; } = null;


        ///// <summary>
        ///// The training effort, as the average of the single WOs efforts
        ///// </summary>
        //public TrainingIntensityParametersValue TrainingIntensity { get; private set; } = null;


        ///// <summary>
        ///// The training density parameters, as the sum of the params of the single WOs
        ///// </summary>
        //public TrainingDensityParametersValue TrainingDensity { get; private set; } = null;


        //private List<WorkoutTemplateReferenceEntity> _workouts = new List<WorkoutTemplateReferenceEntity>();

        ///// <summary>
        ///// The IDs of the Workouts belonging to the TW
        ///// Provides a value copy: the instance fields must be modified through the instance methods
        ///// </summary>
        //public IReadOnlyCollection<WorkoutTemplateReferenceEntity> Workouts
        //{
        //    get => _workouts?.Clone().ToList().AsReadOnly() 
        //        ?? new List<WorkoutTemplateReferenceEntity>().AsReadOnly();
        //}

        private List<uint?> _workoutIds = new List<uint?>();

        /// <summary>
        /// FK to the WorkoutTemplate aggregate
        /// </summary>
        public IReadOnlyCollection<uint?> WorkoutIds
        {
            get => _workoutIds?.AsReadOnly() ?? new List<uint?>().AsReadOnly();
        }


        /// <summary>
        /// The type of the training week
        /// </summary>
        public TrainingWeekTypeEnum TrainingWeekType { get; private set; } = null;




        #region Ctors

        private TrainingWeekEntity() : base(null) {}

        private TrainingWeekEntity(uint? id, uint progressiveNumber, IEnumerable<uint?> workouts = null, TrainingWeekTypeEnum weekType = null) : base(id)
        {
            ProgressiveNumber = progressiveNumber;
            TrainingWeekType = weekType ?? TrainingWeekTypeEnum.Generic;

            _workoutIds = workouts?.ToList() ?? new List<uint?>();

            TestBusinessRules();
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The ID of the Training Week</param>
        /// <param name="workouts">The ID of the Workouts belonging to the Training Week</param>
        /// <param name="progressiveNumber">The progressive number of the Training Week</param>
        /// <param name="weekType">The type of the Training Week - optional</param>
        /// <returns>The TrainingWeekTemplate instance</returns>
        public static TrainingWeekEntity PlanTrainingWeek(uint? id, uint progressiveNumber, IEnumerable<uint?> workouts = null, TrainingWeekTypeEnum weekType = null)

            => new TrainingWeekEntity(id, progressiveNumber, workouts, weekType);


        /// <summary>
        /// Factory method for transient entities
        /// </summary>
        /// <param name="workouts">The ID of the Workouts belonging to the Training Week</param>
        /// <param name="progressiveNumber">The progressive number of the Training Week</param>
        /// <param name="weekType">The type of the Training Week - optional</param>
        /// <returns>The TrainingWeekTemplate instance</returns>
        public static TrainingWeekEntity PlanTransientTrainingWeek(uint progressiveNumber, IEnumerable<uint?> workouts = null, TrainingWeekTypeEnum weekType = null)

            => new TrainingWeekEntity(null, progressiveNumber, workouts, weekType);


        /// <summary>
        /// Factory method for full rest weeks
        /// </summary>
        /// <param name="progressiveNumber">The progressive number of the Training Week</param>
        /// <returns>The TrainingWeekTemplate instance</returns>
        public static TrainingWeekEntity PlanTransientFullRestWeek(uint progressiveNumber)

            => new TrainingWeekEntity(null, progressiveNumber, new List<uint?>(), TrainingWeekTypeEnum.FullRest);


        /// <summary>
        /// Factory method for full rest weeks
        /// </summary>
        /// <param name="id">The ID of the Training Week</param>
        /// <param name="progressiveNumber">The progressive number of the Training Week</param>
        /// <returns>The TrainingWeekTemplate instance</returns>
        public static TrainingWeekEntity PlanFullRestWeek(uint? id, uint progressiveNumber)

            => new TrainingWeekEntity(id, progressiveNumber, new List<uint?>(), TrainingWeekTypeEnum.FullRest);

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
            _workoutIds.Clear();
            TrainingWeekType = TrainingWeekTypeEnum.FullRest;
            TestBusinessRules();
        }

        #endregion


        #region Workout Methods

        /// <summary>
        /// Add the Workout to the Training Week
        /// </summary>
        /// <param name="id">The ID of the workout to be added to the Week</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void PlanWorkout(uint id)
        {
            _workoutIds.Add(id);
            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Workout from the Training Week
        /// </summary>
        /// <param name="workoutId">The ID of the Workout to be removed</param>
        /// <exception cref="InvalidOperationException">If no workouts or more than one found</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void UnplanWorkout(uint workoutId)
        {
            _workoutIds.Remove(
                _workoutIds.Single(x => x.Value == workoutId));

            //ForceConsecutiveWorkoutProgressiveNumbers(workoutPnum);
            TestBusinessRules();
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Build the next valid progressive number
        /// To be used before adding the WS to the list
        /// </summary>
        /// <returns>The WS Progressive Number</returns>
        private uint BuildWorkoutProgressiveNumber()

            => (uint)_workoutIds.Count();


        ///// <summary>
        ///// Force the WOs to have consecutive progressive numbers
        ///// It works by assuming that the WSs are added in a sorted fashion.
        ///// </summary>
        //private void ForceConsecutiveWorkoutProgressiveNumbers()
        //{
        //    List<WorkoutTemplateReferenceEntity> sortedCopy = new List<WorkoutTemplateReferenceEntity>();

        //    // Just overwrite all the progressive numbers
        //    for (int iwo = 0; iwo < _workoutIds.Count(); iwo++)
        //    {
        //        WorkoutTemplateReferenceEntity srcWorkout = _workoutIds.ElementAt(iwo);

        //        sortedCopy.Add(
        //            srcWorkout.MoveToNewProgressiveNumber((uint)iwo));
        //    }

        //    // Copy back
        //    _workoutIds = sortedCopy;
        //}


        ///// <summary>
        ///// Force the WOs to have consecutive progressive numbers
        ///// It works by assuming that the WSs are added in a sorted fashion.
        ///// This algorithm is more efficient as it ignores the elments before pnum, provided that they are already sorted
        ///// </summary>
        ///// <param name="fromPnum">The Progressive number from which the order is not respected</param>
        //private void ForceConsecutiveWorkoutProgressiveNumbers(uint fromPnum)
        //{
        //    // Just overwrite all the progressive numbers
        //    for (int iwo = (int)fromPnum; iwo < _workoutIds.Count(); iwo++)
        //        _workoutIds[iwo] = _workoutIds[iwo].MoveToNewProgressiveNumber((uint)iwo);
        //}

        #endregion


        #region Business Rules Validation

        /// <summary>
        /// The Training Week must have no NULL workouts.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullWorkouts() => _workoutIds.All(x => x != null);


        /// <summary>
        /// Cannot create a Training Week without any Workout unless it is a Full Rest one.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool AtLeastOneWorkout() => _workoutIds?.Count > 0 || TrainingWeekType == TrainingWeekTypeEnum.FullRest;


        /// <summary>
        /// Full Rest week must have no scheduled Workouts.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool FullRestWeekHasNoWorkouts() => _workoutIds?.Count == 0 || TrainingWeekType != TrainingWeekTypeEnum.FullRest;


        ///// <summary>
        ///// Workouts of the same Training Week must have consecutive progressive numbers.
        ///// </summary>
        ///// <returns>True if business rule is met</returns>
        //private bool WorkoutWithConsecutiveProgressiveNumber()
        //{
        //    if (_workoutIds.Count == 0)
        //        return true;

        //    // Check the first element: the sequence must start from 0
        //    if (_workoutIds?.Count() == 1)
        //    {
        //        if (_workoutIds.FirstOrDefault()?.ProgressiveNumber == 0)
        //            return true;
        //        else
        //            return false;
        //    }

        //    // Look for non consecutive numbers - exclude the last one
        //    foreach (int pnum in _workoutIds.Where(x => x.ProgressiveNumber != _workoutIds.Count() - 1)
        //        .Select(x => x.ProgressiveNumber))
        //    {
        //        if (!_workoutIds.Any(x => x.ProgressiveNumber == pnum + 1))
        //            return false;
        //    }

        //    return true;
        //}



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

            //if (!WorkoutWithConsecutiveProgressiveNumber())
            //    throw new TrainingDomainInvariantViolationException($"Workouts of the same Training Week must have consecutive progressive numbers.");
        }

        #endregion


        #region IClonable Interface

        public object Clone()

            => PlanTrainingWeek(Id, ProgressiveNumber, _workoutIds, TrainingWeekType);

        #endregion
    }
}
