﻿using GymProject.Domain.Base;
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


        /// <summary>
        /// The current proficiency achieved by the athlete as a value copy
        /// </summary>
        public UserTrainingProficiencyRelation CurrentTrainingProficiency => FindCurrentProficiencyOrDefault()?.Clone() as UserTrainingProficiencyRelation;

        /// <summary>
        /// The current phase the athlete is performing as a value copy
        /// </summary>
        public UserTrainingPhaseRelation CurrentTrainingPhase => FindPhaseAtOrDefault(DateTime.UtcNow)?.Clone() as UserTrainingPhaseRelation;




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
        /// Make a value copy of the Training Proficiency level reached at a specific date
        /// </summary>
        /// <param name="date">The check datetime - it does not need to be a plain date</param>
        /// <returns>A value copy of the UserTrainingPhaseRelation object or null if nothing found</returns>
        public UserTrainingProficiencyRelation CloneTrainingProficiencyAt(DateTime date)

            //=> _trainingProficiencies.SingleOrDefault(x => x.Period.Includes(date))?.Clone() as UserTrainingProficiencyRelation;
            => _trainingProficiencies.SingleOrDefault(x => x.StartDate <= date.Date
                && (!x.EndDate.HasValue || x.EndDate >= date.Date))
                ?.Clone() as UserTrainingProficiencyRelation;

        /// <summary>
        /// Make a value copy of the Training Phase which the athlete is performing at the specified date
        /// </summary>
        /// <param name="date">The check datetime - it does not need to be a plain date</param>
        /// <returns>A value copy of the UserTrainingPhaseRelation object or null if nothing found</returns>
        public UserTrainingPhaseRelation CloneTrainingPhaseAt(DateTime date)

            => FindPhaseAtOrDefault(date)?.Clone() as UserTrainingPhaseRelation;


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
        /// <param name="startDate">The start datetime the Phase is scheduled from - if not null it must not be placed in the past. The time part will be ignored.</param>
        /// <param name="endDate">The end datetime the Phase is scheduled to. The time part will be ignored.</param>
        /// <param name="ownerNote">The owner note - optional</param>
        /// <returns>A value copy of the UserTrainingPhaseRelation object or null if no phthing found</returns>
        /// <exception cref="TrainingDomainInvariantViolationException">If more open phases found</exception>
        /// <exception cref="InvalidOperationException">If try to start a phase over a past period</exception>
        public void StartTrainingPhase(uint phaseId, EntryStatusTypeEnum entryVisibility,  DateTime? startDate = null, DateTime? endDate = null, PersonalNoteValue ownerNote = null)
        {
            UserTrainingPhaseRelation newPhase;
            UserTrainingPhaseRelation nextPhase = FindNextPhaseOrDefault(startDate ?? DateTime.UtcNow);

            // If a phase has already been planned in a future date, then force the new phase to last until the day before
            if (nextPhase != null)
            {
                if (endDate == null)
                    endDate = nextPhase.StartDate.AddDays(-1);
                //else if (endDate > nextPhase.StartDate)
                //    throw new InvalidOperationException("");      // Letting the Business Rules to handle it
            }

            // If valid period then plan it, otherwise start it from today
            if (startDate != null)
            {
                // Removing this coinstraint would require the Domain to manage potentially overlapping phases
                if (startDate.Value.Date < DateTime.UtcNow.Date)
                    throw new InvalidOperationException($"Cannot start a Training Phase over an elapsed period: {startDate.Value.ToShortTimeString()} - {endDate.Value.ToShortTimeString()} ");

                newPhase = UserTrainingPhaseRelation.PlanPhase(this, phaseId, startDate.Value, endDate, entryVisibility, ownerNote);
            }
            else
                newPhase = UserTrainingPhaseRelation.PlanPhase(this, phaseId, DateTime.UtcNow, endDate, entryVisibility, ownerNote);

            // Close the previous phase, if any
            CloseOpenPhase(startDate ?? DateTime.UtcNow);

            // Start the new one
            _trainingPhases.Add(newPhase);

            TestBusinessRules();
        }

        /// <summary>
        /// Simply assign the Training Phase to the user planning without performing any other action.
        /// <para></para>
        /// <para>WARNING: this function does not ensure the domain invariance hence it should not be exposed to the user.</para>
        /// <para>To be used for testing/repositorying purposes only</para>
        /// </summary>
        /// <param name="phaseId">The Phase ID to schedule</param>
        /// <param name="entryVisibility">The visibilty</param>
        /// <param name="startDate">The start datetime the Phase is scheduled from - if not null it must not be placed in the past. The time part will be ignored.</param>
        /// <param name="endDate">The end datetime the Phase is scheduled to. The time part will be ignored.</param>
        /// <param name="ownerNote">The owner note - optional</param>
        /// <returns>A value copy of the UserTrainingPhaseRelation object or null if no phthing found</returns>
        public void AssignTrainingPhase(uint phaseId, EntryStatusTypeEnum entryVisibility,  DateTime startDate, DateTime? endDate = null, PersonalNoteValue ownerNote = null)

            => _trainingPhases.Add(
                UserTrainingPhaseRelation.PlanPhase(this, phaseId, startDate, endDate, entryVisibility, ownerNote));


        /// <summary>
        /// Close all the phases still not completed
        /// </summary>
        public void CloseCurrentPhase()

            => CloseOpenPhase(DateTime.UtcNow);


        /// <summary>
        /// Change the start date of the Training Phase currently starting from.
        /// Please notice that also the previous phase shifts according to new boundary - However shifting before the previous phase start date will raise an exception.
        /// </summary>
        /// <param name="newStartDate">The new start datetime. The time part will be ignored.</param>
        /// <param name="oldStartdate">The datetime which the phase currently stats from. The time part will be ignored.</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If business rule violated</exception>
        /// <exception cref="InvalidOperationException">If could not find any Training Phase according o the start date specified</exception>
        public void ShiftTrainingPhaseStartDate(DateTime oldStartdate, DateTime newStartDate)
        {
            UserTrainingPhaseRelation phase = FindPhaseStartingFromOrDefault(oldStartdate) 
                ?? throw new InvalidOperationException("Could not find the Phase starting at the specified date");

            UserTrainingPhaseRelation prevPhase = FindPhaseBeforeOrDefault(phase);

            phase.ShiftStartDate(newStartDate);
            prevPhase.Close(newStartDate);

            TestBusinessRules();
        }
        

        /// <summary>
        /// Get a value copy of the User Training Phase starting at the specified date - if any
        /// </summary>
        /// <param name="startDate">The starting datetime - it does not need to be a plain date</param>
        /// <returns>The UserTrainingPhaseRelation reference instance or null if not found</returns>
        public UserTrainingPhaseRelation ClonePhaseStartingFrom(DateTime startDate)

            => FindPhaseStartingFromOrDefault(startDate)?.Clone() as UserTrainingPhaseRelation;

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
        /// <exception cref="TrainingDomainInvariantViolationException">If more open phases found</exception>
        /// <exception cref="InvalidOperationException">If try to start a phase over a past period</exception>
        public void AchieveTrainingProficiency(uint proficiencyId)
        {
            UserTrainingProficiencyRelation proficiency = UserTrainingProficiencyRelation.AchieveTrainingProficiency(proficiencyId, DateTime.UtcNow.Date);

            // Close the previous proficiency level, if any
            CloseTrainingProficiencyLevel();

            // Start the new one
            _trainingProficiencies.Add(proficiency);

            TestBusinessRules();
        }

        /// <summary>
        /// <para>Assign a new Training Proficiency level to the user.</para>
        /// <para></para>
        /// <para>WARNING: this function does not ensure the domain invariance hence it should not be exposed to the user.</para>
        /// <para>To be used for testing/repositorying purposes only</para>
        /// </summary>
        /// <param name="proficiencyId">The Phase ID to schedule</param>
        /// <param name="fromDate">The achivement Datetime - it will be truncated to the plain date</param>
        /// <exception cref="InvalidOperationException">If try to start a phase over a past period</exception>
        public void AssignTrainingProficiency(uint proficiencyId, DateTime fromDate, DateTime toDate)
        
            => _trainingProficiencies.Add(UserTrainingProficiencyRelation.AssignTrainingProficiency(proficiencyId, fromDate, toDate));

        /// <summary>
        /// <para>Assign a new Training Proficiency level to the user.</para>
        /// <para></para>
        /// <para>WARNING: this function does not ensure the domain invariance hence it should not be exposed to the user.</para>
        /// <para>To be used for testing/repositorying purposes only</para>
        /// </summary>
        /// <param name="proficiencyId">The Phase ID to schedule</param>
        /// <param name="fromDate">The achivement Datetime - it will be truncated to the plain date</param>
        /// <exception cref="InvalidOperationException">If try to start a phase over a past period</exception>
        public void AssignTrainingProficiency(uint proficiencyId, DateTime fromDate)
        
            => _trainingProficiencies.Add(UserTrainingProficiencyRelation.AchieveTrainingProficiency(proficiencyId, fromDate));


        /// <summary>
        /// Remove the ssepcified Training Proficiency from the athlete's list
        /// </summary>
        /// <param name="proficiency">The proficiency to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule violated</exception>
        public void RemoveTrainingProficiency(UserTrainingProficiencyRelation proficiency)
        {
            _trainingProficiencies.Remove(proficiency);
            TestBusinessRules();
        }

        /// <summary>
        /// <para>Remove the ssepcified Training Phase from the athlete's list</para>
        /// <para>No consistency operation is performend, thus a gap will be left between the boundary phases</para>
        /// </summary>
        /// <param name="phase">The phase to be removed</param>
        /// <exception cref="TrainingDomainInvariantViolationException">If any business rule violated</exception>
        public void RemoveTrainingPhase(UserTrainingPhaseRelation phase)
        {
            if(_trainingPhases.Remove(phase))
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
        /// Check whether the Athlete has the specified Training Plan in its library
        /// </summary>
        /// <param name="trainingPlanId">The ID of the TrainingPlanRoot to be found</param>
        public void HasTrainingPlan(uint trainingPlanId)

            => _trainingPlans.Any(x => x.TrainingPlanId == trainingPlanId);


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
        /// Close the phase still not completed at the specified date.
        /// This might not corespond to the current date, as planned future phases must be considered as well.
        /// </summary>
        /// <param name="checkDate">The datetime - it does not need to be a plain date</param>
        /// <exception cref="InvalidOperationException">If trying to insert two phases starting on the same date</exception>
        private void CloseOpenPhase(DateTime checkDate)
        {
            UserTrainingPhaseRelation currentPhase = FindPhaseAtOrDefault(checkDate);              // Do not use the public method as we need the reference, not the value copy
            
            //UserTrainingPhaseRelation currentPhase = _trainingPhases.SingleOrDefault(x => x.EndDate == null
            //    || DateRangeValue.IsDateBetween(checkDate, x.StartDate, x.EndDate.Value));              // Do not use the public method as we need the reference, not the value copy 

            if (currentPhase != null)
            {
                if (currentPhase.StartDate == checkDate.Date)
                    throw new InvalidOperationException($"Cannot insert two phases starting on the same date. This case should be handled elsewhere.");
                else 
                    currentPhase.Close(checkDate);  
            }
        }

        /// <summary>
        /// Find the phase the athlete is performing at the specified date - or null if there's no - and return its reference.
        /// </summary>
        /// <param name="checkDate">The datetime - it does not need to be a plain date</param>
        /// <returns>The UserTrainingPhaseRelation refrence or default</returns>
        private UserTrainingPhaseRelation FindPhaseAtOrDefault(DateTime checkDate)
            => _trainingPhases.SingleOrDefault(x =>
                DateRangeValue.IsDateBetween(checkDate.Date, x.StartDate, x.EndDate));

        /// <summary>
        /// Find the phase the first ohase which has been planned after the specified date - if any
        /// </summary>
        /// <param name="checkDate">The datetime - it does not need to be a plain date</param>
        /// <returns>The UserTrainingPhaseRelation reference or default</returns>
        private UserTrainingPhaseRelation FindNextPhaseOrDefault(DateTime checkDate)

            => _trainingPhases.Where(x => x.StartDate > checkDate.Date)?.OrderBy(x => x.StartDate)?.FirstOrDefault();


        /// <summary>
        /// Get the User Training Phase starting at the specified date - if any
        /// </summary>
        /// <param name="startDate">the starting datetime - it does not need to be a plain date</param>
        /// <returns>The UserTrainingPhaseRelation reference instance or null if not found</returns>
        private UserTrainingPhaseRelation FindPhaseStartingFromOrDefault(DateTime startDate)

            => _trainingPhases.SingleOrDefault(x => x.StartDate == startDate.Date);

        /// <summary>
        /// Find the current proficiency - or null if there's no - and return its reference.
        /// </summary>
        /// <returns>The current Proficiency or null</returns>
        private UserTrainingProficiencyRelation FindCurrentProficiencyOrDefault()
            => _trainingProficiencies.SingleOrDefault(x => x.EndDate == null);

        /// <summary>
        /// Close the current User Training Proficiency
        /// </summary>
        private void CloseTrainingProficiencyLevel()

            => FindCurrentProficiencyOrDefault()?.Close();

        /// <summary>
        /// Get the User Training Phase right before the specified one, namely the first phase which start date precedes it
        /// </summary>
        /// <param name="phase">The phase</param>
        /// <returns>The UserTrainingPhaseRelation reference instance or null if nothing found</returns>
        /// <exception cref="InvalidOperationException">If no entity with the specified search key</exception>
        private UserTrainingPhaseRelation FindPhaseBeforeOrDefault(UserTrainingPhaseRelation phase)

            => _trainingPhases?.OrderBy(x => x.StartDate)
                .LastOrDefault(x => x.StartDate < phase.StartDate); 

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

            => _trainingPhases.Count(x => x.EndDate == null || DateRangeValue.IsDateStrictlyBetween(DateTime.UtcNow.Date, x.StartDate, x.EndDate.Value)) <= 1;

        
        private bool NoOverlappingPhases()

            => !_trainingPhases.Any(x => _trainingPhases.Where(y => y != x)
                    .Any(y => DateRangeValue.RangeBetween(y.StartDate, y.EndDate)
                        .Overlaps(DateRangeValue.RangeBetween(x.StartDate, x.EndDate))));

            //=> !_trainingPhases.Any(x => _trainingPhases.Where(y => y != x)
            //        .Any(y => y.Period.Overlaps(x.Period)));

            // Faster version, but it works only if ordered elements - which actually should be
            //=> !_trainingPhases.Any(x => _trainingPhases.SkipWhile(y => y.Period.End <= x.Period.End)
            //    .Any(y => y.Period.Overlaps(x.Period)));



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
            //if (!AtMostOneTrainingPhaseOpen())
            //    throw new TrainingDomainInvariantViolationException($"A user can have no more than one Training Phase open.");
            
            if (!NoOverlappingPhases())
                throw new TrainingDomainInvariantViolationException($"A user cannot have overlapping phases");

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
