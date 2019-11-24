using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Exceptions;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.TrainingDomain.AthleteAggregate
{
    public class AthleteRoot : Entity<uint?>, IAggregateRoot
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
                


        private List<uint?> _trainingPlansIds = new List<uint?>();

        /// <summary>
        /// FK to the trainning plans belonging tu the user library
        /// </summary>
        public IReadOnlyCollection<uint?> TrainingPlansIds
        {
            get => _trainingPlansIds?.AsReadOnly() ?? new List<uint?>().AsReadOnly();
        }






        #region Ctors

        private AthleteRoot() : base(null)
        {
            _trainingPhases =  new List<UserTrainingPhaseRelation>();
            _trainingProficiencies =  new List<UserTrainingProficiencyRelation>();
            _trainingPlansIds = new List<uint?>();
        }


        private AthleteRoot(uint? id, IEnumerable<UserTrainingPhaseRelation> trainingPhasesHistory, IEnumerable<UserTrainingProficiencyRelation>proficienciesHistory,
            IEnumerable<uint?> trainingPlansIds) : base(id)
        {
            _trainingPhases = trainingPhasesHistory?.Clone().ToList() ?? new List<UserTrainingPhaseRelation>();
            _trainingProficiencies = proficienciesHistory?.Clone().ToList() ?? new List<UserTrainingProficiencyRelation>();
            _trainingPlansIds = trainingPlansIds?.ToList() ?? new List<uint?>();

            TestBusinessRules();
        }


        #endregion



        #region Factories

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <returns></returns>
        public static AthleteRoot RegisterAthlete()

            => new AthleteRoot();

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

            => _trainingPhases.SingleOrDefault(x => x.Period.Includes(date))?.Clone() as UserTrainingPhaseRelation;



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

            => _trainingProficiencies.SingleOrDefault(x => x.Period.Includes(date))?.Clone() as UserTrainingProficiencyRelation;



        public uint? GetCurrentTrainingPlan()

            => throw new NotImplementedException();


        public bool GetTrainingPlans()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Business Methods

        /// <summary>
        /// Assign the Training Phase to the user planning it according to the period specified:
        /// - period was not provided or doesn't have a start date -> plan the phase starting form today and leave if not right bounded
        /// - period placed in the past -> exception
        /// - otherwise -> plan the phase over the period specified (which can be either right bounded or not)
        /// Starting a phase automatically closes all the ones still left open.
        /// </summary>
        /// <param name="phaseId">The Phase ID to schedule</param>
        /// <param name="ownerId">The ID of the user which is performing the action - the owner</param>
        /// <param name="entryVisibility">The visibilty</param>
        /// <param name="period">The period to schedule the phase over - if not left default it must have a valid starting date and it must not be placed in the past</param>
        /// <param name="ownerNote">The owner note - optional</param>
        /// <returns>A value copy of the UserTrainingPhaseRelation object or null if no phthing found</returns>
        /// <exception cref="TrainingDomainInvariantViolationException">If more open phases found</exception>
        /// <exception cref="InvalidOperationException">If try to start a phase over a past period</exception>
        public void StartTrainingPhase(uint phaseId, EntryStatusTypeEnum entryVisibility, DateRangeValue period = null, PersonalNoteValue ownerNote = null)
        {
            UserTrainingPhaseRelation newPhase;

            // If valid period then plan it, otherwise start it from today
            if (period != null && period.IsLeftBounded())
            {
                // Removing this coinstraint would require the Domain to manage potentially overlapping phases
                if (period.Start.Value < DateTime.UtcNow)
                    throw new InvalidOperationException($"Cannot start a Training Phase over an elapsed period: {period.Start.Value.ToShortTimeString()} - {period.End.Value.ToShortTimeString()} ");

                newPhase = UserTrainingPhaseRelation.PlanPhase(phaseId, period, entryVisibility, ownerNote);
            }
            else
                newPhase = UserTrainingPhaseRelation.StartPhase(phaseId, DateTime.UtcNow, entryVisibility, ownerNote);

            // Close the previous phase, if any
            CloseOpenPhases(period?.Start ?? DateTime.UtcNow);

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


        public void StartTrainingPlan()
        {

        }


        public void AddTrainingPlanToLibrary()
        {

        }


        public void RemoveTrainingPlanFromLibrary()
        {

        }

        #endregion


        #region Private Methods


        /// <summary>
        /// Close all the phases still not completed at the specified date
        /// </summary>
        /// <param name="checkDate">The date</param>
        private void CloseOpenPhases(DateTime? checkDate = null)
        {
            _trainingPhases.SingleOrDefault(x => !x.Period.IsRightBounded()
                || x.Period.Includes(checkDate ?? DateTime.Now)).Close();       // Do not use the public method as we need the reference, not the value copy   
        }


        /// <summary>
        /// Close all the phases still not completed at the specified date
        /// </summary>
        /// <param name="checkDate">The date</param>
        private void CloseTrainingProficiencyLevel()
        {
            _trainingProficiencies.SingleOrDefault(x => !x.Period.IsRightBounded()).Close();     // Do not use the public method as we need the reference, not the value copy   
        }

        #endregion



        #region Buisness Rules Validation

        private bool AtMostOneTrainingPhaseOpen()

            => _trainingPhases.Count(x => !x.Period.IsRightBounded() || x.Period.Includes(DateTime.UtcNow)) <= 1;


        private bool NoNullTrainingPhases()

            => _trainingPhases.All(x => x != null);


        private bool NoNullTrainingProficiencies()

            => _trainingProficiencies.All(x => x != null);


        private bool AtMostOneTrainingProficiencyAtTheSameTime()

            => _trainingProficiencies.Count(x => !x.Period.IsRightBounded()) <= 1;



        private void TestBusinessRules()
        {
            //if (!NoNullTrainingPhases())
            //    throw new TrainingDomainInvariantViolationException($"A user cannot have no NULL Training Phases.");

            if (!AtMostOneTrainingPhaseOpen())
                throw new TrainingDomainInvariantViolationException($"A user can have no more than one Training Phase open.");

            
            //if (!NoNullTrainingProficiencies())
            //    throw new TrainingDomainInvariantViolationException($"A user cannot have no NULL Training Proficiencies.");

            if (!AtMostOneTrainingProficiencyAtTheSameTime())
                throw new TrainingDomainInvariantViolationException($"A user can have no more than one Training Proficiency levels at the same time.");


            throw new NotImplementedException();
        }

        #endregion


    }
}
