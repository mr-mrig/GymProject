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


        private ICollection<WorkoutTemplateReferenceValue> _workouts = new List<WorkoutTemplateReferenceValue>();

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

        private TrainingWeekTemplate(IdTypeValue id, uint progressiveNumber, IList<WorkoutTemplateReferenceValue> workouts = null, TrainingWeekTypeEnum weekType = null) : base(id)
        {
            ProgressiveNumber = progressiveNumber;
            TrainingWeekType = weekType ?? TrainingWeekTypeEnum.Generic;

            _workouts = workouts?.ToList() ?? new List<WorkoutTemplateReferenceValue>();

            TestBusinessRules();
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
        public static TrainingWeekTemplate AddTrainingWeekToPlan(IdTypeValue id, uint progressiveNumber, IList<WorkoutTemplateReferenceValue> workouts = null, TrainingWeekTypeEnum weekType = null)

            => new TrainingWeekTemplate(id, progressiveNumber, workouts, weekType);


        /// <summary>
        /// Factory method for transient entities
        /// </summary>
        /// <param name="workouts">The reference to the Workouts belonging to the Training Week</param>
        /// <param name="progressiveNumber">The progressive number of the Training Week</param>
        /// <param name="weekType">The type of the Training Week - optional</param>
        /// <returns>The TrainingWeekTemplate instance</returns>
        public static TrainingWeekTemplate AddTransientTrainingWeekToPlan(uint progressiveNumber, IList<WorkoutTemplateReferenceValue> workouts = null, TrainingWeekTypeEnum weekType = null)

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

        #endregion


        #region Workout Methods

        /// <summary>
        /// Add the Workout to the Training Week
        /// </summary>
        /// <param name="workingSets">The list of the WSs which the WO is made up of</param>
        /// <param name="workoutPnum">The name of the WO</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void PlanWorkout(ICollection<WorkingSetTemplate> workingSets)
        {
            _workouts.Add(WorkoutTemplateReferenceValue.BuildLinkToWorkout(
                BuildWorkoutProgressiveNumber(),
                workingSets));

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
            WorkoutTemplateReferenceValue toBeRemoved = _workouts.Single(x => x.ProgressiveNumber == workoutPnum);

            if(_workouts.Remove(toBeRemoved))
            {
                ForceConsecutiveWorkoutProgressiveNumbers();
                TestBusinessRules();
            }
        }


        /// <summary>
        /// Get the WSs of all the Workouts belonging to the Training Week
        /// </summary>
        /// <returns>The list of the Working Sets</returns>
        public IEnumerable<WorkingSetTemplate> CloneAllWorkingSets()

             => _workouts.SelectMany(x => x.WorkingSets);

        #endregion


        #region Workouts Methods

        /// <summary>
        /// Assign a new progressive number to the WO
        /// </summary>
        /// <param name="destPnum">The new Progressive Number - PNums must be consecutive</param>
        /// <param name="srcPnum">The Progressive Number of the WO to be moved</param>
        public void MoveWorkoutToNewProgressiveNumber(uint srcPnum, uint destPnum)
        {
            WorkoutTemplateReferenceValue src = FindWorkout(srcPnum);
            WorkoutTemplateReferenceValue dest = FindWorkout(destPnum);


            // Raise domain events
            throw new NotImplementedException();
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Get a reference to the Workout with the specified Progressive Number
        /// </summary>
        /// <param name="progressiveNumber"></param>
        /// <returns></returns>
        private WorkoutTemplateReferenceValue FindWorkout(uint progressiveNumber)

            => _workouts.Single(x => x.ProgressiveNumber == progressiveNumber);


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
            ICollection<WorkoutTemplateReferenceValue> workoutsLocal = _workouts.OrderBy(x => x.ProgressiveNumber).ToList();

            // Just overwrite all the progressive numbers
            for (int iwo = 0; iwo < _workouts.Count(); iwo++)
            {
                WorkoutTemplateReferenceValue srcWorkout = workoutsLocal.ElementAt(iwo);

                // Check if the WO is not already ordered correctly
                if(srcWorkout.ProgressiveNumber != iwo)
                {
                    AddDomainEvent(new );
                    //WorkoutTemplateReferenceValue movedWorkout = srcWorkout.MoveToNewProgressiveNumber((uint)iwo);

                    // Raise event
                }
            }

            // Copy back
            _workouts = workoutsLocal;

            throw new NotImplementedException();
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

            if (!AtLeastOneWorkout())
                throw new TrainingDomainInvariantViolationException($"Cannot create a Training Week without any Workout unless it is a Full Rest one.");

            if (!FullRestWeekHasNoWorkouts())
                throw new TrainingDomainInvariantViolationException($"Full Rest week must have no scheduled Workouts.");

            if (!WorkoutWithConsecutiveProgressiveNumber())
                throw new TrainingDomainInvariantViolationException($"Workouts of the same Training Week must have consecutive progressive numbers.");
        }

        #endregion


        #region IClonable Interface

        public object Clone()

            => AddTrainingWeekToPlan(Id, ProgressiveNumber, Workouts.ToList(), TrainingWeekType);

        #endregion
    }
}
