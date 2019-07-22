using GymProject.Domain.Base;
using GymProject.Domain.DietDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.DietDomain.DietPlanAggregate
{
    public class DietPlanUnit : Entity<IdType>
    {

        // <summary>
        /// Diet unit period
        /// </summary>
        public DateRangeValue PeriodScheduled { get; private set; } = null;


        // <summary>
        /// The average daily calories for the unit
        /// </summary>
        public CalorieValue AvgDailyCalories { get; private set; } = null;


        ///// <summary>
        ///// Carbohidrates quantity
        ///// </summary>
        //public DateTime PlannedEndDate { get; private set; } = null;

        private ICollection<DietPlanDay> _dietDays;

        /// <summary>
        /// The diet days planned
        /// </summary>
        public IReadOnlyCollection<DietPlanDay> DietDays
        {
            get => _dietDays?.ToList().AsReadOnly();
        }




        #region Ctors

        private DietPlanUnit(DateRangeValue unitPeriod, ICollection<DietPlanDay> dietDays = null)
        {
            PeriodScheduled = unitPeriod;

            if (dietDays != null)
            {
                check
                _dietDays = new List<DietPlanDay>();
                AvgDailyCalories = GetAvgWeeklyCalories(_dietDays);
            }

            if (CheckNullState())
                throw new GlobalDomainGenericException($"Cannot create a {GetType().Name} with all NULL fields");

        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="period">The perdiod to be scheduled</param>
        /// <returns>The DietPlanUnitValue instance</returns>
        public static DietPlanUnit ScheduleDietUnit(DateRangeValue period) => new DietPlanUnit(period);


        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="period">The perdiod to be scheduled</param>
        /// <param name="dietDays">The diet days linked to this unit</param>
        /// <returns>The DietPlanUnitValue instance</returns>
        public static DietPlanUnit ScheduleDietUnit(DateRangeValue period, ICollection<DietPlanDay> dietDays) => new DietPlanUnit(period, dietDays);

        #endregion



        #region Business Methods

        /// <summary>
        /// Reschedule the diet unit
        /// </summary>
        /// <param name="newValue">The new period</param>
        public void Reschedule(DateRangeValue newPeriod) => PeriodScheduled = newPeriod;


        /// <summary>
        /// Extend/shorten the period by changing the end date
        /// </summary>
        /// <param name="newEndDate">The new end date</param>
        public void ChangeEndDate(DateTime newEndDate) => PeriodScheduled = PeriodScheduled.RescheduleEnd(newEndDate);


        /// <summary>
        /// Extend/shorten the period by changing the start date
        /// </summary>
        /// <param name="newStartDate">The new start date</param>
        public void ChangeStartDate(DateTime newStartDate) => PeriodScheduled = PeriodScheduled.RescheduleEnd(newStartDate);


        /// <summary>
        /// Assigns the diet days
        /// </summary>
        /// <param name="newDays">The new end date</param>
        public void AssignDietDays(ICollection<DietPlanDay> newDays)
        {
check

            _dietDays = newDays;
            AvgDailyCalories = GetAvgWeeklyCalories(_dietDays);
        }


        /// <summary>
        /// Schedule a new diet day
        /// </summary>
        /// <param name="newDay">The day to be added</param>
        public void ScheduleNewDay(DietPlanDay newDay)
        {
            _dietDays.Add(newDay);
            //AvgDailyCalories
        }


        /// <summary>
        /// Remove the selected diet day
        /// </summary>
        /// <param name="toRemove">The day to be added</param>
        public void UnscheduleDay(DietPlanDay toRemove)
        {
            _dietDays.Remove(toRemove);
            //AvgDailyCalories
        }

        /// <summary>
        /// Checks whether all the properties are null
        /// </summary>
        /// <returns>True if no there are no non-null properties</returns>
        public bool CheckNullState()
                => GetAtomicValues().All(x => x is null);

        #endregion


        /// <summary>
        /// Get the average weekly calories with respect to the diet days 
        /// </summary>
        /// <param name="days">The diet days</param>
        /// <returns>The CalorieValue</returns>
        private CalorieValue GetAvgWeeklyCalories(IEnumerable<DietPlanDay> days) => CalorieValue.MeasureKcal(days.Sum(x => x.Calories.Value * x.WeeklyOccurrances) / (float)WeekdayEnum.Max);


        /// <summary>
        /// Validates the diet days list with respect to the business rules
        /// </summary>
        /// <param name="dietDays">The diet days list to be verified</param>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when business rule is violated</exception>
        /// <returns>True if </returns>
        private void ValidateBusinessLogic(IEnumerable<DietPlanDay> dietDays)
        {
            // Days exceed the week days
            if (dietDays.Count() > WeekdayEnum.Max)
                throw new DietDomainIvariantViolationException($"The number of Diet Days provided exceeds the number of days in a week");

            // Check for different days linked to the same weekday
            foreach (DietPlanDay day in dietDays
                .Where(x => x?.SpecificWeekday != null && !x.SpecificWeekday.Equals(WeekdayEnum.Generic)))
            {
                if (dietDays.Any(x => x.SpecificWeekday.Equals(day.SpecificWeekday)))
                    throw new DietDomainIvariantViolationException($"Multiple Diet Days assigned to the same Weekday");
            }

            // Check for different days with the same name
            foreach (DietPlanDay day in dietDays
                .Where(x => x?.Name != null && !string.IsNullOrWhiteSpace(x.Name)))
            {
                if (dietDays.Any(x => x.Name.Equals(day.Name)))
                    throw new DietDomainIvariantViolationException($"Mutiple Diet Days with the same name");
            }
        }


        /// <summary>
        /// Check the diet days name and if left null uses the default ones.
        /// IE: MON, TUE, ON, OFF, Refeed, etc.
        /// </summary>
        /// <returns>The default name</returns>
        private IEnumerable<DietPlanDay> SetDietDaysNames(IEnumerable<DietPlanDay> dietDays)
        {
            foreach(DietPlanDay day in dietDays)
            {
                if (day.SpecificWeekday != null && !day.SpecificWeekday.Equals(WeekdayEnum.Generic))
                    day.Rename(day.SpecificWeekday.Abbreviation);

                if (day.DietDayType != null && !day.DietDayType.Name.Equals(DietDayTypeEnum.NotSet))
                    return DietDayType.Name;
            }
        }


        protected IEnumerable<object> GetAtomicValues()
        {
            yield return PeriodScheduled;
            yield return DietDays;
        }

    }
}
