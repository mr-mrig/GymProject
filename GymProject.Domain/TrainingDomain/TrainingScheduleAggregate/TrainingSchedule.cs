using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GymProject.Domain.TrainingDomain.TrainingScheduleAggregate
{
    public class TrainingSchedule : Entity<IdTypeValue>, IAggregateRoot, ICloneable
    {



        /// <summary>
        /// The period which the Training Plan has been scheduled to 
        /// </summary>
        public DateRangeValue ScheduledPeriod { get; private set; } = null;


        /// <summary>
        /// FK to the Training Plan
        /// </summary>
        public IdTypeValue TrainingPlanId { get; private set; } = null;


        private ICollection<TrainingScheduleFeedback> _feedbacks = null;

        /// <summary>
        /// The Feedbacks of the Training Schedule
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<TrainingScheduleFeedback> Feedbacks
        {
            get => _feedbacks?.Clone().ToList().AsReadOnly() ?? new List<TrainingScheduleFeedback>().AsReadOnly();
        }


        #region Ctors

        private TrainingSchedule(IdTypeValue id, IdTypeValue trainingPlanId, DateRangeValue scheduledPeriod, IEnumerable<TrainingScheduleFeedback> feedbacks) : base(id)
        {
            ScheduledPeriod = scheduledPeriod;
            TrainingPlanId = trainingPlanId;

            _feedbacks = feedbacks?.Clone().ToList() ?? new List<TrainingScheduleFeedback>();

            TestBusinessRules();
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The object ID</param>
        /// <param name="trainingPlanId">The ID of the Training Plan which the schedule refers to</param>
        /// <param name="scheduledPeriod">The training plan scheduled duration</param>
        /// <param name="feedbacks">The Feedbacks list</param>
        /// <returns>The TrainingSchedule instance</returns>
        public static TrainingSchedule ScheduleTrainingPlan(IdTypeValue id, IdTypeValue trainingPlanId, DateRangeValue scheduledPeriod, IEnumerable<TrainingScheduleFeedback> feedbacks = null)

            => new TrainingSchedule(id, trainingPlanId, scheduledPeriod, feedbacks);


        /// <summary>
        /// Factory method - Transient object
        /// </summary>
        /// <param name="trainingPlanId">The ID of the Training Plan which the schedule refers to</param>
        /// <param name="scheduledPeriod">The training plan scheduled duration</param>
        /// <param name="feedbacks">The Feedbacks list</param>
        /// <returns>The TrainingSchedule instance</returns>
        public static TrainingSchedule ScheduleTrainingPlanTransient(IdTypeValue trainingPlanId, DateRangeValue scheduledPeriod, IEnumerable<TrainingScheduleFeedback> feedbacks = null)

            => ScheduleTrainingPlan(null, trainingPlanId, scheduledPeriod, feedbacks);

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



        /// <summary>
        /// Find the Working Unit with the progressive number specified - DEFAULT if not found
        /// </summary>
        /// <param name="workingSetPnum">The progressive number to be found</param>
        /// <exception cref="ArgumentNullException">If more elements with the specified Progressive Number are found</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found</returns>
        public WorkUnitTemplate CloneWorkUnit(uint workingSetPnum)

            => FindWorkUnitOrDefault(workingSetPnum)?.Clone() as WorkUnitTemplate;


        /// <summary>
        /// Get a copy of the Working Set with the ID specified - DEFAULT if not found
        /// </summary>
        /// <param name="workUnitPnum">The WU Progressive Number to be found</param>
        /// <param name="workingSetPnum">The WS Progressive Number to be found</param>
        /// <exception cref="ArgumentNullException">If more elements with the specified Progressive Number are found</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found</returns>
        public WorkingSetTemplate CloneWorkingSet(uint workUnitPnum, uint workingSetPnum)

            => CloneWorkUnit(workUnitPnum)?.CloneWorkingSet(workingSetPnum) as WorkingSetTemplate;


        /// <summary>
        /// Add the Working Unit (as a copy) to the Workout - if not already present
        /// </summary>
        /// <param name="toAdd">The Work Unit to be added</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        /// <exception cref="ArgumentException">If Work Unit already present</exception>
        public void AddWorkUnit(WorkUnitTemplate toAdd)
        {
            if (_workUnits.Contains(toAdd))
                throw new ArgumentException("Trying to add a duplicate Work Unit", nameof(toAdd));

            _workUnits.Add(toAdd.Clone() as WorkUnitTemplate);

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
        public void AddTransientWorkUnit(IdTypeValue excerciseId, IEnumerable<WorkingSetTemplate> workingSets, IEnumerable<IdTypeValue> workUnitIntensityTechniquesIds = null, IdTypeValue ownerNoteId = null)
        {
            WorkUnitTemplate toAdd = WorkUnitTemplate.PlanTransientWorkUnit(
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
        public void RemoveWorkUnit(WorkUnitTemplate toRemove)
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
            WorkUnitTemplate toBeRemoved = FindWorkUnit(toRemovePnum);

            bool removed = _workUnits.Remove(toBeRemoved);

            if (removed)
            {
                TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
                TrainingVolume = TrainingVolume.RemoveWorkingSets(toBeRemoved.WorkingSets);
                TrainingDensity = TrainingDensity.RemoveWorkingSets(toBeRemoved.WorkingSets);

                ForceConsecutiveWorkUnitProgressiveNumbers(toRemovePnum);
                TestBusinessRules();
            }
        }



        /// <summary>
        /// Get a copy of the WSs of all the Work Units belonging to the Workout
        /// </summary>
        /// <returns>The list of the Working Sets</returns>
        public IEnumerable<WorkingSetTemplate> CloneAllWorkingSets()

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


        [Obsolete("To be removed: this should be a presentation/application logic task", true)]
        /// <summary>
        /// Get the Intensity parameters computed over the specified excercises
        /// </summary>
        /// <param name="excerciseIdsList">The list of the IDs of the excercises</param>
        /// <returns>The training Intensity Parameters for the specified excercises</returns>
        public TrainingIntensityParametersValue GetIntensityByExcercises(IEnumerable<IdTypeValue> excerciseIdsList)

            => TrainingIntensityParametersValue.ComputeFromWorkingSets(
                _workUnits.Where(x => excerciseIdsList.Contains(x.ExcerciseId)).SelectMany(x => x.WorkingSets));


        #endregion


        #region Feebacks Methods

        /// <summary>
        /// Rate the Training Schedule
        /// </summary>
        /// <param name="rating">The rating value</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void RateTrainingSchedule(IdTypeValue authorId, RatingValue rating)
        {
            Rating = rating;
        }


        /// <summary>
        /// Attach a comment
        /// </summary>
        /// <param name="comment">The body of the comment</param>
        public void WriteComment(IdTypeValue authorId, string comment)
        {
            Comment = comment;
        }


        public void ProvideFeedback()
        {

        }


        public void RemoveFeedback()
        {

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
        /// Sort the Work Unit list wrt the WU progressive numbers
        /// </summary>
        /// <param name="wsIn">The input WU list</param>
        /// <returns>The sorted list</returns>
        private IEnumerable<WorkUnitTemplate> SortWorkUnitByProgressiveNumber(IEnumerable<WorkUnitTemplate> wsIn)

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
                WorkUnitTemplate ws = _workUnits[iws];
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
                WorkUnitTemplate ws = _workUnits[iws];
                ws.MoveToNewProgressiveNumber((uint)iws);
            }
        }

        /// <summary>
        /// Find the Working Unit with the progressive number specified - DEFAULT if not found
        /// </summary>
        /// <param name="workUnitPnum">The progressive number to be found</param>
        /// <exception cref="InvalidOperationException">If more Work Units with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found/returns>
        private WorkUnitTemplate FindWorkUnitOrDefault(uint workUnitPnum)

            => _workUnits.SingleOrDefault(x => x.ProgressiveNumber == workUnitPnum);


        /// <summary>
        /// Find the Working Unit with the progressive number specified
        /// </summary>
        /// <param name="workUnitPnum">The progressive number to be found</param>
        /// <exception cref="InvalidOperationException">If no Work Units or more than one with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object/returns>
        private WorkUnitTemplate FindWorkUnit(uint workUnitPnum)

            => _workUnits.SingleOrDefault(x => x.ProgressiveNumber == workUnitPnum);


        /// <summary>
        /// Find the Working Set according to the Progressive Numbers - DEFAULT if not found
        /// </summary>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which the WS belongs to</param>
        /// <param name="workingSetPnum">The Progressive Number of the Working Set to be found</param>
        /// <exception cref="InvalidOperationException">If more Work Units with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found/returns>
        private WorkingSetTemplate FindWorkingSetOrDefault(uint parentWorkUnitPnum, uint workingSetPnum)
        {
            WorkUnitTemplate parentWorkUnit = FindWorkUnitOrDefault(parentWorkUnitPnum);
            return parentWorkUnit?.WorkingSets.SingleOrDefault(x => x.ProgressiveNumber == workingSetPnum);
        }


        /// <summary>
        /// Find the Working Set according to the Progressive Numbers
        /// </summary>
        /// <param name="parentWorkUnitPnum">The Progressive Number of the WU which the WS belongs to</param>
        /// <param name="workingSetPnum">The Progressive Number of the Working Set to be found</param>
        /// <exception cref="InvalidOperationException">If no WOrk Unit or more than one with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object/returns>
        private WorkingSetTemplate FindWorkingSet(uint parentWorkUnitPnum, uint workingSetPnum)
        {
            WorkUnitTemplate parentWorkUnit = FindWorkUnit(parentWorkUnitPnum);
            return parentWorkUnit.WorkingSets.Single(x => x.ProgressiveNumber == workingSetPnum);
        }

        #endregion


        #region Business Rules Validation

        /// <summary>
        /// The Schedule must refer to a non-NULL Training Plan.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NonNullTrainingPlan() => _workUnits.All(x => x != null);


        /// <summary>
        /// A single User can provide one Training Scheule Feedback only.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool OneUserProvidesOneFeedbackOnly() => _workUnits.All(x => x != null);




        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
            if (!NonNullTrainingPlan())
                throw new TrainingDomainInvariantViolationException($"The Schedule must refer to a non-NULL Training Plan.");

            if (!OneUserProvidesOneFeedbackOnly())
                throw new TrainingDomainInvariantViolationException($"A single User can provide one Training Scheule Feedback only.");
        }

        #endregion


        #region IClonable Implementation

        public object Clone()

            => ScheduleTrainingPlan(Id, TrainingPlanId, ScheduledPeriod, _feedbacks);

        #endregion
    }
}
