using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
{
    public class TrainingPlanRoot : Entity<uint?>, IAggregateRoot, ICloneable
    {




        private List<TrainingWeekEntity> _trainingWeeks = new List<TrainingWeekEntity>();

        /// <summary>
        /// The Training Weeks which the Training Plan is scheduled to
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<TrainingWeekEntity> TrainingWeeks
        {
            get => _trainingWeeks?.Clone().ToList().AsReadOnly()
                ?? new List<TrainingWeekEntity>().AsReadOnly();
        }


        /// <summary>
        /// FK to the owner of the Training Plan - The author or the receiver if Inherited Training Plan
        /// </summary>
        public uint OwnerId { get; private set; }


        /// <summary>
        /// FK to the Workouts scheduled during the whole Training Plan
        /// </summary>
        public IReadOnlyCollection<uint?> WorkoutIds
        {
            get => _trainingWeeks?.SelectMany(x => x.WorkoutIds)?.ToList().AsReadOnly()
                ?? new List<uint?>().AsReadOnly();
        }





        #region Ctors


        private TrainingPlanRoot() : base(null) { }


        private TrainingPlanRoot(uint? id, uint ownerId, IEnumerable<TrainingWeekEntity> trainingWeeks = null) : base(id)
        {
            OwnerId = ownerId;

            _trainingWeeks = trainingWeeks?.Clone().ToList()
                ?? new List<TrainingWeekEntity>
                    {
                        TrainingWeekEntity.PlanTrainingWeek(null, 0)
                    };


            TestBusinessRules();
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method for creating a Training Plan Draft
        /// </summary>
        /// <param name="ownerid">The ID of the owner</param>
        /// <returns>The TrainingPlanRoot instance</returns>
        public static TrainingPlanRoot NewDraft(uint ownerid)

            => CreateTrainingPlan(ownerid);


        /// <summary>
        /// Factory method - Transient
        /// </summary>
        /// <param name="trainingWeeks">The Training Weeks which the Training Plan is made up of</param>
        /// <param name="ownerId">The ID of the owner of the plan</param>
        /// <returns>The TrainingPlanRoot instance</returns>
        public static TrainingPlanRoot CreateTrainingPlan(uint ownerId, IEnumerable<TrainingWeekEntity> trainingWeeks = null)

            => new TrainingPlanRoot(null, ownerId, trainingWeeks);


        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The ID of the Training Plan</param>
        /// <param name="trainingWeeks">The Training Weeks which the Training Plan is made up of</param>
        /// <param name="ownerId">The ID of the owner of the plan</param>
        /// <returns>The TrainingPlanRoot instance</returns>
        public static TrainingPlanRoot CreateTrainingPlan(uint? id, uint ownerId, IEnumerable<TrainingWeekEntity> trainingWeeks = null)

            => new TrainingPlanRoot(id, ownerId, trainingWeeks);

        #endregion



        #region Training Week Methods

        /// <summary>
        /// Add the Training Week to the Plan.
        /// </summary>
        /// <param name="trainingWeek">The input Training Week</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        /// <exception cref="ArgumentNullException">If null input</exception>
        /// <exception cref="ArgumentException">If already present Training Week</exception>
        public void PlanTrainingWeek(TrainingWeekEntity trainingWeek)
        {
            if (trainingWeek == null)
                throw new ArgumentNullException(nameof(trainingWeek), "Null input when trying to create a new Training Week.");

            if (_trainingWeeks.Contains(trainingWeek))
                throw new ArgumentException("The Training Week has already been added.", nameof(trainingWeek));

            if (trainingWeek.IsFullRestWeek())
                PlanFullRestWeek(trainingWeek);
            else
            {
                _trainingWeeks.Add(trainingWeek.Clone() as TrainingWeekEntity);
                TestBusinessRules();
            }
        }


        /// <summary>
        /// Add the Training Week to the Plan. This is not meant to work with Full Rest Weeks.
        /// </summary>
        /// <param name="workoutsIds">The IDs of the Workouts planned for the specified Training Week</param>
        /// <param name="weekType">The type of the Training Week</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        /// <exception cref="ArgumentException">If the week type is a Full Rest one</exception>
        public void PlanTransientTrainingWeek(TrainingWeekTypeEnum weekType, IEnumerable<uint?> workoutsIds)
        {
            if (weekType == TrainingWeekTypeEnum.FullRest)
                throw new ArgumentException("Cannot add Full Rest Weeks with this function.", nameof(weekType));

            TrainingWeekEntity toAdd = TrainingWeekEntity.PlanTransientTrainingWeek(
                BuildTrainingWeekProgressiveNumber(),
                workoutsIds,
                weekType);

            _trainingWeeks.Add(toAdd);

            TestBusinessRules();
        }


        /// <summary>
        /// Add a Training Week Draft to the Plan.
        /// </summary>
        /// <param name="weekType">The type of the Training Week</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void PlanDraftTrainingWeek(TrainingWeekTypeEnum weekType)
        {

            TrainingWeekEntity toAdd = TrainingWeekEntity.PlanTransientTrainingWeek(
                BuildTrainingWeekProgressiveNumber(),
                new List<uint?>(),
                weekType);

            _trainingWeeks.Add(toAdd);

            TestBusinessRules();
        }


        /// <summary>
        /// Add a Full Rest Week to the Plan
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void PlanTransientFullRestWeek()
        {
            _trainingWeeks.Add(TrainingWeekEntity.PlanTransientFullRestWeek(BuildTrainingWeekProgressiveNumber()));
            TestBusinessRules();
        }


        /// <summary>
        /// Add the Training Week to the Plan
        /// </summary>
        /// <param name="restWeek">The Training Week, which must be a Full Rest one</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        /// <exception cref="ArgumentException">If the input week is not a Full Rest one</exception>
        /// <exception cref="ArgumentNullException">If null input</exception>
        public void PlanFullRestWeek(TrainingWeekEntity restWeek)
        {
            if (restWeek == null)
                throw new ArgumentNullException(nameof(restWeek), "Null input when trying to create a new Full Rest Week.");

            if (!restWeek.IsFullRestWeek())
                throw new ArgumentException("Trying to add a Full Rest Week from a non-rest one.", nameof(restWeek));

            _trainingWeeks.Add(restWeek.Clone() as TrainingWeekEntity);
            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Training Week from the Plan
        /// </summary>
        /// <param name="weekPnum">The Progressive Number of the Training Plan to be removed</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void UnplanTrainingWeek(uint weekPnum)
        {
            TrainingWeekEntity toBeRemoved = FindTrainingWeekByProgressiveNumber((int)weekPnum);

            bool removed = _trainingWeeks.Remove(toBeRemoved);

            if (removed)
            {
                //ForceConsecutiveTrainingWeeksProgressiveNumbers(weekPnum);
                ForceConsecutiveTrainingWeeksProgressiveNumbers();
                TestBusinessRules();
            }
        }


        /// <summary>
        /// Assign a new progressive number to the Training Week
        /// </summary>
        /// <param name="destPnum">The new Progressive Number - PNums must be consecutive</param>
        /// <param name="srcPnum">The Progressive Number of the Training Week to be moved</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void MoveTrainingWeekToNewProgressiveNumber(uint srcPnum, uint destPnum)
        {
            TrainingWeekEntity src = FindTrainingWeekByProgressiveNumber((int)srcPnum);
            TrainingWeekEntity dest = FindTrainingWeekByProgressiveNumber((int)destPnum);

            src.MoveToNewProgressiveNumber(srcPnum);
            dest.MoveToNewProgressiveNumber(destPnum);

            ForceConsecutiveTrainingWeeksProgressiveNumbers();
            TestBusinessRules();
        }


        /// <summary>
        /// Assign the specific Week Type
        /// </summary>
        /// <param name="weekType">The training week type</param>
        /// <param name="weekPnum">The Progressive Number of the week to be moved</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void MarkTrainingWeekAs(uint weekPnum, TrainingWeekTypeEnum weekType)
        {
            FindTrainingWeekByProgressiveNumber((int)weekPnum).AssignSpecificWeekType(weekType);
            TestBusinessRules();
        }


        /// <summary>
        /// Make the week a full rest one. 
        /// This function also clears all the workouts scheduled, in order to meet the business rule.
        /// </summary>
        /// <param name="weekPnum">The Progressive Number of the week to be moved</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void MarkTrainingWeekAsFullRest(uint weekPnum)
        {
            FindTrainingWeekByProgressiveNumber((int)weekPnum).MarkAsFullRestWeek();
            TestBusinessRules();
        }


        /// <summary>
        /// Get a copy of the Training Week with the specified progressive number
        /// </summary>
        /// <param name="weekProgressiveNumber">The Progressive Number of the Week</param>
        /// <exception cref="InvalidOperationException">If no Training Week or more than one found</exception>
        public TrainingWeekEntity CloneTrainingWeek(uint weekProgressiveNumber)

            => _trainingWeeks.Single(x => x.ProgressiveNumber == weekProgressiveNumber).Clone() as TrainingWeekEntity;


        /// <summary>
        /// Get the next available WO progressive number for the specified Training Week
        /// </summary>
        /// <param name="weekProgressiveNumber">The Progressive Number of the Week</param>
        /// <exception cref="InvalidOperationException">If no Training Week or more than one found</exception>
        /// <returns>The next WO progressive number available</returns>
        public int GetNextWorkoutProgressiveNumber(uint weekProgressiveNumber)

            => _trainingWeeks.Single(x => x.ProgressiveNumber == weekProgressiveNumber)
                    .WorkoutIds.Count();

        #endregion


        #region Workout Methods


        /// <summary>
        /// Add the Workout to the specified Training Week
        /// </summary>
        /// <param name="workoutId">The ID of the Workout to be planned</param>
        /// <param name="weekPnum">The Progressive Number of the Week to which to add the WO to</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void PlanWorkout(uint weekPnum, uint workoutId)

            => FindTrainingWeekByProgressiveNumber((int)weekPnum).PlanWorkout(workoutId);


        /// <summary>
        /// Remove the Workout from the specified Training Week
        /// </summary>
        /// <param name="workoutId">The ID of the WO to be removed</param>
        /// <exception cref="InvalidOperationException">If no workouts or more than one found</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void UnplanWorkout(uint workoutId)
        {
            FindTrainingWeekByWorkoutId((int)workoutId).UnplanWorkout(workoutId);
            TestBusinessRules();
        }


        /// <summary>
        /// Get the ID of the Workouts of the whole Training Plan
        /// </summary>
        /// <param name="weekProgressiveNumber">The Progressive Number of the Week</param>
        /// <exception cref="InvalidOperationException">If no workouts or more than one found</exception>
        public IEnumerable<uint?> CloneWorkouts(uint weekProgressiveNumber)

            => _trainingWeeks.Single(x => x.ProgressiveNumber == weekProgressiveNumber).WorkoutIds;

        /// <summary>
        /// Get the average number of Workouts per Training Week of the Plan
        /// </summary>
        /// <returns>The average number of weekly workouts</returns>
        public float GetAverageWorkoutsPerWeek()

            => (float)_trainingWeeks.Where(x => x?.WorkoutIds != null).DefaultIfEmpty()?.Average(x => x?.WorkoutIds.Count ?? 0);


        /// <summary>
        /// Get the minimum number of weekly Workouts
        /// </summary>
        /// <returns>The minimum number of weekly workouts</returns>
        public int GetMinimumWorkoutsPerWeek()

            => (int)_trainingWeeks.Where(x => x?.WorkoutIds != null).DefaultIfEmpty()?.Min(x => x?.WorkoutIds.Count ?? 0);


        /// <summary>
        /// Get the amximum number of weekly Workouts
        /// </summary>
        /// <returns>The maximum number of weekly workouts</returns>
        public int GetMaximumWorkoutsPerWeek()

            => (int)_trainingWeeks.Where(x => x?.WorkoutIds != null).DefaultIfEmpty()?.Max(x => x?.WorkoutIds.Count ?? 0);


        #endregion


        #region Private Methods

        /// <summary>
        /// Find the Training Week with the ID specified
        /// </summary>
        /// <param name="id">The Id to be found</param>
        /// <exception cref="ArgumentNullException">If null ID</exception>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <returns>The TrainingWeekEntity object/returns>
        private TrainingWeekEntity FindTrainingWeekById(uint? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id), $"Cannot find a Training Week with NULL id");

            TrainingWeekEntity week = _trainingWeeks.SingleOrDefault(x => x.Id == id);

            if (week == default)
                throw new ArgumentException($"Training Week with Id {id.ToString()} could not be found", nameof(id));

            return week;
        }


        /// <summary>
        /// Find the Training Week with the progressive number specified
        /// </summary>
        /// <param name="pNum">The progressive number to be found</param>
        /// <returns>The TrainingWeekEntity object/returns>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        private TrainingWeekEntity FindTrainingWeekByProgressiveNumber(int pNum)
        {
            TrainingWeekEntity week = _trainingWeeks?.SingleOrDefault(x => x.ProgressiveNumber == pNum);

            if (week == default)
                throw new ArgumentException($"Training Week with Progressive number {pNum.ToString()} could not be found", nameof(pNum));

            return week;
        }


        /// <summary>
        /// Find the Training Week which the specified workout has been planned to
        /// </summary>
        /// <param name="workoutId">The ID to be found</param>
        /// <returns>The TrainingWeekEntity object/returns>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        private TrainingWeekEntity FindTrainingWeekByWorkoutId(int workoutId)
        {
            TrainingWeekEntity week = _trainingWeeks?.SingleOrDefault(x => x.WorkoutIds.ToList().Contains((uint?)workoutId));

            if (week == default)
                throw new ArgumentException($"The Workout with ID {workoutId.ToString()} could not be found", nameof(workoutId));

            return week;
        }

        /// <summary>
        /// Build the next valid progressive number
        /// To be used before adding the Training Week to the list
        /// </summary>
        /// <returns>The Training Week Progressive Number</returns>
        private uint BuildTrainingWeekProgressiveNumber()

            => (uint)_trainingWeeks.Count();


        /// <summary>
        /// Force the WOs to have consecutive progressive numbers
        /// It works by assuming that the WSs are added in a sorted fashion.
        /// </summary>
        private void ForceConsecutiveTrainingWeeksProgressiveNumbers()
        {
            _trainingWeeks = SortByProgressiveNumber(_trainingWeeks).ToList();

            // Just overwrite all the progressive numbers
            for (int itw = 0; itw < _trainingWeeks.Count(); itw++)
            {
                TrainingWeekEntity ws = _trainingWeeks[itw];
                ws.MoveToNewProgressiveNumber((uint)itw);
            }
        }


        /// <summary>
        /// Force the WOs to have consecutive progressive numbers
        /// It works by assuming that the WSs are added in a sorted fashion.
        /// This algorithm is more efficient as it ignores the elments before pnum, provided that they are already sorted
        /// </summary>
        /// <param name="fromPnum">The Progressive number from which the order is not respected</param>
        private void ForceConsecutiveTrainingWeeksProgressiveNumbers(uint fromPnum)
        {
            throw new NotImplementedException("Does not work");
            // Just overwrite all the progressive numbers
            for (int iweek = (int)fromPnum; iweek < _trainingWeeks.Count(); iweek++)
                _trainingWeeks[iweek].MoveToNewProgressiveNumber((uint)iweek);

            //for (int iweek = (int)fromPnum; iweek < _trainingWeeks.Count(); iweek++)
            //{
            //    TrainingWeekTemplate week = _trainingWeeks.ElementAt(iweek);
            //    week.MoveToNewProgressiveNumber((uint)iweek);
            //}

        }


        /// <summary>
        /// Sort the Training Week list wrt their progressive numbers
        /// </summary>
        /// <param name="inputWeeks">The input Training Week list</param>
        /// <returns>The sorted list</returns>
        private IEnumerable<TrainingWeekEntity> SortByProgressiveNumber(IEnumerable<TrainingWeekEntity> inputWeeks)

            => inputWeeks.OrderBy(x => x.ProgressiveNumber);

        #endregion


        #region Business Rules Validation

        /// <summary>
        /// Training Weeks of the same Training Plan must have consecutive progressive numbers.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool TrainingWeeksWithConsecutiveProgressiveNumber()
        {
            if (_trainingWeeks.Count == 0)
                return true;

            // Check the first element: the sequence must start from 0
            if (_trainingWeeks?.Count() == 1)
            {
                if (_trainingWeeks.FirstOrDefault()?.ProgressiveNumber == 0)
                    return true;
                else
                    return false;
            }

            // Look for non consecutive numbers - exclude the last one
            //foreach (int pnum in _trainingWeeks.Where(x => x.ProgressiveNumber != _trainingWeeks.Count() - 1)
            foreach (int pnum in _trainingWeeks.Where((_, i) => i != _trainingWeeks.Count - 1)
                .Select(x => x.ProgressiveNumber))
            {
                if (!_trainingWeeks.Any(x => x.ProgressiveNumber == pnum + 1))
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
            if (!TrainingWeeksWithConsecutiveProgressiveNumber())
                throw new TrainingDomainInvariantViolationException($"Training Weeks of the same Training Plan must have consecutive progressive numbers.");
        }

        #endregion



        #region IClonable Interface

        public object Clone()

            => CreateTrainingPlan(Id, OwnerId, TrainingWeeks);

        #endregion
    }
}
