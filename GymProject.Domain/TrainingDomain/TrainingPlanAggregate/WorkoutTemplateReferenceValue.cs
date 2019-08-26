using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate;
using GymProject.Domain.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
{


    public class WorkoutTemplateReferenceValue : ValueObject
    {





        /// <summary>
        /// The progressive number of the Workout
        /// </summary>
        public uint ProgressiveNumber { get; private set; }


        private ICollection<WorkingSetTemplate> _workingSets;

        /// <summary>
        /// The Working Sets of the workout
        /// </summary>
        public IReadOnlyCollection<WorkingSetTemplate> WorkingSets
        {
            get => _workingSets?.Clone()?.ToList().AsReadOnly() ?? new List<WorkingSetTemplate>().AsReadOnly();
        }




        #region Ctors

        private WorkoutTemplateReferenceValue(uint workoutProgressiveNumber, IEnumerable<WorkingSetTemplate> workingSets)
        {
            ProgressiveNumber = workoutProgressiveNumber;
            _workingSets = workingSets?.Clone()?.ToList();

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
        public static WorkoutTemplateReferenceValue BuildLinkToWorkout(uint workoutProgressiveNumber, IEnumerable<WorkingSetTemplate> workoutWorkingSets)

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
    }
}
