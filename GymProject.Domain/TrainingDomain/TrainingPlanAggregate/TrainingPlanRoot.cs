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
    public class TrainingPlanRoot : Entity<uint?>, IAggregateRoot, ICloneable
    {



        public uint Id { get; private set; }

        /// <summary>
        /// The name of the Training Plan
        /// </summary>
        public string Name { get; private set; } = string.Empty;


        ///// <summary>
        ///// The description of the Training Plan
        ///// </summary>
        //public PersonalNoteValue Description { get; private set; } = null;


        /// <summary>
        /// A Training Plan is marked as 'Template' if exists at least one variant plan created from it. Derived property.
        /// </summary>
        public bool IsTemplate
            => _relationsWithChildPlans.Count(x => x?.ChildPlanId != null && x?.ChildPlanType == TrainingPlanTypeEnum.Variant) > 0;


        /// <summary>
        /// The Training Plan has been set as 'Bookmarked'
        /// </summary>
        public bool IsBookmarked { get; private set; } = false;


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


        /// <summary>
        /// FK to the Training Plan note - Optional
        /// </summary>
        public uint? PersonalNoteId { get; private set; } = null;


        private  IList<TrainingWeekEntity> _trainingWeeks = new List<TrainingWeekEntity>();

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
        public uint? OwnerId { get; private set; } = null;


        private ICollection<uint> _trainingScheduleIds = new List<uint>();

        /// <summary>
        /// FK to the Training Schedules
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<uint?> TrainingScheduleIds
        {
            get => _trainingScheduleIds?.ToList().AsReadOnly() ?? new List<uint?>().AsReadOnly();
        }


        private  ICollection<TrainingPlanRelation> _relationsWithChildPlans = new List<TrainingPlanRelation>();

        /// <summary>
        /// Get the relations which occur with the child plans - if any - of this one
        /// </summary>
        public IReadOnlyCollection<TrainingPlanRelation> RelationsWithChildPlans
        {
            get => _relationsWithChildPlans?.ToList().AsReadOnly()
                ?? new List<TrainingPlanRelation>().AsReadOnly();
        }


        private  ICollection<TrainingPlanRelation> _relationsWithParentPlans = new List<TrainingPlanRelation>();

        /// <summary>
        /// Get the relations which occur with the child plans - if any - of this one
        /// </summary>
        public IReadOnlyCollection<TrainingPlanRelation> RelationsWithParentPlans
        {
            get => _relationsWithParentPlans?.ToList().AsReadOnly()
                ?? new List<TrainingPlanRelation>().AsReadOnly();
        }


        private  ICollection<TrainingPlanPhaseRelation> _trainingPlanPhases = new List<TrainingPlanPhaseRelation>();

        /// <summary>
        /// FK to the Training Plan target Phases
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<uint?> TrainingPhaseIds
        {
            get => _trainingPlanPhases?.Select(x => x.TrainingPhaseId).ToList().AsReadOnly()
                ?? new List<uint?>().AsReadOnly();
        }


        private  ICollection<TrainingPlanProficiencyRelation> _trainingPlanProficiencies = new List<TrainingPlanProficiencyRelation>();

        /// <summary>
        /// FK to the Training target Proficiencies
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<uint?> TrainingProficiencyIds
        {
            get => _trainingPlanProficiencies?.Select(x => x.TrainingProficiencyId).ToList().AsReadOnly()
                ?? new List<uint?>().AsReadOnly();
        }


        private  ICollection<TrainingPlanMuscleFocusRelation> _trainingPlanMuscleFocusIds = new List<TrainingPlanMuscleFocusRelation>();

        /// <summary>
        /// FK to the Training Muscle focus
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<uint?> MuscleFocusIds
        {
            get => _trainingPlanMuscleFocusIds?.Select(x => x.MuscleId).ToList().AsReadOnly()
                ?? new List<uint?>().AsReadOnly();
        }


        private  ICollection<TrainingPlanHashtagRelation> _trainingPlanHashtags = new List<TrainingPlanHashtagRelation>();

        /// <summary>
        /// FK to the Training Hashtags
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<uint?> HashtagIds
        {
            get => _trainingPlanHashtags?.Select(x => x.HashtagId).ToList().AsReadOnly()
                ?? new List<uint?>().AsReadOnly();
        }





        #region Ctors


        private TrainingPlanRoot() { } //: base(null) { }


        private TrainingPlanRoot(
            uint? id,
            string name,
            bool isBookmarked,
            uint? ownerId,
            uint? personalNoteId = null,
            IEnumerable<TrainingWeekEntity> trainingWeeks = null,
            IEnumerable<uint?> trainingScheduleIds = null,
            IEnumerable<uint?> trainingPhaseIds = null,
            IEnumerable<uint?> trainingPlanProficiencyIds = null,
            IEnumerable<uint?> trainingMuscleFocusIds = null,
            IEnumerable<uint?> hashtags = null,
            IEnumerable<TrainingPlanRelation> relationsWithChildPlans = null) : base(id)
        {
            Name = name ?? string.Empty;
            IsBookmarked = isBookmarked;
            OwnerId = ownerId;
            PersonalNoteId = personalNoteId;

            _trainingWeeks = trainingWeeks?.Clone().ToList() ?? new List<TrainingWeekEntity>();

            _trainingScheduleIds = trainingScheduleIds?.ToList() ?? new List<uint?>();
            _relationsWithChildPlans = relationsWithChildPlans?.ToList() ?? new List<TrainingPlanRelation>();
            //_relationsWithParentPlans = relationsWithChildPlans?.ToList() ?? new List<TrainingPlanRelation>();        // Not Supported yet

            _trainingPlanHashtags = new List<TrainingPlanHashtagRelation>();
            _trainingPlanMuscleFocusIds = new List<TrainingPlanMuscleFocusRelation>();
            _trainingPlanProficiencies = new List<TrainingPlanProficiencyRelation>();
            _trainingPlanPhases = new List<TrainingPlanPhaseRelation>();

            // Build  many-to-many relations
            foreach (uint? hashtag in hashtags ?? new List<uint?>())
                _trainingPlanHashtags.Add(TrainingPlanHashtagRelation.BuildLink(this, hashtag));

            foreach (uint? muscle in trainingMuscleFocusIds ?? new List<uint?>())
                _trainingPlanMuscleFocusIds.Add(TrainingPlanMuscleFocusRelation.BuildLink(this, muscle));

            foreach (uint proficiency in trainingPlanProficiencyIds ?? new List<uint>())
                _trainingPlanProficiencies.Add(TrainingPlanProficiencyRelation.BuildLink(this, proficiency));

            foreach (uint phase in trainingPhaseIds ?? new List<uint>())
                _trainingPlanPhases.Add(TrainingPlanPhaseRelation.BuildLink(this, phase));

            TestBusinessRules();

            // Training Parameters
            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets());
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets());
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
        }



        private TrainingPlanRoot(
            string name,
            bool isBookmarked,
            uint ownerId,
            uint personalNoteId = default,
            IEnumerable<TrainingWeekEntity> trainingWeeks = null,
            IEnumerable<uint> trainingScheduleIds = null,
            IEnumerable<uint> trainingPhaseIds = null,
            IEnumerable<uint> trainingPlanProficiencyIds = null,
            IEnumerable<uint> trainingMuscleFocusIds = null,
            IEnumerable<uint> hashtags = null,
            IEnumerable<TrainingPlanRelation> relationsWithChildPlans = null) //: base(id)
        {
            Name = name ?? string.Empty;
            IsBookmarked = isBookmarked;
            OwnerId = ownerId;
            PersonalNoteId = personalNoteId;

            _trainingWeeks = trainingWeeks?.Clone().ToList() ?? new List<TrainingWeekEntity>();

            _trainingScheduleIds = trainingScheduleIds?.ToList() ?? new List<uint>();
            _relationsWithChildPlans = relationsWithChildPlans?.ToList() ?? new List<TrainingPlanRelation>();
            //_relationsWithParentPlans = relationsWithChildPlans?.ToList() ?? new List<TrainingPlanRelation>();    // Not supported yet

            _trainingPlanHashtags = new List<TrainingPlanHashtagRelation>();
            _trainingPlanMuscleFocusIds = new List<TrainingPlanMuscleFocusRelation>();
            _trainingPlanProficiencies = new List<TrainingPlanProficiencyRelation>();
            _trainingPlanPhases = new List<TrainingPlanPhaseRelation>();

            // Build  many-to-many relations
            foreach (uint hashtag in hashtags ?? new List<uint>())
                _trainingPlanHashtags.Add(TrainingPlanHashtagRelation.BuildLink(this, hashtag));

            foreach (uint muscle in trainingMuscleFocusIds ?? new List<uint>())
                _trainingPlanMuscleFocusIds.Add(TrainingPlanMuscleFocusRelation.BuildLink(this, muscle));

            foreach (uint proficiency in trainingPlanProficiencyIds ?? new List<uint>())
                _trainingPlanProficiencies.Add(TrainingPlanProficiencyRelation.BuildLink(this, proficiency));

            foreach (uint? phase in trainingPhaseIds ?? new List<uint?>())
                _trainingPlanPhases.Add(TrainingPlanPhaseRelation.BuildLink(this, phase));

            TestBusinessRules();

            // Training Parameters
            TrainingVolume = TrainingVolumeParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets());
            TrainingDensity = TrainingDensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets());
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method for creating a Training Plan Draft
        /// </summary>
        /// <param name="ownerid">The ID of the owner</param>
        /// <returns>The TrainingPlanRoot instance</returns>
        public static TrainingPlanRoot NewDraft(uint ownerid)

            => CreateTrainingPlan(string.Empty, false, ownerid);


        /// <summary>
        /// Factory method - Transient
        /// </summary>
        /// <param name="trainingWeeks">The Training Weeks which the Training Plan is made up of</param>
        /// <param name="name">The name of the Training Plan</param>
        /// <param name="isBookmarked">The Training Plan has been flagged as Bookmarked</param>
        /// <param name="ownerId">The ID of the owner of the plan</param>
        /// <param name="personalNoteId">The ID of the note</param>
        /// <param name="relationsWithChildPlans">The list of the relations among the plan and its child ones (Variant, Inherited etc.)</param>
        /// <param name="hashtagIds">The list of the IDS of the Hashtags which the Training Plan has been tagged with</param>
        /// <param name="trainingPhaseIds">The list of the IDs of the Training Phases which the Training Plan has been tagged with</param>
        /// <param name="trainingPlanProficiencyIds">The list of the  IDs of the Training Proficiencies which the Training Plan has been tagged with</param>
        /// <param name="trainingMuscleFocusIds">The list of the  IDs of the Muscles which the Training Plan focuses on</param>
        /// <param name="trainingScheduleIds">The list of the IDs of the Training Schedules which the Training Plan has been scheduled to</param>
        /// <returns>The TrainingPlanRoot instance</returns>
        public static TrainingPlanRoot CreateTrainingPlan(
            string name,
            bool isBookmarked,
            uint? ownerId,
            uint? personalNoteId = null,
            IEnumerable<TrainingWeekEntity> trainingWeeks = null,
            IEnumerable<uint?> trainingScheduleIds = null,
            IEnumerable<uint?> trainingPhaseIds = null,
            IEnumerable<uint?> trainingPlanProficiencyIds = null,
            IEnumerable<uint?> trainingMuscleFocusIds = null,
            IEnumerable<uint?> hashtagIds = null,
            IEnumerable<TrainingPlanRelation> relationsWithChildPlans = null)

            => new TrainingPlanRoot(name, isBookmarked, ownerId, personalNoteId, trainingWeeks,
                trainingScheduleIds, trainingPhaseIds, trainingPlanProficiencyIds, trainingMuscleFocusIds, hashtagIds, relationsWithChildPlans);


        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The ID of the Training Plan</param>
        /// <param name="trainingWeeks">The Training Weeks which the Training Plan is made up of</param>
        /// <param name="name">The name of the Training Plan</param>
        /// <param name="isBookmarked">The Training Plan has been flagged as Bookmarked</param>
        /// <param name="ownerId">The ID of the owner of the plan</param>
        /// <param name="personalNoteId">The ID of the note</param>
        /// <param name="relationsWithChildPlans">The list of the relations among the plan and its child ones (Variant, Inherited etc.)</param
        /// <param name="hashtagIds">The list of the IDS of the Hashtags which the Training Plan has been tagged with</param>
        /// <param name="trainingPhaseIds">The list of the IDs of the Training Phases which the Training Plan has been tagged with</param>
        /// <param name="trainingPlanProficiencyIds">The list of the  IDs of the Training Proficiencies which the Training Plan has been tagged with</param>
        /// <param name="trainingMuscleFocusIds">The list of the  IDs of the Muscles which the Training Plan focuses on</param>
        /// <param name="trainingScheduleIds">The list of the IDs of the Training Schedules which the Training Plan has been scheduled to</param>
        /// <returns>The TrainingPlanRoot instance</returns>
        public static TrainingPlanRoot CreateTrainingPlan(
            uint? id,
            string name,
            bool isBookmarked,
            uint? ownerId,
            uint? personalNoteId = null,
            IEnumerable<TrainingWeekEntity> trainingWeeks = null,
            IEnumerable<uint?> trainingScheduleIds = null,
            IEnumerable<uint?> trainingPhaseIds = null,
            IEnumerable<uint?> trainingPlanProficiencyIds = null,
            IEnumerable<uint?> trainingMuscleFocusIds = null,
            IEnumerable<uint?> hashtagIds = null,
            IEnumerable<TrainingPlanRelation> relationsWithChildPlans = null)

            => new TrainingPlanRoot(id, name, isBookmarked, ownerId, personalNoteId, trainingWeeks,
                trainingScheduleIds, trainingPhaseIds, trainingPlanProficiencyIds, trainingMuscleFocusIds, hashtagIds, relationsWithChildPlans);


        ///// <summary>
        ///// Factory method for Inherited Training Plans
        ///// </summary>
        ///// <param name="id">The ID of the Training Plan</param>
        ///// <param name="rootPlan">The Training Plan to be sent</param>
        ///// <param name="destinationUserId">The ID of the receiving User</param>
        ///// <param name="trainingScheduleId">The ID of the Training Schedule for this Plan - Optional</param>
        ///// <returns></returns>
        //public static TrainingPlanRoot SendInheritedTrainingPlan(
        //    uint? id,
        //    TrainingPlanRoot rootPlan,
        //    uint? destinationUserId,
        //    uint? trainingScheduleId = null)

        //    => CreateTrainingPlan(
        //        id: id,
        //        name: rootPlan.Name,
        //        isBookmarked: false,
        //        isTemplate: false,
        //        ownerId: destinationUserId,
        //        trainingPlanType: TrainingPlanTypeEnum.Inherited,
        //        personalNoteId: null,
        //        trainingWeeks: rootPlan.TrainingWeeks.ToList(),
        //        trainingScheduleIds: new List<uint?>() { trainingScheduleId });


        ///// <summary>
        ///// Factory method for Variant Training Plans
        ///// </summary>
        ///// <param name="id">The ID of the Training Plan</param>
        ///// <param name="rootPlan">The Training Plan to create a Varaint of</param>
        ///// <returns></returns>
        //public static TrainingPlanRoot CreateVariantTrainingPlan(
        //    uint? id,
        //    TrainingPlanRoot rootPlan)

        //    => CreateTrainingPlan(
        //        id: id,
        //        name: rootPlan.Name,
        //        isBookmarked: false,
        //        isTemplate: false,
        //        ownerId: rootPlan.OwnerId,
        //        trainingPlanType: TrainingPlanTypeEnum.Variant,
        //        personalNoteId: null,
        //        trainingWeeks: rootPlan.TrainingWeeks.ToList(),
        //        trainingScheduleIds: null,
        //        trainingPhaseIds: rootPlan.TrainingPhaseIds.ToList(),
        //        trainingPlanProficiencyIds: rootPlan.TrainingProficiencyIds.ToList(),
        //        trainingMuscleFocusIds: rootPlan.MuscleFocusIds.ToList(),
        //        hashtags: rootPlan.HashtagIds.ToList(),
        //        relationsWithChildPlans: null);

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

        ///// <summary>
        ///// Mark the Training Plan as variant, if not already marked as a special type plan. Raises an exception otherwise
        ///// </summary>
        ///// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        ///// <exception cref="InvalidOperationException">If trying to change the type of a plan which has already been marked as a special type</exception>
        //public void MarkAsVariant()
        //{
        //    if (TrainingPlanType == TrainingPlanTypeEnum.Variant)
        //        return;

        //    if (TrainingPlanType != TrainingPlanTypeEnum.NotSet)
        //        throw new InvalidOperationException($"Trying to change the type of a Training Plan which has already a non-generic type.");

        //    TrainingPlanType = TrainingPlanTypeEnum.Variant;
        //    TestBusinessRules();
        //}

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
        public void WriteNote(uint? trainingPlanNoteId) => PersonalNoteId = trainingPlanNoteId;


        /// <summary>
        /// Remove the Training Plan Note ID
        /// </summary>
        public void CleanNote() => PersonalNoteId = default;


        /// <summary>
        /// Make the Training Plan Template by attaching the specified one
        /// </summary>
        /// <param name="childPlanId">The Training Plan ID to be added to the child ones</param>
        /// <param name="childPlanType">The type of the training plan in relation to the parent one</param>
        /// <param name="messageId">The ID of the message to be attached - optional, valid for Inherited Plans only</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void AttachChildPlan(uint? childPlanId, TrainingPlanTypeEnum childPlanType, uint? messageId = null)
        {
            if (childPlanId == null)
                throw new ArgumentNullException($"Child Plan ID must be non-NULL when attaching it to the Training Plan", nameof(childPlanId));


            if (FindRelationByChildIdOrDefault(childPlanId) != default)
                return;

            _relationsWithChildPlans.Add(
                TrainingPlanRelation.EnstablishRelation(Id, childPlanId, childPlanType, messageId));

            TestBusinessRules();
        }


        /// <summary>
        /// Removes the specified Training Plan from this one childs.
        /// If no child left, then mark the plan as 'Not Template'
        /// </summary>
        /// <param name="childPlanId">The Training Plan ID to be removed from the child ones</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="ArgumentException">If the ID not found</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void DetachChildPlan(uint? childPlanId)
        {
            if (childPlanId == null)
                throw new ArgumentNullException($"Child Plan ID must be non-NULL when detaching it from the Training Plan", nameof(childPlanId));

            TrainingPlanRelation relationEntry = FindRelationByChildIdOrDefault(childPlanId);

            if (relationEntry == default)
                throw new ArgumentException(nameof(childPlanId), $"Child Plan ID could not be found.");

            else
            {
                if (_relationsWithChildPlans.Remove(relationEntry))
                    TestBusinessRules();
            }
        }


        /// <summary>
        /// Schedule the Training Plan by assigning a Schedule ID
        /// </summary>
        /// <param name="scheduleId">The Schedule ID to be added</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void ScheduleTraining(uint? scheduleId)
        {
            if (scheduleId == null)
                throw new ArgumentNullException($"Schedule ID must be non-NULL when tagging the Training Plan", nameof(scheduleId));

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
        /// Get the Workout given the cohordinates
        /// </summary>
        /// <param name="weekPnum">The Progressive Number of the Training Week</param>
        /// <param name="workoutPnum">The Progressive Number of the Workout</param>
        /// <returns>The list of the Working Sets</returns>
        public WorkoutTemplateReferenceValue CloneWorkout(uint weekPnum, uint workoutPnum)

            => FindTrainingWeekByProgressiveNumber((int)weekPnum).CloneWorkout(workoutPnum);


        /// <summary>
        /// Get the WSs of all the Workouts beuinting to the Plan
        /// </summary>
        /// <returns>The list of the Working Sets</returns>
        public IEnumerable<WorkingSetTemplateEntity> CloneAllWorkingSets()

             => _trainingWeeks.SelectMany(x => x.CloneAllWorkingSets());

        #endregion


        #region Many-to-Many Relations

        /// <summary>
        /// Add the Hastag to the Training Plan
        /// </summary>
        /// <param name="hashtagId">The Hashtag ID to be added</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void TagAs(uint? hashtagId)
        {
            if (hashtagId == null)
                throw new ArgumentNullException($"Hashtag ID must be valid when tagging the Training Plan", nameof(hashtagId));

            if (IsTaggedAs(hashtagId))
                return;

            _trainingPlanHashtags.Add(TrainingPlanHashtagRelation.BuildLink(this, hashtagId));
            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Hastag from the Training Plan
        /// </summary>
        /// <param name="hashtagId">The Hashtag ID to be removed</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void Untag(uint? hashtagId)
        {
            if (hashtagId == null)
                throw new ArgumentNullException($"Hashtag ID must be valid when tagging the Training Plan", nameof(hashtagId));

            if (_trainingPlanHashtags.Remove(TrainingPlanHashtagRelation.BuildLink(this, hashtagId)))
                TestBusinessRules();
        }


        /// <summary>
        /// Link the Phase to the Training Plan
        /// </summary>
        /// <param name="phaseId">The Phase ID to be added</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void TagPhase(uint? phaseId)
        {
            if (phaseId == null)
                throw new ArgumentNullException(nameof(phaseId), $"Non-NULL Phase ID is required when tagging to it");

            if (HasTargetPhase(phaseId))
                return;

            _trainingPlanPhases.Add(TrainingPlanPhaseRelation.BuildLink(this, phaseId));
            TestBusinessRules();
        }


        /// <summary>
        /// Unlink the Phase from the Training Plan
        /// </summary>
        /// <param name="phaseId">The Hashtag ID to be removed</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void UntagPhase(uint? phaseId)
        {
            if (phaseId == null)
                throw new ArgumentNullException(nameof(phaseId), $"Non-NULL Phase ID is required when untagging it");

            if (_trainingPlanPhases.Remove(TrainingPlanPhaseRelation.BuildLink(this, phaseId)))
                TestBusinessRules();
        }


        /// <summary>
        /// Link the Proficiency to the Training Plan
        /// </summary>
        /// <param name="proficiencyId">The Proficiency ID to be added</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void LinkTargetProficiency(uint? proficiencyId)
        {
            if (proficiencyId == null)
                throw new ArgumentNullException(nameof(proficiencyId), $"Non-NULL Proficiency ID is required when tagging to it");

            if (HasTargetProficiency(proficiencyId))
                return;

            _trainingPlanProficiencies.Add(TrainingPlanProficiencyRelation.BuildLink(this, proficiencyId));
            TestBusinessRules();
        }


        /// <summary>
        /// Unlink the Proficiency from the Training Plan
        /// </summary>
        /// <param name="proficiencyId">The Proficiency ID to be removed</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void UnlinkTargetProficiency(uint? proficiencyId)
        {
            if (proficiencyId == null)
                throw new ArgumentNullException(nameof(proficiencyId), $"Non-NULL Proficiency ID is required when untagging it");

            if (_trainingPlanProficiencies.Remove(TrainingPlanProficiencyRelation.BuildLink(this, proficiencyId)))
                TestBusinessRules();
        }


        /// <summary>
        /// Give focus to the Muscle
        /// </summary>
        /// <param name="muscleId">The Muscle ID to be added</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void FocusOnMuscle(uint? muscleId)
        {
            if (muscleId == null)
                throw new ArgumentNullException(nameof(muscleId), $"Non-NULL Muscle ID is required when giving focus to it");

            if (DoesFocusOn(muscleId))
                return;

            _trainingPlanMuscleFocusIds.Add(TrainingPlanMuscleFocusRelation.BuildLink(this, muscleId));
            TestBusinessRules();
        }


        /// <summary>
        /// Remove the focus to the Muscle
        /// </summary>
        /// <param name="muscleId">The Muscle ID to be removed</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void UnfocusMuscle(uint? muscleId)
        {
            if (muscleId == null)
                throw new ArgumentNullException(nameof(muscleId), $"Non-NULL Muscle ID is required when removing focus to it");

            if (_trainingPlanMuscleFocusIds.Remove(TrainingPlanMuscleFocusRelation.BuildLink(this, muscleId)))
                TestBusinessRules();
        }


        /// <summary>
        /// Check whether the Plan is tagged with the specifed Hashtag
        /// </summary>
        /// <param name="hashtagId">The Id of the Hashtag to seek for</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <returns>True if the the plan is tagged with the Hashtag/returns>
        public bool IsTaggedAs(uint? hashtagId)

            => _trainingPlanHashtags.SingleOrDefault(x => x.HashtagId == hashtagId) != default;


        /// <summary>
        /// Check whether the Plan focuses on the specified muscle
        /// </summary>
        /// <param name="muscleId">The Id of the Muscle to seek for</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <returns>True if the the plan is tagged with the Muscle Focus/returns>
        public bool DoesFocusOn(uint? muscleId)

            => _trainingPlanMuscleFocusIds.SingleOrDefault(x => x.MuscleId == muscleId) != default;


        /// <summary>
        /// Check whether the Plan targets the specified Training Proficiency
        /// </summary>
        /// <param name="proficiencyId">The Id of the Proficiency to seek for</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <returns>True if the the plan is tagged with the Training Proficiency/returns>
        public bool HasTargetProficiency(uint? proficiencyId)

            => _trainingPlanProficiencies.SingleOrDefault(x => x.TrainingProficiencyId == proficiencyId) != default;


        /// <summary>
        /// Check whether the Plan targets the specifed Training Phase
        /// </summary>
        /// <param name="phaseId">The Id of the Phase to seek for</param>
        /// <exception cref="ArgumentException">If ID could not be found</exception>
        /// <returns>True if the the plan is tagged with the Training Phase/returns>
        public bool HasTargetPhase(uint? phaseId)

            => _trainingPlanPhases.SingleOrDefault(x => x.TrainingPhaseId == phaseId) != default;


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

                TrainingVolume = TrainingVolume.AddWorkingSets(trainingWeek.CloneAllWorkingSets());
                TrainingDensity = TrainingDensity.AddWorkingSets(trainingWeek.CloneAllWorkingSets());
                TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
            }
        }


        /// <summary>
        /// Add the Training Week to the Plan. This is not meant to work with Full Rest Weeks.
        /// </summary>
        /// <param name="workoutsReferences">The list of the WOs which the Training Plan is made up of</param>
        /// <param name="weekType">The type of the Training Week</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If a business rule is violated</exception>
        /// <exception cref="ArgumentException">If the week type is a Full Rest one</exception>
        public void PlanTransientTrainingWeek(TrainingWeekTypeEnum weekType, IList<WorkoutTemplateReferenceValue> workoutsReferences)
        {
            if (weekType == TrainingWeekTypeEnum.FullRest)
                throw new ArgumentException("Cannot add Full Rest Weeks with this function.", nameof(weekType));

            TrainingWeekEntity toAdd = TrainingWeekEntity.PlanTransientTrainingWeek(
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

            //_trainingWeeks.Add(
            //    TrainingWeekTemplate.PlanFullRestWeek(
            //        restWeek,
            //        BuildTrainingWeekProgressiveNumber()));

            _trainingWeeks.Add(restWeek.Clone() as TrainingWeekEntity);

            TestBusinessRules();

            // Training Parameters don't change
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
            TrainingWeekEntity src = FindTrainingWeekByProgressiveNumber((int)srcPnum);
            TrainingWeekEntity dest = FindTrainingWeekByProgressiveNumber((int)destPnum);

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
        public void PlanWorkout(uint weekPnum, IEnumerable<WorkingSetTemplateEntity> workingSets)
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
            TrainingWeekEntity week = FindTrainingWeekByProgressiveNumber((int)weekPnum);
            IEnumerable<WorkingSetTemplateEntity> removedWorkingSets = week.CloneWorkoutWorkingSets(workoutPnum);

            week.UnplanWorkout(workoutPnum);

            TestBusinessRules();

            TrainingVolume = TrainingVolume.RemoveWorkingSets(removedWorkingSets);
            TrainingDensity = TrainingDensity.RemoveWorkingSets(removedWorkingSets);
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
        }


        /// <summary>
        /// Modify the Workout by adding the selected Working Sets
        /// </summary>
        /// <param name="workingSets">The list of the WSs to add to the WO</param>
        /// <param name="workoutPnum">The Progressive Number of the Workout to be modified</param>
        /// <param name="trainingWeekPnum">The Progressive Number of the Training Week whicch to add the WS to</param>
        /// <exception cref="ArgumentOutOfRangeException">If Workout not found</exception>
        public void AddWorkingSets(uint trainingWeekPnum, uint workoutPnum, IEnumerable<WorkingSetTemplateEntity> workingSets)
        {
            FindTrainingWeekByProgressiveNumber((int)trainingWeekPnum).AddWorkingSets(workoutPnum, workingSets);

            TrainingVolume = TrainingVolume.AddWorkingSets(workingSets);
            TrainingDensity = TrainingDensity.AddWorkingSets(workingSets);
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
        }


        /// <summary>
        /// Modify the Workout by removing the selected Working Sets
        /// </summary>
        /// <param name="workingSets">The list of the WSs to remove from the WO</param>
        /// <param name="workoutPnum">The Progressive Number of the Workout to be modified</param>
        /// <param name="trainingWeekPnum">The Progressive Number of the Training Week whicch to remove the WS from</param>
        /// <exception cref="ArgumentOutOfRangeException">If Workout not found</exception>
        /// <exception cref="InvalidOperationException">If trying to remove transient Working Sets</exception>
        public void RemoveWorkingSets(uint trainingWeekPnum, uint workoutPnum, IEnumerable<WorkingSetTemplateEntity> workingSets)
        {
            FindTrainingWeekByProgressiveNumber((int)trainingWeekPnum).RemoveWorkingSets(workoutPnum, workingSets);

            TrainingVolume = TrainingVolume.AddWorkingSets(workingSets);
            TrainingDensity = TrainingDensity.AddWorkingSets(workingSets);
            TrainingIntensity = TrainingIntensityParametersValue.ComputeFromWorkingSets(CloneAllWorkingSets(), GetMainEffortType());
        }


        /// <summary>
        /// Assign a new progressive number to the WO of the Training Week
        /// </summary>
        /// <param name="destPnum">The new Progressive Number - PNums must be consecutive</param>
        /// <param name="srcPnum">The Progressive Number of the WO to be moved</param>
        public void MoveWorkoutToNewProgressiveNumber(uint weekPnum, uint srcPnum, uint destPnum)
        {
            TrainingWeekEntity week = FindTrainingWeekByProgressiveNumber((int)weekPnum);
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
        /// Find the Relation entry according to the child Training Plan ID provided
        /// </summary>
        /// <param name="childPlanId">The Id to be found</param>
        /// <returns>The TrainingPlanRelation object/returns>
        private TrainingPlanRelation FindRelationByChildIdOrDefault(uint? childPlanId)
        {
            if (childPlanId == null)
                throw new ArgumentNullException(nameof(childPlanId), $"Cannot find a Child Plan with NULL id");

            TrainingPlanRelation child = _relationsWithChildPlans.SingleOrDefault(x => x.ChildPlanId == childPlanId);

            //if (child == default)
            //    throw new ArgumentException($"Child Plan with Id {childPlanId.ToString()} could not be found", nameof(childPlanId));

            return child;
        }


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
        /// The Training Plan must have no NULL Training Weeks.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullWeeks() => _trainingWeeks.All(x => x != null);


        /// <summary>
        /// The Owner of the Training Plan cannot be NULL.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool OwnerIsNotNull() => OwnerId != default;


        /// <summary>
        /// The Training Plan must have no NULL Training Schedules.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullSchedules() => _trainingScheduleIds.All(x => x != default);


        /// <summary>
        /// The Training Plan must have no NULL Training Phases.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullPhases() => _trainingPlanPhases.All(x => x?.TrainingPhaseId != null);


        /// <summary>
        /// The Training Plan must have no NULL Training Hashtags.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullHashtags() => _trainingPlanHashtags.All(x => x?.HashtagId != null);

        /// <summary>
        /// The Training Plan must have no NULL Training Proficiency.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullProficiencies() => _trainingPlanProficiencies.All(x => x?.TrainingProficiencyId != null);


        /// <summary>
        /// The Training Plan must have no NULL Muscle Focus entries.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool NoNullMuscleFocus() => _trainingPlanMuscleFocusIds.All(x => x?.MuscleId != null);


        /// <summary>
        /// The Training Plan must be the parent of the Relation.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool TrainingPlanIsParentOnly()

            => _relationsWithChildPlans.All(x => x?.ParentPlanId == Id);
        //=> _relationsWithChildPlans.All(x => x?.ChildPlanId != Id) && _relationsWithChildPlans.All(x => x?.ParentPlanId == Id);


        /// <summary>
        /// No duplicate relations between the Parent Plan and the same Child.
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool OnlyOneRelationPerEachChild() => !_relationsWithChildPlans.ContainsDuplicates();


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
            if (!NoNullWeeks())
                throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Training Weeks.");

            if (!OwnerIsNotNull())
                throw new TrainingDomainInvariantViolationException($"The Owner of the Training Plan cannot be NULL.");

            if (!NoNullSchedules())
                throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Training Schedules.");

            if (!NoNullPhases())
                throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Training Phases.");

            if (!NoNullHashtags())
                throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Training Hashtags.");

            if (!NoNullProficiencies())
                throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Training Proficiency.");

            if (!NoNullMuscleFocus())
                throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Muscle Focus entries.");

            if (!TrainingPlanIsParentOnly())
                throw new TrainingDomainInvariantViolationException($"The Training Plan must be the parent of the Relation.");

            if (!OnlyOneRelationPerEachChild())
                throw new TrainingDomainInvariantViolationException($"No duplicate relations between the Parent Plan and the same Child.");

            //if (!NameIsNotEmpty())
            //    throw new TrainingDomainInvariantViolationException($"The Training Plan must have a valid name.");

            if (!TrainingWeeksWithConsecutiveProgressiveNumber())
                throw new TrainingDomainInvariantViolationException($"Training Weeks of the same Training Plan must have consecutive progressive numbers.");
        }

        #endregion


        #region IClonable Interface

        public object Clone()

            => CreateTrainingPlan(
                    Id, Name, IsBookmarked, OwnerId, PersonalNoteId, TrainingWeeks, TrainingScheduleIds,
                        TrainingPhaseIds, TrainingProficiencyIds, MuscleFocusIds, HashtagIds, RelationsWithChildPlans);

        #endregion
    }
}
