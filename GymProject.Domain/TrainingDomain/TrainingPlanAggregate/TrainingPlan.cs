using GymProject.Domain.Base;
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
    public class TrainingPlan : Entity<IdTypeValue>, ICloneable
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
        public IdTypeValue PersonalNoteId { get; private set; } = null;


        /// <summary>
        /// FK to the Training Plan Message, when sent to another user -> Only Inherited Plans can have it - Optional
        /// </summary>
        public IdTypeValue AttachedMessageId { get; private set; } = null;


        /// <summary>
        /// FK to the owner of the Training Plan - The author or the receiver if Inherited Training Plan
        /// </summary>
        public IdTypeValue OwnerId { get; private set; } = null;


        private ICollection<IdTypeValue> _trainingScheduleIds = null;

        /// <summary>
        /// FK to the Training Schedules
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<IdTypeValue> TrainingScheduleIds
        {
            get => _trainingScheduleIds?.ToList().AsReadOnly() ?? new List<IdTypeValue>().AsReadOnly();
        }


        private ICollection<IdTypeValue> _childTrainingPlanIds = null;

        /// <summary>
        /// FK to the Training Plans which are derived from this one
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<IdTypeValue> ChildTrainingPlanIds
        {
            get => _childTrainingPlanIds?.ToList().AsReadOnly() ?? new List<IdTypeValue>().AsReadOnly();
        }


        private ICollection<IdTypeValue> _trainingPhaseIds = null;

        /// <summary>
        /// FK to the Training Plan target Phases
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<IdTypeValue> TrainingPhaseIds
        {
            get => _trainingPhaseIds?.ToList().AsReadOnly() ?? new List<IdTypeValue>().AsReadOnly();
        }


        private ICollection<IdTypeValue> _trainingProficiencyIds = null;

        /// <summary>
        /// FK to the Training target Proficiencies
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<IdTypeValue> TrainingProficiencyIds
        {
            get => _trainingProficiencyIds?.ToList().AsReadOnly() ?? new List<IdTypeValue>().AsReadOnly();
        }


        private ICollection<IdTypeValue> _muscleFocusIds = null;

        /// <summary>
        /// FK to the Training Muscle focus
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<IdTypeValue> MuscleFocusIds
        {
            get => _muscleFocusIds?.ToList().AsReadOnly() ?? new List<IdTypeValue>().AsReadOnly();
        }


        private ICollection<IdTypeValue> _hashtags = null;

        /// <summary>
        /// FK to the Training target Proficiencies
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<IdTypeValue> Hashtags
        {
            get => _hashtags?.ToList().AsReadOnly() ?? new List<IdTypeValue>().AsReadOnly();
        }





        #region Ctors

        private TrainingPlan(
            IdTypeValue id,
            string name,
            bool isBookmarked,
            bool isTemplate,
            IdTypeValue ownerId,
            TrainingPlanTypeEnum trainingPlanType = null,
            IdTypeValue personalNoteId = null,
            IdTypeValue attachedMessageId = null,
            IList<TrainingWeekTemplate> trainingWeeks = null,
            ICollection<IdTypeValue> trainingScheduleIds = null,
            ICollection<IdTypeValue> trainingPhaseIds = null,
            ICollection<IdTypeValue> trainingPlanProficiencyIds = null,
            ICollection<IdTypeValue> trainingMuscleFocusIds = null,
            ICollection<IdTypeValue> hashtags = null,
            ICollection<IdTypeValue> childTrainingPlanIds = null)   : base(id)
        {
            Name = name ?? string.Empty;
            IsBookmarked = isBookmarked;
            IsTemplate = isTemplate;
            OwnerId = ownerId;
            PersonalNoteId = personalNoteId;
            AttachedMessageId = attachedMessageId;
            TrainingPlanType = trainingPlanType ?? TrainingPlanTypeEnum.NotSet;

            _trainingWeeks = trainingWeeks?.Clone().ToList() ?? new List<TrainingWeekTemplate>();

            _trainingScheduleIds = trainingScheduleIds?.ToList() ?? new List<IdTypeValue>();
            _childTrainingPlanIds = childTrainingPlanIds?.ToList() ?? new List<IdTypeValue>();
            _trainingPhaseIds = trainingPhaseIds?.ToList() ?? new List<IdTypeValue>();
            _trainingProficiencyIds = trainingPlanProficiencyIds?.ToList() ?? new List<IdTypeValue>();
            _muscleFocusIds = trainingMuscleFocusIds?.ToList() ?? new List<IdTypeValue>();
            _hashtags = hashtags?.ToList() ?? new List<IdTypeValue>();

            TestBusinessRules();

            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets());
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets());
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
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
            IdTypeValue id,
            string name,
            bool isBookmarked,
            bool isTemplate,
            IdTypeValue ownerId,
            TrainingPlanTypeEnum trainingPlanType = null,
            IdTypeValue personalNoteId = null,
            IdTypeValue attachedMessageId = null,
            IList<TrainingWeekTemplate> trainingWeeks = null,
            ICollection<IdTypeValue> trainingScheduleIds = null,
            ICollection<IdTypeValue> trainingPhaseIds = null,
            ICollection<IdTypeValue> trainingPlanProficiencyIds = null,
            ICollection<IdTypeValue> trainingMuscleFocusIds = null,
            ICollection<IdTypeValue> hashtags = null,
            ICollection<IdTypeValue> childTrainingPlanIds = null)

            => new TrainingPlan(id, name, isBookmarked, isTemplate, ownerId, trainingPlanType, personalNoteId, attachedMessageId, trainingWeeks,
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
            IdTypeValue id,
            TrainingPlan rootPlan,
            IdTypeValue destinationUserId,
            IdTypeValue attachedMessageId = null,
            IdTypeValue trainingScheduleId = null)

            => CreateTrainingPlan(
                id: id,
                name: rootPlan.Name,
                isBookmarked: false,
                isTemplate: false,
                ownerId: destinationUserId,
                trainingPlanType: TrainingPlanTypeEnum.Inherited,
                personalNoteId: null,
                attachedMessageId: attachedMessageId,
                trainingWeeks: rootPlan.TrainingWeeks.ToList(),
                trainingScheduleIds: new List<IdTypeValue>() { trainingScheduleId });


        /// <summary>
        /// Factory method for Variant Training Plans
        /// </summary>
        /// <param name="id">The ID of the Training Plan</param>
        /// <param name="rootPlan">The Training Plan to create a Varaint of</param>
        /// <returns></returns>
        public static TrainingPlan CreateVariantTrainingPlan(
            IdTypeValue id,
            TrainingPlan rootPlan)

            => CreateTrainingPlan(
                id: id,
                name: rootPlan.Name,
                isBookmarked: false,
                isTemplate: false,
                ownerId: rootPlan.OwnerId,
                trainingPlanType: TrainingPlanTypeEnum.Variant,
                personalNoteId: null,
                attachedMessageId: null,
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


        // Training plan type should be assigned only when creating a new instance - Excpet for variant maybe
        //public void SetTrainingPlanType() =>


        // Unschedule shouldn't be possible -> reschedule instead (IE: change the Schedule object)
        //public void UnscheduleTraining() =>

        // A template is created only when linked to a child -> look at the proper function
        //public void ChangeTemplateFlag(bool templateFlag) => IsTemplate = templateFlag;

        /// <summary>
        /// Mark the Training Plan as variant, if not already marked as a special type plan. Raises an exception otherwise
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        /// <exception cref="InvalidOperationException">If trying to change the type of a plan which has already been marked as a special type</exception>
        public void MarkAsVariant()
        {
            if (TrainingPlanType == TrainingPlanTypeEnum.Variant)
                return;

            if (TrainingPlanType != TrainingPlanTypeEnum.NotSet)
                throw new InvalidOperationException($"Trying to change the type of a Training Plan which has already a non-generic type.");

            TrainingPlanType = TrainingPlanTypeEnum.Variant;
            TestBusinessRules();
        }

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
        /// Assign the Training Plan Note ID
        /// </summary>
        /// <param name="trainingPlanNoteId">The note ID</param>
        public void WriteNote(IdTypeValue trainingPlanNoteId) => PersonalNoteId = trainingPlanNoteId;


        /// <summary>
        /// Remove the Training Plan Note ID
        /// </summary>
        public void CleanNote() => WriteNote(null);


        /// <summary>
        /// Make the Training Plan Template by attaching the specified one
        /// </summary>
        /// <param name="childPlanId">The Training Plan ID to be added to the child ones</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void AttachChildToTemplatePlan(IdTypeValue childPlanId)
        {
            if (childPlanId == null)
                throw new ArgumentNullException($"Child Plan ID must be valid when attaching to the Training Plan", nameof(childPlanId));

            if (_childTrainingPlanIds.Contains(childPlanId))
                return;

            _childTrainingPlanIds.Add(childPlanId);

            IsTemplate = true;
            TestBusinessRules();
        }


        /// <summary>
        /// Removes the specified Training Plan from this one childs.
        /// If no child left, then mark the plan as 'Not Template'
        /// </summary>
        /// <param name="childPlanId">The Training Plan ID to be removed from the child ones</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void DetachChildToTemplatePlan(IdTypeValue childPlanId)
        {
            if (childPlanId == null)
                throw new ArgumentNullException($"Child Plan ID must be valid when detaching from the Training Plan", nameof(childPlanId));

            _childTrainingPlanIds.Remove(childPlanId);

            if (_childTrainingPlanIds.Count == 0)
                IsTemplate = false;

            TestBusinessRules();
        }


        /// <summary>
        /// Add the Hastag to the Training Plan
        /// </summary>
        /// <param name="hashtagId">The Hashtag ID to be added</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void AddHashtag(IdTypeValue hashtagId)
        {
            if (hashtagId == null)
                throw new ArgumentNullException($"Hashtag ID must be valid when tagging the Training Plan", nameof(hashtagId));

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
        public void RemoveHashtag(IdTypeValue hashtagId)
        {
            if (hashtagId == null)
                throw new ArgumentNullException($"Hashtag ID must be valid when tagging the Training Plan", nameof(hashtagId));

            if(_hashtags.Remove(hashtagId))
                TestBusinessRules();
        }


        /// <summary>
        /// Link the Phase to the Training Plan
        /// </summary>
        /// <param name="phaseId">The Phase ID to be added</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void LinkTargetPhase(IdTypeValue phaseId)
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
        public void UnlinkTargetPhase(IdTypeValue phaseId)
        {
            if (phaseId == null)
                throw new ArgumentNullException($"Phase ID must be valid when tagging the Training Plan", nameof(phaseId));

            if(_trainingPhaseIds.Remove(phaseId))
                TestBusinessRules();
        }


        /// <summary>
        /// Link the Proficiency to the Training Plan
        /// </summary>
        /// <param name="proficiencyId">The Proficiency ID to be added</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void LinkTargetProficiency(IdTypeValue proficiencyId)
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
        public void UnlinkTargetProficiency(IdTypeValue proficiencyId)
        {
            if (proficiencyId == null)
                throw new ArgumentNullException($"Proficiency ID must be valid when tagging the Training Plan", nameof(proficiencyId));

            if(_trainingProficiencyIds.Remove(proficiencyId))
                TestBusinessRules();
        }


        /// <summary>
        /// Give focus to the Muscle
        /// </summary>
        /// <param name="muscleId">The Muscle ID to be added</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void GiveFocusToMuscle(IdTypeValue muscleId)
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
        public void RemoveFocusToMuscle(IdTypeValue muscleId)
        {
            if (muscleId == null)
                throw new ArgumentNullException($"Proficiency ID must be valid when tagging the Training Plan", nameof(muscleId));

            if(_muscleFocusIds.Remove(muscleId))
                TestBusinessRules();
        }


        /// <summary>
        /// Schedule the Training Plan by assigning a Schedule ID
        /// </summary>
        /// <param name="scheduleId">The Schedule ID to be added</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void ScheduleTraining(IdTypeValue scheduleId)
        {
            if (scheduleId == null)
                throw new ArgumentNullException($"Schedule ID must be valid when tagging the Training Plan", nameof(scheduleId));

            if (_trainingScheduleIds.Contains(scheduleId))
                return;

            _trainingScheduleIds.Add(scheduleId);
            TestBusinessRules();
        }


        /// <summary>
        /// Get the main effort type as the effort of most of the WSs of the WU 
        /// </summary>
        /// <returns>The training effort type</returns>
        public TrainingEffortTypeEnum GetMainEffortType()

            => _trainingWeeks.Sum(x => x?.CloneAllWorkingSets().Count()) == 0 

                ? TrainingEffortTypeEnum.IntensityPerc
                : CloneAllWorkingSets().GroupBy(x => x.Effort.EffortType).Select(x
                     => new
                     {
                         Counter = x.Count(),
                         EffortType = x.Key
                     }).OrderByDescending(x => x.Counter).First().EffortType;


        /// <summary>
        /// Get the WSs of all the Workouts belonging to the Plan
        /// </summary>
        /// <returns>The list of the Working Sets</returns>
        public IEnumerable<WorkingSetTemplate> CloneAllWorkingSets()

             => _trainingWeeks.SelectMany(x => x.CloneAllWorkingSets());

        #endregion


        #region Training Week Methods

        /// <summary>
        /// Add the Training Week to the Plan.
        /// </summary>
        /// <param name="trainingWeek">The input Training Week</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        /// <exception cref="ArgumentNullException">If null input</exception>
        public void AddTrainingWeek(TrainingWeekTemplate trainingWeek)
        {
            if (trainingWeek == null)
                throw new ArgumentNullException(nameof(trainingWeek), "Null input when trying to create a new Training Week.");

            if (trainingWeek.IsFullRestWeek())
                AddFullRestWeek();
            else
                AddTrainingWeek(trainingWeek.TrainingWeekType, trainingWeek.Workouts.ToList());
        }


        /// <summary>
        /// Add the Training Week to the Plan. This is not meant to work with Full Rest Weeks.
        /// </summary>
        /// <param name="workoutsReferences">The list of the WOs which the Training Plan is made up of</param>
        /// <param name="weekType">The type of the Training Week</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        /// <exception cref="ArgumentException">If the week type is a Full Rest one</exception>
        public void AddTrainingWeek(TrainingWeekTypeEnum weekType, IList<WorkoutTemplateReferenceValue> workoutsReferences)
        {
            if (weekType == TrainingWeekTypeEnum.FullRest)
                throw new ArgumentException("Cannot add Full Rest Weeks with this function.", nameof(weekType));

            TrainingWeekTemplate toAdd = TrainingWeekTemplate.PlanTransientTrainingWeek(
                BuildTrainingWeekProgressiveNumber(),
                workoutsReferences,
                weekType);

            _trainingWeeks.Add(toAdd);

            TestBusinessRules();

            TrainingVolume = TrainingVolume.AddWorkingSets(toAdd.CloneAllWorkingSets());
            TrainingDensity = TrainingDensity.AddWorkingSets(toAdd.CloneAllWorkingSets());
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
        }


        /// <summary>
        /// Add a Full Rest Week to the Plan
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void AddFullRestWeek()
        {
            _trainingWeeks.Add(TrainingWeekTemplate.PlanTransientFullRestWeek(BuildTrainingWeekProgressiveNumber()));

            TestBusinessRules();

            // Training Parameters don't change
        }


        /// <summary>
        /// Add the Training Week to the Plan
        /// </summary>
        /// <param name="restWeek">The Training Week, which must be a Full Rest one</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        /// <exception cref="ArgumentException">If the input week is not a Full Rest one</exception>
        /// <exception cref="ArgumentNullException">If null input</exception>
        public void AddFullRestWeek(TrainingWeekTemplate restWeek)
        {
            if (restWeek == null)
                throw new ArgumentNullException(nameof(restWeek), "Null input when trying to create a new Full Rest Week.");

            if (!restWeek.IsFullRestWeek())
                throw new ArgumentException("Trying to add a Full Rest Week from a non-rest one.", nameof(restWeek));

            _trainingWeeks.Add(
                TrainingWeekTemplate.PlanFullRestWeek(
                    restWeek.Id,
                    BuildTrainingWeekProgressiveNumber()));

            TestBusinessRules();

            // Training Parameters don't change
        }


        /// <summary>
        /// Remove the Training Week from the Plan
        /// </summary>
        /// <param name="toRemoveId">The Id of the Training Plan to be removed</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="ArgumentNullException">If ID is NULL</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void RemoveTrainingWeek(IdTypeValue toRemoveId)
        {
            TrainingWeekTemplate toBeRemoved = FindTrainingWeekById(toRemoveId);

            bool removed = _trainingWeeks.Remove(toBeRemoved);

            if (removed)
            {
                ForceConsecutiveTrainingWeeksProgressiveNumbers(toBeRemoved.ProgressiveNumber);
                TestBusinessRules();

                TrainingVolume = TrainingVolume.RemoveWorkingSets(toBeRemoved.CloneAllWorkingSets());
                TrainingDensity = TrainingDensity.RemoveWorkingSets(toBeRemoved.CloneAllWorkingSets());
                TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
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
            TrainingWeekTemplate src = FindTrainingWeekByProgressiveNumber((int)srcPnum);
            TrainingWeekTemplate dest = FindTrainingWeekByProgressiveNumber((int)destPnum);

            src.MoveToNewProgressiveNumber(srcPnum);
            dest.MoveToNewProgressiveNumber(destPnum);

            ForceConsecutiveTrainingWeeksProgressiveNumbers();
            TestBusinessRules();
        }


        /// <summary>
        /// Make the week a full rest one. 
        /// This function also clears all the workouts scheduled, in order to meet the business rule.
        /// </summary>
        /// <param name="weekType">The training week type</param>
        /// <param name="weekPnum">The Progressive Number of the week to be moved</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void AssignSpecificWeekType(uint weekPnum, TrainingWeekTypeEnum weekType)
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

        #endregion


        #region Workout Methods


        /// <summary>
        /// Add the Workout to the specified Training Week
        /// </summary>
        /// <param name="workingSets">The list of the WSs which the WO is made up of</param>
        /// <param name="weekPnum">The Progressive Number of the Week to which to add the WO to</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void PlanWorkout(uint weekPnum, ICollection<WorkingSetTemplate> workingSets)
        {
            FindTrainingWeekByProgressiveNumber((int)weekPnum).PlanWorkout(workingSets);

            TestBusinessRules();

            TrainingVolume = TrainingVolume.AddWorkingSets(workingSets);
            TrainingDensity = TrainingDensity.AddWorkingSets(workingSets);
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
        }


        /// <summary>
        /// Remove the Workout from the specified Training Week
        /// </summary>
        /// <param name="workoutPnum">The Progressive Number of the WO to be removed</param>
        /// <param name="weekPnum">The Progressive Number of the Week to which to remove the WO from</param>
        /// <exception cref="InvalidOperationException">If no workouts or more than one found</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        public void UnplanWorkout(uint weekPnum, uint workoutPnum)
        {
            TrainingWeekTemplate week = FindTrainingWeekByProgressiveNumber((int)weekPnum);
            IEnumerable<WorkingSetTemplate> removedWorkingSets = week.CloneWorkoutWorkingSets(workoutPnum);

            week.UnplanWorkout(workoutPnum);

            TestBusinessRules();

            TrainingVolume = TrainingVolume.RemoveWorkingSets(removedWorkingSets);
            TrainingDensity = TrainingDensity.RemoveWorkingSets(removedWorkingSets);
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
        }


        /// <summary>
        /// Assign a new progressive number to the WO of the Training Week
        /// </summary>
        /// <param name="destPnum">The new Progressive Number - PNums must be consecutive</param>
        /// <param name="srcPnum">The Progressive Number of the WO to be moved</param>
        public void MoveWorkoutToNewProgressiveNumber(uint weekPnum, uint srcPnum, uint destPnum)
        {
            TrainingWeekTemplate week = FindTrainingWeekByProgressiveNumber((int)weekPnum);
            week.MoveWorkoutToNewProgressiveNumber(srcPnum, destPnum);
        }


        /// <summary>
        /// Get the average number of Workouts per Training Week of the Plan
        /// </summary>
        /// <returns>The average number of weekly workouts</returns>
        public float GetAverageWorkoutsPerWeek()

            => (float)_trainingWeeks.Where(x => x?.Workouts != null).DefaultIfEmpty()?.Average(x => x?.Workouts.Count ?? 0);


        /// <summary>
        /// Get the minimum number of weekly Workouts
        /// </summary>
        /// <returns>The minimum number of weekly workouts</returns>
        public int GetMinimumWorkoutsPerWeek()

            => (int)_trainingWeeks.Where(x => x?.Workouts != null).DefaultIfEmpty()?.Min(x => x?.Workouts.Count ?? 0);


        /// <summary>
        /// Get the amximum number of weekly Workouts
        /// </summary>
        /// <returns>The maximum number of weekly workouts</returns>
        public int GetMaximumWorkoutsPerWeek()

            => (int)_trainingWeeks.Where(x => x?.Workouts != null).DefaultIfEmpty()?.Max(x => x?.Workouts.Count ?? 0);


        #endregion


        #region Private Methods


        /// <summary>
        /// Find the Training Week with the ID specified
        /// </summary>
        /// <param name="id">The Id to be found</param>
        /// <exception cref="ArgumentNullException">If ID could not be found</exception>
        /// <returns>The WokringSetTemplate object/returns>
        private TrainingWeekTemplate FindTrainingWeekById(IdTypeValue id)
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
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        private TrainingWeekTemplate FindTrainingWeekByProgressiveNumber(int pNum)
        {
            TrainingWeekTemplate week = _trainingWeeks.Where(x => x.ProgressiveNumber == pNum).FirstOrDefault();

            if (week == default)
                throw new ArgumentException($"Training Week with Progressive number {pNum.ToString()} could not be found");

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
                TrainingWeekTemplate ws = _trainingWeeks[itw];
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
            // Just overwrite all the progressive numbers
            for (int iwo = (int)fromPnum; iwo < _trainingWeeks.Count(); iwo++)
                _trainingWeeks[iwo].MoveToNewProgressiveNumber((uint)iwo);
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
        /// A Template plan cannot have itself as a Child.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool PlanIsNotTemplateAndChild() => _childTrainingPlanIds.All(x => x != Id);


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

            if (!PlanIsNotTemplateAndChild())
                throw new TrainingDomainInvariantViolationException($"A Template plan cannot have itself as a Child.");
        }

        #endregion


        #region IClonable Interface

        public object Clone()

            => CreateTrainingPlan(Id, Name, IsBookmarked, IsTemplate, OwnerId, TrainingPlanType, PersonalNoteId, AttachedMessageId,
                TrainingWeeks.ToList(), TrainingScheduleIds.ToList(), TrainingPhaseIds.ToList(), TrainingProficiencyIds.ToList(), MuscleFocusIds.ToList(),
                Hashtags.ToList(), ChildTrainingPlanIds.ToList());

        #endregion
    }
}
