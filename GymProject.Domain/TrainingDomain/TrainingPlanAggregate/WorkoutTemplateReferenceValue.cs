using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Domain.Utils.Extensions;
using System.Collections.Generic;
using System;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
{


    public class WorkoutTemplateReferenceValue : ValueObject, ICloneable
    {





        /// <summary>
        /// The progressive number of the Workout
        /// </summary>
        public uint ProgressiveNumber { get; private set; }


        private ICollection<WorkingSetTemplateEntity> _workingSets;

        /// <summary>
        /// The Working Sets of the workout
        /// </summary>
        public IReadOnlyCollection<WorkingSetTemplateEntity> WorkingSets
        {
            get => _workingSets?.Clone()?.ToList().AsReadOnly() ?? new List<WorkingSetTemplateEntity>().AsReadOnly();
        }




        #region Ctors

        private WorkoutTemplateReferenceValue(uint workoutProgressiveNumber, IEnumerable<WorkingSetTemplateEntity> workingSets)
        {
            ProgressiveNumber = workoutProgressiveNumber;
            _workingSets = workingSets?.Clone()?.ToList() ?? new List<WorkingSetTemplateEntity>();

            TestBusinessRules();
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="workoutProgressiveNumber">The Progressive Number of the workout</param>
        /// <param name="workoutWorkingSets">The list of the Workout Working Sets</param>
        /// <returns>The WorkoutTemplateReferenceValue instance</returns>
        public static WorkoutTemplateReferenceValue BuildLinkToWorkout(uint workoutProgressiveNumber, IEnumerable<WorkingSetTemplateEntity> workoutWorkingSets)

            => new WorkoutTemplateReferenceValue(workoutProgressiveNumber, workoutWorkingSets);


        #endregion




        #region Public Methods

        /// <summary>
        /// Move the Workout Reference to the new Progressive Number by creating a new ValueObject
        /// </summary>
        /// <param name="newPnum">The new progressive number</param>
        /// <returns>The new WorkoutTemplateReferenceValue instance</returns>
        public WorkoutTemplateReferenceValue MoveToNewProgressiveNumber(uint newPnum)

            => BuildLinkToWorkout(newPnum, _workingSets);


        /// <summary>
        /// Add the specifed Working Sets to the Workout by creating a new ValueObject
        /// </summary>
        /// <param name="workingSets">The Working Sets to be added</param>
        /// <returns>The new WorkoutTemplateReferenceValue instance</returns>
        /// <exception cref="ArgumentException">If trying to add duplicate Working Sets to the Training Week</exception>
        public WorkoutTemplateReferenceValue AddWorkingSets(IEnumerable<WorkingSetTemplateEntity> workingSets)
        {
            if (workingSets.ContainsDuplicates())
                throw new ArgumentException("Trying to add duplicate Working Sets to the Training Week", nameof(workingSets));

            // Check for already added WSs
            if (workingSets.Any(x => _workingSets.Contains(x)))
                throw new ArgumentException("Trying to add duplicate Working Sets to the Training Week", nameof(workingSets));

            return BuildLinkToWorkout(ProgressiveNumber, _workingSets.Union(workingSets));
        }



        /// <summary>
        /// Remove the specifed Working Sets to the Workout by creating a new ValueObject
        /// </summary>
        /// <param name="workingSets">The Working Sets to be removed</param>
        /// <returns>The new WorkoutTemplateReferenceValue instance</returns>
        /// <exception cref="InvalidOperationException">If trying to remove transient Working Sets</exception>
        /// <exception cref="ArgumentException">If at least one of the Working Sets couldn't be found</exception>
        public WorkoutTemplateReferenceValue RemoveWorkingSets(IEnumerable<WorkingSetTemplateEntity> workingSets)
        {
            if (workingSets?.DefaultIfEmpty() == default)
                return this;

            // Transient entities
            if (workingSets.Any(x => x.IsTransient()) || _workingSets.Any(x => x.IsTransient()))
                throw new InvalidOperationException($"Cannot remove transient Working Sets");

            IEnumerable<WorkingSetTemplateEntity> newWorkingSets = _workingSets.Except(workingSets);

            // Missing WS
            if (newWorkingSets.Count() != _workingSets.Count - workingSets.Count())
                throw new ArgumentException($"Working Set couldn't be found");

            return BuildLinkToWorkout(ProgressiveNumber, _workingSets.Except(workingSets));
        }


        /// <summary>
        /// Get the main effort type as the effort of most of the WSs of the WO
        /// </summary>
        /// <returns>The training effort type</returns>
        public TrainingEffortTypeEnum GetMainEffortType()

            => _workingSets.Count() == 0
                ? TrainingEffortTypeEnum.IntensityPerc
                : _workingSets.GroupBy(x => x.Effort.EffortType).Select(x
                     => new
                     {
                         Counter = x.Count(),
                         EffortType = x.Key
                     }).OrderByDescending(x => x.Counter).First().EffortType;

        #endregion



        #region Business Rules Validation

        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
        }

        #endregion




        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return ProgressiveNumber;
            yield return WorkingSets;
        }

        public object Clone()

            => BuildLinkToWorkout(ProgressiveNumber, WorkingSets);
    }
}
