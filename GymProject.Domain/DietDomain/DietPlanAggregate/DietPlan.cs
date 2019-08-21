using GymProject.Domain.Base;
using GymProject.Domain.DietDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
using GymProject.Domain.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymProject.Domain.DietDomain.DietPlanAggregate
{
    public class DietPlan : Entity<IdTypeValue>
    {



        public const int MaximumDietPlanUnits = int.MaxValue;



        /// <summary>
        /// The Diet Plan name
        /// </summary>
        public string Name { get; private set; } = string.Empty;


        /// <summary>
        /// The author of the diet plan
        /// </summary>
        public Owner Trainer { get; private set; } = null;


        /// <summary>
        /// The recipient of the diet plan
        /// </summary>
        public Trainee Trainee { get; private set; } = null;

        /// <summary>
        /// The Owner's note
        /// </summary>
        public PersonalNoteValue OwnerNote { get; private set; } = null;


        /// <summary>
        /// The number of free meals allowed per week
        /// </summary>
        public WeeklyOccuranceValue WeeklyFreeMeals { get; private set; } = null;


        // <summary>
        /// The Diet plan period - It is updated each time a Diet Plan Unit is confirmed
        /// </summary>
        public DateRangeValue PeriodScheduled { get; private set; } = null;


        // <summary>
        /// The average daily calories for the whole plan
        /// </summary>
        public CalorieValue AvgDailyCalories { get; private set; } = null;


        // <summary>
        /// FK to Post
        /// </summary>
        public IdTypeValue PostId { get; private set; } = null;


        private ICollection<IdTypeValue> _hashtagIds = null;

        // <summary>
        /// FK to Hashtag
        /// </summary>
        public IReadOnlyCollection<IdTypeValue> HashtagIds
        {
            get => _hashtagIds?.Clone().ToList().AsReadOnly() ?? new List<IdTypeValue>().AsReadOnly();
        }


        private ICollection<DietPlanUnit> _dietUnits;

        /// <summary>
        /// The diet days planned.
        /// Provides a value copy: the instance fields must be modified through the instance methods
        /// </summary>
        public IReadOnlyCollection<DietPlanUnit> DietUnits
        {
            get => _dietUnits?.Clone().ToList().AsReadOnly() ?? new List<DietPlanUnit>().AsReadOnly();
        }




        #region Ctors

        private DietPlan(Owner author, Trainee trainee, ICollection<DietPlanUnit> dietUnits)
        {
            Trainer = author;
            Trainee = trainee;
            _dietUnits = dietUnits.Clone().ToList() ?? new List<DietPlanUnit>();
        }


        private DietPlan
         (
            IdTypeValue postId,
            string name,
            Owner owner,
            Trainee trainee,
            ICollection<DietPlanUnit> dietUnits,
            PersonalNoteValue ownerNote = null,
            WeeklyOccuranceValue weeklyFreeMeals = null)
        {
            Name = name;
            Trainer = owner;
            Trainee = trainee;
            OwnerNote = ownerNote;
            WeeklyFreeMeals = weeklyFreeMeals;
            PostId = postId;

            dietUnits = dietUnits ?? new List<DietPlanUnit>();

            AssignDietUnits(dietUnits);     // Throws
            TestBusinessRules();        // Throws
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="name">The name of the plan</param>
        /// <param name="dietPlanUnits">The diet plan units linked to the plan</param>
        /// <param name="owner">The author of the plan</param>
        /// <param name="trainee">The rplan recipient</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <param name="postId">The parent Post Id</param>
        /// <param name="weeklyFreeMeals">The number of weekly free meals allowed</param>
        /// <returns>The DietPlanValue instance</returns>
        public static DietPlan ScheduleFullDietPlan
        (
            IdTypeValue postId,
            string name,
            Owner owner,
            Trainee trainee,
            ICollection<DietPlanUnit> dietPlanUnits,
            PersonalNoteValue ownerNote = null,
            WeeklyOccuranceValue weeklyFreeMeals = null
        )
            => new DietPlan(postId, name, owner, trainee, dietPlanUnits, ownerNote, weeklyFreeMeals);



        /// <summary>
        /// Factory method for creating drafts, IE empty templates
        /// </summary>
        /// <param name="sourceTrainer">The owner of the plan</param>
        /// <param name="sourceTrainer">The plan recipient</param>
        /// <returns>The DietPlanValue instance</returns>
        public static DietPlan NewDraft(Owner sourceTrainer, Trainee destTrainee)
        {
            List<DietPlanUnit> draftUnits = new List<DietPlanUnit>()
            {
                DietPlanUnit.NewDraft(new IdTypeValue(1)),
            };

            return new DietPlan(sourceTrainer, destTrainee, draftUnits);
        }

        #endregion



        #region Aggregate Root Methods

        /// <summary>
        /// Closes the Diet Plan by closing its Period, IE: if scheduling a new plan while this one was set as unbounded
        /// </summary>
        /// <param name="upTo">The last day of the plan</param>
        /// <exception cref="InvalidOperationException">Thrown if no Units have been scheduled</exception>
        public void CloseDietPlan(DateTime upTo)
        {
            if (_dietUnits.Count == 0)
                throw new InvalidOperationException($"No Diet Plan Units linked to the Diet Plan");

            DietPlanUnit last = GetLastScheduledDietPlanUnit();

            last.RescheduleEndDate(upTo);

            AvgDailyCalories = GetAvgDailyCalories(_dietUnits);
            PeriodScheduled = GetPlanPeriod(_dietUnits);
        }


        /// <summary>
        /// Finalize the plan by publishing it on the Journal
        /// </summary>
        /// <param name="howManyFreeMeals">The number of free meals</param>
        /// <param name="planName">The diet plan name</param>
        /// <param name="ownerNote">The owner's note</param>
        /// <param name="postId">The parent Post Id</param>
        public void FinalizePlan(IdTypeValue postId, PersonalNoteValue ownerNote = null, string planName = null, WeeklyOccuranceValue howManyFreeMeals = null)
        {
            GiveName(planName);
            WriteOwnerNote(ownerNote);
            GrantFreeMeals(howManyFreeMeals);
            PostId = postId;

            TestBusinessRules();
            TestDietUnitsBusinessRules();
        }


        /// <summary>
        /// Set the Owner's note
        /// </summary>
        /// <param name="ownerNote">The owner note</param>
        public void WriteOwnerNote(PersonalNoteValue ownerNote) => OwnerNote = ownerNote;


        /// <summary>
        /// Set the Diet Plan name
        /// </summary>
        /// <param name="dietPlanName">The Plan name</param>
        public void GiveName(string dietPlanName) => Name = dietPlanName ?? string.Empty;


        /// <summary>
        /// Set the number of free meals allowed
        /// </summary>
        /// <param name="howMany">The number of free meals</param>
        public void GrantFreeMeals(WeeklyOccuranceValue howMany) => WeeklyFreeMeals = howMany;


        /// <summary>
        /// Link the Diet Plan to a Post
        /// </summary>
        /// <param name="postId">The parent Post ID</param>
        public void LinkToPost(IdTypeValue postId) => PostId = postId;

        #endregion


        #region Diet Units Methods

        /// <summary>
        /// Creates a new Diet Day inside a specific Diet Plan Unit
        /// </summary>
        /// <param name="dietUnitId">The Diet Plan Unit id</param>
        /// <param name="dayName">The name which identifies the day</param>
        /// <param name="dailyCarbs">Carbohidrates quantity</param>
        /// <param name="dailyFats">Fats quantity</param>
        /// <param name="dailyProteins">Proteins quantity</param>
        /// <param name="salt">Salt quantity</param>
        /// <param name="water">Water quantity</param>
        /// <param name="specificWeekday">The specific weekday number - or null</param>
        /// <param name="dayType">The day type</param>
        /// <param name="weeklyOccurrances">How many times the day is repetead throghout the week</param>
        /// <exception cref="ArgumentException">Thrown if the DietPlanUnit doesn't belong to the Plan </exception>
        public void PlanDietDay(IdTypeValue dietUnitId,
            MacronutirentWeightValue dailyCarbs,
            MacronutirentWeightValue dailyFats,
            MacronutirentWeightValue dailyProteins,
            MicronutirentWeightValue salt = null,
            VolumeValue water = null,
            WeeklyOccuranceValue weeklyOccurrances = null,
            string dayName = null,
            WeekdayEnum specificWeekday = null,
            DietDayTypeEnum dayType = null)
        {
            DietPlanUnit toBeChanged = FindUnitById(dietUnitId);

            if (toBeChanged == default)
                throw new ArgumentException($"The Diet Plan Unit - Id={dietUnitId.ToString()} - does not belong to the Diet Plan");

            toBeChanged.PlanDietDay(

                dailyCarbs,
                dailyFats,
                dailyProteins,
                weeklyOccurrances,
                dayName,
                salt,
                water,
                specificWeekday,
                dayType);
        }


        /// <summary>
        /// Remove the Diet Day to a Diet Plan Unit
        /// </summary>
        /// <param name="dietUnitId">The Diet Plan Unit id</param>
        /// <param name="dietDayId">The Day to be removed</param>
        /// <exception cref="ArgumentException">Thrown if the DietPlanUnit doesn't belong to the Plan </exception>
        public void UnplanDietDay(IdTypeValue dietUnitId, IdTypeValue dietDayId)
        {
            DietPlanUnit toBeChanged = FindUnitById(dietUnitId);

            if (toBeChanged == default)
                throw new ArgumentException($"The Diet Plan Unit - Id={dietUnitId.ToString()} - does not belong to the Diet Plan");

            toBeChanged.UnplanDietDay(dietDayId);
        }


        /// <summary>
        /// Changes the Diet Plan Day of the specified Unit
        /// </summary>
        /// <param name="dietUnitId">The Diet Plan Unit id</param>
        /// <param name="dayId">The ID of the day to be changed</param>
        /// <exception cref="ArgumentException">Thrown if the DietPlanUnit doesn't belong to the Plan </exception>
        public void ChangeDietDay
        (
            IdTypeValue dietUnitId, 
            IdTypeValue dayId,
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
            DietPlanUnit toBeChanged = FindUnitById(dietUnitId);

            if (toBeChanged == default)
                throw new ArgumentException($"The Diet Plan Unit - Id={dietUnitId.ToString()} - does not belong to the Diet Plan.");

            toBeChanged.ChangeDietDay(
                dayId,
                dailyCarbs,
                dailyFats,
                dailyProteins,
                salt,
                water,
                weeklyOccurrances,
                specificWeekday,
                dayType, 
                dayName);

            TestDietUnitsBusinessRules();
        }


        /// <summary>
        /// Create a new Diet Plan Unit Draft and schedules it after the last one
        /// The new unit is not right bounded
        /// </summary>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        /// <exception cref="InvalidOperationException">Thrown if trying to add a new unit before the ones before have been scheduled</exception>
        public void AppendDietPlanUnitDraft()
            => AppendDietPlanUnitDraft(DateTime.MaxValue);
        //{
        //    IdType newId = BuildDietPlanUnitId();

        //    if (PeriodScheduled?.End == null || PeriodScheduled.End == DateTime.MaxValue)
        //        throw new InvalidOperationException($"Can't add a new Diet Plan Unit before scheduling the previous one");

        //    _dietUnits.Add(DietPlanUnit.NewDraft(newId));

        //    AvgDailyCalories = GetAvgDailyCalories(_dietUnits);
        //    PeriodScheduled = GetPlanPeriod(_dietUnits);
        //}


        /// <summary>
        /// Create a new Diet Plan Unit Draft and schedules it after the last one
        /// </summary>
        /// <param name="upTo">The last day of the new unit</param>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        /// <exception cref="InvalidOperationException">Thrown if trying to add a new unit before the ones before have been scheduled</exception>
        public void AppendDietPlanUnitDraft(DateTime upTo)
        {
            IdTypeValue newId = BuildDietPlanUnitId();

            if (PeriodScheduled?.End == null || PeriodScheduled.End == DateTime.MaxValue)
                throw new InvalidOperationException($"Can't add a new Diet Plan Unit before closing the previous one");

            _dietUnits.Add(
                 DietPlanUnit.NewScheduledDraft(newId, DateRangeValue.RangeBetween(PeriodScheduled.End.AddDays(1), upTo))
                 );

            DietPlanUnit previous = FindUnitById(newId - 1);

            AvgDailyCalories = GetAvgDailyCalories(_dietUnits);
            PeriodScheduled = GetPlanPeriod(_dietUnits);
        }


        /// <summary>
        /// Assigns a schedule to the specified DietPlanUnit - which must already be created
        /// </summary>
        /// <param name="planUnitId">The ID of the Diet Plan Unit to be scheduled</param>
        /// <param name="startingFrom">The left boundary date</param>
        /// <param name="upTo">The right boundary date</param>
        /// <exception cref="ArgumentException">If the ID specified is not between the Plan DietUnits</exception>
        /// <exception cref="DietDomainIvariantViolationException">If business ruels violated</exception>
        public void CloseDietPlanUnit(IdTypeValue planUnitId, DateTime startingFrom, DateTime upTo)
        {
            DietPlanUnit toBeScheduled = FindUnitById(planUnitId);

            if (toBeScheduled == default)
                throw new ArgumentException($"The Diet Plan Unit - Id={planUnitId.ToString()} - does not belong to the Diet Plan");

            toBeScheduled.Reschedule(DateRangeValue.RangeBetween(startingFrom, upTo));

            // If the previous unit is opened - Infinite DateRange - close it
            // Should never happen
            DietPlanUnit unit = _dietUnits.FirstOrDefault(x => x != toBeScheduled && !x.PeriodScheduled.IsRightBounded());
            unit?.RescheduleEndDate(startingFrom.AddDays(-1));

            FinalizeDietPlanUnitsAdded();
        }


        /// <summary>
        /// Schedule the start of the specified DietPlanUnit - which must already be created
        /// </summary>
        /// <param name="planUnitId">The ID of the Diet Plan Unit to be scheduled</param>
        /// <param name="startingFrom">The left boundary date</param>
        /// <exception cref="ArgumentException">If the ID specified is not between the Plan DietUnits</exception>
        /// <exception cref="DietDomainIvariantViolationException">If business ruels violated</exception>
        public void CloseDietPlanUnitToInfinite(IdTypeValue planUnitId, DateTime startingFrom) => CloseDietPlanUnit(planUnitId, startingFrom, DateTime.MaxValue);


        /// <summary>
        /// Assigns a schedule to the specified DietPlanUnit - which must already be created
        /// The function requires the start date to be already scheduled.
        /// </summary>
        /// <param name="planUnitId">The ID of the Diet Plan Unit to be scheduled</param>
        /// <param name="startingFrom">The left boundary date</param>
        /// <exception cref="ArgumentException">If the ID specified is not between the Plan DietUnits</exception>
        /// <exception cref="DietDomainIvariantViolationException">If business ruels violated</exception>
        public void CloseDietPlanUnit(IdTypeValue planUnitId, DateTime upTo)
        {
            DietPlanUnit toBeScheduled = FindUnitById(planUnitId);

            if (FindUnitById(planUnitId) == default)
                throw new ArgumentException($"The Diet Plan Unit - Id={planUnitId.ToString()} - does not belong to the Diet Plan");

            if (toBeScheduled?.PeriodScheduled?.Start == null)
                throw new DietDomainIvariantViolationException($"Cannot close a DietPlanUnit which has no start period.");

            CloseDietPlanUnit(planUnitId, toBeScheduled.PeriodScheduled.Start, upTo);
        } 


        /// <summary>
        /// Remove the selected diet plan unit
        /// </summary>
        /// <param name="toRemoveId">The Diet Plan Unit to be removed</param>
        /// <exception cref="ArgumentException">Thrown if unit not found</exception>
        public void UnscheduleDietPlanUnit(IdTypeValue toRemoveId)
        {
            DietPlanUnit toBeRemoved = FindUnitById(toRemoveId);

            if (_dietUnits.Remove(toBeRemoved))
            {
                AvgDailyCalories = GetAvgDailyCalories(_dietUnits);
                PeriodScheduled = GetPlanPeriod(_dietUnits);
            }
            else
                throw new ArgumentException($"The Diet Plan Unit - Id={toRemoveId.ToString()} - does not belong to the Diet Plan");


            FinalizeDietPlanUnitsChanged();

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
        public void MoveDietPlanUnit(IdTypeValue toMoveId, DateRangeValue newPeriod)
        {
            DietPlanUnit toBeMoved = FindUnitById(toMoveId);

            if (toBeMoved == default)
                throw new ArgumentException($"The Diet Plan Unit - Id={toMoveId.ToString()} - does not belong to the Diet Plan");

            toBeMoved.Reschedule(newPeriod);

            _dietUnits = MakeUnitsContiguous(_dietUnits, toMoveId);
            TestDietUnitsBusinessRules();
        }


        /// <summary>
        /// Assigns the diet plan units
        /// </summary>
        /// <param name="dietUnits">The diet plan units</param>
        /// <exception cref="DietDomainIvariantViolationException">Thrown when invalid state</exception>
        public void AssignDietUnits(ICollection<DietPlanUnit> dietUnits)
        {
            _dietUnits = dietUnits.Clone().ToList();
            FinalizeDietPlanUnitsAdded();
        }

        /// <summary>
        /// Find the Diet Plan Unit with the ID specified
        /// </summary>
        /// <param name="id">The Id to be found</param>
        /// <returns>The DietPlanUnit or DEFAULT if not found/returns>
        public DietPlanUnit FindUnitById(IdTypeValue id) => _dietUnits.Where(x => x.Id == id).FirstOrDefault();


        /// <summary>
        /// Get the last scheduled diet plan unit - chronologically, which might differ from the diet units list order.
        /// </summary>
        public DietPlanUnit GetLastScheduledDietPlanUnit()
        {
            var maxIndex =
                (
                  from x
                  in _dietUnits
                  orderby x.PeriodScheduled.End
                  select _dietUnits.Where(u => u.PeriodScheduled?.End != null).ToList().IndexOf(x)
                ).Last();

            return _dietUnits.ToList()[maxIndex];
        }


        /// <summary>
        /// Get the first scheduled diet plan unit - chronologically, which might differ from the diet units list order.
        /// </summary>
        public DietPlanUnit GetFirstScheduledDietPlanUnit()
        {
            var minIndex =
                (
                  from x
                  in _dietUnits
                  orderby x.PeriodScheduled.End
                  select _dietUnits.Where(u => u.PeriodScheduled?.End != null).ToList().IndexOf(x)
                ).First();

            return _dietUnits.ToList()[minIndex];
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Finalization step for consolidating a change/deletion in the Diet Plan Unit list.
        /// It differs from the *Added version by forcing the hte units to be sorted
        /// </summary>
        private void FinalizeDietPlanUnitsChanged()
        {
            _dietUnits = MakeUnitsContiguous(_dietUnits);

            FinalizeDietPlanUnitsAdded();
        }


        /// <summary>
        /// Finalization step for consolidating an addition in the Diet Plan Unit list.
        /// It performs common actions and tests  the business logic
        /// </summary>
        private void FinalizeDietPlanUnitsAdded()
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
            CalorieValue calories = CalorieValue.MeasureKcal(units.Where(unit => unit.DietDays.Count > 0)   // Skip draft units
                .Sum(x => x.AvgDailyCalories.Value) / (float)_dietUnits.Count);

            //if (calories == null)
            //    throw new DietDomainIvariantViolationException($"Invalid Diet Days linked to the Diet Unit {Id.ToString()}");yj5ye

            return calories;
        }

        /// <summary>
        /// Get the period whitch the Diet Plan is scheduled to, according to the non-draft Diet Units
        /// </summary>
        /// <param name="units">The diet units</param>
        /// <returns>The DateRangeValue representing the scheduled period</returns>
        private DateRangeValue GetPlanPeriod(IEnumerable<DietPlanUnit> units)
        {
            IEnumerable<DietPlanUnit> nonDraftUnits = units.Where(unit => unit.DietDays.Count > 0 && unit.PeriodScheduled != null);

            return DateRangeValue.RangeBetween(
                nonDraftUnits.Select(x => x.PeriodScheduled.Start).Min(),
                nonDraftUnits.Select(x => x.PeriodScheduled.End).Max());       // End = DateTime.MaxValue() if one unit end date is not set
        }


        /// <summary>
        /// Build the next valid Diet Plan Unit id
        /// </summary>
        /// <returns>The Diet Plan Unit Id</returns>
        private IdTypeValue BuildDietPlanUnitId()
        {
            if (_dietUnits.Count == 0)
                return new IdTypeValue(1); 

            else
                return new IdTypeValue(_dietUnits.Max(x => x.Id.Id) + 1);
        }


        /// <summary>
        /// Get the Start Period -sorted list of diet plan units
        /// </summary>
        /// <param name="units">The list to be sorted</param>
        /// <returns>The sorted list</returns>
        private IList<DietPlanUnit> GetSortedUnits(ICollection<DietPlanUnit> units) 
            
            => _dietUnits.Where(x => x.PeriodScheduled?.Start != null).OrderBy(x => x.PeriodScheduled.Start).ToList();


        /// <summary>
        /// Modify the the DietPlanUnits list so that each element is chronologically contiguous
        /// </summary>where each element is chronologically contiguous
        /// <param name="units">The units to be shuffled</param>
        /// <returns>The shiuffled list</returns>
        private ICollection<DietPlanUnit> MakeUnitsContiguous(ICollection<DietPlanUnit> units)
        {
            IList<DietPlanUnit> sorted = GetSortedUnits(_dietUnits);

            for (int i = 0; i < sorted.Count() - 1; i++) // Skip the last one
            {
                DietPlanUnit current = sorted[i];
                DietPlanUnit nextOne = sorted[i + 1];

                nextOne.Reschedule(DateRangeValue.RangeBetween(
                    current.PeriodScheduled.End.AddDays(1),
                    current.PeriodScheduled.End.AddDays(nextOne.PeriodScheduled.GetLength())));
            }

            return units;
        }


        /// <summary>
        /// Modify all the the DietPlanUnits but the moved one so that each element is chronologically contiguous
        /// The moved unit will not be modifed, as the user forced it,
        /// </summary>where each element is chronologically contiguous
        /// <param name="units">The units to be shuffled</param>
        /// <param name="movedId">The DietPlanUnit Id which has been moved</param>
        /// <returns>The shiuffled list</returns>
        private ICollection<DietPlanUnit> MakeUnitsContiguous(ICollection<DietPlanUnit> units, IdTypeValue movedId)
        {
            IList<DietPlanUnit> sorted = GetSortedUnits(_dietUnits);

            for (int i = 0; i < sorted.Count() - 1; i++) // Skip the last one
            {
                DietPlanUnit current = sorted[i];
                DietPlanUnit nextOne = sorted[i + 1];

                if(nextOne.Id == movedId)
                {
                    // Reschedule the current one according to the next one
                    current.Reschedule(DateRangeValue.RangeBetween(
                        current.PeriodScheduled.Start,
                        nextOne.PeriodScheduled.Start.AddDays(-1)));
                }
                else
                {
                    // Reschedule the next one according to the current one
                    nextOne.Reschedule(DateRangeValue.RangeBetween(
                        current.PeriodScheduled.End.AddDays(1),
                        current.PeriodScheduled.End.AddDays(nextOne.PeriodScheduled.GetLength())));
                }
            }

            return units;
        }
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
            IList<DietPlanUnit> sorted = GetSortedUnits(_dietUnits);

            for(int i = 0; i< sorted.Count() - 1; i++)      // Skip last unit
            {
                DietPlanUnit current = sorted[i];
                DietPlanUnit next = sorted[i + 1];

                if (current.PeriodScheduled.Overlaps(next.PeriodScheduled))
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
            if (_dietUnits.Count <= 1)
                return true;

            IList<DietPlanUnit> sorted = GetSortedUnits(_dietUnits);

            for (int i = 0; i < sorted.Count() - 1; i++)      // Skip last unit
            {
                DietPlanUnit current = sorted[i];
                DietPlanUnit next = sorted[i + 1];

                if (current.PeriodScheduled.End != next.PeriodScheduled.Start.AddDays(-1))
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
        /// The Diet Plan Units must have unique Ids inside the Diet Plan
        /// </summary>
        /// <returns>True if the rule is not violated</returns>
        private bool DietPlanUnitsHaveUniqueIds() => _dietUnits.Any(x => x.Id == null) || _dietUnits.GroupBy(x => x.Id).All(g => g.Count() == 1);


        /// <summary>
        /// The Diet Days of each Diet Unit must cover all the week.
        /// </summary>
        /// <returns>True if the rule is not violated</returns>
        private bool DietPlanUnitsHaveOneDayForEachWeekday() => _dietUnits.All(unit => unit.DietDays.Sum(day => day.WeeklyOccurrances.Value) == WeekdayEnum.AllTheWeek);


        /// <summary>
        /// Test the Diet Plan Units business rules and manages invalid states
        /// </summary>
        /// <exception cref="DietDomainIvariantViolationException">Thrown if business rules are broken</exception>
        private void TestDietUnitsBusinessRules()
        {
            if (!DietPlanUnitsHaveUniqueIds())
                throw new DietDomainIvariantViolationException($"The Diet Plan Units must have unique Ids inside the Diet Plan.");

            if (!DietPlanUnitsNotEmpty())
                throw new DietDomainIvariantViolationException($"The Diet Plan must have at least one Unit");

            if (!DietPlanUnitsNotOverlapping())
                throw new DietDomainIvariantViolationException($"he Diet Plan Units must not overlap each other.");

            if (!DietPlanUnitsAreContiguous())
                throw new DietDomainIvariantViolationException($"The Diet Plan Units must be contiguous.");

            if (!DietPlanUnitsHaveAtLeastOneDay())
                throw new DietDomainIvariantViolationException($"The Diet Plan Units must have at least one Diet Day.");

            if (!DietPlanUnitsHaveOneDayForEachWeekday())
                throw new DietDomainIvariantViolationException($"The Diet Days of each Diet Unit must cover all the week.");
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
            yield return Trainer;
            yield return OwnerNote;
            yield return PeriodScheduled;
            yield return WeeklyFreeMeals;
            yield return DietUnits;
            yield return AvgDailyCalories;
            yield return PostId;
        }

    }
}
