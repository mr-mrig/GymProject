using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
{
    public class WorkUnitTemplate : Entity<IdType>
    {


        /// <summary>
        /// The progressive number of the work unit
        /// </summary>
        public uint ProgressiveNumber { get; private set; } = 0;


        /// <summary>
        /// A note about the WU made by the owner
        /// </summary>
        public PersonalNoteValue OwnerNote { get; private set; } = null;


        /// <summary>
        /// The training volume parameters, as the sum of the params of the single WSs
        /// </summary>
        public TrainingVolumeParametersValue TrainingVolume { get; private set; } = null;


        /// <summary>
        /// The training effort, as the average of the single WSs efforts
        /// </summary>
        public TrainingIntensityParametersValue TrainingIntensity { get; private set; } = null;


        /// <summary>
        /// The training density parameters, as the sum of the params of the single WSs
        /// </summary>
        public TrainingDensityParametersValue TrainingDensity { get; private set; } = null;


        private IList<WorkingSetTemplate> _workingSets = new List<WorkingSetTemplate>();

        /// <summary>
        /// The Working Sets belonging to the WU
        /// </summary>
        public IReadOnlyCollection<WorkingSetTemplate> WorkingSets
        {
            get => _workingSets?.ToList().AsReadOnly() ?? new List<WorkingSetTemplate>().AsReadOnly();
        }


        /// <summary>
        /// FK to the Excercise Id
        /// </summary>
        public IdType ExcerciseId { get; private set; } = null;




        #region Ctors

        private WorkUnitTemplate(uint progressiveNumber, IdType excerciseId, IList<WorkingSetTemplate> workingSets, PersonalNoteValue ownerNote = null)
        {
            ProgressiveNumber = progressiveNumber;
            OwnerNote = ownerNote;
            ExcerciseId = excerciseId;

            _workingSets = workingSets ?? new List<WorkingSetTemplate>();

            TestBusinessRules();

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(workingSets);
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(workingSets);
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(workingSets);
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="progressiveNumber">The progressive number of the WS</param>
        /// <param name="ownerNote">The owner note</param>
        /// <param name="workingSets">The working sets list - cannot be empty or null</param>
        /// <returns>The WorkUnitTemplate instance</returns>
        public static WorkUnitTemplate PlanWorkUnit(uint progressiveNumber, IdType excerciseId, IList<WorkingSetTemplate> workingSets, PersonalNoteValue ownerNote = null)

            => new WorkUnitTemplate(progressiveNumber, excerciseId, workingSets, ownerNote);

        #endregion



        #region Public Methods

        /// <summary>
        /// Attach a note to the WU, or repleace it if already present
        /// </summary>
        /// <param name="newNote">The note to be attached</param>
        public void WriteNote(PersonalNoteValue newNote)
        {
            OwnerNote = newNote;
        }

        /// <summary>
        /// Assign an excercise to the Work Unit
        /// </summary>
        /// <param name="excerciseId">The ID of the excercise</param>
        public void AssignExcercise(IdType excerciseId)
        {
            ExcerciseId = excerciseId;
        }


        /// <summary>
        /// Add the Working Set to the Work Unit
        /// </summary>
        /// <param name="repetitions">The WS repetitions</param>
        /// <param name="rest">The rest period between the WS and the following</param>
        /// <param name="effort">The WS effort</param>
        /// <param name="tempo">The WS lifting tempo</param>
        /// <param name="intensityTechniqueIds">The ids of the WS intensity techniques</param>
        public void AddWorkingSet(WSRepetitionValue repetitions, RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null, IList<IdType> intensityTechniqueIds = null)
        {
            _workingSets.Add(
                WorkingSetTemplate.AddWorkingSet(
                    BuildWorkingSetProgressiveNumber(),
                    repetitions,
                    rest,
                    effort,
                    tempo,
                    intensityTechniqueIds
                ));

            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Working Set from the Work Unit
        /// </summary>
        /// <param name="toRemoveId">The Id of the WS to be removed</param>
        public void RemoveWorkingSet(IdType toRemoveId)
        {
            WorkingSetTemplate toBeRemoved = FindWorkingSetById(toRemoveId);

            _workingSets.Remove(toBeRemoved);
            ForceConsecutiveWorkingSetsProgressiveNumbers();
            TestBusinessRules();
        }


        /// <summary>
        /// Find the Working Set with the ID specified
        /// </summary>
        /// <param name="id">The Id to be found</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <returns>The WokringSetTemplate object/returns>
        public WorkingSetTemplate FindWorkingSetById(IdType id)
        {
            if (Id == null)
                throw new ArgumentException($"Cannot find a WS with NULL id");

            WorkingSetTemplate ws = _workingSets.Where(x => x.Id == id).FirstOrDefault();

            if (ws == default)
                throw new ArgumentException($"Working Set with Id {id} could not be found");

            return ws;
        }

        /// <summary>
        /// Find the Working Set with the progressive number specified
        /// </summary>
        /// <param name="pNum">The progressive number to be found</param>
        /// <returns>The WokringSetTemplate object or DEFAULT if not found/returns>
        public WorkingSetTemplate FindWorkingSetByProgressiveNumber(uint pNum) => _workingSets.Where(x => x.ProgressiveNumber == pNum).FirstOrDefault();

        #endregion



        #region Working Sets Methods

        /// <summary>
        /// Change the repetitions
        /// </summary>
        /// <param name="newReps">The new target repetitions</param>
        /// <param name="workingSetId">The Id of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="ArgumentException">Thrown if WS not found</exception>
        public void ChangeWorkingSetRepetitions(IdType workingSetId, WSRepetitionValue newReps)
        {
            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);

            ws.ChangeRepetitions(newReps);
            TestBusinessRules();
        }


        /// <summary>
        /// Change the effort
        /// </summary>
        /// <param name="newEffort">The new value</param>
        /// <param name="workingSetId">The Id of the WS to be changed</param>/// <param name="workingSetId">The Id of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void ChangeWorkingSetEffort(IdType workingSetId, TrainingEffortValue newEffort)
        {
            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);

            ws.ChangeEffort(newEffort);
            TestBusinessRules();
        }


        /// <summary>
        /// Change the rest period of the WS specified
        /// </summary>
        /// <param name="newRest">The new value</param>
        /// <param name="workingSetId">The Id of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void ChangeWorkingSetRestPeriod(IdType workingSetId, RestPeriodValue newRest)
        {
            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);

            ws.ChangeRestPeriod(newRest);
            TestBusinessRules();
        }


        /// <summary>
        /// Change the lifting tempo
        /// </summary>
        /// <param name="newTempo">The new value</param>
        /// <param name="workingSetId">The Id of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void ChangeWorkingSetLiftingTempo(IdType workingSetId, TUTValue newTempo)
        {
            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);

            ws.ChangeLiftingTempo(newTempo);
            TestBusinessRules();
        }


        /// <summary>
        /// Add an intensity technique - Do nothing if already present in the list
        /// </summary>
        /// <param name="workingSetId">The Id of the WS to be changed</param>
        /// <param name="toAddId">The id to be added</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public void AddWorkingSetIntensityTechnique(IdType workingSetId, IdType toAddId)
        {

            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);

            ws.AddIntensityTechnique(toAddId);
            TestBusinessRules();
        }


        /// <summary>
        /// Remove an intensity technique 
        /// </summary>
        /// <param name="workingSetId">The Id of the WS to be changed</param>
        /// <param name="toRemoveId">The id to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        public bool RemoveWorkingSetIntensityTechnique(IdType workingSetId, IdType toRemoveId)
        {
            WorkingSetTemplate ws = FindWorkingSetById(workingSetId);

            bool ok = ws.RemoveIntensityTechnique(toRemoveId);
            TestBusinessRules();
            return ok;
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Build the next valid id
        /// </summary>
        /// <returns>The WS Id</returns>
        private IdType BuildWorkingSetId()
        {
            if (_workingSets.Count == 0)
                return new IdType(1);

            else
                return _workingSets.Last().Id + 1;
        }


        /// <summary>
        /// Build the next valid progressive number
        /// To be used before adding the WS to the list
        /// </summary>
        /// <returns>The WS Progressive Number</returns>
        private uint BuildWorkingSetProgressiveNumber() => (uint)_workingSets.Count();


        /// <summary>
        /// Force the WSs to have consecutive progressive numbers
        /// It works by assuming that the WSs are added in a sorted fashion.
        /// </summary>
        private void ForceConsecutiveWorkingSetsProgressiveNumbers()
        {
            // Just overwrite all the progressive numbers
            for(int iws = 0; iws < _workingSets.Count(); iws++)
            {
                WorkingSetTemplate ws = _workingSets[iws];
                ws.ChangeProgressiveNumber((uint)iws);
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
            // Check the first element: the sequence must start from 0
            if (_workingSets?.Count() <= 1)
            {
                if (_workingSets.FirstOrDefault()?.ProgressiveNumber == 0)
                    return true;
                else
                    return false;
            }

            // Look for non consecutive numbers - exclude the last one
            foreach(int pnum in _workingSets.Where(x => x.ProgressiveNumber != _workingSets.Count() - 1)
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
                throw new TrainingDomainInvariantViolationException($"The Work Unit musthave no NULL working sets.");

            if (!ExcerciseSpecified())
                throw new TrainingDomainInvariantViolationException($"The Work Unit must be linked to an excercise.");

            if (!AtLeastOneWorkingSet())
                throw new TrainingDomainInvariantViolationException($"Cannot create a Work Unit without any WS.");

            if (!WorkingSetsWithConsecutiveProgressiveNumber())
                throw new TrainingDomainInvariantViolationException($"Working Sets of the same Work Unit must have consecutive progressive numbers.");
        }
        #endregion

    }
}


