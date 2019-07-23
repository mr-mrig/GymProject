using GymProject.Domain.Base;
using GymProject.Domain.DietDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.DietDomain.DietPlanAggregate
{
    public class DietPlan : Entity<IdType>
    {


        /// <summary>
        /// The Diet Plan name
        /// </summary>
        public string Name { get; private set; } = string.Empty;


        /// <summary>
        /// The author of the diet plan
        /// </summary>
        public Owner Owner { get; private set; } = null;


        /// <summary>
        /// The Owner's note
        /// </summary>
        public PersonalNoteValue OwnerNote { get; private set; } = null;


        /// <summary>
        /// The number of free meals allowed per week
        /// </summary>
        public WeeklyOccuranceValue WeeklyFreeMeals { get; private set; } = null;


        // <summary>
        /// The Diet plan period - Might be not bounded on the right
        /// </summary>
        public DateRangeValue PeriodScheduled { get; private set; } = null;


        // <summary>
        /// The average daily calories for the whole plan
        /// </summary>
        public CalorieValue AvgDailyCalories { get; private set; } = null;


        // <summary>
        /// FK to Post
        /// </summary>
        public IdType PostId { get; private set; } = null;


        private ICollection<DietPlanUnit> _dietUnits;

        /// <summary>
        /// The diet days planned
        /// </summary>
        public IReadOnlyCollection<DietPlanUnit> DietUnits
        {
            get => _dietUnits?.ToList().AsReadOnly();
        }




        #region Ctors

        private DietPlan
         (
            IdType postId,
            string name,
            Owner owner = null,
            PersonalNoteValue ownerNote = null,
            WeeklyOccuranceValue weeklyFreeMeals = null,
            ICollection<DietPlanUnit> dietUnits = null)
        {
            Name = name;
            Owner = owner;
            OwnerNote = ownerNote;
            WeeklyFreeMeals = weeklyFreeMeals;
            PostId = postId;

            if (dietUnits == null || dietUnits.Count == 0)
                _dietUnits = new List<DietPlanUnit>();
            else
                AssignDietUnits(dietUnits);     // Throws

            TestBusinessRules();        // Throws
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="period">The perdiod to be scheduled</param>
        /// <returns>The DietPlanValue instance</returns>
        public static DietPlan ScheduleDietPlan
        (
            IdType postId,
            string name,
            Owner owner = null,
            PersonalNoteValue ownerNote = null,
            WeeklyOccuranceValue weeklyFreeMeals = null
        )
            => new DietPlan(postId, name, owner, ownerNote, weeklyFreeMeals);

        #endregion



        #region Business Methods

        /// <summary>
        /// Assigns the diet plan units
        /// </summary>
        /// <param name="dietUnits">The diet plan units</param>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void AssignDietUnits(ICollection<DietPlanUnit> dietUnits)
        {
            _dietUnits = dietUnits;

            AvgDailyCalories = GetAvgDailyCalories(_dietUnits);
            PeriodScheduled = GetPlanPeriod(_dietUnits);

            TestDietUnitsBusinessRules();        // Throws
        }


        /// <summary>
        /// Schedule a new diet plan unit putting it after the last one (chronologically)
        /// </summary>
        /// <param name="upTo">The last day of the new unit</param>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void AppendNewUnit(DateTime upTo)
        {
            _dietUnits.Add(
                 DietPlanUnit.ScheduleDietUnit(DateRangeValue.RangeBetween(PeriodScheduled.End.AddDays(1), upTo))
                 );

            AssignDietUnits(_dietUnits);
        }


        /// <summary>
        /// Schedule a new diet plan unit putting it after the last one (chronologically)
        /// The new unit is not right bounded
        /// </summary>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void AppendNewUnit()
        {
            _dietUnits.Add(
                 DietPlanUnit.ScheduleDietUnit(DateRangeValue.RangeStartingFrom(PeriodScheduled.End.AddDays(1)))
                 );

            AssignDietUnits(_dietUnits);
        }


        /// <summary>
        /// Schedule a new diet plan unit putting it after the last one (chronologically)
        /// The new unit is not right bounded
        /// </summary>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void ScheduleNewUnit(ebwerbwr)
        {
            _dietUnits.Add(
                 DietPlanUnit.ScheduleDietUnit(DateRangeValue.RangeStartingFrom(PeriodScheduled.End.AddDays(1)))
                 );

            AssignDietUnits(_dietUnits);
        }


        /// <summary>
        /// Remove the selected diet day
        /// </summary>
        /// <param name="toRemove">The day to be added</param>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void UnscheduleUnit(DietPlanDay toRemove)
        {
            if (_dietDays.Remove(toRemove))
            {
                SetDietDaysNames(_dietDays);
                GetAvgDailyCalories(_dietDays);
            }
        }


        /// <summary>
        /// Reschedule the diet unit
        /// </summary>
        /// <param name="newValue">The new period</param>
        /// <exception cref="ValueObjectInvariantViolationException">Thrown if period breaks business rules</exception>
        public void RescheduleUnit(DateRangeValue newPeriod) =>
        #endregion


        #region Private Methods

        /// <summary>
        /// Get the average weekly calories with respect to the diet days of the units
        /// </summary>
        /// <param name="units">The diet units</param>
        /// <returns>The CalorieValue</returns>
        private CalorieValue GetAvgDailyCalories(IEnumerable<DietPlanUnit> units)
        {
            CalorieValue calories = CalorieValue.MeasureKcal(units.Sum(x => x.AvgDailyCalories.Value) / (float)_dietUnits.Count);

            //if (calories == null)
            //    throw new DietDomainIvariantViolationException($"Invalid Diet Days linked to the Diet Unit {Id.ToString()}");yj5ye

            return calories;
        }

        /// <summary>
        /// Get the period which the Diet Plan is scheduled to
        /// </summary>
        /// <param name="units">The diet units</param>
        /// <returns>The DateRangeValue representing the scheduled period</returns>
        private DateRangeValue GetPlanPeriod(IEnumerable<DietPlanUnit> units)

            => DateRangeValue.RangeBetween(units.Select(x => x.PeriodScheduled.Start).Min(), units.Select(x => x.PeriodScheduled.End).Max());       // End = DateTime.MaxValue() if one unit end date is not set


        #endregion



        #region Business Rules Specifications

        /// <summary>
        /// The Diet Plan must be linked to a Post
        /// </summary>
        /// <returns>True if the business rule is met</returns>
        private bool DietPlanLinkedToPost() => PostId != null;


        /// <summary>
        /// The Diet Plan must have at least one Unit
        /// </summary>
        /// <returns>True if the business rule is met</returns>
        private bool DietPlanUnitsNotEmpty() => _dietUnits.Count > 0;

        /// <summary>
        /// The Diet Units must not overlap each other
        /// </summary>
        /// <returns>True if the business rule is met</returns>
        private bool DietPlanUnitsNotOverlapping()
        {

            foreach(DietPlanUnit unit in _dietUnits)
            {
                if (_dietUnits.SkipWhile(x => x != unit).Any(x => x.PeriodScheduled.Overlaps(unit.PeriodScheduled)))
                    return false;
            }

            return true;
        }


        /// <summary>
        /// The Diet Units must be contiguous, IE: the right boundary of one is one day before the left boundary of the next
        /// </summary>
        /// <returns>True if the rule is not violated</returns>
        private bool DietPlanUnitsAreContiguous()
        {
            foreach (DietPlanUnit unit in _dietUnits)
            {
                if (!(_dietUnits.Any(x => x != unit && x.PeriodScheduled.Start != unit.PeriodScheduled.End.AddDays(1)))
                    || _dietUnits.Any(x => x != unit && x.PeriodScheduled.End != unit.PeriodScheduled.Start.AddDays(-1)))
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Test the Diet Plan Units business rules and manages invalid states
        /// </summary>
        /// <exception cref="DietDomainIvariantViolationException">Thrown if business rules are broken</exception>
        private void TestDietUnitsBusinessRules()
        {
            if (!DietPlanUnitsNotEmpty())
                throw new DietDomainIvariantViolationException($"The Diet Plan must have at least one Unit");

            if (!DietPlanUnitsNotOverlapping())
                throw new DietDomainIvariantViolationException($"he Diet Units must not overlap each other.");

            //if (!DietPlanUnitsAreContiguous())
            //    throw new DietDomainIvariantViolationException($"The Diet Units must be contiguous."); ;
        }


        /// <summary>
        /// Test the Diet Plan business rules and manages invalid states
        /// </summary>
        /// <exception cref="DietDomainIvariantViolationException">Thrown if business rules are broken</exception>
        private void TestBusinessRules()
        {
            if (!DietPlanLinkedToPost())
                throw new DietDomainIvariantViolationException($"The Diet Plan must be linked to a Post");
        }

        #endregion


        protected IEnumerable<object> GetAtomicValues()
        {
            yield return Name;
            yield return Owner;
            yield return OwnerNote;
            yield return PeriodScheduled;
            yield return WeeklyFreeMeals;
            yield return DietUnits;
            yield return AvgDailyCalories;
            yield return PostId;
        }

    }
}
