//using GymProject.Domain.Base;
//using GymProject.Domain.SharedKernel;
//using GymProject.Domain.TrainingDomain.Common;
//using GymProject.Domain.TrainingDomain.Exceptions;
//using GymProject.Domain.Utils.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Linq;


//namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
//{
//    public class TrainingWeekTemplate : Entity<IdType>
//    {


//        /// <summary>
//        /// The progressive number of the training week - Starts from 0
//        /// </summary>
//        public uint ProgressiveNumber { get; private set; } = 0;


//        /// <summary>
//        /// The training volume parameters, as the sum of the params of the single WOs
//        /// </summary>
//        public TrainingVolumeParametersValue TrainingVolume { get; private set; } = null;


//        /// <summary>
//        /// The training effort, as the average of the single WOs efforts
//        /// </summary>
//        public TrainingIntensityParametersValue TrainingIntensity { get; private set; } = null;


//        /// <summary>
//        /// The training density parameters, as the sum of the params of the single WOs
//        /// </summary>
//        public TrainingDensityParametersValue TrainingDensity { get; private set; } = null;


//        private IList<WorkoutTemplate> _workouts = new List<WorkoutTemplate>();

//        /// <summary>
//        /// The Workouts belonging to the TW
//        /// </summary>
//        public IReadOnlyCollection<WorkoutTemplate> Workouts
//        {
//            get => _workouts?.Clone().ToList().AsReadOnly() ?? new List<WorkoutTemplate>().AsReadOnly();
//        }


//        /// <summary>
//        /// The type of the training week
//        /// </summary>
//        public TrainingWeekTypeEnum TrainingWeekType { get; private set; } = null;




//        #region Ctors

//        private TrainingWeekTemplate(IdType id, uint progressiveNumber, IdType excerciseId, IList<WorkingSetTemplate> workingSets, IdType ownerNoteId = null)
//        {
//            Id = id;
//            ProgressiveNumber = progressiveNumber;
//            OwnerNoteId = ownerNoteId;
//            ExcerciseId = excerciseId;

//            _workouts = workingSets?.Clone().ToList() ?? new List<WorkingSetTemplate>();

//            TestBusinessRules();

//            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(workingSets);
//            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(workingSets);
//            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(workingSets, GetMainEffortType());
//        }
//        #endregion



//        #region Factories

//        /// <summary>
//        /// Factory method
//        /// </summary>
//        /// <param name="id">The ID of the WU</param>
//        /// <param name="excerciseId">The ID of the excercise which the WU consists of</param>
//        /// <param name="progressiveNumber">The progressive number of the WS</param>
//        /// <param name="ownerNoteId">The ID of the Work Unit Owner's note</param>
//        /// <param name="workingSets">The working sets list - cannot be empty or null</param>
//        /// <returns>The TrainingWeekTemplate instance</returns>
//        public static TrainingWeekTemplate PlanWorkUnit(IdType id, uint progressiveNumber, IdType excerciseId, IList<WorkingSetTemplate> workingSets, IdType ownerNoteId = null)

//            => new TrainingWeekTemplate(id, progressiveNumber, excerciseId, workingSets, ownerNoteId);

//        #endregion



//        #region Public Methods

//        /// <summary>
//        /// Assign an excercise to the Work Unit
//        /// </summary>
//        /// <param name="excerciseId">The ID of the excercise</param>
//        public void AssignExcercise(IdType excerciseId)
//        {
//            ExcerciseId = excerciseId;
//        }


//        /// <summary>
//        /// Add the Working Set to the Work Unit
//        /// </summary>
//        /// <param name="repetitions">The WS repetitions</param>
//        /// <param name="rest">The rest period between the WS and the following</param>
//        /// <param name="effort">The WS effort</param>
//        /// <param name="tempo">The WS lifting tempo</param>
//        /// <param name="intensityTechniqueIds">The ids of the WS intensity techniques</param>
//        public void AddWorkingSet(WSRepetitionValue repetitions, RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null, IList<IdType> intensityTechniqueIds = null)
//        {
//            _workouts.Add(
//                WorkingSetTemplate.AddWorkingSet(
//                    BuildWorkoutId(),
//                    BuildWorkoutProgressiveNumber(),
//                    repetitions,
//                    rest,
//                    effort,
//                    tempo,
//                    intensityTechniqueIds
//                ));

//            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(_workouts);
//            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(_workouts);
//            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(_workouts, GetMainEffortType());

//            TestBusinessRules();
//        }


//        /// <summary>
//        /// Remove the Working Set from the Work Unit
//        /// </summary>
//        /// <param name="toRemoveId">The Id of the WS to be removed</param>
//        public void RemoveWorkingSet(IdType toRemoveId)
//        {
//            WorkingSetTemplate toBeRemoved = FindWorkingSetById(toRemoveId);

//            _workouts.Remove(toBeRemoved);

//            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(_workouts);
//            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(_workouts);
//            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(_workouts, GetMainEffortType());

//            ForceConsecutiveWorkoutProgressiveNumbers();
//            TestBusinessRules();
//        }


//        /// <summary>
//        /// Remove the Working Set from the Work Unit
//        /// </summary>
//        /// <param name="progressiveNumber">The progressive number of the WS to be removed</param>
//        public void RemoveWorkingSet(int progressiveNumber)
//        {
//            WorkingSetTemplate toBeRemoved = FindWorkingSetByProgressiveNumber(progressiveNumber);

//            _workouts.Remove(toBeRemoved);

//            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(_workouts);
//            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(_workouts);
//            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(_workouts, GetMainEffortType());

//            ForceConsecutiveWorkoutProgressiveNumbers();
//            TestBusinessRules();
//        }


//        /// <summary>
//        /// Find the Working Set with the ID specified
//        /// </summary>
//        /// <param name="id">The Id to be found</param>
//        /// <exception cref="ArgumentException">If ID could not be found</exception>
//        /// <returns>The WokringSetTemplate object/returns>
//        public WorkoutTemplate FindWorkoutById(IdType id)
//        {
//            if (id == null)
//                throw new ArgumentException($"Cannot find a WS with NULL id");

//            WorkoutTemplate ws = _workouts.Where(x => x.Id == id).FirstOrDefault();

//            if (ws == default)
//                throw new ArgumentException($"Working Set with Id {id} could not be found");

//            return ws;
//        }

//        /// <summary>
//        /// Find the Working Set with the progressive number specified
//        /// </summary>
//        /// <param name="pNum">The progressive number to be found</param>
//        /// <returns>The WokringSetTemplate object or DEFAULT if not found/returns>
//        public WorkoutTemplate FindWorkoutByProgressiveNumber(int pNum) => _workouts.Where(x => x.ProgressiveNumber == pNum).FirstOrDefault();

//        #endregion



//        #region Working Sets Methods

//        /// <summary>
//        /// Change the repetitions
//        /// </summary>
//        /// <param name="newReps">The new target repetitions</param>
//        /// <param name="workingSetId">The Id of the WS to be changed</param>
//        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
//        /// <exception cref="ArgumentException">Thrown if WS not found</exception>
//        public void ChangeWorkingSetRepetitions(IdType workingSetId, WSRepetitionValue newReps)
//        {
//            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);

//            ws.ChangeRepetitions(newReps);
//            TestBusinessRules();
//        }


//        /// <summary>
//        /// Change the effort
//        /// </summary>
//        /// <param name="newEffort">The new value</param>
//        /// <param name="workingSetId">The Id of the WS to be changed</param>/// <param name="workingSetId">The Id of the WS to be changed</param>
//        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
//        public void ChangeWorkingSetEffort(IdType workingSetId, TrainingEffortValue newEffort)
//        {
//            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);

//            ws.ChangeEffort(newEffort);
//            TestBusinessRules();
//        }


//        /// <summary>
//        /// Change the rest period of the WS specified
//        /// </summary>
//        /// <param name="newRest">The new value</param>
//        /// <param name="workingSetId">The Id of the WS to be changed</param>
//        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
//        public void ChangeWorkingSetRestPeriod(IdType workingSetId, RestPeriodValue newRest)
//        {
//            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);

//            ws.ChangeRestPeriod(newRest);
//            TestBusinessRules();
//        }


//        /// <summary>
//        /// Change the lifting tempo
//        /// </summary>
//        /// <param name="newTempo">The new value</param>
//        /// <param name="workingSetId">The Id of the WS to be changed</param>
//        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
//        public void ChangeWorkingSetLiftingTempo(IdType workingSetId, TUTValue newTempo)
//        {
//            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);

//            ws.ChangeLiftingTempo(newTempo);
//            TestBusinessRules();
//        }


//        /// <summary>
//        /// Add an intensity technique - Do nothing if already present in the list
//        /// </summary>
//        /// <param name="workingSetId">The Id of the WS to be changed</param>
//        /// <param name="toAddId">The id to be added</param>
//        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
//        public void AddWorkingSetIntensityTechnique(IdType workingSetId, IdType toAddId)
//        {

//            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);

//            ws.AddIntensityTechnique(toAddId);
//            TestBusinessRules();
//        }


//        /// <summary>
//        /// Remove an intensity technique 
//        /// </summary>
//        /// <param name="workingSetId">The Id of the WS to be changed</param>
//        /// <param name="toRemoveId">The id to be removed</param>
//        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
//        public bool RemoveWorkingSetIntensityTechnique(IdType workingSetId, IdType toRemoveId)
//        {
//            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);

//            bool ok = ws.RemoveIntensityTechnique(toRemoveId);
//            TestBusinessRules();
//            return ok;
//        }

//        #endregion


//        #region Private Methods

//        /// <summary>
//        /// Build the next valid id
//        /// </summary>
//        /// <returns>The WS Id</returns>
//        private IdType BuildWorkoutId()
//        {
//            if (_workouts.Count == 0)
//                return new IdType(1);

//            else
//                return _workouts.Last().Id + 1;
//        }


//        /// <summary>
//        /// Build the next valid progressive number
//        /// To be used before adding the WS to the list
//        /// </summary>
//        /// <returns>The WS Progressive Number</returns>
//        private uint BuildWorkoutProgressiveNumber() => (uint)_workouts.Count();


//        /// <summary>
//        /// Force the WSs to have consecutive progressive numbers
//        /// It works by assuming that the WSs are added in a sorted fashion.
//        /// </summary>
//        private void ForceConsecutiveWorkoutProgressiveNumbers()
//        {
//            // Just overwrite all the progressive numbers
//            for (int iws = 0; iws < _workouts.Count(); iws++)
//            {
//                WorkingSetTemplate ws = _workouts[iws];
//                ws.ChangeProgressiveNumber((uint)iws);
//            }
//        }

//        ///// <summary>
//        ///// Get the main effort type as the effort of most of the WSs of the WU 
//        ///// </summary>
//        ///// <returns>The training effort type</returns>
//        //private TrainingEffortTypeEnum GetMainEffortType()

//        //    => _workouts.Count == 0 ? TrainingEffortTypeEnum.IntensityPerc : _workouts.GroupBy(x => x.Effort.EffortType).Select(x
//        //         => new
//        //         {
//        //             Counter = x.Count(),
//        //             EffortType = x.Key
//        //         }).OrderByDescending(x => x.Counter).First().EffortType;

//        #endregion


//        #region Business Rules Validation

//        /// <summary>
//        /// The Training Week must have no NULL workouts.
//        /// </summary>
//        /// <returns>True if business rule is met</returns>
//        private bool NoNullWorkouts() => _workouts.All(x => x != null);


//        /// <summary>
//        /// Cannot create a Training Week without any Workout.
//        /// </summary>
//        /// <returns>True if business rule is met</returns>
//        private bool AtLeastOneWorkout() => _workouts?.Count > 0;


//        /// <summary>
//        /// Workouts of the same Training Week must have consecutive progressive numbers.
//        /// </summary>
//        /// <returns>True if business rule is met</returns>
//        private bool WorkoutWithConsecutiveProgressiveNumber()
//        {
//            // Check the first element: the sequence must start from 0
//            if (_workouts?.Count() <= 1)
//            {
//                if (_workouts.FirstOrDefault()?.ProgressiveNumber == 0)
//                    return true;
//                else
//                    return false;
//            }

//            // Look for non consecutive numbers - exclude the last one
//            foreach (int pnum in _workouts.Where(x => x.ProgressiveNumber != _workouts.Count() - 1)
//                .Select(x => x.ProgressiveNumber))
//            {
//                if (!_workouts.Any(x => x.ProgressiveNumber == pnum + 1))
//                    return false;
//            }

//            return true;
//        }



//        /// <summary>
//        /// Tests that all the business rules are met and manages invalid states
//        /// </summary>
//        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
//        private void TestBusinessRules()
//        {
//            if (!NoNullWorkouts())
//                throw new TrainingDomainInvariantViolationException($"The Training Week must have no NULL workouts.");

//            if (!AtLeastOneWorkout())
//                throw new TrainingDomainInvariantViolationException($"Cannot create a Training Week without any Workout.");

//            if (!WorkoutWithConsecutiveProgressiveNumber())
//                throw new TrainingDomainInvariantViolationException($"Workouts of the same Training Week must have consecutive progressive numbers.");
//        }
//        #endregion

//    }
//}
