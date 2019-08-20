﻿using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Common;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GymProject.Domain.TrainingDomain.TrainingPlanAggregate
{
    public class TrainingPlan : Entity<IdType>, ICloneable
    {



        /// <summary>
        /// The name of the Training Plan
        /// </summary>
        public string Name { get; private set; } = string.Empty;


        ///// <summary>
        ///// The description of the Training Plan
        ///// </summary>
        //public PersonalNoteValue Description { get; private set; } = null;


        /// <summary>
        /// The Training Plan has been set as 'Bookmarked'
        /// </summary>
        public bool IsBookmarked { get; private set; } = false;


        /// <summary>
        /// The Training Plan is a Template for other plans
        /// </summary>
        public bool IsTemplate { get; private set; } = false;


        /// <summary>
        /// The type of the Training Plan - Template, Variant, Inherited etc.
        /// </summary>
        public TrainingPlanTypeEnum TrainingPlanType { get; private set; } = null;


        /// <summary>
        /// The training volume parameters, as the sum of the params of the single Training Weeks
        /// </summary>
        public TrainingVolumeParametersValue TrainingVolume { get; private set; } = null;


        /// <summary>
        /// The training effort, as the average of the single Training Weeks efforts
        /// </summary>
        public TrainingIntensityParametersValue TrainingIntensity { get; private set; } = null;


        /// <summary>
        /// The training density parameters, as the sum of the params of the single Training Weeks
        /// </summary>
        public TrainingDensityParametersValue TrainingDensity { get; private set; } = null;


        private IList<TrainingWeekTemplate> _trainingWeeks = new List<TrainingWeekTemplate>();

        /// <summary>
        /// The Training Weeks which the Training Plan is scheduled to
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<TrainingWeekTemplate> TrainingWeeks
        {
            get => _trainingWeeks?.Clone().ToList().AsReadOnly() ?? new List<TrainingWeekTemplate>().AsReadOnly();
        }


        /// <summary>
        /// FK to the Training Plan note - Optional
        /// </summary>
        public IdType PersonalNoteId { get; private set; } = null;


        /// <summary>
        /// FK to the Training Plan Message, when sent to another user -> Only Inherited Plans can have it - Optional
        /// </summary>
        public IdType AttachedMessageId { get; private set; } = null;


        /// <summary>
        /// FK to the owner of the Training Plan - The author or the receiver if Inherited Training Plan
        /// </summary>
        public IdType OwnerId { get; private set; } = null;


        private ICollection<IdType> _trainingScheduleIds = null;

        /// <summary>
        /// FK to the Training Schedules
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<IdType> TrainingScheduleIds
        {
            get => _trainingScheduleIds?.Clone().ToList().AsReadOnly() ?? new List<IdType>().AsReadOnly();
        }


        private ICollection<IdType> _childTrainingPlanIds = null;

        /// <summary>
        /// FK to the Training Plans which are derived from this one
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<IdType> ChildTrainingPlanIds
        {
            get => _childTrainingPlanIds?.Clone().ToList().AsReadOnly() ?? new List<IdType>().AsReadOnly();
        }


        private ICollection<IdType> _trainingPhaseIds = null;

        /// <summary>
        /// FK to the Training Plan target Phases
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<IdType> TrainingPhaseIds
        {
            get => _trainingPhaseIds?.Clone().ToList().AsReadOnly() ?? new List<IdType>().AsReadOnly();
        }


        private ICollection<IdType> _trainingProficiencyIds = null;

        /// <summary>
        /// FK to the Training target Proficiencies
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<IdType> TrainingProficiencyIds
        {
            get => _trainingProficiencyIds?.Clone().ToList().AsReadOnly() ?? new List<IdType>().AsReadOnly();
        }


        private ICollection<IdType> _muscleFocusIds = null;

        /// <summary>
        /// FK to the Training Muscle focus
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<IdType> MuscleFocusIds
        {
            get => _muscleFocusIds?.Clone().ToList().AsReadOnly() ?? new List<IdType>().AsReadOnly();
        }


        private ICollection<IdType> _hashtags = null;

        /// <summary>
        /// FK to the Training target Proficiencies
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<IdType> Hashtags
        {
            get => _hashtags?.Clone().ToList().AsReadOnly() ?? new List<IdType>().AsReadOnly();
        }





        #region Ctors

        private TrainingPlan(
            IdType id, 
            string name,
            bool isBookmarked,
            bool isTemplate,
            IdType ownerId,
            IdType personalNoteId = null, 
            IdType attachedMessageId = null,
            TrainingPlanTypeEnum trainingPlanType = null,
            IList<TrainingWeekTemplate> trainingWeeks = null,
            ICollection<IdType> trainingScheduleIds = null,
            ICollection<IdType> trainingPhaseIds = null,
            ICollection<IdType> trainingPlanProficiencyIds = null,
            ICollection<IdType> trainingMuscleFocusIds = null,
            ICollection<IdType> hashtags = null,
            ICollection<IdType> childTrainingPlanIds = null)
        {
            Id = id;
            Name = name ?? string.Empty;
            IsBookmarked = isBookmarked;
            IsTemplate = isTemplate;
            OwnerId = ownerId;
            PersonalNoteId = personalNoteId;
            AttachedMessageId = attachedMessageId;
            TrainingPlanType = trainingPlanType ?? TrainingPlanTypeEnum.NotSet;

            _trainingWeeks = trainingWeeks?.Clone().ToList() ?? new List<TrainingWeekTemplate>();

            _trainingScheduleIds = trainingScheduleIds?.NoDuplicatesClone().ToList() ?? new List<IdType>();
            _childTrainingPlanIds = childTrainingPlanIds?.NoDuplicatesClone().ToList() ?? new List<IdType>();
            _trainingPhaseIds = trainingPhaseIds?.NoDuplicatesClone().ToList() ?? new List<IdType>();
            _trainingProficiencyIds = trainingPlanProficiencyIds?.NoDuplicatesClone().ToList() ?? new List<IdType>();
            _muscleFocusIds = trainingMuscleFocusIds?.NoDuplicatesClone().ToList() ?? new List<IdType>();
            _hashtags = hashtags?.Clone().ToList() ?? new List<IdType>();

            TestBusinessRules();

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets(), GetMainEffortType());
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The ID of the Training Plan</param>
        /// <param name="trainingWeeks">The Training Weeks which the Training Plan is made up of</param>
        /// <param name="name">The name of the Training Plan</param>
        /// <param name="trainingPlanType">The type of the Training Plan - optional</param>
        /// <param name="attachedMessageId">The ID of the message to be attached</param>
        /// <param name="isBookmarked">The Training Plan has been flagged as Bookmarked</param>
        /// <param name="isTemplate">The Training Plan has been flagged as Template</param>
        /// <param name="ownerId">The ID of the owner of the plan</param>
        /// <param name="personalNoteId">The ID of the note</param>
        /// <param name="childTrainingPlanIds">The list of the child training plans (Variant, Inherited etc.)</param>
        /// <param name="hashtags">The list of the IDS of the Hashtags which the Training Plan has been tagged with</param>
        /// <param name="trainingPhaseIds">The list of the IDs of the Training Phases which the Training Plan has been tagged with</param>
        /// <param name="trainingPlanProficiencyIds">The list of the  IDs of the Training Proficiencies which the Training Plan has been tagged with</param>
        /// <param name="trainingMuscleFocusIds">The list of the  IDs of the Muscles which the Training Plan focuses on</param>
        /// <param name="trainingScheduleIds">The list of the IDs of the Training Schedules which the Training Plan has been scheduled to</param>
        public static TrainingPlan CreateTrainingPlan(
            IdType id,
            string name,
            bool isBookmarked,
            bool isTemplate,
            IdType ownerId,
            IdType personalNoteId = null,
            IdType attachedMessageId = null,
            TrainingPlanTypeEnum trainingPlanType = null,
            IList<TrainingWeekTemplate> trainingWeeks = null,
            ICollection<IdType> trainingScheduleIds = null,
            ICollection<IdType> trainingPhaseIds = null,
            ICollection<IdType> trainingPlanProficiencyIds = null,
            ICollection<IdType> trainingMuscleFocusIds = null,
            ICollection<IdType> hashtags = null,
            ICollection<IdType> childTrainingPlanIds = null)

            => new TrainingPlan(id, name, isBookmarked, isTemplate, ownerId, personalNoteId, attachedMessageId, trainingPlanType, trainingWeeks, 
                trainingScheduleIds, trainingPhaseIds, trainingPlanProficiencyIds, trainingMuscleFocusIds, hashtags, childTrainingPlanIds);


        /// <summary>
        /// Factory method for Inherited Training Plans
        /// </summary>
        /// <param name="id">The ID of the Training Plan</param>
        /// <param name="rootPlan">The Training Plan to be sent</param>
        /// <param name="destinationUserId">The ID of the receiving User</param>
        /// <param name="attachedMessageId">The ID of the message to be sent with the Plan - Optional</param>
        /// <param name="trainingScheduleId">The ID of the Training Schedule for this Plan - Optional</param>
        /// <returns></returns>
        public static TrainingPlan SendInheritedTrainingPlan(
            IdType id,
            TrainingPlan rootPlan,
            IdType destinationUserId,
            IdType attachedMessageId = null,
            IdType trainingScheduleId = null)

            => CreateTrainingPlan(
                id: id,
                name: rootPlan.Name,
                isBookmarked: false,
                isTemplate: false,
                ownerId: destinationUserId,
                personalNoteId: null,
                attachedMessageId: attachedMessageId,
                trainingPlanType: TrainingPlanTypeEnum.Inherited,
                trainingWeeks: rootPlan.TrainingWeeks.ToList(),
                trainingScheduleIds: new List<IdType>() { trainingScheduleId });


        /// <summary>
        /// Factory method for Variant Training Plans
        /// </summary>
        /// <param name="id">The ID of the Training Plan</param>
        /// <param name="rootPlan">The Training Plan to create a Varaint of</param>
        /// <returns></returns>
        public static TrainingPlan CreateVariantTrainingPlan(
            IdType id,
            TrainingPlan rootPlan)

            => CreateTrainingPlan(
                id: id,
                name: rootPlan.Name,
                isBookmarked: false,
                isTemplate: false,
                ownerId: rootPlan.OwnerId,
                personalNoteId: null,
                attachedMessageId: null,
                trainingPlanType: TrainingPlanTypeEnum.Variant,
                trainingWeeks: rootPlan.TrainingWeeks.ToList(),
                trainingScheduleIds: null,
                trainingPhaseIds: rootPlan.TrainingPhaseIds.ToList(),
                trainingPlanProficiencyIds: rootPlan.TrainingProficiencyIds.ToList(),
                trainingMuscleFocusIds: rootPlan.MuscleFocusIds.ToList(),
                hashtags: rootPlan.Hashtags.ToList(),
                childTrainingPlanIds: null);

        #endregion



        #region Public Methods


        // Messages should be attached only when sending plans!
        //public void AttachMessage(IdType messageId) =>


        // Training plan type should be assigned only when creating a new instance
        //public void SetTrainingPlanType() =>

        // Training plan relations should be assigned only when creating a new instance
        //public void AddChildTrainingPlan() =>

        // Unschedule shouldn't be possible -> reschedule instead (IE: change the Schedule object)
        //public void UnscheduleTraining() =>


        /// <summary>
        /// Assign the name to the Training Plan
        /// </summary>
        /// <param name="trainingPlanName">The Training Plan name</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void GiveName(string trainingPlanName)
        {
            Name = trainingPlanName;
            TestBusinessRules();
        }


        /// <summary>
        /// Assign the IsBookmarked flag
        /// </summary>
        /// <param name="bookmarkedFlag">The flag</param>
        public void ChangeBookmarkedFlag(bool bookmarkedFlag) => IsBookmarked = bookmarkedFlag;



        /// <summary>
        /// Assign the IsTemplate flag
        /// </summary>
        /// <param name="templateFlag">The flag</param>
        public void ChangeTemplateFlag(bool templateFlag) => IsTemplate = templateFlag;


        /// <summary>
        /// Assign the Training Plan Note ID
        /// </summary>
        /// <param name="trainingPlanNoteId">The note ID</param>
        public void WriteNote(IdType trainingPlanNoteId) => PersonalNoteId = trainingPlanNoteId;


        /// <summary>
        /// Add the Hastag to the Training Plan
        /// </summary>
        /// <param name="hashtagId">The Hashtag ID to be added</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void AddHashtag(IdType hashtagId)
        {
            if (hashtagId == null)
                throw new ArgumentNullException($"Hashtag ID must be valid when tagging the Training Plan",nameof(hashtagId));

            if (_hashtags.Contains(hashtagId))
                return;

            _hashtags.Add(hashtagId);
            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Hastag from the Training Plan
        /// </summary>
        /// <param name="hashtagId">The Hashtag ID to be removed</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void RemoveHashtag(IdType hashtagId)
        {
            if (hashtagId == null)
                throw new ArgumentNullException($"Hashtag ID must be valid when tagging the Training Plan", nameof(hashtagId));

            _hashtags.Remove(hashtagId);
            TestBusinessRules();
        }


        /// <summary>
        /// Link the Phase to the Training Plan
        /// </summary>
        /// <param name="phaseId">The Phase ID to be added</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void LinkTargetPhase(IdType phaseId)
        {
            if (phaseId == null)
                throw new ArgumentNullException($"Phase ID must be valid when tagging the Training Plan", nameof(phaseId));

            if (_trainingPhaseIds.Contains(phaseId))
                return;

            _trainingPhaseIds.Add(phaseId);
            TestBusinessRules();
        }


        /// <summary>
        /// Unlink the Phase from the Training Plan
        /// </summary>
        /// <param name="phaseId">The Hashtag ID to be removed</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void UnlinkTargetPhase(IdType phaseId)
        {
            if (phaseId == null)
                throw new ArgumentNullException($"Phase ID must be valid when tagging the Training Plan", nameof(phaseId));

            _trainingPhaseIds.Remove(phaseId);
            TestBusinessRules();
        }


        /// <summary>
        /// Link the Proficiency to the Training Plan
        /// </summary>
        /// <param name="proficiencyId">The Proficiency ID to be added</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void LinkTargetProficiency(IdType proficiencyId)
        {
            if (proficiencyId == null)
                throw new ArgumentNullException($"Proficiency ID must be valid when tagging the Training Plan", nameof(proficiencyId));

            if (_trainingProficiencyIds.Contains(proficiencyId))
                return;

            _trainingProficiencyIds.Add(proficiencyId);
            TestBusinessRules();
        }


        /// <summary>
        /// Unlink the Proficiency from the Training Plan
        /// </summary>
        /// <param name="proficiencyId">The Proficiency ID to be removed</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void UnlinkTargetProficiency(IdType proficiencyId)
        {
            if (proficiencyId == null)
                throw new ArgumentNullException($"Proficiency ID must be valid when tagging the Training Plan", nameof(proficiencyId));

            _trainingProficiencyIds.Remove(proficiencyId);
            TestBusinessRules();
        }


        /// <summary>
        /// Give focus to the Muscle
        /// </summary>
        /// <param name="proficiencyId">The Muscle ID to be added</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void GiveFocusToMuscle(IdType muscleId)
        {
            if (muscleId == null)
                throw new ArgumentNullException($"Proficiency ID must be valid when tagging the Training Plan", nameof(muscleId));

            if (_muscleFocusIds.Contains(muscleId))
                return;

            _muscleFocusIds.Add(muscleId);
            TestBusinessRules();
        }


        /// <summary>
        /// Remove the focus to the Muscle
        /// </summary>
        /// <param name="muscleId">The Muscle ID to be removed</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void RemoveFocusToMuscle(IdType muscleId)
        {
            if (muscleId == null)
                throw new ArgumentNullException($"Proficiency ID must be valid when tagging the Training Plan", nameof(muscleId));

            _muscleFocusIds.Remove(muscleId);
            TestBusinessRules();
        }


        /// <summary>
        /// Schedule the Training Plan by assigning a Schedule ID
        /// </summary>
        /// <param name="scheduleId">The Schedule ID to be added</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void ScheduleTraining(IdType scheduleId)
        {
            if (scheduleId == null)
                throw new ArgumentNullException($"Schedule ID must be valid when tagging the Training Plan", nameof(scheduleId));

            if (_trainingScheduleIds.Contains(scheduleId))
                return;

            _trainingScheduleIds.Add(scheduleId);
            TestBusinessRules();
        }


        /// <summary>
        /// Add the Training Week to the Plan
        /// </summary>
        /// <param name="workouts">The list of the WOs which the Training Plan is made up of</param>
        /// <param name="weekType">The type of the Training Week</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void AddTrainingWeek(IList<WorkoutTemplate> workouts, TrainingWeekTypeEnum weekType)
        {
            TrainingWeekTemplate toAdd = TrainingWeekTemplate.AddTrainingWeekToPlan(
                BuildTrainingWeekId(),
                BuildTrainingWeekProgressiveNumber(),
                workouts,
                weekType);

            _trainingWeeks.Add(toAdd);

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets(), GetMainEffortType());

            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Training Week from the Plan
        /// </summary>
        /// <param name="toRemoveId">The Id of the Training Plan to be removed</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void RemoveTrainingWeek(IdType toRemoveId)
        {
            TrainingWeekTemplate toBeRemoved = FindTrainingWeekById(toRemoveId);

            bool removed = _trainingWeeks.Remove(toBeRemoved);

            if(removed)
            {
                TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
                TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets());
                TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(GetAllWorkingSets(), GetMainEffortType());

                ForceConsecutiveTrainingWeeksProgressiveNumbers();
                TestBusinessRules();
            }
        }


        /// <summary>
        /// Find the Training Week with the ID specified
        /// </summary>
        /// <param name="id">The Id to be found</param>
        /// <exception cref="ArgumentNullException">If ID could not be found</exception>
        /// <returns>The WokringSetTemplate object/returns>
        public TrainingWeekTemplate FindTrainingWeekById(IdType id)
        {
            if (id == null)
                throw new ArgumentNullException($"Cannot find a Training Week with NULL id");

            TrainingWeekTemplate week = _trainingWeeks.Where(x => x.Id == id).FirstOrDefault();

            if (week == default)
                throw new ArgumentException($"Working Set with Id {id.ToString()} could not be found");

            return week;
        }

        /// <summary>
        /// Find the Training Week with the progressive number specified
        /// </summary>
        /// <param name="pNum">The progressive number to be found</param>
        /// <returns>The WokringSetTemplate object/returns>
        /// <exception cref="ArgumentNullException">If ID could not be found</exception>
        public TrainingWeekTemplate FindTrainingWeekByProgressiveNumber(int pNum)
        {
            TrainingWeekTemplate week = _trainingWeeks.Where(x => x.ProgressiveNumber == pNum).FirstOrDefault();

            if (week == default)
                throw new ArgumentException($"Training Week with Progressive number {pNum.ToString()} could not be found");

            return week;
        }


        /// <summary>
        /// Get the WUs of all the Workouts belonging to the Training Week
        /// </summary>
        /// <returns>The list of the Working Sets</returns>
        public IEnumerable<WorkUnitTemplate> GetAllWorkUnits()

             => _trainingWeeks.SelectMany(x => x.GetAllWorkUnits());


        /// <summary>
        /// Get the WSs of all the Workouts belonging to the Plan
        /// </summary>
        /// <returns>The list of the Working Sets</returns>
        public IEnumerable<WorkingSetTemplate> GetAllWorkingSets()

             => _trainingWeeks.SelectMany(x => x.GetAllWorkingSets());


        /// <summary>
        /// Get the main effort type as the effort of most of the WSs of the WU 
        /// </summary>
        /// <returns>The training effort type</returns>
        public TrainingEffortTypeEnum GetMainEffortType()

            => _trainingWeeks.Count == 0 ? TrainingEffortTypeEnum.IntensityPerc
                : GetAllWorkingSets().GroupBy(x => x.Effort.EffortType).Select(x
                     => new
                     {
                         Counter = x.Count(),
                         EffortType = x.Key
                     }).OrderByDescending(x => x.Counter).First().EffortType;

        #endregion


        #region Private Methods

        /// <summary>
        /// Build the next valid id
        /// </summary>
        /// <returns>The Training Week Id</returns>
        private IdType BuildTrainingWeekId()
        {
            if (_trainingWeeks.Count == 0)
                return new IdType(1);

            else
                return new IdType(_trainingWeeks.Max(x => x.Id.Id) + 1);
        }


        /// <summary>
        /// Build the next valid progressive number
        /// To be used before adding the Training Week to the list
        /// </summary>
        /// <returns>The Training Week Progressive Number</returns>
        private uint BuildTrainingWeekProgressiveNumber() => (uint)_trainingWeeks.Count();


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
                TrainingWeekTemplate ws = _trainingWeeks[itw];
                ws.MoveToNewProgressiveNumber((uint)itw);
            }
        }


        /// <summary>
        /// Sort the Training Week list wrt their progressive numbers
        /// </summary>
        /// <param name="inputWeeks">The input Training Week list</param>
        /// <returns>The sorted list</returns>
        private IEnumerable<TrainingWeekTemplate> SortByProgressiveNumber(IEnumerable<TrainingWeekTemplate> inputWeeks)

            => inputWeeks.OrderBy(x => x.ProgressiveNumber);

        #endregion


        #region Business Rules Validation

        /// <summary>
        /// The Training Plan must have no NULL Training Weeks.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullWeeks() => _trainingWeeks.All(x => x != null);


        /// <summary>
        /// The Owner of the Training Plan cannot be NULL.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool OwnerIsNotNull() => OwnerId != null;


        /// <summary>
        /// Non-Inherited Training Plans must have no message attached.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NonInheritedPlansHasNoMessage() => TrainingPlanType == TrainingPlanTypeEnum.Inherited || AttachedMessageId == null;


        /// <summary>
        /// The Training Plan must have no NULL Training Schedules.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullSchedules() => _trainingScheduleIds.All(x => x != null);


        /// <summary>
        /// The Training Plan must have no NULL Training Phases.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullPhases() => _trainingPhaseIds.All(x => x != null);


        /// <summary>
        /// The Training Plan must have no NULL Training Proficiency.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullProficiencies() => _trainingProficiencyIds.All(x => x != null);


        /// <summary>
        /// The Training Plan must have no NULL Child Training Plans.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullChildPlans() => _childTrainingPlanIds.All(x => x != null);


        /// <summary>
        /// The Training Plan must have no NULL Muscle Focus entries.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullMuscleFocus() => _muscleFocusIds.All(x => x != null);
        

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
            foreach (int pnum in _trainingWeeks.Where(x => x.ProgressiveNumber != _trainingWeeks.Count() - 1)
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
            if (!NoNullWeeks())
                throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Training Weeks.");

            if (!OwnerIsNotNull())
                throw new TrainingDomainInvariantViolationException($"The Owner of the Training Plan cannot be NULL.");

            if (!NoNullSchedules())
                throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Training Schedules.");

            if (!NoNullPhases())
                throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Training Phases.");

            if (!NoNullProficiencies())
                throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Training Proficiency.");

            if (!NoNullChildPlans())
                throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Child Training Plans.");

            if (!NoNullMuscleFocus())
                throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Muscle Focus entries.");

            //if (!NameIsNotEmpty())
            //    throw new TrainingDomainInvariantViolationException($"The Training Plan must have a valid name.");

            if (!NonInheritedPlansHasNoMessage())
                throw new TrainingDomainInvariantViolationException($"Non-Inherited Training Plans must have no message attached.");

            if (!TrainingWeeksWithConsecutiveProgressiveNumber())
                throw new TrainingDomainInvariantViolationException($"Training Weeks of the same Training Plan must have consecutive progressive numbers.");
        }

        #endregion


        #region IClonable Interface

        public object Clone()

            => CreateTrainingPlan(Id, Name, IsBookmarked, IsTemplate, OwnerId, PersonalNoteId, AttachedMessageId, TrainingPlanType,
                TrainingWeeks.ToList(), TrainingScheduleIds.ToList(), TrainingPhaseIds.ToList(), TrainingProficiencyIds.ToList(), MuscleFocusIds.ToList(), 
                Hashtags.ToList(), ChildTrainingPlanIds.ToList());

        #endregion
    }
}