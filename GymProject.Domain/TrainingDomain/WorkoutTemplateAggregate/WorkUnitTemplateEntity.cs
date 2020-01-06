using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.Utils;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.WorkoutTemplateAggregate
{
    public class WorkUnitTemplateEntity : Entity<uint?>, ICloneable
    {


        /// <summary>
        /// The progressive number of the work unit - Starts from 0
        /// </summary>
        public uint ProgressiveNumber { get; private set; } = 0;



        ///// <summary>
        ///// The Work Unit which is linked to this one - If any
        ///// </summary>
        //public LinkedWorkValue LinkedWorkUnit { get; private set; } = null;


        ///// <summary>
        ///// The Work Unit which is linked to this one - If any
        ///// </summary>
        //public uint? LinkedWorkUnitId { get; private set; } = null;

        /// <summary>
        /// The Intensity Technique which links the WUs - if any linked WU
        /// </summary>
        public uint? LinkingIntensityTechniqueId { get; private set; } = null;


        private List<WorkingSetTemplateEntity> _workingSets = new List<WorkingSetTemplateEntity>();

        /// <summary>
        /// The Working Sets belonging to the WU, sorted by Progressive Numbers.
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<WorkingSetTemplateEntity> WorkingSets
        {
            get => _workingSets?.Clone().ToList().AsReadOnly()
                ?? new List<WorkingSetTemplateEntity>().AsReadOnly();  // Objects are not referencally equal
        }
        /// </summary>
        public uint? WorkUnitNoteId { get; private set; } = null;


        /// <summary>
        /// FK to the Excercise Aggregate
        /// </summary>
        public uint? ExcerciseId { get; private set; } = null;




        #region Ctors

        private WorkUnitTemplateEntity() : base(null)
        {

        }


        private WorkUnitTemplateEntity(uint? id, uint progressiveNumber, uint? excerciseId,
            IEnumerable<WorkingSetTemplateEntity> workingSets, uint? linkingIntensityTechniqueId = null, uint? ownerNoteId = null) : base(id)
        {
            ProgressiveNumber = progressiveNumber;
            WorkUnitNoteId = ownerNoteId;
            ExcerciseId = excerciseId;
            //LinkedWorkUnitId = linkedWorkUnitId;
            LinkingIntensityTechniqueId = linkingIntensityTechniqueId;

            _workingSets = workingSets?.Clone().ToList() ?? new List<WorkingSetTemplateEntity>();


            //foreach (WorkingSetTemplateEntity ws in _workingSets)
            //    ws.AddIntensityTechnique(LinkedWorkUnit.LinkingIntensityTechniqueId.Value);

            TestBusinessRules();
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method - Creates an empty Work Unit. Only the mandatory fields are required
        /// </summary>
        /// <param name="excerciseId">The ID of the excercise which the WU consists of</param>
        /// <param name="progressiveNumber">The progressive number of the WS</param>
        /// <returns>The WorkUnitTemplateEntity instance</returns>
        public static WorkUnitTemplateEntity NewDraft(uint progressiveNumber, uint? excerciseId)

            => PlanTransientWorkUnit(progressiveNumber, excerciseId, null, null, null);


        /// <summary>
        /// Factory method - Creates a transient WU
        /// </summary>
        /// <param name="excerciseId">The ID of the excercise which the WU consists of</param>
        /// <param name="progressiveNumber">The progressive number of the WS</param>
        /// <param name="ownerNoteId">The ID of the Work Unit Owner's note</param>
        /// <param name="workingSets">The working sets list - cannot be empty or null</param>
        /// <param name="linkingIntensityTechniqueId">The ID of the Intensity Technique which links the WUs - if any</param>
        /// <param name="linkedWorkUnitId">The ID of the Work Unit linked to this one - if any</param>
        /// <returns>The WorkUnitTemplateEntity instance</returns>
        public static WorkUnitTemplateEntity PlanTransientWorkUnit(uint progressiveNumber, uint? excerciseId,
            IEnumerable<WorkingSetTemplateEntity> workingSets, uint? linkingIntensityTechniqueId = null, uint? ownerNoteId = null)

            => new WorkUnitTemplateEntity(null, progressiveNumber, excerciseId, workingSets, linkingIntensityTechniqueId, ownerNoteId);


        /// <summary>
        /// Factory method - Loads a WU with the specified ID
        /// </summary>
        /// <param name="id">The Work Unit ID</param>
        /// <param name="excerciseId">The ID of the excercise which the WU consists of</param>
        /// <param name="progressiveNumber">The progressive number of the WS</param>
        /// <param name="ownerNoteId">The ID of the Work Unit Owner's note</param>
        /// <param name="workingSets">The working sets list - cannot be empty or null</param>
        /// <param name="linkingIntensityTechniqueId">The ID of the Intensity Technique which links the WUs - if any</param>
        /// <param name="linkedWorkUnitId">The ID of the Work Unit linked to this one - if any</param>
        /// <returns>The WorkUnitTemplateEntity instance</returns>
        public static WorkUnitTemplateEntity PlanWorkUnit(uint? id, uint progressiveNumber, uint? excerciseId,
            IEnumerable<WorkingSetTemplateEntity> workingSets, uint? linkingIntensityTechniqueId = null, uint? ownerNoteId = null)

            => new WorkUnitTemplateEntity(id, progressiveNumber, excerciseId, workingSets, linkingIntensityTechniqueId, ownerNoteId);

        #endregion



        #region Public Methods

        /// <summary>
        /// Assign a new progressive number to the WU
        /// </summary>
        /// <param name="newPnum">The new progressive number - PNums must be consecutive</param>
        public void MoveToNewProgressiveNumber(uint newPnum)
        {
            ProgressiveNumber = newPnum;
            //TestBusinessRules();
        }


        /// <summary>
        /// Link the specified Work Unit by appending it to this one linking them with the selected Intensity Technique
        /// </summary>
        /// <param name="linkingTechniqueId">The ID of the linking intensity technique</param>
        /// <param name="linkedWorkUnitId">The ID of the Work Unit to linked</param>
        public void LinkToNext(uint linkingTechniqueId)
        {
            //LinkedWorkUnitId = linkedWorkUnitId;
            LinkingIntensityTechniqueId = linkingTechniqueId;

            //LinkedWorkUnit = LinkedWorkValue.LinkTo(linkedWorkUnit.Id.Value, linkingTechniqueId);

            // Assume everything is already sorted by Progressive Number
            //List<WorkingSetTemplateEntity> linkedWorkingSets = linkedWorkUnit.WorkingSets.ToList();

            //foreach (WorkingSetTemplateEntity ws in _workingSets)
            //    ws.LinkTo(linkedWorkingSets[(int)ws.ProgressiveNumber].Id.Value, linkingTechniqueId);

            //TestBusinessRules();
        }


        /// <summary>
        /// Remove the Link to another Work Unit
        /// </summary>
        public void Unlink()
        {
            //LinkedWorkUnitId = null;
            LinkingIntensityTechniqueId = null;

            // Unlink the Working Sets
            //foreach (WorkingSetTemplateEntity ws in _workingSets)
            //    ws.Unlink();
        }


        /// <summary>
        /// Attach a note to the WU, or repleace it if already present
        /// </summary>
        /// <param name="newNoteId">The ID of the note to be attached</param>
        public void AssignNote(uint? newNoteId)
        {
            WorkUnitNoteId = newNoteId;
        }


        /// <summary>
        /// Remove the Owner's note
        /// </summary>
        public void RemoveNote()
        {
            AssignNote(null);
        }


        /// <summary>
        /// Assign an excercise to the Work Unit
        /// </summary>
        /// <param name="excerciseId">The ID of the excercise</param>
        public void AssignExcercise(uint? excerciseId)
        {
            ExcerciseId = excerciseId;
        }


        /// <summary>
        /// Add the Working Set to the Work Unit - copy not reference
        /// </summary>
        /// <param name="toAdd">The WS to be added</param>
        /// <exception cref="ArgumentException">If Working Set already present</exception>
        public void AddWorkingSet(WorkingSetTemplateEntity toAdd)
        {
            WorkingSetTemplateEntity copy = toAdd.Clone() as WorkingSetTemplateEntity;

            if (_workingSets.Contains(copy))
                throw new ArgumentException("Trying to add a duplicate Working Set", nameof(toAdd));

            //if(LinkedWorkUnit != null)
            //    copy.AddIntensityTechnique(LinkedWorkUnit.LinkingIntensityTechniqueId.Value);

            _workingSets.Add(copy);

            TestBusinessRules();
        }


        /// <summary>
        /// Add the Working Set to the Work Unit
        /// </summary>
        /// <param name="repetitions">The WS repetitions</param>
        /// <param name="rest">The rest period between the WS and the following</param>
        /// <param name="effort">The WS effort</param>
        /// <param name="tempo">The WS lifting tempo</param>
        /// <param name="intensityTechniqueIds">The ids of the WS intensity techniques</param>
        public void AddTransientWorkingSet(WSRepetitionsValue repetitions, RestPeriodValue rest = null, TrainingEffortValue effort = null, TUTValue tempo = null,
            IEnumerable<uint?> intensityTechniqueIds = null)
        {
            List<uint?> localIntensityTechniqueIds = CommonUtilities.RemoveDuplicatesFrom(intensityTechniqueIds)?.ToList() ?? new List<uint?>();

            // Apply the WU-wise intensity techniques - if any - to the added WS
            //if (LinkedWorkUnit != null)
            //    localIntensityTechniqueIds.Add(LinkedWorkUnit.LinkingIntensityTechniqueId);

            WorkingSetTemplateEntity toAdd = WorkingSetTemplateEntity.PlanTransientWorkingSet(
                    BuildWorkingSetProgressiveNumber(),
                    repetitions,
                    rest,
                    effort,
                    tempo,
                    localIntensityTechniqueIds
                );

            _workingSets.Add(toAdd);

            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Working Set from the Work Unit
        /// </summary>
        /// <param name="toRemove">The WS to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        /// <returns>True if remove successful</returns>
        public void RemoveWorkingSet(WorkingSetTemplateEntity toRemove)
        {
            if (toRemove == null)
                return;

            uint progressiveNumber = toRemove.ProgressiveNumber;

            if (_workingSets.Remove(toRemove))
            {
                ManageLinkedWorkingSets(toRemove);
                ForceConsecutiveWorkingSetsProgressiveNumbers(progressiveNumber);
                TestBusinessRules();
            }
        }


        /// <summary>
        /// Remove the Working Set from the Work Unit
        /// </summary>
        /// <param name="progressiveNumber">The progressive number of the WS to be removed</param>
        /// <exception cref="InvalidOperationException">If more Working Sets with the specified Progressive Number</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void RemoveWorkingSet(uint progressiveNumber)
        {
            RemoveWorkingSet(FindWorkingSet(progressiveNumber));
        }


        /// <summary>
        /// Remove the Working Set from the Work Unit
        /// </summary>
        /// <param name="workingSetId">The ID of the WS to be removed</param>
        /// <exception cref="InvalidOperationException">If more Working Sets with the specified Progressive Number</exception>
        /// <exception cref="TrainingDomainInvariantViolationException"></exception>
        public void RemoveWorkingSetById(uint workingSetId)
        {
            RemoveWorkingSet(FindWorkingSetById(workingSetId));
        }


        /// <summary>
        /// Get a copy of the Working Set with the progressive number specified or DEFAULT if not found
        /// </summary>
        /// <param name="pNum">The progressive number to be found</param>
        /// <exception cref="InvalidOperationException">If more Working Sets with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if not found</returns>
        public WorkingSetTemplateEntity CloneWorkingSet(uint pNum)

            => FindWorkingSetOrDefault(pNum)?.Clone() as WorkingSetTemplateEntity;


        /// <summary>
        /// Check whether the Work Unit is linked to another one
        /// </summary>
        /// <returns>True if the Work Unit is linked to another one</returns>
        public bool HasLinkedUnit()

            //=> LinkedWorkUnitId != null;
            => LinkingIntensityTechniqueId != null;

        /// <summary>
        /// Get the main effort type as the effort of most of the WSs of the WU 
        /// </summary>
        /// <returns>The training effort type</returns>
        public TrainingEffortTypeEnum GetMainEffortType()

            => _workingSets.GroupBy(x => x.ToEffort().EffortType).Select(x
                    => new
                    {
                        Counter = x.Count(),
                        EffortType = x.Key
                    })
                    .OrderByDescending(x => x.Counter).FirstOrDefault()?.EffortType
                ?? TrainingEffortTypeEnum.IntensityPercentage;
        #endregion



        #region Working Sets Methods

        /// <summary>
        /// Assign a new progressive number to the WS
        /// </summary>
        /// <param name="newPnum">The new progressive number - PNums must be consecutive</param>
        /// <param name="toMovePnum">The Progressive Number of the WS to be moved</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void MoveWorkingSetToNewProgressiveNumber(uint toMovePnum, uint newPnum)
        {
            WorkingSetTemplateEntity dest = FindWorkingSet(newPnum);
            WorkingSetTemplateEntity source = FindWorkingSet(toMovePnum);

            // Switch sets
            dest.MoveToNewProgressiveNumber(toMovePnum);
            source.MoveToNewProgressiveNumber(newPnum);

            ForceConsecutiveWorkingSetsProgressiveNumbers();
            TestBusinessRules();
        }


        /// <summary>
        /// Change the repetitions
        /// </summary>
        /// <param name="newReps">The new target repetitions</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void ReviseWorkingSetRepetitions(uint workingSetPnum, WSRepetitionsValue newReps)
        {
            WorkingSetTemplateEntity ws = FindWorkingSet(workingSetPnum);
            ws.ReviseRepetitions(newReps);
        }


        /// <summary>
        /// Change the effort
        /// </summary>
        /// <param name="newEffort">The new value</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void ReviseWorkingSetEffort(uint workingSetPnum, TrainingEffortValue newEffort)
        {
            WorkingSetTemplateEntity ws = FindWorkingSet(workingSetPnum);
            ws.ReviseEffort(newEffort);
        }


        /// <summary>
        /// Change the rest period of the WS specified
        /// </summary>
        /// <param name="newRest">The new value</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void ReviseWorkingSetRestPeriod(uint workingSetPnum, RestPeriodValue newRest)
        {
            WorkingSetTemplateEntity ws = FindWorkingSet(workingSetPnum);
            ws.ReviseRestPeriod(newRest);
        }


        /// <summary>
        /// Change the lifting tempo
        /// </summary>
        /// <param name="newTempo">The new value</param>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be changed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void ReviseWorkingSetLiftingTempo(uint workingSetPnum, TUTValue newTempo)
        {
            WorkingSetTemplateEntity ws = FindWorkingSet(workingSetPnum);
            ws.ReviseLiftingTempo(newTempo);
        }


        /// <summary>
        /// Add an intensity technique - Do nothing if already present in the list
        /// </summary>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be changed</param>
        /// <param name="toAddId">The id to be added</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void AddWorkingSetIntensityTechnique(uint workingSetPnum, uint toAddId)
        {
            WorkingSetTemplateEntity ws = FindWorkingSet(workingSetPnum);
            ws.AddIntensityTechnique(toAddId);
        }


        /// <summary>
        /// Remove an intensity technique 
        /// </summary>
        /// <param name="workingSetPnum">The Progressive Number of the WS to be changed</param>
        /// <param name="toRemoveId">The id to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        /// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        public void RemoveWorkingSetIntensityTechnique(uint workingSetPnum, uint toRemoveId)
        {
            WorkingSetTemplateEntity ws = FindWorkingSet(workingSetPnum);

            ws.RemoveIntensityTechnique(toRemoveId);
            TestBusinessRules();
        }

        #endregion



        #region COMMENTED - Linked Working Sets, Might be needed at a later stage

        /// <summary>
        /// Manage the linked WSs - if any - to the one that has just been removed
        /// </summary>
        /// <param name="removedWorkingSet">The Working Set which has been removed</param>
        private void ManageLinkedWorkingSets(WorkingSetTemplateEntity removedWorkingSet)
        {

            //
            // Linked WS disabled -> might be needed at a later stage
            //


            //uint progressiveNumber = removedWorkingSet.ProgressiveNumber;

            //// If the removed WS was linked to the previous one, then replace the removed with the following one
            //if (progressiveNumber > 0)
            //{
            //    WorkingSetTemplateEntity previousWorkingSet = FindWorkingSet(progressiveNumber - 1);

            //    // If nothing can be linked, then unlink
            //    if (IsLastWorkingSet(progressiveNumber - 1))
            //        previousWorkingSet.Unlink();
            //    else
            //    {
            //        // If the removed WS had a linked one, then link the two WSs
            //        uint? removedLinkingIntensityTechnique = removedWorkingSet.LinkedWorkingSet?.LinkingIntensityTechniqueId;

            //        if (removedLinkingIntensityTechnique.HasValue)
            //            previousWorkingSet.LinkTo(progressiveNumber, removedLinkingIntensityTechnique.Value);
            //        else
            //            previousWorkingSet.Unlink();
            //    }
            //}
        }


        ///// <summary>
        ///// Link the specified Working Set to the following one
        ///// </summary>
        ///// <param name="startingWorkingSetPnum">The Progressive Number of the WS which starts the linked serie</param>
        ///// <param name="linkingIntensityTechnique">The ID of the Intensity Technique which linkes the two WSs</param>
        ///// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        ///// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum, or if the WS is not the last one</exception>
        //public void LinkWorkingSet(uint startingWorkingSetPnum, uint linkingIntensityTechnique)
        //{
        //    if (IsLastWorkingSet(startingWorkingSetPnum))
        //        throw new InvalidOperationException($"No WS to link to the selected one as it is the last one.");

        //    WorkingSetTemplateEntity ws = FindWorkingSet(startingWorkingSetPnum);
        //    ws.LinkTo(startingWorkingSetPnum + 1, linkingIntensityTechnique);
        //}


        ///// <summary>
        ///// Append the specified number of WSs linked by the selected Intensity Technique
        ///// </summary>
        ///// <param name="numberOfLinkedWSets">The number of linked Working Sets to be added</param>
        ///// <param name="linkingIntensityTechnique">The ID of the Intensity Technique which linkes the WSs</param>
        ///// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        ///// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        //public void AppendLinkedWorkingSets(uint numberOfLinkedWSets, uint linkingIntensityTechnique)
        //{
        //    for(int iws = 0; iws < numberOfLinkedWSets)
        //    {
        //        WorkingSetTemplateEntity toAdd = WorkingSetTemplateEntity.PlanTransientWorkingSet(
        //            BuildWorkingSetProgressiveNumber(),
        //            repetitions,
        //            rest,
        //            effort,
        //            tempo,
        //            localIntensityTechniqueIds
        //        );

        //        _workingSets.Add(toAdd);
        //    }

        //    WorkingSetTemplateEntity ws = FindWorkingSet(startingWorkingSetPnum);
        //    ws.LinkTo(startingWorkingSetPnum + 1, linkingIntensityTechnique);
        //}


        ///// <summary>
        ///// Unlink the specified working set from the following one
        ///// </summary>
        ///// <param name="startingWorkingSetPnum">The Progressive Number of the WS to remove from the following one</param>
        ///// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules not met</exception>
        ///// <exception cref="InvalidOperationException">Thrown if no WS or more than one with the same Pnum</exception>
        //public void UnlinkWorkingSet(uint startingWorkingSetPnum)
        //{
        //    FindWorkingSet(startingWorkingSetPnum).Unlink();

        //    ForceConsecutiveWorkingSetsProgressiveNumbers();
        //    TestBusinessRules();
        //}
        #endregion



        #region Private Methods

        /// <summary>
        /// Build the next valid progressive number
        /// To be used before adding the WS to the list
        /// </summary>
        /// <returns>The WS Progressive Number</returns>
        private uint BuildWorkingSetProgressiveNumber() => (uint)_workingSets.Count();


        /// <summary>
        /// Check wether the WS with the specified progressive number is the last one of the Work Unit
        /// </summary>
        /// <param name="workingSetProgressiveNumber">The progressive numebr of the WS to be checked</param>
        /// <returns>True if the WS is the last one</returns>
        private bool IsLastWorkingSet(uint workingSetProgressiveNumber)

            => workingSetProgressiveNumber == _workingSets.Count - 1;

        /// <summary>
        /// Sort the Working Sets list wrt the WS progressive numbers
        /// </summary>
        /// <param name="wsIn">The input WS list</param>
        /// <returns>The sorted list</returns>
        private IEnumerable<WorkingSetTemplateEntity> SortWorkingSetsByProgressiveNumber(IEnumerable<WorkingSetTemplateEntity> wsIn)

            => wsIn.OrderBy(x => x.ProgressiveNumber);

        /// <summary>
        /// Force the WSs to have consecutive progressive numbers
        /// It works by assuming that the WSs are added in a sorted fashion.
        /// </summary>
        private void ForceConsecutiveWorkingSetsProgressiveNumbers()
        {
            _workingSets = SortWorkingSetsByProgressiveNumber(_workingSets).ToList();

            // Just overwrite all the progressive numbers
            for (int iws = 0; iws < _workingSets.Count(); iws++)
            {
                WorkingSetTemplateEntity ws = _workingSets[iws];
                ws.MoveToNewProgressiveNumber((uint)iws);
            }
        }

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
                WorkingSetTemplateEntity ws = _workingSets[iws];
                ws.MoveToNewProgressiveNumber((uint)iws);
            }
        }


        /// <summary>
        /// Find the Working Set with the progressive number specified
        /// </summary>
        /// <param name="pNum">The progressive number to be found</param>
        /// <exception cref="InvalidOperationException">If more Working Sets with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object or DEFAULT if notfound</returns>
        private WorkingSetTemplateEntity FindWorkingSetOrDefault(uint pNum)

            => _workingSets.SingleOrDefault(x => x.ProgressiveNumber == pNum);


        /// <summary>
        /// Find the Working Set with the progressive number specified
        /// </summary>
        /// <param name="pNum">The progressive number to be found</param>
        /// <exception cref="InvalidOperationException">If more Working Sets, or zero, with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object</returns>
        private WorkingSetTemplateEntity FindWorkingSet(uint pNum)

            => _workingSets.Single(x => x.ProgressiveNumber == pNum);


        /// <summary>
        /// Find the Working Set with the specified ID
        /// </summary>
        /// <param name="id">The ID to be found</param>
        /// <exception cref="InvalidOperationException">If more Working Sets, or zero, with the specified Progressive Number</exception>
        /// <returns>The WorkingSetTemplate object</returns>
        private WorkingSetTemplateEntity FindWorkingSetById(uint id)

            => _workingSets.Single(x => x.Id == id);

        #endregion


        #region Business Rules Validation

        ///// <summary>
        ///// The Work Unit cannot be linked to itself.
        ///// </summary>
        ///// <returns>True if business rule is met</returns>
        //private bool IsNotLinkedToItself() => LinkedWorkUnitId == null || LinkedWorkUnitId != Id;


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
        /// Working sets must have all the work unit intensity techniques.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool AllWorkingSetsHaveWorkUnitWiseIntensityTechniques()
        {
            if (LinkingIntensityTechniqueId == null)
                return true;

            return _workingSets.All(x 
                => x.IntensityTechniqueIds.Contains(LinkingIntensityTechniqueId));
        }


        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {
            if (!ExcerciseSpecified())
                throw new TrainingDomainInvariantViolationException($"The Work Unit must be linked to an excercise.");

            //if (!IsNotLinkedToItself())
            //    throw new TrainingDomainInvariantViolationException($"The Work Unit cannot be linked to itself.");

            //if (!AtLeastOneWorkingSet())
            //    throw new TrainingDomainInvariantViolationException($"Cannot create a Work Unit without any WS.");

            //if (!AllWorkingSetsHaveWorkUnitWiseIntensityTechniques())
            //    throw new TrainingDomainInvariantViolationException($"Working sets must have all the work unit intensity techniques.");

            if (!WorkingSetsWithConsecutiveProgressiveNumber())
                throw new TrainingDomainInvariantViolationException($"Working Sets of the same Work Unit must have consecutive progressive numbers.");
        }

        #endregion


        #region IClonable Implementation

        public object Clone()

            => PlanWorkUnit(Id, ProgressiveNumber, ExcerciseId, WorkingSets.ToList(), LinkingIntensityTechniqueId, WorkUnitNoteId);

        #endregion
    }
}


