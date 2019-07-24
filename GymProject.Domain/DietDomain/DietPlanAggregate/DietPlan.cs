using GymProject.Domain.Base;
using GymProject.Domain.DietDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        private DietPlan(Owner author, ICollection<DietPlanUnit> dietUnits)
        {
            Owner = author;
            _dietUnits = dietUnits ?? new List<DietPlanUnit>();
        }


        private DietPlan
         (
            IdType postId,
            string name,
            ICollection<DietPlanUnit> dietUnits,
            Owner owner = null,
            PersonalNoteValue ownerNote = null,
            WeeklyOccuranceValue weeklyFreeMeals = null)
        {
            Name = name;
            Owner = owner;
            OwnerNote = ownerNote;
            WeeklyFreeMeals = weeklyFreeMeals;
            PostId = postId;

            AssignDietUnits(dietUnits);     // Throws
            TestBusinessRules();        // Throws
        }
        #endregion



        #region Factories

        ///// <summary>
        ///// Factory method
        ///// </summary>
        ///// <param name="name">The name of the plan</param>
        ///// <param name="owner">The author of the plan</param>
        ///// <param name="ownerNote">The owner's note</param>
        ///// <param name="postId">The parent Post Id</param>
        ///// <param name="weeklyFreeMeals">The number of weekly free meals allowed</param>
        ///// <returns>The DietPlanValue instance</returns>
        //public static DietPlan ScheduleDietPlan
        //(
        //    IdType postId,
        //    string name,
        //    Owner owner = null,
        //    PersonalNoteValue ownerNote = null,
        //    WeeklyOccuranceValue weeklyFreeMeals = null
        //)
        //    => ScheduleDietPlan(postId, name, null, owner, ownerNote, weeklyFreeMeals);


        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="name">The name of the plan</param>
        /// <param name="dietPlanUnits">The diet plan units linked to the plan</param>
        /// <param name="owner">The author of the plan</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <param name="postId">The parent Post Id</param>
        /// <param name="weeklyFreeMeals">The number of weekly free meals allowed</param>
        /// <returns>The DietPlanValue instance</returns>
        public static DietPlan ScheduleDietPlan
        (
            IdType postId,
            string name,
            ICollection<DietPlanUnit> dietPlanUnits,
            Owner owner = null,
            PersonalNoteValue ownerNote = null,
            WeeklyOccuranceValue weeklyFreeMeals = null
        )
            => new DietPlan(postId, name, dietPlanUnits, owner, ownerNote, weeklyFreeMeals);



        /// <summary>
        /// Factory method for creating drafts, IE empty templates
        /// </summary>
        /// <param name="planAuthor">The owner of the plan</param>
        /// <returns>The DietPlanValue instance</returns>
        public static DietPlan NewDraft(Owner planAuthor)
        {
            DietPlanUnit draftUnit = DietPlanUnit.NewDraft();
            List<DietPlanUnit> draftUnits = new List<DietPlanUnit>() { draftUnit, };

            return new DietPlan(planAuthor, draftUnits);
        }

        #endregion



        #region Business Methods

        ///// <summary>
        ///// Finalize and submit the Diet Plan
        ///// </summary>
        ///// <param name="name">The diet plan name</param>
        ///// <param name="ownerNote">The ownerìs message</param>
        //public void Submit(string name, PersonalNoteValue ownerNote)
        //{
        //    Name = name;
        //    OwnerNote = ownerNote;

        //    // Check owner can submit it

        //    //TestBusinessRules();
        //    //TestDietUnitsBusinessRules();
        //}


        /// <summary>
        /// Set the Owner's note
        /// </summary>
        /// <param name="ownerNote">The owner note</param>
        public void GiveName(PersonalNoteValue ownerNote) => OwnerNote = ownerNote;


        /// <summary>
        /// Set the Diet Plan name
        /// </summary>
        /// <param name="dietPlanName">The Plan name</param>
        public void GiveName(string dietPlanName) => Name = dietPlanName;


        /// <summary>
        /// Link the Diet Plan to a Post
        /// </summary>
        /// <param name="postId">The parent Post ID</param>
        public void LinkToPost(IdType postId) => PostId = postId;

        #endregion


        #region Diet Units Methods

        /// <summary>
        /// Add the Diet Day to a Diet Plan Unit
        /// </summary>
        /// <param name="dietUnitId">The Diet Plan Unit id</param>
        /// <param name="toAdd">The Day to be added</param>
        /// <exception cref="ArgumentException">Thrown if the DietPlanUnit doesn't belong to the Plan </exception>
        public void PlanDietDay(IdType dietUnitId, DietPlanDay toAdd)
        {
            DietPlanUnit toBeChanged = FindUnitById(dietUnitId);

            if (toBeChanged == default)
                throw new ArgumentException($"The Diet Plan Unit - Id={dietUnitId.ToString()} - does not belong to the Diet Plan - Id={Id.ToString()} -");

            toBeChanged.PlanDietDay(toAdd);
        }


        /// <summary>
        /// Remove the Diet Day to a Diet Plan Unit
        /// </summary>
        /// <param name="dietUnitId">The Diet Plan Unit id</param>
        /// <param name="toRemove">The Day to be added</param>
        /// <exception cref="ArgumentException">Thrown if the DietPlanUnit doesn't belong to the Plan </exception>
        public void UnplanDietDay(IdType dietUnitId, DietPlanDay toRemove)
        {
            DietPlanUnit toBeChanged = FindUnitById(dietUnitId);

            if (toBeChanged == default)
                throw new ArgumentException($"The Diet Plan Unit - Id={dietUnitId.ToString()} - does not belong to the Diet Plan - Id={Id.ToString()} -");

            toBeChanged.UnplanDietDay(toRemove);
        }


        /// <summary>
        /// Create a new Diet Plan Unit Draft and schedules it after the last one
        /// The new unit is not right bounded
        /// </summary>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void AppendDietPlanUnitDraft()
        {
            _dietUnits.Add(
                 DietPlanUnit.NewScheduledDraft(DateRangeValue.RangeStartingFrom(PeriodScheduled.End.AddDays(1)))
                 );

            AvgDailyCalories = GetAvgDailyCalories(_dietUnits);
            PeriodScheduled = GetPlanPeriod(_dietUnits);
        }


        /// <summary>
        /// Create a new Diet Plan Unit Draft and schedules it after the last one
        /// </summary>
        /// <param name="upTo">The last day of the new unit</param>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void AppendDietPlanUnitDraft(DateTime upTo)
        {
            _dietUnits.Add(
                 DietPlanUnit.NewScheduledDraft(DateRangeValue.RangeBetween(PeriodScheduled.End.AddDays(1), upTo))
                 );

            AvgDailyCalories = GetAvgDailyCalories(_dietUnits);
            PeriodScheduled = GetPlanPeriod(_dietUnits);
        }


        /// <summary>
        /// Assigns a schedule to the specified DietPlanUnit
        /// </summary>
        /// <param name="planUnitId">The ID of the Diet Plan Unit to be scheduled</param>
        /// <param name="startingFrom">The left boundary date</param>
        /// <param name="upTo">The right boundary date</param>
        /// <exception cref="ArgumentException">If the ID specified is not between the Plan DietUnits</exception>
        /// <exception cref="DietDomainIvariantViolationException">If business ruels violated</exception>
        public void ScheduleDietPlanUnit(IdType planUnitId, DateTime startingFrom, DateTime upTo)
        {
            DietPlanUnit toBeScheduled = FindUnitById(planUnitId);

            if (toBeScheduled == default)
                throw new ArgumentException($"The Diet Plan Unit - Id={planUnitId.ToString()} - does not belong to the Diet Plan - Id={Id.ToString()} -");

            toBeScheduled.Reschedule(DateRangeValue.RangeBetween(startingFrom, upTo));

            FinalizeDietPlanUnitsChanged();
        }


        /// <summary>
        /// Remove the selected diet plan unit
        /// </summary>
        /// <param name="toRemoveId">The Diet Plan Unit to be removed</param>
        /// <exception cref="ArgumentException">Thrown if unit not found</exception>
        public void UnscheduleDietPlanUnit(IdType toRemoveId)
        {
            DietPlanUnit toBeRemoved = FindUnitById(toRemoveId);

            if (_dietUnits.Remove(toBeRemoved))
            {
                AvgDailyCalories = GetAvgDailyCalories(_dietUnits);
                PeriodScheduled = GetPlanPeriod(_dietUnits);
            }
            else
                throw new ArgumentException($"The Diet Plan Unit - Id={toRemoveId.ToString()} - does not belong to the Diet Plan - Id={Id.ToString()} -");

            // If no more days then raise the event - No invariant check, the handler will decide what to do
            if (_dietUnits.Count == 0)
                AddDomainEvent(new DietPlanHasBeenClearedDomainEvent(this, PostId));
        }


        /// <summary>
        /// Reschedule the Diet Plan Unit
        /// </summary>
        /// <param name="newPeriod">The new period</param>
        /// <param name="toMoveId">The unit to be moved</param>
        /// <exception cref="ValueObjectInvariantViolationException">Thrown if period breaks business rules</exception>
        /// <exception cref="ArgumentException">If the Unit doesn't belong to the Diet Plan</exception>
        public void MoveDietPlanUnit(IdType toMoveId, DateRangeValue newPeriod)
        {
            DietPlanUnit toBeMoved = FindUnitById(Id);

            if (toBeMoved == default)
                throw new ArgumentException($"The Diet Plan Unit - Id={toMoveId.ToString()} - does not belong to the Diet Plan - Id={Id.ToString()} -");

            toBeMoved.Reschedule(newPeriod);

            TestDietUnitsBusinessRules();
        }


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
        #endregion


        #region Private Methods

        /// <summary>
        /// Find the Diet Plan Unit with the ID specified
        /// </summary>
        /// <param name="id">The Id to be found</param>
        /// <returns>The DietPlanUnit or DEFAULT if not found/returns>
        private DietPlanUnit FindUnitById(IdType id) => _dietUnits.Where(x => x.Id == id).FirstOrDefault();


        /// <summary>
        /// Finalization step for consolidating a change in the Diet Plan Unit list
        /// </summary>
        private void FinalizeDietPlanUnitsChanged()
        {
            TestDietUnitsBusinessRules();        // Throws

            AvgDailyCalories = GetAvgDailyCalories(_dietUnits);
            PeriodScheduled = GetPlanPeriod(_dietUnits);

            AddDomainEvent(new DietPlanChangedDomainEvent(this, PostId));
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
        private bool DietPlanUnitsNotEmpty() => _dietUnits != null && _dietUnits.Count > 0;

        /// <summary>
        /// The Diet Units must not overlap each other
        /// </summary>
        /// <returns>True if the business rule is met</returns>
        private bool DietPlanUnitsNotOverlapping()
        {

            foreach(DietPlanUnit unit in _dietUnits)
            {
                if (_dietUnits.SkipWhile(x => x != unit).Any(x => x.PeriodScheduled != null && x.PeriodScheduled.Overlaps(unit.PeriodScheduled)))
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
        private bool DietPlanUnitsHaveAtLeastOneDay() => _dietUnits.All(unit => unit.DietDays?.Count(day => day != null) > 0);


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
