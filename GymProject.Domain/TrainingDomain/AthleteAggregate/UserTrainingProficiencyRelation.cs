using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.TrainingDomain.Exceptions;
using System;
using System.Collections.Generic;

namespace GymProject.Domain.TrainingDomain.AthleteAggregate
{

    public class UserTrainingProficiencyRelation : ValueObject, ICloneable
    {



        // <summary>
        /// The Phase period
        /// </summary>
        public DateRangeValue Period { get; private set; } = null;


        /// <summary>
        /// FK to the Training Proficiency entry
        /// </summary>
        public uint? ProficiencyId { get; private set; } = null;




        #region Ctors

        private UserTrainingProficiencyRelation()
        {

        }

        private UserTrainingProficiencyRelation(uint? proficiencyId, DateRangeValue period)
        {
            Period = period;
            ProficiencyId = proficiencyId;

            if (ProficiencyId == null)
                throw new TrainingDomainInvariantViolationException($"Cannot create a Training Proficiency without a Proficiency linked to it");

            if (Period == null || !Period.IsLeftBounded())
                throw new TrainingDomainInvariantViolationException($"Cannot create a Training Proficiency with an invalid period");
        }

        #endregion



        #region Factories

        /// <summary>
        /// Factory method for assigning a Training Proficiency
        /// </summary>
        /// <param name="period">The period which the Proficiency level is valid over</param>
        /// <param name="proficiencyId">The ID of the Training Proficiency</param>
        /// <param name="owner">The one who is setting the Proficiency</param>
        /// <param name="athlete">The athlete which this Proficiency level refers to</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserTrainingProficiencyRelation AssignTrainingProficiency(uint? proficiencyId, DateRangeValue period)

            => new UserTrainingProficiencyRelation(proficiencyId, period);


        /// <summary>
        /// Factory method for assigning a Training Proficiency
        /// </summary>
        /// <param name="startingFrom">The starting date</param>
        /// <param name="proficiencyId">The ID of the Training Proficiency</param>
        /// <param name="athlete">The athlete which this Proficiency level refers to</param>
        /// <returns>A new UserPhase instance</returns>
        public static UserTrainingProficiencyRelation AchieveTrainingProficiency(uint? proficiencyId, DateTime startingFrom)

            => new UserTrainingProficiencyRelation(proficiencyId, DateRangeValue.RangeStartingFrom(startingFrom));


        #endregion



        #region Business Methods

        /// <summary>
        /// Close the Proficiency as a new one is started.
        /// The previous Proficiency level finishes the day before the current one
        /// </summary>
        public void Close() => Period = DateRangeValue.RangeBetween(Period.Start, DateTime.UtcNow.AddDays(-1));


        /// <summary>
        /// Change the start date
        /// </summary>
        /// <param name="newStartDate">The new start date</param>
        public void ShiftStartDate(DateTime newStartDate)
        {
            if (Period.IsRightBounded())
                Period = DateRangeValue.RangeBetween(newStartDate, Period.End);

            else
                Period = DateRangeValue.RangeStartingFrom(newStartDate);
        }

        #endregion


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Period;
            yield return ProficiencyId;
        }


        public object Clone()

            => AssignTrainingProficiency(ProficiencyId, Period);
    }
}
