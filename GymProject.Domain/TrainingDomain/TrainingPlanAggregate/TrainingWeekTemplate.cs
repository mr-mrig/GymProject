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
    public class TrainingWeekTemplate : Entity<IdTypeValue>, ICloneable
    {


        /// <summary>
        /// The progressive number of the training week - Starts from 0
        /// </summary>
        public uint ProgressiveNumber { get; private set; } = 0;


        private ICollection<IdTypeValue> _workoutsIds = new List<IdTypeValue>();

        /// <summary>
        /// The IDs of the Workouts belonging to the TW
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<IdTypeValue> WorkoutsIds
        {
            get => _workoutsIds?.ToList().AsReadOnly() ?? new List<IdTypeValue>().AsReadOnly();
        }


        /// <summary>
        /// The type of the training week
        /// </summary>
        public TrainingWeekTypeEnum TrainingWeekType { get; private set; } = null;




        #region Ctors

        private TrainingWeekTemplate(IdTypeValue id, uint progressiveNumber, IList<IdTypeValue> workouts, TrainingWeekTypeEnum weekType = null) : base(id)
        {
            Id = id;
            ProgressiveNumber = progressiveNumber;
            TrainingWeekType = weekType ?? TrainingWeekTypeEnum.Generic;

            _workoutsIds = workouts?.ToList() ?? new List<IdTypeValue>();

            TestBusinessRules();
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The ID of the Training Week</param>
        /// <param name="workouts">The Workouts belonging to the Training Week</param>
        /// <param name="progressiveNumber">The progressive number of the Training Week</param>
        /// <param name="weekType">The type of the Training Week - optional</param>
        /// <returns>The TrainingWeekTemplate instance</returns>
        public static TrainingWeekTemplate AddTrainingWeekToPlan(IdTypeValue id, uint progressiveNumber, IList<IdTypeValue> workouts, TrainingWeekTypeEnum weekType = null)

            => new TrainingWeekTemplate(id, progressiveNumber, workouts, weekType);


        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The ID of the Training Week</param>
        /// <param name="workouts">The Workouts belonging to the Training Week</param>
        /// <param name="progressiveNumber">The progressive number of the Training Week</param>
        /// <param name="weekType">The type of the Training Week - optional</param>
        /// <returns>The TrainingWeekTemplate instance</returns>
        public static TrainingWeekTemplate AddTransientTrainingWeekToPlan(uint progressiveNumber, IList<IdTypeValue> workouts, TrainingWeekTypeEnum weekType = null)

            => new TrainingWeekTemplate(null, progressiveNumber, workouts, weekType);


        #endregion



        #region Public Methods

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
        /// </summary>
        /// <param name="weekType">The training week type</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void AssignSpecificWeekType(TrainingWeekTypeEnum weekType)
        {
            TrainingWeekType = weekType;
            TestBusinessRules();
        }


        /// <summary>
        /// Add the Workout to the Training Week
        /// </summary>
        /// <param name="workUnits">The list of the WUs which the WO is made up of</param>
        /// <param name="workoutName">The name of the WO</param>
        /// <param name="specificWeekday">The specific weekday which the WO is scheduled to</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void AddWorkout(IList<WorkUnitTemplate> workUnits, string workoutName, WeekdayEnum specificWeekday = null)
        {
            IdTypeValue toAdd = IdTypeValue.PlanWorkout(
                BuildWorkoutId(),
                BuildWorkoutProgressiveNumber(),
                workUnits,
                workoutName,
                specificWeekday);

            _workoutsIds.Add(toAdd);

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets(), GetMainEffortType());

            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Workingout from the Training Week
        /// </summary>
        /// <param name="toRemoveId">The Id of the WO to be removed</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void RemoveWorkout(IdTypeValue toRemoveId)
        {
            
            _workoutsIds.Remove(toBeRemoved);

            ForceConsecutiveWorkoutProgressiveNumbers();
            TestBusinessRules();
        }


        /// <summary>
        /// Find the Workout with the ID specified
        /// </summary>
        /// <param name="id">The Id to be found</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <returns>The WokringSetTemplate object/returns>
        public IdTypeValue FindWorkoutById(IdTypeValue id)
        {
            if (id == null)
                throw new ArgumentException($"Cannot find a WO with NULL id");

            IdTypeValue ws = _workoutsIds.Where(x => x.Id == id).FirstOrDefault();

            if (ws == default)
                throw new ArgumentException($"Workout with Id {id.ToString()} could not be found");

            return ws;
        }

        /// <summary>
        /// Find the Workout with the progressive number specified
        /// </summary>
        /// <param name="pNum">The progressive number to be found</param>
        /// <returns>The WokringSetTemplate objectd/returns>
        public IdTypeValue FindWorkoutByProgressiveNumber(int pNum)
        {
            IdTypeValue wo = _workoutsIds.Where(x => x.ProgressiveNumber == pNum).FirstOrDefault();

            if (wo == default)
                throw new ArgumentException($"Workout with Id {pNum.ToString()} could not be found");

            return wo;
        }


        /// <summary>
        /// Get the WUs of all the Workouts belonging to the Training Week
        /// </summary>
        /// <returns>The list of the Working Sets</returns>
        public IEnumerable<WorkUnitTemplate> GetAllWorkUnits()

             => _workoutsIds.SelectMany(x => x.WorkUnits);


        /// <summary>
        /// Get the WSs of all the Workouts belonging to the Training Week
        /// </summary>
        /// <returns>The list of the Working Sets</returns>
        public IEnumerable<WorkingSetTemplate> GetAllWorkingSets()

             => _workoutsIds.SelectMany(x => x.GetAllWorkingSets());


        /// <summary>
        /// Get the main effort type as the effort of most of the WSs of the WU 
        /// </summary>
        /// <returns>The training effort type</returns>
        public TrainingEffortTypeEnum GetMainEffortType()

            => _workoutsIds.Count == 0 ? TrainingEffortTypeEnum.IntensityPerc
                : GetAllWorkingSets().GroupBy(x => x.Effort.EffortType).Select(x
                     => new
                     {
                         Counter = x.Count(),
                         EffortType = x.Key
                     }).OrderByDescending(x => x.Counter).First().EffortType;

        #endregion


        #region Workouts Methods



        /// <summary>
        /// Assign a new progressive number to the WO
        /// </summary>
        /// <param name="newPnum">The new progressive number - PNums must be consecutive</param>
        /// <param name="workoutId">The ID of the WO to be moved</param>
        public void MoveWorkoutToNewProgressiveNumber(IdTypeValue workoutId, uint newPnum)
        {
            uint oldPnum = FindWorkoutById(workoutId).ProgressiveNumber;

            // Switch sets
            FindWorkoutByProgressiveNumber((int)newPnum).MoveToNewProgressiveNumber(oldPnum);
            FindWorkoutById(workoutId).MoveToNewProgressiveNumber(newPnum);

            ForceConsecutiveWorkoutProgressiveNumbers();
            TestBusinessRules();
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Build the next valid id
        /// </summary>
        /// <returns>The WO Id</returns>
        private IdTypeValue BuildWorkoutId()
        {
            if (_workoutsIds.Count == 0)
                return IdTypeValue.Create(1);

            else
                return IdTypeValue.Create(_workoutsIds.Max(x => x.Id.Id) + 1);
        }


        /// <summary>
        /// Build the next valid progressive number
        /// To be used before adding the WS to the list
        /// </summary>
        /// <returns>The WS Progressive Number</returns>
        private uint BuildWorkoutProgressiveNumber() => (uint)_workoutsIds.Count();


        /// <summary>
        /// Force the WOs to have consecutive progressive numbers
        /// It works by assuming that the WSs are added in a sorted fashion.
        /// </summary>
        private void ForceConsecutiveWorkoutProgressiveNumbers()
        {
            _workoutsIds = SortWorkoutsByProgressiveNumber(_workoutsIds).ToList();

            // Just overwrite all the progressive numbers
            for (int iwo = 0; iwo < _workoutsIds.Count(); iwo++)
            {
                IdTypeValue ws = _workoutsIds[iwo];
                ws.MoveToNewProgressiveNumber((uint)iwo);
            }
        }

        #endregion


        #region Business Rules Validation

        /// <summary>
        /// The Training Week must have no NULL workouts.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullWorkouts() => _workoutsIds.All(x => x != null);


        /// <summary>
        /// Cannot create a Training Week without any Workout unless it is a Full Rest one.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool AtLeastOneWorkout() => _workoutsIds?.Count > 0 || TrainingWeekType == TrainingWeekTypeEnum.FullRest;


        /// <summary>
        /// Full Rest week must have no scheduled Workouts.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool FullRestWeekHasNoWorkouts() => _workoutsIds?.Count == 0 || TrainingWeekType != TrainingWeekTypeEnum.FullRest;


        /// <summary>
        /// Workouts of the same Training Week must not be planned on overlapping days.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool WorkoutWithNonOverlappingSpecificDays()
        {
            //foreach(IdTypeValue wo in _workouts.Where(x => !x.SpecificWeekday.IsGeneric()))
            //{

            //}

            return _workoutsIds.GroupBy(x => x.SpecificWeekday).Select(x => new { x.Key, Count = x.Count() }).Any(x => x.Count > 0);
        }


        /// <summary>
        /// Sort the Workout list wrt the WO progressive numbers
        /// </summary>
        /// <param name="inputWorkouts">The input WO list</param>
        /// <returns>The sorted list</returns>
        private IEnumerable<IdTypeValue> SortWorkoutsByProgressiveNumber(IEnumerable<IdTypeValue> inputWorkouts)

            => inputWorkouts.OrderBy(x => x.ProgressiveNumber);


        /// <summary>
        /// Workouts of the same Training Week must have consecutive progressive numbers.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool WorkoutWithConsecutiveProgressiveNumber()
        {
            if (_workoutsIds.Count == 0)
                return true;

            // Check the first element: the sequence must start from 0
            if (_workoutsIds?.Count() == 1)
            {
                if (_workoutsIds.FirstOrDefault()?.ProgressiveNumber == 0)
                    return true;
                else
                    return false;
            }

            // Look for non consecutive numbers - exclude the last one
            foreach (int pnum in _workoutsIds.Where(x => x.ProgressiveNumber != _workoutsIds.Count() - 1)
                .Select(x => x.ProgressiveNumber))
            {
                if (!_workoutsIds.Any(x => x.ProgressiveNumber == pnum + 1))
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

            if (!AtLeastOneWorkout())
                throw new TrainingDomainInvariantViolationException($"Cannot create a Training Week without any Workout unless it is a Full Rest one.");

            if (!FullRestWeekHasNoWorkouts())
                throw new TrainingDomainInvariantViolationException($"Full Rest week must have no scheduled Workouts.");

            //if (!WorkoutWithNonOverlappingSpecificDays())
            //    throw new TrainingDomainInvariantViolationException($"Workouts of the same Training Week must not be planned on overlapping days.");

            if (!WorkoutWithConsecutiveProgressiveNumber())
                throw new TrainingDomainInvariantViolationException($"Workouts of the same Training Week must have consecutive progressive numbers.");
        }

        #endregion


        #region IClonable Interface

        public object Clone()

            => AddTrainingWeekToPlan(Id, ProgressiveNumber, WorkoutsIds.ToList(), TrainingWeekType);

        #endregion
    }
}
