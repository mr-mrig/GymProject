using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.AthleteAggregate
{
    public class AthleteRoot : Entity<uint?>, IAggregateRoot, ICloneable
    {



        private List<UserTrainingProficiencyRelation> _trainingProficiencies = new List<UserTrainingProficiencyRelation>();

        /// <summary>
        /// The Training Proficinecy levels the user has achieved
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<UserTrainingProficiencyRelation> TrainingProficiencies
        {
            get => _trainingProficiencies?.AsReadOnly()
                ?? new List<UserTrainingProficiencyRelation>().AsReadOnly();
        }
        


        private List<UserTrainingPhaseRelation> _trainingPhases = new List<UserTrainingPhaseRelation>();

        /// <summary>
        /// The Training Phases the user has been performing
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<UserTrainingPhaseRelation> TrainingPhases
        {
            get => _trainingPhases?.AsReadOnly()
                ?? new List<UserTrainingPhaseRelation>().AsReadOnly();
        }
                


        private List<UserTrainingPlanEntity> _trainingPlans = new List<UserTrainingPlanEntity>();

        /// <summary>
        /// FK to the trainning plans belonging tu the user library
        /// </summary>
        public IReadOnlyCollection<UserTrainingPlanEntity> TrainingPlans
        {
            get => _trainingPlans?.AsReadOnly() ?? new List<UserTrainingPlanEntity>().AsReadOnly();
        }


        /// <summary>
        /// The ID of the Training Plan the user is currently on
        /// </summary>
        public uint? CurrentTrainingPlanId { get; private set; }


        /// <summary>
        /// The ID of the Training Week the user is currently on
        /// </summary>
        public uint? CurrentTrainingWeekId { get; private set; }



        #region Ctors

        private AthleteRoot() : base(null)
        {
            _trainingPhases =  new List<UserTrainingPhaseRelation>();
            _trainingProficiencies =  new List<UserTrainingProficiencyRelation>();
            _trainingPlans = new List<UserTrainingPlanEntity>();
        }


        private AthleteRoot(uint? id, IEnumerable<UserTrainingPhaseRelation> trainingPhasesHistory = null, IEnumerable<UserTrainingProficiencyRelation>proficienciesHistory = null,
            IEnumerable<UserTrainingPlanEntity> trainingPlans = null, uint? currentPlanId = null) : base(id)
        {
            _trainingPhases = trainingPhasesHistory?.Clone().ToList() ?? new List<UserTrainingPhaseRelation>();
            _trainingProficiencies = proficienciesHistory?.Clone().ToList() ?? new List<UserTrainingProficiencyRelation>();
            _trainingPlans = trainingPlans?.ToList() ?? new List<UserTrainingPlanEntity>();
            CurrentTrainingPlanId = currentPlanId;

            TestBusinessRules();
        }


        #endregion



        #region Factories

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <returns>The new instance</returns>
        public static AthleteRoot RegisterAthlete()

            => new AthleteRoot();

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <param name="userId">The ID of the Athlete</param>
        /// <returns>The new instance</returns>
        public static AthleteRoot RegisterAthlete(uint userId)

            => new AthleteRoot(userId);


        /// <summary>
        /// Factory Method, should be used for testing purposes only.
        /// Deprecated: the entity should be either loaded from the DB or the parameterless ctor should be used
        /// </summary>
        /// <param name="userId">The ID of the Athlete</param>
        /// <param name="trainingPhasesHistory">The history of the Training Phases the athlete has performed</param>
        /// <param name="proficienciesHistory">The history of the Proficiencies the athlete has achieved</param>
        /// <param name="trainingPlansRelations">The training plans in the user library - remember to set the Current Plan Id as weell to enssure consistency</param>
        /// <param name="currentPlanId">The ID of the training plan which is currently on schedule</param>
        /// <returns>The new instance</returns>
        public static AthleteRoot RegisterAthlete(uint? userId, IEnumerable<UserTrainingPhaseRelation> trainingPhasesHistory = null, IEnumerable<UserTrainingProficiencyRelation> proficienciesHistory = null,
            IEnumerable<UserTrainingPlanEntity> trainingPlansRelations = null, uint? currentPlanId = null)

            => new AthleteRoot(userId, trainingPhasesHistory, proficienciesHistory, trainingPlansRelations, currentPlanId);

        #endregion



        #region Getters


        /// <summary>
        /// Make a value copy of the Training Phase the user currently is in.
        /// </summary>
        /// <returns>A value copy of the UserTrainingPhaseRelation object or null if nothing found</returns>
        /// <exception cref="InvalidOperationException">If more than one element found</exception>
        public UserTrainingPhaseRelation CloneCurrentTrainingPhase()

            => CloneTrainingPhaseAt(DateTime.UtcNow);


        /// <summary>
        /// Make a value copy of the Training Phase the user was in at a specific date
        /// </summary>
        /// <param name="date">The check date</param>
        /// <returns>A value copy of the UserTrainingPhaseRelation object or null if nothing found</returns>
        /// <exception cref="InvalidOperationException">If more than one element found</exception>
        public UserTrainingPhaseRelation CloneTrainingPhaseAt(DateTime date)

            //=> _trainingPhases.SingleOrDefault(x => x.Period.Includes(date))?.Clone() as UserTrainingPhaseRelation;
            => _trainingPhases.SingleOrDefault(x => x.StartDate <= date 
                && (!x.EndDate.HasValue || x.EndDate >= date))
                ?.Clone() as UserTrainingPhaseRelation;



        /// <summary>
        /// Make a value copy of the current Training Proficiency level the user reached
        /// </summary>
        /// <returns>A value copy of the UserTrainingPhaseRelation object or null if nothing found</returns>
        /// <exception cref="InvalidOperationException">If more than one element found</exception>
        public UserTrainingProficiencyRelation CloneCurrentTrainingProficiency()

            => CloneTrainingProficiencyAt(DateTime.UtcNow);


        /// <summary>
        /// Make a value copy of the Training Proficiency level reached at a specific date
        /// </summary>
        /// <param name="date">The check date</param>
        /// <returns>A value copy of the UserTrainingPhaseRelation object or null if nothing found</returns>
        /// <exception cref="InvalidOperationException">If more than one element found</exception>
        public UserTrainingProficiencyRelation CloneTrainingProficiencyAt(DateTime date)

            //=> _trainingProficiencies.SingleOrDefault(x => x.Period.Includes(date))?.Clone() as UserTrainingProficiencyRelation;
            => _trainingProficiencies.SingleOrDefault(x => x.StartDate <= date
                && (!x.EndDate.HasValue || x.EndDate >= date))
                ?.Clone() as UserTrainingProficiencyRelation;


        /// <summary>
        /// Get the value copy of the TrainingPlan library entry with the specified ID. It returns NULL if no entry was found.
        /// </summary>
        /// <param name="trainingPlanId">The TrianingPlan Id</param>
        /// <returns>The copy of the UserTrainingPlanEntity or NULL if not found</returns>
        /// <exception cref="InvalidOperationException">If more than one element found</exception>
        public UserTrainingPlanEntity CloneTrainingPlanOrDefault(uint trainingPlanId)

            => _trainingPlans.SingleOrDefault(x => x.TrainingPlanId == trainingPlanId)?.Clone() as UserTrainingPlanEntity;


        #endregion


        #region Phases Methods

        /// <summary>
        /// Assign the Training Phase to the user planning it according to the period specified:
        /// - period was not provided or doesn't have a start date -> plan the phase starting form today and leave if not right bounded
        /// - period placed in the past -> exception
        /// - otherwise -> plan the phase over the period specified (which can be either right bounded or not)
        /// Starting a phase automatically closes all the ones still left open.
        /// </summary>
        /// <param name="phaseId">The Phase ID to schedule</param>
        /// <param name="entryVisibility">The visibilty</param>
        /// <param name="startDate">The start date the Phase is scheduled from - if not null it must not be placed in the past</param>
        /// <param name="endDate">The end date the Phase is scheduled to</param>
        /// <param name="ownerNote">The owner note - optional</param>
        /// <returns>A value copy of the UserTrainingPhaseRelation object or null if no phthing found</returns>
        /// <exception cref="TrainingDomainInvariantViolationException">If more open phases found</exception>
        /// <exception cref="InvalidOperationException">If try to start a phase over a past period</exception>
        public void StartTrainingPhase(uint phaseId, EntryStatusTypeEnum entryVisibility,  DateTime? startDate = null, DateTime? endDate = null, PersonalNoteValue ownerNote = null)
        {
            UserTrainingPhaseRelation newPhase;

            // If valid period then plan it, otherwise start it from today
            if (startDate != null)
            {
                // Removing this coinstraint would require the Domain to manage potentially overlapping phases
                if (startDate.Value < DateTime.UtcNow)
                    throw new InvalidOperationException($"Cannot start a Training Phase over an elapsed period: {startDate.Value.ToShortTimeString()} - {endDate.Value.ToShortTimeString()} ");

                newPhase = UserTrainingPhaseRelation.PlanPhase(phaseId, startDate.Value, endDate, entryVisibility, ownerNote);
            }
            else
                newPhase = UserTrainingPhaseRelation.StartPhase(phaseId, DateTime.UtcNow, entryVisibility, ownerNote);

            // Close the previous phase, if any
            CloseOpenPhases(startDate ?? DateTime.UtcNow);

            // Start the new one
            _trainingPhases.Add(newPhase);

            TestBusinessRules();
        }


        /// <summary>
        /// Close all the phases still not completed
        /// </summary>
        public void CloseCurrentPhase()

            => CloseOpenPhases(DateTime.UtcNow);


        /// <summary>
        /// Change the start date of the Training Phase currently starting from
        /// </summary>
        /// <param name="newStartDate">The new start date</param>
        /// <param name="oldStartdate">The date which the phase currently stats from</param>
        /// <exception cref="ValueObjectInvariantViolationException">If chronological order violated</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If business rule violated</exception>
        /// <exception cref="InvalidOperationException">If could not find any Training Phase accordint o the start date specified</exception>
        public void ShiftTrainingPhaseStartDate(DateTime oldStartdate, DateTime newStartDate)
        {
            UserTrainingPhaseRelation userPhase = FindPhaseStartingFrom(oldStartdate);
            userPhase.ShiftStartDate(newStartDate);
            TestBusinessRules();
        }


        /// <summary>
        /// Attach the note of the owner
        /// </summary>
        /// <param name="newNote">The Owner's note</param>
        //public void WriteNote(string newNote) => OwnerNote = PersonalNoteValue.Write(newNote);


        /// <summary>
        /// Change the status of this entry
        /// </summary>
        /// <param name="newStartDate">The new start date</param>
        //public void ChangeStatus(EntryStatusTypeEnum status)
        //{

        //}
        #endregion


        #region Proficiencies Methods

        /// <summary>
        /// Assign a new Training Proficiency level to the user starting from the current date.
        /// The previous level is automatically closed.
        /// </summary>
        /// <param name="proficiencyId">The Phase ID to schedule</param>
        /// <returns>A value copy of the UserTrainingPhaseRelation object or null if no phthing found</returns>
        /// <exception cref="TrainingDomainInvariantViolationException">If more open phases found</exception>
        /// <exception cref="InvalidOperationException">If try to start a phase over a past period</exception>
        public void AchieveTrainingProficiency(uint proficiencyId)
        {
            UserTrainingProficiencyRelation proficiency = UserTrainingProficiencyRelation.AchieveTrainingProficiency(proficiencyId, DateTime.UtcNow);

            // Close the previous proficiency level, if any
            CloseTrainingProficiencyLevel();

            // Start the new one
            _trainingProficiencies.Add(proficiency);

            TestBusinessRules();
        }


        /// <summary>
        /// Close the Proficiency as a new one is started.
        /// The previous Proficiency level finishes the day before the current one
        /// </summary>
        //public void Close()
        //{ }


        /// <summary>
        /// Change the start date
        /// </summary>
        /// <param name="newStartDate">The new start date</param>
        //public void ShiftStartDate(DateTime newStartDate)
        //{

        //}

        #endregion


        #region Training Plans Methods

        ///// <summary>
        ///// Pin the Training Plan as the one the user is currently performing
        ///// </summary>
        ///// <param name="trainingPlanId">The ID of the UserTrainingPlan entity, which must be in the User Training Plans library</param>
        ///// <exception cref="TrainingDomainInvariantViolationException">If any business rule violated</exception>
        //public void StartTrainingPlan(uint trainingPlanId)
        //{
        //    CurrentTrainingPlanId = trainingPlanId;
        //    TestBusinessRules();
        //}

        
        ///// <summary>
        ///// Unpin the current Training Plan
        ///// </summary>
        ///// <exception cref="TrainingDomainInvariantViolationException">If any business rule violated</exception>
        //public void AbortTrainingPlan()
        //{
        //    CurrentTrainingPlanId = null;
        //    TestBusinessRules();
        //}


        /// <summary>
        /// Add the training plan with the specified ID to the User library.
        /// If the ID is already present, then do nothing.
        /// </summary>
        /// <param name="trainingPlanId">The ID of the TrainingPlanRoot entity</param>
        /// <exception cref="InvalidOperationException">If more than one element with the same ID</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule violated</exception>
        public void AddTrainingPlanToLibrary(uint trainingPlanId)

            => AddTrainingPlanToLibrary(UserTrainingPlanEntity.NewDraft(trainingPlanId));


        /// <summary>
        /// Add the training plan with the specified ID to the User library.
        /// If the ID is already present, then do nothing.
        /// </summary>
        /// <param name="trainingPlanId">The ID of the TrainingPlanRoot entity</param>
        /// <exception cref="InvalidOperationException">If more than one element with the same ID</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule violated</exception>
        public void AddTrainingPlanToLibrary(UserTrainingPlanEntity userPlanRelation)
        {
            if (CloneTrainingPlanOrDefault(userPlanRelation.TrainingPlanId) != default)
                return;

            _trainingPlans.Add(userPlanRelation);

            TestBusinessRules();
        }


        /// <summary>
        /// Remove the training plan with the specified ID from the User library - if any.
        /// If the plan was the current one also, then reset it
        /// </summary>
        /// <param name="trainingPlanId">The ID of the TrainingPlanRoot entity</param>
        /// <exception cref="InvalidOperationException">If no, or more than one, UserPlan with the specified TrainingPlan ID</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule violated</exception>
        public void RemoveTrainingPlanFromLibrary(uint trainingPlanId)
        {
            if (_trainingPlans.Remove(FindTrainingPlan(trainingPlanId)))
            {
                if (trainingPlanId == CurrentTrainingPlanId)
                    CurrentTrainingPlanId = null;

                TestBusinessRules();
                AddDomainEvent(new TrainingPlanRemovedFromLibraryDomainEvent(trainingPlanId));
            }
        }

        /// <summary>
        /// Assign the name to the Training Plan
        /// </summary>
        /// <param name="trainingPlanName">The Training Plan name</param>
        /// <param name="trainingPlanId">The ID of the Training Plan Root entity which relation has to be modified</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void RenameTrainingPlan(uint trainingPlanId, string trainingPlanName) => FindTrainingPlan(trainingPlanId).GiveName(trainingPlanName);


        /// <summary>
        /// Assign the IsBookmarked flag
        /// </summary>
        /// <param name="bookmarkedFlag">The flag</param>
        /// <param name="trainingPlanId">The ID of the Training Plan Root entity which relation has to be modified</param>
        public void BookmarkTrainingPlan(uint trainingPlanId, bool bookmarkedFlag = true) => FindTrainingPlan(trainingPlanId).ChangeBookmarkedFlag(bookmarkedFlag);


        /// <summary>
        /// Assign the Training Plan Note with the specified ID to the Selected User Training Plan
        /// </summary>
        /// <param name="trainingPlanNoteId">The note ID</param>
        /// <param name="trainingPlanId">The ID of the Training Plan Root entity which relation has to be modified</param>
        public void AttachTrainingPlanNote(uint trainingPlanId, uint? trainingPlanNoteId) => FindTrainingPlan(trainingPlanId).AttachNote(trainingPlanNoteId);


        /// <summary>
        /// Remove the Training Plan Note ID
        /// </summary>
        /// <param name="trainingPlanId">The ID of the Training Plan Root entity which relation has to be modified</param>
        public void CleanTrainingPlanNote(uint trainingPlanId) => FindTrainingPlan(trainingPlanId).CleanNote();


        /// <summary>
        /// Make the Training Plan a variant of the speicified one
        /// </summary>
        /// <param name="trainingPlanId">The ID of the Training Plan Root entity which relation has to be modified</param>
        /// <param name="parentPlanId">The ID of the User Training Plan which is the parent of this one</param>
        public void MakeTrainingPlanVariantOf(uint trainingPlanId, uint parentPlanId) => FindTrainingPlan(trainingPlanId).MakeItVariantOf(parentPlanId);


        /// <summary>
        /// Make the Training Plan not a Variant anymore
        /// </summary>
        /// <param name="trainingPlanId">The ID of the Training Plan Root entity which relation has to be modified</param>
        public void MakeTrainingPlanNotVariantOfAny(uint trainingPlanId) => FindTrainingPlan(trainingPlanId).RemoveVariantOf();


        /// <summary>
        /// Give focus to the Muscle
        /// </summary>
        /// <param name="muscleId">The Muscle ID to be added</param>
        /// <param name="trainingPlanId">The ID of the Training Plan Root entity which relation has to be modified</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void FocusTrainingPlanOnMuscle(uint trainingPlanId, uint muscleId) => FindTrainingPlan(trainingPlanId).FocusOnMuscle(muscleId);



        /// <summary>
        /// Remove the focus to the Muscle
        /// </summary>
        /// <param name="muscleId">The Muscle ID to be removed</param>
        /// <param name="trainingPlanId">The ID of the Training Plan Root entity which relation has to be modified</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void UnfocusTrainingPlanFromMuscle(uint trainingPlanId, uint muscleId) => FindTrainingPlan(trainingPlanId).UnfocusMuscle(muscleId);


        /// <summary>
        /// Link the Proficiency to the Training Plan
        /// </summary>
        /// <param name="proficiencyId">The Proficiency ID to be added</param>
        /// <param name="trainingPlanId">The ID of the Training Plan Root entity which relation has to be modified</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void MarkTrainingPlanAsSuitableForProficiencyLevel(uint trainingPlanId, uint proficiencyId) => FindTrainingPlan(trainingPlanId).LinkTargetProficiency(proficiencyId);


        /// <summary>
        /// Unlink the Proficiency from the Training Plan
        /// </summary>
        /// <param name="proficiencyId">The Proficiency ID to be removed</param>
        /// <param name="trainingPlanId">The ID of the Training Plan Root entity which relation has to be modified</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void UnlinkTrainingPlanTargetProficiency(uint trainingPlanId, uint proficiencyId) => FindTrainingPlan(trainingPlanId).UnlinkTargetProficiency(proficiencyId);

        /// <summary>
        /// Add the Hasw to the Training Plan
        /// </summary>
        /// <param name="hashtagId">The Hashtag ID to be added</param>
        /// <param name="trainingPlanId">The ID of the Training Plan Root entity which relation has to be modified</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void TagTrainingPlanAs(uint trainingPlanId, uint hashtagId) => FindTrainingPlan(trainingPlanId).TagAs(hashtagId);


        /// <summary>
        /// Remove the Hastag from the Training Plan
        /// </summary>
        /// <param name="hashtagId">The Hashtag ID to be removed - non NULL</param>
        /// <param name="trainingPlanId">The ID of the Training Plan Root entity which relation has to be modified</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void UntagTrainingPlan(uint trainingPlanId, uint hashtagId) =>  FindTrainingPlan(trainingPlanId).Untag(hashtagId);


        /// <summary>
        /// Link the Phase to the Training Plan
        /// </summary>
        /// <param name="phaseId">The Phase ID to be added</param>
        /// <param name="trainingPlanId">The ID of the Training Plan Root entity which relation has to be modified</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void TagTrainingPlanWithPhase(uint trainingPlanId, uint phaseId) => FindTrainingPlan(trainingPlanId).TagPhase(phaseId);


        /// <summary>
        /// Unlink the Phase from the Training Plan
        /// </summary>
        /// <param name="phaseId">The Hashtag ID to be removed</param>
        /// <param name="trainingPlanId">The ID of the Training Plan Root entity which relation has to be modified</param>
        /// <exception cref="ArgumentNullException">If the input ID is null</exception>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule is violated</exception>
        public void UntagTrainingPlanWithPhase(uint trainingPlanId, uint phaseId) => FindTrainingPlan(trainingPlanId).UntagPhase(phaseId);


        #endregion


        #region Private Methods

        /// <summary>
        /// Close all the phases still not completed at the specified date
        /// </summary>
        /// <param name="checkDate">The date</param>
        private void CloseOpenPhases(DateTime checkDate)
        {
            _trainingPhases.SingleOrDefault(x => x.EndDate == null 
                || DateRangeValue.IsDateBetween(checkDate, x.StartDate, x.EndDate.Value))?.Close();       // Do not use the public method as we need the reference, not the value copy   
        }


        /// <summary>
        /// Close all the phases still not completed at the specified date
        /// </summary>
        /// <param name="checkDate">The date</param>
        private void CloseTrainingProficiencyLevel()
        {
            _trainingProficiencies.SingleOrDefault(x => x.EndDate == null)?.Close();     // Do not use the public method as we need the reference, not the value copy   
        }


        /// <summary>
        /// Get the Training Plan with the specified ID
        /// </summary>
        /// <param name="trainingPlanId">the ID</param>
        /// <returns>The UserTrainingPlan reference instance</returns>
        /// <exception cref="InvalidOperationException">If no entity with the specified search key</exception>
        private UserTrainingPhaseRelation FindPhaseStartingFrom(DateTime startDate)

            => _trainingPhases.Single(x => x.StartDate == startDate);


        /// <summary>
        /// Get the Training Plan with the specified ID
        /// </summary>
        /// <param name="trainingPlanId">the ID</param>
        /// <returns>The UserTrainingPlan reference instance</returns>
        /// <exception cref="InvalidOperationException">If no entity with the specified search key</exception>
        private UserTrainingPlanEntity FindTrainingPlan(uint trainingPlanId)

            => _trainingPlans.Single(x => x.TrainingPlanId == trainingPlanId);

        #endregion



        #region Buisness Rules Validation

        private bool AtMostOneTrainingPhaseOpen()

            => _trainingPhases.Count(x => x.EndDate == null || DateRangeValue.IsDateBetween(DateTime.UtcNow, x.StartDate, x.EndDate.Value)) <= 1;

        
        private bool NoOverlappingPhases()

            => !_trainingPhases.Any(x => _trainingPhases.Where(y => y != x)
                    .Any(y => DateRangeValue.RangeBetween(y.StartDate, y.EndDate)
                        .Overlaps(DateRangeValue.RangeBetween(x.StartDate, x.EndDate))));

            //=> !_trainingPhases.Any(x => _trainingPhases.Where(y => y != x)
            //        .Any(y => y.Period.Overlaps(x.Period)));

            // Faster version, but it works only if ordered elements - which actually should be
            //=> !_trainingPhases.Any(x => _trainingPhases.SkipWhile(y => y.Period.End <= x.Period.End)
            //    .Any(y => y.Period.Overlaps(x.Period)));


        private bool NoNullTrainingPhases()

            => _trainingPhases.All(x => x != null);


        private bool NoNullTrainingProficiencies()

            => _trainingProficiencies.All(x => x != null);


        private bool AtMostOneTrainingProficiencyAtTheSameTime()

            => _trainingProficiencies.Count(x => !x.EndDate.HasValue) <= 1;
        
        private bool NoOverlappingProficiencies()

            => !_trainingProficiencies.Any(x => _trainingProficiencies.Where(y => y != x)
                    .Any(y => DateRangeValue.RangeBetween(y.StartDate, y.EndDate)
                        .Overlaps(DateRangeValue.RangeBetween(x.StartDate, x.EndDate))));

        // Faster version, but it works only if ordered elements - which actually should be
        //=> !_trainingProficiencies.Any(x => _trainingProficiencies.SkipWhile(y => y.Period.End <= x.Period.End)
        //    .Any(y => y.Period.Overlaps(x.Period)));

        private bool NoDuplicateTrainingPlans()

            //=> !_trainingPlans.Any(x => _trainingPlans.Count(y => y.TrainingPlanId == x.TrainingPlanId) > 1);
            => _trainingPlans.GroupBy(x => x.TrainingPlanId).All(g => g.Count() == 1);  // Might be faster 

        private bool CurrentPlanIsInUserPlansLibrary()

            => CurrentTrainingPlanId == null
                || _trainingPlans.SingleOrDefault(x => x.TrainingPlanId == CurrentTrainingPlanId) != default;




        private void TestBusinessRules()
        {
            //if (!NoNullTrainingPhases())
            //    throw new TrainingDomainInvariantViolationException($"A user cannot have no NULL Training Phases.");

            if (!AtMostOneTrainingPhaseOpen())
                throw new TrainingDomainInvariantViolationException($"A user can have no more than one Training Phase open.");
            
            if (!NoOverlappingPhases())
                throw new TrainingDomainInvariantViolationException($"A user cannot have overlapping phases");

            
            //if (!NoNullTrainingProficiencies())
            //    throw new TrainingDomainInvariantViolationException($"A user cannot have no NULL Training Proficiencies.");

            if (!AtMostOneTrainingProficiencyAtTheSameTime())
                throw new TrainingDomainInvariantViolationException($"A user can have no more than one Training Proficiency levels at the same time.");
                    
            if (!NoOverlappingProficiencies())
                throw new TrainingDomainInvariantViolationException($"A user cannot have overlapping proficiencies");
    
            if (!NoDuplicateTrainingPlans())
                throw new TrainingDomainInvariantViolationException($"The same training plan cannot be included to the User library more than once.");    

            if (!CurrentPlanIsInUserPlansLibrary())
                throw new TrainingDomainInvariantViolationException($"The User cannot have a Training Plan which is not in its library as the current one.");
        }

        #endregion


        #region ICloneable Implementation

        public object Clone()
            => RegisterAthlete(Id, _trainingPhases, _trainingProficiencies, _trainingPlans);

        #endregion
    }
}
