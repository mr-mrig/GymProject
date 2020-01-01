using GymProject.Domain.Base;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using System.Collections.Generic;

namespace GymProject.Domain.TrainingDomain.AthleteAggregate
{

    public class UserTrainingProficiencyRelation : RelationEntity, ICloneable
    {



        /// <summary>
        /// The Date the Training Proficiency is set
        /// </summary>
        public DateTime StartDate { get; private set; }


        /// <summary>
        /// The Date the Training Proficiency is no longer valid beause another one is set
        /// </summary>
        public DateTime? EndDate { get; private set; }


        /// <summary>
        /// FK to the Training Proficiency entry
        /// </summary>
        public uint? ProficiencyId { get; private set; } = null;




        #region Ctors

        private UserTrainingProficiencyRelation()
        {

        }

        private UserTrainingProficiencyRelation(uint? proficiencyId, DateTime startDate, DateTime? endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
            ProficiencyId = proficiencyId;

            if (ProficiencyId == null)
                throw new TrainingDomainInvariantViolationException($"Cannot create a Training Proficiency without a Proficiency linked to it");

            TestBusinessRules();
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method for assigning a Training Proficiency
        /// This should not be used as the Proficiency cannot be planned. <see cref="AchieveTrainingProficiency"/> shoulde be used
        /// </summary>
        /// <param name="startDate">The starting date</param>
        /// <param name="endDate">The date the Training Proficiency is no looger valid/param>
        /// <param name="proficiencyId">The ID of the Training Proficiency</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserTrainingProficiencyRelation AssignTrainingProficiency(uint? proficiencyId, DateTime startDate, DateTime endDate)

            => new UserTrainingProficiencyRelation(proficiencyId, startDate, endDate);


        /// <summary>
        /// Factory method for assigning a Training Proficiency
        /// </summary>
        /// <param name="startingFrom">The starting date</param>
        /// <param name="proficiencyId">The ID of the Training Proficiency</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserTrainingProficiencyRelation AchieveTrainingProficiency(uint? proficiencyId, DateTime startingFrom)

            => new UserTrainingProficiencyRelation(proficiencyId, startingFrom, null);


        #endregion



        #region Business Methods

        /// <summary>
        /// Close the Proficiency as a new one is started.
        /// The previous Proficiency level finishes the day before the current one
        /// </summary>
        public void Close() => EndDate = DateTime.UtcNow.AddDays(-1);



        /// <summary>
        /// Change the start date
        /// </summary>
        /// <param name="newStartDate">The new start date</param>
        public void ShiftStartDate(DateTime newStartDate)
        {
            StartDate = newStartDate;
        }

        #endregion



        #region Business Rules Validation

        private bool StartDateBeforeEndDate()

        => EndDate == null || StartDate < EndDate;


        private void TestBusinessRules()
        {
            if (!StartDateBeforeEndDate())
                throw new TrainingDomainInvariantViolationException($"Invalid chronological order: start date must preceed end date");
        }

        #endregion


        #region ICloneable Implementation
        public object Clone()

            => EndDate.HasValue 
                ? AssignTrainingProficiency(ProficiencyId, StartDate, EndDate.Value)
                : AchieveTrainingProficiency(ProficiencyId, StartDate);
        #endregion

        protected override IEnumerable<object> GetIdentifyingFields()
        {
            yield return ProficiencyId;
            yield return StartDate;
        }
    }
}
