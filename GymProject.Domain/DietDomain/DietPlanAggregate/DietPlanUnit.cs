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

        private DietPlanUnit(IdType id, DateRangeValue period = null)
        {
            Id = id;
            PeriodScheduled = period;
            _dietDays = new List<DietPlanDay>();

            if (!DietUnitIdNotNull())
                throw new DietDomainIvariantViolationException($"The Diet Unit ID period must be valid.");
        }


        private DietPlanUnit(IdType id, DateRangeValue unitPeriod, ICollection<DietPlanDay> dietDays)
        {
            Id = id;
            PeriodScheduled = unitPeriod;

            if (dietDays == null || dietDays?.Count == 0)
                _dietDays = new List<DietPlanDay>();
            else
                AssignDietDays(dietDays);
            

            if(!DietUnitIdNotNull())
                throw new DietDomainIvariantViolationException($"The Diet Unit ID period must be valid.");

            if (DietUnitPeriodIsNotNull())
                    throw new DietDomainIvariantViolationException($"Cannot create a {GetType().Name} with no period associated.");
        }
        #endregion


        #region Factories

        /// <summary>
        /// Factory method for creating drafts, IE: blank unit templates
        /// </summary>
        /// <param name="id">The diet plan unit ID</param>
        /// <returns>The DietPlanUnitValue instance</returns>
        public static DietPlanUnit NewDraft(IdType id)

            => new DietPlanUnit(id);

        /// <summary>
        /// Factory method for creating drafts, IE: blank unit templates
        /// </summary>
        /// <param name="id">The diet plan unit ID</param>
        /// <param name="period">The perdiod to be scheduled</param>
        /// <returns>The DietPlanUnitValue instance</returns>
        public static DietPlanUnit NewScheduledDraft(IdType id, DateRangeValue period) 
            
            => new DietPlanUnit(id, period);


        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="id">The diet plan unit ID</param>
        /// <param name="period">The perdiod to be scheduled</param>
        /// <param name="dietDays">The diet days linked to this unit</param>
        /// <returns>The DietPlanUnitValue instance</returns>
        public static DietPlanUnit ScheduleDietUnit(IdType id, DateRangeValue period, ICollection<DietPlanDay> dietDays) 
            
            => new DietPlanUnit(id, period, dietDays);

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


        ///// <summary>
        ///// Modifes the already planned Diet Day
        ///// </summary>
        ///// <param name="newDay">The day to be added</param>
        ///// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        ///// <exception cref="ArgumentException">Thrown when newDay is not found</exception>
        //public void ChangePlanDay
        //(
        //    IdType ,
        //    MacronutirentWeightValue dailyCarbs,
        //    MacronutirentWeightValue dailyFats,
        //    MacronutirentWeightValue dailyProteins,
        //    MicronutirentWeightValue salt = null,
        //    VolumeValue water = null,
        //    WeeklyOccuranceValue weeklyOccurrances = null,
        //    string dayName = null,
        //    WeekdayEnum specificWeekday = null,
        //    DietDayTypeEnum dayType = null
        //)
        //{
        //    DietPlanDay toBeChanged = FindDayById(newDay.Id);

        //    if (toBeChanged == default)
        //        throw new ArgumentException($"The Diet Plan Day - Id={toBeChanged.Id.ToString()} - does not belong to the Diet Plan Unit - Id={Id.ToString()} -");

        //    toBeChanged = newDay;

        //    FinalizeDietPlanDaysChanged();
        //}


        /// <summary>
        /// Adds the Diet Plan Day or modifies it if already planned
        /// </summary>
        /// <param name="oldId">The ID of the day to be changed</param>
        /// <param name="dayName">The name which identifies the day</param>
        /// <param name="dailyCarbs">Carbohidrates quantity</param>
        /// <param name="dailyFats">Fats quantity</param>
        /// <param name="dailyProteins">Proteins quantity</param>
        /// <param name="salt">Salt quantity</param>
        /// <param name="water">Water quantity</param>
        /// <param name="specificWeekday">The specific weekday number - or null</param>
        /// <param name="dayType">The day type</param>
        /// <param name="weeklyOccurrances">How many times the day is repetead throghout the week</param>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        /// <exception cref="ArgumentException">Thrown when oldId not found</exception>
        public void ChangeDietDay
        (
            IdType oldId,
            MacronutirentWeightValue dailyCarbs,
            MacronutirentWeightValue dailyFats,
            MacronutirentWeightValue dailyProteins,
            MicronutirentWeightValue salt = null,
            VolumeValue water = null,
            WeeklyOccuranceValue weeklyOccurrances = null,
            WeekdayEnum specificWeekday = null,
            DietDayTypeEnum dayType = null,
            string dayName = null
        )
        {
            DietPlanDay toBeChanged = FindDayById(oldId);

            if (toBeChanged == default)
                throw new ArgumentException($"The Diet Plan Day - Id={oldId.ToString()} - does not belong to the Diet Plan Unit - Id={Id.ToString()} -");

            if (_dietDays.Remove(toBeChanged))

                _dietDays.Add(DietPlanDay.AddDayToPlan(oldId,
                    dayName,
                    weeklyOccurrances,
                    dailyCarbs,
                    dailyFats,
                    dailyProteins,
                    salt,
                    water,
                    specificWeekday,
                    dayType));


            FinalizeDietPlanDaysChanged();
        }

        /// <summary>
        /// Adds the Diet Plan Day or modifies it if already planned
        /// </summary>
        /// <param name="dayName">The name which identifies the day</param>
        /// <param name="dailyCarbs">Carbohidrates quantity</param>
        /// <param name="dailyFats">Fats quantity</param>
        /// <param name="dailyProteins">Proteins quantity</param>
        /// <param name="salt">Salt quantity</param>
        /// <param name="water">Water quantity</param>
        /// <param name="specificWeekday">The specific weekday number - or null</param>
        /// <param name="dayType">The day type</param>
        /// <param name="weeklyOccurrances">How many times the day is repetead throghout the week</param>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void PlanDietDay
        (
            MacronutirentWeightValue dailyCarbs,
            MacronutirentWeightValue dailyFats,
            MacronutirentWeightValue dailyProteins,
            WeeklyOccuranceValue weeklyOccurrances = null,
            string dayName = null,
            MicronutirentWeightValue salt = null,
            VolumeValue water = null,
            WeekdayEnum specificWeekday = null,
            DietDayTypeEnum dayType = null
        )
        {
            IdType newId = BuildDietPlanDayId();

            _dietDays.Add(

                DietPlanDay.AddDayToPlan(newId,
                dayName,
                weeklyOccurrances,
                dailyCarbs,
                dailyFats,
                dailyProteins,
                salt,
                water,
                specificWeekday,
                dayType
            ));

            FinalizeDietPlanDaysChanged();
        }


        /// <summary>
        /// Remove the selected diet day
        /// </summary>
        /// <param name="toRemoveId">The ID of the day to be removed</param>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void UnplanDietDay(IdType toRemoveId)
        {
            DietPlanDay toBeRemoved = FindDayById(toRemoveId);

            if (_dietDays.Remove(toBeRemoved))
            {
                SetDietDaysNames(_dietDays);
                GetAvgDaylyCalories(_dietDays);
            }
            else
                throw new ArgumentException($"The Diet Plan Day - Id={toRemoveId.ToString()} - does not belong to the Diet Plan Unit - Id={Id.ToString()} -");

            // If no more days then raise the event - No invariant check, the handler will decide what to do
            if (_dietDays.Count == 0)
                AddDomainEvent(new DietPlanUnitHasBeenClearedDomainEvent(this));
        }

        /// <summary>
        /// Find the Diet Plan Unit with the ID specified
        /// </summary>
        /// <param name="id">The Id to be found</param>
        /// <returns>The DietPlanUnit or DEFAULT if not found/returns>
        public DietPlanDay FindDayById(IdType id) => _dietDays.FirstOrDefault(x => x.Id == id);

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
            CalorieValue calories = CalorieValue.MeasureKcal(days.Sum(x => x.Calories.Value * x.WeeklyOccurrances.Value) / (float)WeekdayEnum.AllTheWeek);

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



        /// <summary>
        /// Build the next valid Diet Plan Unit id
        /// </summary>
        /// <returns>The Diet Plan Unit Id</returns>
        private IdType BuildDietPlanDayId()
        {
            if (_dietDays.Count == 0)

                return new IdType(1);

            else
                return _dietDays.Last().Id + 1;
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
        private bool DietUnitDaysNotExceedingWeekdays() => _dietDays.Count <= WeekdayEnum.AllTheWeek && _dietDays.Sum(x => x.WeeklyOccurrances.Value) <= WeekdayEnum.AllTheWeek;


        /// <summary>
        /// There cannot be multiple Diet Days associated to the same Weekday
        /// </summary>
        /// <returns>True if the rule is not violatedr</returns>
        private bool DietUnitDaysNoDuplicateWeekdays()
        {
            foreach (DietPlanDay day in _dietDays
                    .Where(x => !x.SpecificWeekday.Equals(WeekdayEnum.Generic)))
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
                .Where(x => !string.IsNullOrWhiteSpace(x.Name)))
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
        /// The Diet Unit ID period must be valid.
        /// </summary>
        /// <returns>True if the rule is not violated</returns>
        private bool DietUnitIdNotNull() => Id != null && Id > 0;


        /// <summary>
        /// The Diet Plan Days must have unique Ids inside the Diet Plan Unit
        /// </summary>
        /// <returns>True if the rule is not violated</returns>
        private bool DietPlanDaysHaveUniqueIds() => _dietDays.GroupBy(x => x.Id).All(g => g.Count() == 1);


        /// <summary>
        /// Checks the Diet Days for the business rules
        /// </summary>
        /// <exception cref="DietDomainIvariantViolationException">Thrown if business rules are broken</exception>
        private void TestDietDaysBusinessRules()
        {
            if (!DietPlanDaysHaveUniqueIds())
                throw new DietDomainIvariantViolationException($"The Diet Plan Days must have unique Ids inside the Diet Plan Unit.");

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
