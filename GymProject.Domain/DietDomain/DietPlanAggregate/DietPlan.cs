using GymProject.Domain.Base;
using GymProject.Domain.DietDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Domain.DietDomain.DietPlanAggregate
{
    public class DietPlan : Entity<IdType>, INotificationHandler<DietPlanUnitHasBeenClearedDomainEvent>
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
            FinalizeDietPlanUnitsChanged();
        }


        /// <summary>
        /// Schedule a new diet plan unit putting it immediatly after the last one (chronologically)
        /// The new unit is not right bounded
        /// </summary>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void AppendDietPlanUnit()
        {
            _dietUnits.Add(
                 DietPlanUnit.ScheduleDietUnit(DateRangeValue.RangeStartingFrom(PeriodScheduled.End.AddDays(1)))
                 );

            FinalizeDietPlanUnitsChanged();
        }


        /// <summary>
        /// Schedule a new diet plan unit putting it immediatly after the last one (chronologically)
        /// </summary>
        /// <param name="upTo">The last day of the new unit</param>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void AppendDietPlanUnit(DateTime upTo) => ScheduleDietPlanUnit(PeriodScheduled.End.AddDays(1), upTo);


        /// <summary>
        /// Schedule a new diet plan unit
        /// </summary>
        /// <param name="startingFrom">The left boundary date</param>
        /// <param name="upTo">The right boundary date</param>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void ScheduleDietPlanUnit(DateTime startingFrom, DateTime upTo)
        {
            _dietUnits.Add(
                 DietPlanUnit.ScheduleDietUnit(DateRangeValue.RangeBetween(startingFrom, upTo))
                 );

            FinalizeDietPlanUnitsChanged();
        }


        /// <summary>
        /// Remove the selected diet plan unit
        /// </summary>
        /// <param name="toRemove">The Diet Plan Unit to be removed</param>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void UnscheduleDietPlanUnit(DietPlanUnit toRemove)
        {
            if (_dietUnits.Remove(toRemove))
            {
                AvgDailyCalories = GetAvgDailyCalories(_dietUnits);
                PeriodScheduled = GetPlanPeriod(_dietUnits);
            }

            // If no more days then raise the event - No invariant check, the handler will decide what to do
            if (_dietUnits.Count == 0)
                DomainEvents.Add(new DietPlanHasBeenClearedDomainEvent(this, PostId));
        }


        /// <summary>
        /// Reschedule the Diet Plan Unit
        /// </summary>
        /// <param name="newPeriod">The new period</param>
        /// <param name="toBeMoved">The unit to be moved</param>
        /// <exception cref="ValueObjectInvariantViolationException">Thrown if period breaks business rules</exception>
        /// <exception cref="ArgumentException">If the Unit doesn't belong to the Diet Plan</exception>
        public void MoveDietPlanUnit(DietPlanUnit toBeMoved, DateRangeValue newPeriod)
        {
            if (!_dietUnits.Contains(toBeMoved))
                throw new ArgumentException($"The Diet Plan Unit - Id={toBeMoved.Id} - does not belong to the Diet Plan - Id={Id} -");

            toBeMoved.Reschedule(newPeriod);

            TestDietUnitsBusinessRules();
        }
        #endregion


        #region Private Methods

        /// <summary>
        /// Finalization step for consolidating a change in the Diet Plan Unit list
        /// </summary>
        private void FinalizeDietPlanUnitsChanged()
        {
            AvgDailyCalories = GetAvgDailyCalories(_dietUnits);
            PeriodScheduled = GetPlanPeriod(_dietUnits);

            TestDietUnitsBusinessRules();        // Throws
        }


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
        /// The Diet Plan Units must have at least one Diet Day
        /// </summary>
        /// <returns>True if the rule is not violated</returns>
        private bool DietPlanUnitsHaveAtLeastOneDay() => _dietUnits.All(unit => unit.DietDays.Count(day => day != null) > 0);


        /// <summary>
        /// Test the Diet Plan Units business rules and manages invalid states
        /// </summary>
        /// <exception cref="DietDomainIvariantViolationException">Thrown if business rules are broken</exception>
        private void TestDietUnitsBusinessRules()
        {
            if (!DietPlanUnitsNotEmpty())
                throw new DietDomainIvariantViolationException($"The Diet Plan must have at least one Unit");

            if (!DietPlanUnitsNotOverlapping())
                throw new DietDomainIvariantViolationException($"he Diet Plan Units must not overlap each other.");

            //if (!DietPlanUnitsAreContiguous())
            //    throw new DietDomainIvariantViolationException($"The Diet Plan Units must be contiguous."); ;

            if (!DietPlanUnitsHaveAtLeastOneDay())
                throw new DietDomainIvariantViolationException($"The Diet Plan Units must have at least one Diet Day.");
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


        #region Mediator
        public Task Handle(DietPlanUnitHasBeenClearedDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
