using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.TrainingDomain.WorkoutSessionAggregate
{
    public class WorkUnitEntity : Entity<IdTypeValue>, ICloneable
    {


        /// <summary>
        /// The progressive number of the work unit - Starts from 0
        /// </summary>
        public uint ProgressiveNumber { get; private set; } = 0;


        /// <summary>
        /// The rate the user gives to the Work Unit
        /// </summary>
        public RatingValue UserRating { get; private set; } = null;


        /// <summary>
        /// The training volume parameters, as the sum of the params of the single WSs
        /// </summary>
        public TrainingVolumeParametersValue TrainingVolume { get; private set; } = null;


        private IList<WorkingSetEntity> _workingSets = new List<WorkingSetEntity>();

        /// <summary>
        /// The Working Sets belonging to the WU, sorted by Progressive Numbers.
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IEnumerable<WorkingSetEntity> WorkingSets => _workingSets.ToList() ?? new List<WorkingSetEntity>();


        /// <summary>
        /// FK to the Excercise Aggregate
        /// </summary>
        public IdTypeValue ExcerciseId { get; private set; } = null;




        #region Ctors


        private WorkUnitEntity() : base(null) { }


        private WorkUnitEntity(IdTypeValue id, uint progressiveNumber, IdTypeValue excerciseId, RatingValue rating, IEnumerable<WorkingSetEntity> workingSets) 
            : base(id)
        {
            ProgressiveNumber = progressiveNumber;
            ExcerciseId = excerciseId;
            UserRating = rating;

            _workingSets = workingSets?.Clone().ToList() ?? new List<WorkingSetEntity>();

            TestBusinessRules();

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(workingSets);
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method - Creates a transient WU
        /// </summary>
        /// <param name="excerciseId">The ID of the excercise which the WU consists of</param>
        /// <param name="progressiveNumber">The progressive number of the WS</param>
        /// <returns>The WorkUnitEntity instance</returns>
        public static WorkUnitEntity StartExcercise(uint progressiveNumber, IdTypeValue excerciseId)

            => TrackExcercise(null, progressiveNumber, excerciseId, null, null);


        /// <summary>
        /// Factory method - Loads a WU with the specified ID
        /// </summary>
        /// <param name="id">The Work Unit ID</param>
        /// <param name="excerciseId">The ID of the excercise which the WU consists of</param>
        /// <param name="progressiveNumber">The progressive number of the WS</param>
        /// <param name="workingSets">The working sets list - cannot be empty or null</param>
        /// <param name="userRating">The rate the user gave to the Work Unit</param>
        /// <returns>The WorkUnitEntity instance</returns>
        public static WorkUnitEntity TrackExcercise(IdTypeValue id, uint progressiveNumber, IdTypeValue excerciseId, RatingValue userRating,
            IEnumerable<WorkingSetEntity> workingSets)

            => new WorkUnitEntity(id, progressiveNumber, excerciseId, userRating, workingSets);

        #endregion



        #region Public Methods


        /// <summary>
        /// Give a rating to the Performance when the Work Unit has been completed
        /// </summary>
        /// <param name="userRating">The rating</param>
        public void RatePerformance(RatingValue userRating) => UserRating = userRating;


        /// <summary>
        /// Get a copy of the Working Set with the progressive number specified or DEFAULT if not found
        /// </summary>
        /// <param name="pNum">The progressive number to be found</param>
        /// <exception cref="InvalidOperationException">If more Working Sets with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found</returns>
        public WorkingSetEntity CloneWorkingSet(uint pNum)

            => FindWorkingSetOrDefault(pNum)?.Clone() as WorkingSetEntity;

        #endregion


        #region Working Sets Methods

        /// <summary>
        /// Add the Working Set to the Work Unit
        /// </summary>
        /// <param name="toAdd">The WS to be added</param>
        /// <exception cref="ArgumentException">If Working Set already present</exception>
        public void TrackWorkingSet(WorkingSetEntity toAdd)
        {
            WorkingSetEntity copy = toAdd.Clone() as WorkingSetEntity;

            if (_workingSets.Contains(copy))
                throw new ArgumentException("Trying to add a duplicate Working Set", nameof(toAdd));

            _workingSets.Add(copy);

            TestBusinessRules();
            TrainingVolume = TrainingVolume.AddWorkingSet(copy);
        }


        /// <summary>
        /// Add the Working Set to the Work Unit
        /// </summary>
        /// <param name="repetitions">The WS repetitions</param>
        /// <param name="load">The Weight used</param>
        public void TrackWorkingSet(WSRepetitionsValue repetitions, WeightPlatesValue load)
        {

            WorkingSetEntity toAdd = WorkingSetEntity.TrackTransientWorkingSet(
                    BuildWorkingSetProgressiveNumber(),
                    repetitions,
                    load,
                    null
                );

            _workingSets.Add(toAdd);

            TestBusinessRules();
            TrainingVolume = TrainingVolume.AddWorkingSet(toAdd);
        }


        /// <summary>
        /// Remove the Working Set from the Work Unit
        /// </summary>
        /// <param name="toRemove">The WS to be removed</param>
        /// <exception cref="ArgumentNullException">If null input</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void UntrackWorkingSet(WorkingSetEntity toRemove)
        {
            if (toRemove == null)
                throw new ArgumentNullException(nameof(toRemove), "Trying to remove a NULL Working Set");

            UntrackWorkingSet(toRemove.ProgressiveNumber);
        }

        /// <summary>
        /// Remove the Working Set from the Work Unit
        /// </summary>
        /// <param name="progressiveNumber">The progressive number of the WS to be removed</param>
        /// <exception cref="InvalidOperationException">If more Working Sets with the specified Progressive Number</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void UntrackWorkingSet(uint progressiveNumber)
        {
            WorkingSetEntity toBeRemoved = FindWorkingSet(progressiveNumber);

            if (_workingSets.Remove(toBeRemoved))
            {
                TrainingVolume = TrainingVolume.RemoveWorkingSet(toBeRemoved);

                ForceConsecutiveWorkingSetsProgressiveNumbers(progressiveNumber);
                TestBusinessRules();
            }
        }


        /// <summary>
        /// Change the repetitions
        /// </summary>
        /// <param name="newReps">The new repetitions</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void ReviseWorkingSetRepetitions(uint workingSetPnum, WSRepetitionsValue newReps)
        {
            WorkingSetEntity ws = FindWorkingSet(workingSetPnum);

            TrainingVolume = TrainingVolume.RemoveWorkingSet(ws);

            ws.TrackRepetitions(newReps);

            TrainingVolume = TrainingVolume.AddWorkingSet(ws);
        }


        /// <summary>
        /// Change the load
        /// </summary>
        /// <param name="newLoad">The new load</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void ReviseWorkingSetLoad(uint workingSetPnum, WeightPlatesValue newLoad)
        {
            WorkingSetEntity ws = FindWorkingSet(workingSetPnum);

            TrainingVolume = TrainingVolume.RemoveWorkingSet(ws);

            ws.TrackLoad(newLoad);

            TrainingVolume = TrainingVolume.AddWorkingSet(ws);
        }


        /// <summary>
        /// Change the progressive number
        /// </summary>
        /// <param name="newNumber">The new value - PNums must be consecutive</param>
        public void MoveToNewProgressiveNumber(uint newNumber)
        {
            ProgressiveNumber = newNumber;
        }

        #endregion



        #region Private Methods

        /// <summary>
        /// Build the next valid progressive number
        /// To be used before adding the WS to the list
        /// </summary>
        /// <returns>The WS Progressive Number</returns>
        private uint BuildWorkingSetProgressiveNumber() => (uint)_workingSets.Count();


        /// <summary>
        /// Find the Working Set with the progressive number specified
        /// </summary>
        /// <param name="pNum">The progressive number to be found</param>
        /// <exception cref="InvalidOperationException">If more Working Sets with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if notfound</returns>
        private WorkingSetEntity FindWorkingSetOrDefault(uint pNum)

            => _workingSets.SingleOrDefault(x => x.ProgressiveNumber == pNum);


        /// <summary>
        /// Find the Working Set with the progressive number specified
        /// </summary>
        /// <param name="pNum">The progressive number to be found</param>
        /// <exception cref="InvalidOperationException">If more Working Sets, or zero, with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object</returns>
        private WorkingSetEntity FindWorkingSet(uint pNum)

            => _workingSets.Single(x => x.ProgressiveNumber == pNum);


        /// <summary>
        /// Force the WSs to have consecutive progressive numbers
        /// This algorithm is more efficient as it ignores the elments before pnum, provided that they are already sorted
        /// </summary>
        /// <param name="fromPnum">The Progressive number from which the order is not respected</param>
        private void ForceConsecutiveWorkingSetsProgressiveNumbers(uint fromPnum)
        {
            // Just overwrite all the progressive numbers
            for (int iws = (int)fromPnum; iws < _workingSets.Count(); iws++)
            {
                WorkingSetEntity ws = _workingSets[iws];
                ws.MoveToNewProgressiveNumber((uint)iws);
            }
        }

        #endregion


        #region Business Rules Validation

        /// <summary>
        /// The Work Unit musthave no NULL working sets.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullWorkingSets() => _workingSets.All(x => x != null);


        /// <summary>
        /// The Work Unit must be linked to an excercise.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool ExcerciseSpecified() => ExcerciseId != null;


        /// <summary>
        /// Cannot create a Work Unit without any WS 
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool AtLeastOneWorkingSet() => _workingSets?.Count > 0;


        /// <summary>
        /// Working Sets of the same Work Unit must have consecutive progressive numbers.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool WorkingSetsWithConsecutiveProgressiveNumber()
        {
            if (_workingSets.Count == 0)
                return true;

            // Check the first element: the sequence must start from 0
            if (_workingSets?.Count() == 1)
            {
                if (_workingSets.FirstOrDefault()?.ProgressiveNumber == 0)
                    return true;
                else
                    return false;
            }

            // Look for non consecutive numbers - exclude the last one
            foreach (int pnum in _workingSets.Where(x => x.ProgressiveNumber != _workingSets.Count() - 1)
                .Select(x => x.ProgressiveNumber))
            {
                if (!_workingSets.Any(x => x.ProgressiveNumber == pnum + 1))
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
            if (!NoNullWorkingSets())
                throw new TrainingDomainInvariantViolationException($"The Work Unit must have no NULL working sets.");

            if (!ExcerciseSpecified())
                throw new TrainingDomainInvariantViolationException($"The Work Unit must be linked to an excercise.");

            if (!AtLeastOneWorkingSet())
                throw new TrainingDomainInvariantViolationException($"Cannot create a Work Unit without any WS.");

            if (!WorkingSetsWithConsecutiveProgressiveNumber())
                throw new TrainingDomainInvariantViolationException($"Working Sets of the same Work Unit must have consecutive progressive numbers.");
        }

        #endregion


        #region IClonable Implementation

        public object Clone()

            => TrackExcercise(Id, ProgressiveNumber, ExcerciseId, UserRating, WorkingSets.ToList());

        #endregion
    }
}
