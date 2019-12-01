using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;


namespace GymProject.Domain.TrainingDomain.AthleteAggregate
{
    public class UserTrainingPlanEntity : Entity<uint?>, ICloneable
    {




        /// <summary>
        /// The name of the Training Plan
        /// </summary>
        public string Name { get; private set; } = string.Empty;



        /// <summary>
        /// The Training Plan has been set as 'Bookmarked'
        /// </summary>
        public bool IsBookmarked { get; private set; } = false;


        /// <summary>
        /// FK to the Training Plan note - Optional
        /// </summary>
        public uint? TrainingPlanNoteId { get; private set; } = null;
        

        /// <summary>
        /// FK to the Training Plan which this one is a variant of
        /// </summary>
        public uint? ParentPlanId { get; private set; } = null;


        private List<uint?> _trainingScheduleIds = new List<uint?>();

        /// <summary>
        /// FK to the Training Schedules
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<uint?> TrainingScheduleIds
        {
            get => _trainingScheduleIds?.ToList().AsReadOnly()
                ?? new List<uint?>().AsReadOnly();
        }



        private List<TrainingPlanPhaseRelation> _trainingPlanPhases = new List<TrainingPlanPhaseRelation>();

        /// <summary>
        /// FK to the Training Plan target Phases
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<uint?> TrainingPhaseIds
        {
            get => _trainingPlanPhases?.Select(x => x.TrainingPhaseId).ToList().AsReadOnly()
                ?? new List<uint?>().AsReadOnly();
        }


        private List<TrainingPlanProficiencyRelation> _trainingPlanProficiencies = new List<TrainingPlanProficiencyRelation>();

        /// <summary>
        /// FK to the Training target Proficiencies
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<uint?> TrainingProficiencyIds
        {
            get => _trainingPlanProficiencies?.Select(x => x.TrainingProficiencyId).ToList().AsReadOnly()
                ?? new List<uint?>().AsReadOnly();
        }


        private List<TrainingPlanMuscleFocusRelation> _trainingPlanMuscleFocusIds = new List<TrainingPlanMuscleFocusRelation>();

        /// <summary>
        /// FK to the Training Muscle focus
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<uint?> MuscleFocusIds
        {
            get => _trainingPlanMuscleFocusIds?.Select(x => x.MuscleGroupId).ToList().AsReadOnly()
                ?? new List<uint?>().AsReadOnly();
        }


        private List<TrainingPlanHashtagRelation> _trainingPlanHashtags = new List<TrainingPlanHashtagRelation>();

        /// <summary>
        /// FK to the Training Hashtags
        /// </summary>
        public IReadOnlyCollection<uint?> HashtagIds
        {
            get => _trainingPlanHashtags?.Select(x => x.HashtagId).ToList().AsReadOnly()
                ?? new List<uint?>().AsReadOnly();
        }


        /// <summary>
        /// FK to the Training Plan
        /// </summary>
        public uint TrainingPlanId { get; private set; }





        #region Ctors


        private UserTrainingPlanEntity() : base(null) { }


        private UserTrainingPlanEntity(
            uint? id,
            uint trainingPlanId,
            string name,
            bool isBookmarked,
            uint? parentPlanId = null,
            uint? personalNoteId = null,
            IEnumerable<uint?> trainingScheduleIds = null,
            IEnumerable<uint?> trainingPhaseIds = null,
            IEnumerable<uint?> trainingPlanProficiencyIds = null,
            IEnumerable<uint?> trainingMuscleFocusIds = null,
            IEnumerable<uint?> hashtags = null) : base(id)
        {
            TrainingPlanId = trainingPlanId;
            Name = name ?? string.Empty;
            IsBookmarked = isBookmarked;
            TrainingPlanNoteId = personalNoteId;
            ParentPlanId = parentPlanId;

            _trainingScheduleIds = trainingScheduleIds?.ToList() ?? new List<uint?>();

            _trainingPlanHashtags = new List<TrainingPlanHashtagRelation>();
            _trainingPlanMuscleFocusIds = new List<TrainingPlanMuscleFocusRelation>();
            _trainingPlanProficiencies = new List<TrainingPlanProficiencyRelation>();
            _trainingPlanPhases = new List<TrainingPlanPhaseRelation>();

            // Build  many-to-many relations
            uint hashtagCounter = 0;

            foreach (uint? hashtag in hashtags ?? new List<uint?>())
                _trainingPlanHashtags.Add(TrainingPlanHashtagRelation.BuildLink(this, hashtagCounter++, hashtag));

            foreach (uint? muscle in trainingMuscleFocusIds ?? new List<uint?>())
                _trainingPlanMuscleFocusIds.Add(TrainingPlanMuscleFocusRelation.BuildLink(this, muscle));

            foreach (uint? proficiency in trainingPlanProficiencyIds ?? new List<uint?>())
                _trainingPlanProficiencies.Add(TrainingPlanProficiencyRelation.BuildLink(this, proficiency));

            foreach (uint? phase in trainingPhaseIds ?? new List<uint?>())
                _trainingPlanPhases.Add(TrainingPlanPhaseRelation.BuildLink(this, phase));

            TestBusinessRules();
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method for creating a Training Plan Draft
        /// </summary>
        /// <param name="trainingPlanId">The Triaining Plan ID</param>
        /// <returns>The TrainingPlanRoot instance</returns>
        public static UserTrainingPlanEntity NewDraft(uint trainingPlanId)

            => InclueTrainingPlanInUserLibrary(trainingPlanId, string.Empty);


        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="trainingPlanId">The Triaining Plan ID</param>
        /// <param name="name">The name of the Training Plan</param>
        /// <param name="isBookmarked">The Training Plan has been flagged as Bookmarked</param>
        /// <param name="personalNoteId">The ID of the note</param>
        /// <param name="hashtagIds">The list of the IDS of the Hashtags which the Training Plan has been tagged with</param>
        /// <param name="trainingPhaseIds">The list of the IDs of the Training Phases which the Training Plan has been tagged with</param>
        /// <param name="trainingPlanProficiencyIds">The list of the  IDs of the Training Proficiencies which the Training Plan has been tagged with</param>
        /// <param name="trainingMuscleFocusIds">The list of the  IDs of the Muscles which the Training Plan focuses on</param>
        /// <param name="trainingScheduleIds">The list of the IDs of the Training Schedules which the Training Plan has been scheduled to</param>
        /// <returns>The TrainingPlanRoot instance</returns>
        public static UserTrainingPlanEntity InclueTrainingPlanInUserLibrary(
            uint trainingPlanId, 
            string name = null,
            bool isBookmarked = false,
            uint? parentPlanId = null,
            uint? personalNoteId = null,
            IEnumerable<uint?> trainingScheduleIds = null,
            IEnumerable<uint?> trainingPhaseIds = null,
            IEnumerable<uint?> trainingPlanProficiencyIds = null,
            IEnumerable<uint?> trainingMuscleFocusIds = null,
            IEnumerable<uint?> hashtagIds = null)

            => new UserTrainingPlanEntity(null, trainingPlanId, name, isBookmarked, parentPlanId, personalNoteId,
                trainingScheduleIds, trainingPhaseIds, trainingPlanProficiencyIds, trainingMuscleFocusIds, hashtagIds);


        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The ID</param>
        /// <param name="trainingPlanId">The Triaining Plan ID</param>
        /// <param name="name">The name of the Training Plan</param>
        /// <param name="isBookmarked">The Training Plan has been flagged as Bookmarked</param>
        /// <param name="personalNoteId">The ID of the note</param>
        /// <param name="hashtagIds">The list of the IDS of the Hashtags which the Training Plan has been tagged with</param>
        /// <param name="trainingPhaseIds">The list of the IDs of the Training Phases which the Training Plan has been tagged with</param>
        /// <param name="trainingPlanProficiencyIds">The list of the  IDs of the Training Proficiencies which the Training Plan has been tagged with</param>
        /// <param name="trainingMuscleFocusIds">The list of the  IDs of the Muscles which the Training Plan focuses on</param>
        /// <param name="trainingScheduleIds">The list of the IDs of the Training Schedules which the Training Plan has been scheduled to</param>
        /// <returns>The TrainingPlanRoot instance</returns>
        public static UserTrainingPlanEntity InclueTrainingPlanInUserLibrary(
            uint? id,
            uint trainingPlanId, 
            string name = null,
            bool isBookmarked = false,
            uint? parentPlanId = null,
            uint? personalNoteId = null,
            IEnumerable<uint?> trainingScheduleIds = null,
            IEnumerable<uint?> trainingPhaseIds = null,
            IEnumerable<uint?> trainingPlanProficiencyIds = null,
            IEnumerable<uint?> trainingMuscleFocusIds = null,
            IEnumerable<uint?> hashtagIds = null)

            => new UserTrainingPlanEntity(id, trainingPlanId, name, isBookmarked, parentPlanId, personalNoteId,
                trainingScheduleIds, trainingPhaseIds, trainingPlanProficiencyIds, trainingMuscleFocusIds, hashtagIds);

        #endregion



        #region Public Methods

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
        /// Check whether it is a variant plan
        /// </summary>
        public bool IsVariant() => ParentPlanId != null;


        /// <summary>
        /// Assign the IsBookmarked flag
        /// </summary>
        /// <param name="bookmarkedFlag">The flag</param>
        public void ChangeBookmarkedFlag(bool bookmarkedFlag) => IsBookmarked = bookmarkedFlag;


        /// <summary>
        /// Assign the Training Plan Note ID
        /// </summary>
        /// <param name="trainingPlanNoteId">The note ID</param>
        public void AttachNote(uint? trainingPlanNoteId) => TrainingPlanNoteId = trainingPlanNoteId;


        /// <summary>
        /// Remove the Training Plan Note ID
        /// </summary>
        public void CleanNote() => TrainingPlanNoteId = null;


        /// <summary>
        /// Make the Training Plan a variant of the speicified one
        /// </summary>
        public void MakeItVariantOf(uint parentPlanId)
        {
            ParentPlanId = parentPlanId;
            TestBusinessRules();
        }


        /// <summary>
        /// Make the Training Plan not a Variant anymore
        /// </summary>
        public void RemoveVariantOf() => ParentPlanId = null;



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
        /// Schedule the Training Plan by assigning a Schedule ID
        /// No Exception is raised if the Schedule ID is not present
        /// </summary>
        /// <param name="scheduleId">The Schedule ID to be added</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void UnscheduleTraining(uint? scheduleId)
        {
            if (scheduleId == null)
                throw new ArgumentNullException($"Schedule ID must be non-NULL when tagging the Training Plan", nameof(scheduleId));

            if (_trainingScheduleIds.Remove(scheduleId))
                TestBusinessRules();
        }

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

            _trainingPlanHashtags.Add(TrainingPlanHashtagRelation.BuildLink(this, (uint)_trainingPlanHashtags.Count, hashtagId));
            TestBusinessRules();
        }


        /// <summary>
        /// Remove the Hastag from the Training Plan
        /// </summary>
        /// <param name="hashtagId">The Hashtag ID to be removed - non NULL</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void Untag(uint? hashtagId)
        {
            if (hashtagId == null)
                throw new ArgumentNullException($"Hashtag ID must be valid when tagging the Training Plan", nameof(hashtagId));

            if (_trainingPlanHashtags.Remove(TrainingPlanHashtagRelation.BuildLink(this, 0, hashtagId)))
            {
                //ForceConsecutiveHashtagProgressiveNumbers();
                TestBusinessRules();
            }
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

            => _trainingPlanMuscleFocusIds.SingleOrDefault(x => x.MuscleGroupId == muscleId) != default;


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



        #region Private Methods

        /// <summary>
        /// Force the WOs to have consecutive progressive numbers
        /// It works by assuming that the WSs are added in a sorted fashion.
        /// </summary>
        private void ForceConsecutiveHashtagProgressiveNumbers()
        {
            _trainingPlanHashtags = _trainingPlanHashtags.OrderBy(x => x.ProgressiveNumber).ToList();

            // Just overwrite all the progressive numbers
            for (int irel = 0; irel < _trainingPlanHashtags.Count(); irel++)
            {
                TrainingPlanHashtagRelation rel = _trainingPlanHashtags.ElementAt(irel);
                rel.ProgressiveNumber = (uint)irel;
            }
        }

        #endregion


        #region Business Rules Validation

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
        private bool NoNullMuscleFocus() => _trainingPlanMuscleFocusIds.All(x => x?.MuscleGroupId != null);


        /// <summary>
        /// The Training Plan cannot have itself as the Parent Plan
        /// </summary>
        /// <returns>True if business rule is met</returns>
        private bool ParentPlanDifferentFromThisOne()

            => ParentPlanId == null || ParentPlanId != TrainingPlanId;


        /// <summary>
        /// Tests that all the business rules are met and manages invalid states
        /// </summary>
        /// <exception cref="TrainingDomainInvariantViolationException">Thrown if business rules violation</exception>
        private void TestBusinessRules()
        {

            //if (!NoNullSchedules())
            //    throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Training Schedules.");

            //if (!NoNullPhases())
            //    throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Training Phases.");

            //if (!NoNullHashtags())
            //    throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Training Hashtags.");

            //if (!NoNullProficiencies())
            //    throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Training Proficiency.");

            //if (!NoNullMuscleFocus())
            //    throw new TrainingDomainInvariantViolationException($"The Training Plan must have no NULL Muscle Focus entries.");
            
            if (!ParentPlanDifferentFromThisOne())
                throw new TrainingDomainInvariantViolationException($"The Training Plan cannot have itself as the Parent Plan.");
            
            //if (!NameIsNotEmpty())
            //    throw new TrainingDomainInvariantViolationException($"The Training Plan must have a valid name.");

        }


        #endregion



        #region IClonable Interface

        public object Clone()

            => InclueTrainingPlanInUserLibrary(
                    Id, TrainingPlanId, Name, IsBookmarked, ParentPlanId, TrainingPlanNoteId, TrainingScheduleIds, TrainingPhaseIds, TrainingProficiencyIds, MuscleFocusIds, HashtagIds);

        #endregion
    }
}
