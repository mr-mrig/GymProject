using GymProject.Domain.Base;
using GymProject.Domain.DietDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
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

        private DietPlanUnit()
        {

        }


        private DietPlanUnit(DateRangeValue unitPeriod, ICollection<DietPlanDay> dietDays)
        {
            PeriodScheduled = unitPeriod;

            if (dietDays == null || dietDays?.Count == 0)
                _dietDays = new List<DietPlanDay>();
            else
                AssignDietDays(dietDays);
            
            if (DietUnitPeriodIsNotNull())
                throw new DietDomainIvariantViolationException($"Cannot create a {GetType().Name} with no period associated.");
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="period">The perdiod to be scheduled</param>
        /// <returns>The DietPlanUnitValue instance</returns>
        public static DietPlanUnit ScheduleDietUnit(DateRangeValue period) 
            
            => ScheduleDietUnit(period, null);


        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="period">The perdiod to be scheduled</param>
        /// <param name="dietDays">The diet days linked to this unit</param>
        /// <returns>The DietPlanUnitValue instance</returns>
        public static DietPlanUnit ScheduleDietUnit(DateRangeValue period, ICollection<DietPlanDay> dietDays) 
            
            => new DietPlanUnit(period, dietDays);


        /// <summary>
        /// Factory method for creating drafts, IE: blank unit templates
        /// </summary>
        /// <param name="period">The perdiod to be scheduled</param>
        /// <param name="dietDays">The diet days linked to this unit</param>
        /// <returns>The DietPlanUnitValue instance</returns>
        public static DietPlanUnit NewDraft()

            => new DietPlanUnit();

        #endregion



        #region Business Methods

        /// <summary>
        /// Reschedule the diet unit
        /// </summary>
        /// <param name="newValue">The new period</param>
        /// <exception cref="ValueObjectInvariantViolationException">Thrown if period breaks business rules</exception>
        public void Reschedule(DateRangeValue newPeriod) => PeriodScheduled = newPeriod;

        /// <summary>
        /// Extend/shorten the period by changing the end date
        /// </summary>
        /// <param name="newEndDate">The new end date</param>
        /// <exception cref="ValueObjectInvariantViolationException">Thrown if period breaks business rules</exception>
        public void RescheduleEndDate(DateTime newEndDate) => PeriodScheduled = PeriodScheduled.RescheduleEnd(newEndDate);


        /// <summary>
        /// Extend/shorten the period by changing the start date
        /// </summary>
        /// <param name="newStartDate">The new start date</param>
        /// <exception cref="ValueObjectInvariantViolationException">Thrown if period breaks business rules</exception>
        public void RescheduleStartDate(DateTime newStartDate) => PeriodScheduled = PeriodScheduled.RescheduleEnd(newStartDate);


        /// <summary>
        /// Assigns the diet days
        /// </summary>
        /// <param name="newDays">The new end date</param>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void AssignDietDays(ICollection<DietPlanDay> newDays)
        {
            _dietDays = newDays;
            FinalizeDietPlanDaysChanged();
        }


        /// <summary>
        /// Adds the Diet Plan Day or modifies it if already planned
        /// </summary>
        /// <param name="newDay">The day to be added</param>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void PlanDietDay(DietPlanDay newDay)
        {
            DietPlanDay toBeChanged = _dietDays.Where(x => x == newDay).FirstOrDefault();

            if (toBeChanged == default)
                _dietDays.Add(newDay);
            else
                toBeChanged = newDay;

            FinalizeDietPlanDaysChanged();
        }


        /// <summary>
        /// Remove the selected diet day
        /// </summary>
        /// <param name="toRemove">The day to be added</param>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void UnplanDietDay(DietPlanDay toRemove)
        {
            if(_dietDays.Remove(toRemove))
            {
                SetDietDaysNames(_dietDays);
                GetAvgDaylyCalories(_dietDays);
            }
            
            // If no more days then raise the event - No invariant check, the handler will decide what to do
            if(_dietDays.Count == 0)
                AddDomainEvent(new DietPlanUnitHasBeenClearedDomainEvent(this));
        }

        #endregion


        #region Private Methods


        /// <summary>
        /// Finalization step for consolidating a change in the Diet Plan Day list 
        /// </summary>
        private void FinalizeDietPlanDaysChanged()
        {
            AvgDailyCalories = GetAvgDaylyCalories(_dietDays);

            SetDietDaysNames(_dietDays);
            TestDietDaysBusinessRules();        // Throws
        }


        /// <summary>
        /// Get the average daily calories with respect to the diet days 
        /// </summary>
        /// <param name="days">The diet days</param>
        /// <returns>The CalorieValue</returns>
        private CalorieValue GetAvgDaylyCalories(IEnumerable<DietPlanDay> days)
        {
            CalorieValue calories = CalorieValue.MeasureKcal(days.Sum(x => x.Calories.Value * x.WeeklyOccurrances.Value) / (float)WeekdayEnum.Max);

            //if (calories == null)
            //    throw new DietDomainIvariantViolationException($"Invalid Diet Days linked to the Diet Unit {Id.ToString()}");yj5ye

            return calories;
        }


        /// <summary>
        /// Check the diet days name and if left null assigne the default ones.
        /// IE: MON, TUE, ON, OFF, Refeed, etc.
        /// </summary>
        /// <returns>The renamed Diet Days</returns>
        private IEnumerable<DietPlanDay> SetDietDaysNames(IEnumerable<DietPlanDay> dietDays)
        {
            int genericNameCounter = 0;


            foreach (DietPlanDay day in dietDays)
            {
                if (string.IsNullOrWhiteSpace(day.Name))
                {
                    // Mon, Tue, etc.
                    if (!day.SpecificWeekday.Equals(WeekdayEnum.Generic))
                        day.Rename(day.SpecificWeekday.Abbreviation);

                    else
                    {
                        // ON, OFF etc. - Duplicate names allowed here
                        if (!day.DietDayType.Name.Equals(DietDayTypeEnum.NotSet))
                            day.Rename(day.DietDayType.Name);

                        // Day1, Day2, etc
                        else
                            day.Rename($"Day{(++genericNameCounter).ToString()}");

                    }
                }
            }
            return dietDays;
        }

        #endregion



        #region Business Rules Specifications

        /// <summary>
        /// The Diet Plan Unit must have at least one Diet Day
        /// </summary>
        /// <returns>True if number of Diet Days is positive</returns>
        private bool DietUnitDaysNotEmpty() => _dietDays.Count > 0;


        /// <summary>
        /// The Diet Days must be at most one per weekday
        /// </summary>
        /// <returns>True if number of Diet Days doesn't exceed the weekdays number</returns>
        private bool DietUnitDaysNotExceedingWeekdays() => _dietDays.Count <= WeekdayEnum.Max;


        /// <summary>
        /// There cannot be multiple Diet Days associated to the same Weekday
        /// </summary>
        /// <returns>True if the rule is not violatedr</returns>
        private bool DietUnitDaysNoDuplicateWeekdays()
        {
            foreach (DietPlanDay day in _dietDays
                    .Where(x => x?.SpecificWeekday != null && !x.SpecificWeekday.Equals(WeekdayEnum.Generic)))
            {
                if (_dietDays.Any(x => x.SpecificWeekday.Equals(day.SpecificWeekday)))
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Each Diet Day name must be unique among the other unit ones
        /// </summary>
        /// <returns>True if the rule is not violated</returns>
        private bool DietUnitDaysNoDuplicateName()
        {
            foreach (DietPlanDay day in _dietDays
                .Where(x => x?.Name != null && !string.IsNullOrWhiteSpace(x.Name)))
            {
                if (_dietDays.Any(x => x.Name.Equals(day.Name)))
                    return false;
            }
            return true;
        }


        /// <summary>
        /// The Diet Unit period is set
        /// </summary>
        /// <returns>True if the rule is not violated</returns>
        private bool DietUnitPeriodIsNotNull() => PeriodScheduled != null;


        /// <summary>
        /// Checks the Diet Days for the business rules
        /// </summary>
        /// <exception cref="DietDomainIvariantViolationException">Thrown if business rules are broken</exception>
        private void TestDietDaysBusinessRules()
        {

            if (!DietUnitDaysNotEmpty())
                throw new DietDomainIvariantViolationException($"The Diet Plan Unit must have at least one Diet Day");

            //if (DietUnitDaysNoDuplicateName())
            //    throw new DietDomainIvariantViolationException($"Each Diet Day name must be unique among the other Unit ones.");

            if (!DietUnitDaysNoDuplicateWeekdays())
                throw new DietDomainIvariantViolationException($"There cannot be multiple Diet Days associated to the same Weekday.");

            if (!DietUnitDaysNotExceedingWeekdays())
                throw new DietDomainIvariantViolationException($"The Diet Days must be at most one per weekday."); ;
        }

        #endregion


        protected IEnumerable<object> GetAtomicValues()
        {
            yield return PeriodScheduled;
            yield return DietDays;
            yield return AvgDailyCalories;
        }

    }
}
