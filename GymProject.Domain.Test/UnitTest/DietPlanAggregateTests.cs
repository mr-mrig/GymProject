using GymProject.Domain.Base;
using GymProject.Domain.DietDomain;
using GymProject.Domain.DietDomain.DietPlanAggregate;
using GymProject.Domain.DietDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using GymProject.Domain.Utils;
using Xunit;

namespace GymProject.Domain.Test.UnitTest
{
    public class DietPlanAggregateTests
    {


        [Fact]
        public void GetFirstUnit()
        {
            IdType unitId = new IdType(1);

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");

            DietPlan plan = DietPlan.NewDraft(owner, dest);

            // New Unit
            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100));
            plan.CloseDietPlanUnit(unitId, DateTime.Today.AddDays(11), DateTime.Today.AddDays(21));

            // New Unit
            unitId = unitId + 1;
            plan.AppendDietPlanUnitDraft();
            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(50), MacronutirentWeightValue.MeasureGrams(50), MacronutirentWeightValue.MeasureGrams(50));
            plan.CloseDietPlanUnit(unitId, DateTime.Today, DateTime.Today.AddDays(10));

            DietPlanUnit firstOne = plan.GetFirstScheduledDietPlanUnit();

            Assert.Equal(plan.FindUnitById(unitId), firstOne);       // The first one is the first in the list
        }


        [Fact]
        public void GetFirstUnitWithUnbounded()
        {
            IdType unitId = new IdType(1);

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");

            DietPlan plan = DietPlan.NewDraft(owner, dest);

            // New Unit
            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100));
            plan.CloseDietPlanUnit(unitId, DateTime.Today, DateTime.Today.AddDays(10));

            // New Unit
            unitId = unitId + 1;
            plan.AppendDietPlanUnitDraft();
            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(50), MacronutirentWeightValue.MeasureGrams(50), MacronutirentWeightValue.MeasureGrams(50));
            plan.CloseDietPlanUnit(unitId, DateTime.Today.AddDays(11));

            DietPlanUnit firstone = plan.GetFirstScheduledDietPlanUnit();

            Assert.Equal(plan.FindUnitById(unitId - 1), firstone);
        }


        [Fact]
        public void GetFirstUnitWithOnlyOneUnit()
        {
            IdType unitId = new IdType(1);

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");

            DietPlan plan = DietPlan.NewDraft(owner, dest);

            // New Unit
            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100));
            plan.CloseDietPlanUnitToInfinite(unitId, DateTime.Today.AddDays(11));

            DietPlanUnit firstone = plan.GetFirstScheduledDietPlanUnit();
            Assert.Equal(plan.FindUnitById(unitId), firstone);       // The first one is the first in the list
        }


        [Fact]
        public void GetLastUnit()
        {
            IdType unitId = new IdType(1);

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");

            DietPlan plan = DietPlan.NewDraft(owner, dest);

            // New Unit
            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100));
            plan.CloseDietPlanUnit(unitId, DateTime.Today.AddDays(11), DateTime.Today.AddDays(21));

            // New Unit
            unitId = unitId + 1;
            plan.AppendDietPlanUnitDraft();
            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(50), MacronutirentWeightValue.MeasureGrams(50), MacronutirentWeightValue.MeasureGrams(50));
            plan.CloseDietPlanUnit(unitId, DateTime.Today, DateTime.Today.AddDays(10));

            DietPlanUnit lastOne = plan.GetLastScheduledDietPlanUnit();

            Assert.Equal(plan.FindUnitById(unitId - 1), lastOne);       // The last one is the first in the list
        }


        [Fact]
        public void GetLastUnitWithUnbounded()
        {
            IdType unitId = new IdType(1);

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");

            DietPlan plan = DietPlan.NewDraft(owner, dest);

            // New Unit
            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100));
            plan.CloseDietPlanUnit(unitId, DateTime.Today, DateTime.Today.AddDays(10));

            // New Unit
            unitId = unitId + 1;
            plan.AppendDietPlanUnitDraft();
            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(50), MacronutirentWeightValue.MeasureGrams(50), MacronutirentWeightValue.MeasureGrams(50));
            plan.CloseDietPlanUnit(unitId, DateTime.Today.AddDays(11));

            DietPlanUnit lastOne = plan.GetLastScheduledDietPlanUnit();

            Assert.Equal(plan.FindUnitById(unitId), lastOne);
        }


        [Fact]
        public void GetLastUnitWithOnlyOneUnit()
        {
            IdType unitId = new IdType(1);

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");

            DietPlan plan = DietPlan.NewDraft(owner, dest);

            // New Unit
            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100));
            plan.CloseDietPlanUnitToInfinite(unitId, DateTime.Today.AddDays(11));

            DietPlanUnit lastOne = plan.GetLastScheduledDietPlanUnit();
            Assert.Equal(plan.FindUnitById(unitId), lastOne);       // The last one is the first in the list
        }

        [Fact]
        public void DietPlanNullUnitsFail()
        {
            IdType id = new IdType(1);
            string name = "My Plan";

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");

            PersonalNoteValue note = PersonalNoteValue.Write("my note.");
            WeeklyOccuranceValue freeMeals = WeeklyOccuranceValue.TrackOccurance(2);

            Assert.Throws<DietDomainIvariantViolationException>(() => DietPlan.ScheduleFullDietPlan(id, name, owner, dest, null, note, freeMeals));
        }

        [Fact]
        public void DietPlanEmptyUnitsFail()
        {
            IdType id = new IdType(1);
            string name = "My Plan";

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");

            PersonalNoteValue note = PersonalNoteValue.Write("my note.");
            WeeklyOccuranceValue freeMeals = WeeklyOccuranceValue.TrackOccurance(2);

            List<DietPlanUnit> units = new List<DietPlanUnit>();

            Assert.Throws<DietDomainIvariantViolationException>(() => DietPlan.ScheduleFullDietPlan(id, name, owner, dest, units, note, freeMeals));
        }

        [Fact]
        public void DietPlanNewDraft()
        {
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            List<DietPlanUnit> units = plan.DietUnits.ToList();

            Assert.NotNull(plan);
            Assert.Equal(owner, plan.Trainer);
            Assert.Equal(string.Empty, plan.Name);

            Assert.Single(units);
            Assert.NotNull(units);
            Assert.Empty(units.FirstOrDefault().DietDays);

            Assert.Null(plan.OwnerNote);
            Assert.Null(plan.PeriodScheduled);
            Assert.Null(plan.PostId);
            Assert.Null(plan.WeeklyFreeMeals);
            Assert.Null(plan.AvgDailyCalories);
        }

        [Fact]
        public void DietPlanChangeDraft()
        {
            IdType postId = new IdType(11);
            PersonalNoteValue note = PersonalNoteValue.Write("note.");
            PersonalNoteValue note2 = PersonalNoteValue.Write("Final note.");
            WeeklyOccuranceValue fmeals = WeeklyOccuranceValue.TrackOccurance(1);
            WeeklyOccuranceValue fmeals2 = WeeklyOccuranceValue.TrackOccurance(2);
            string name = "name";
            string name2 = "final name";

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);
            
            plan.WriteOwnerNote(note);
            plan.WriteOwnerNote(note2);
            plan.GiveName(name);
            plan.GiveName(name2);
            plan.GrantFreeMeals(fmeals);
            plan.GrantFreeMeals(fmeals2);
            plan.LinkToPost(postId);

            List<DietPlanUnit> units = plan.DietUnits.ToList();

            Assert.NotNull(plan);
            Assert.Equal(owner, plan.Trainer);
            Assert.Equal(name2, plan.Name);
            Assert.Equal(note2, plan.OwnerNote);
            Assert.Equal(fmeals2, plan.WeeklyFreeMeals);
            Assert.Equal(postId, plan.PostId);

            Assert.Single(units);
            Assert.NotNull(units);
            Assert.Empty(units.FirstOrDefault().DietDays);

            Assert.Null(plan.PeriodScheduled);
            Assert.Null(plan.AvgDailyCalories);
        }

        [Fact]
        public void DietPlanChangeDayNotFoundFail()
        {
            IdType dayId = new IdType(1);
            IdType unitId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);


            DietPlanDay day1 = DietPlanDay.AddDayToPlan(
                unitId, "", WeeklyOccuranceValue.TrackOccurance(7), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                MacronutirentWeightValue.MeasureGrams(100));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            Assert.Throws<ArgumentException>(() =>
            plan.ChangeDietDay(unitId, dayId + 100, MacronutirentWeightValue.MeasureGrams(500), MacronutirentWeightValue.MeasureGrams(100),
                MacronutirentWeightValue.MeasureGrams(100), weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(5)));
        }

        [Fact]
        public void DietPlanChangeUnitNotFoundFail()
        {
            IdType dayId = new IdType(1);
            IdType unitId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);


            DietPlanDay day1 = DietPlanDay.AddDayToPlan(
                unitId, "", WeeklyOccuranceValue.TrackOccurance(7), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                MacronutirentWeightValue.MeasureGrams(100));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            Assert.Throws<ArgumentException>(() =>
            plan.ChangeDietDay(unitId + 100, dayId, MacronutirentWeightValue.MeasureGrams(500), MacronutirentWeightValue.MeasureGrams(100),
                MacronutirentWeightValue.MeasureGrams(100), weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(5)));
        }

        [Fact]
        public void DietPlanChangeInvalidDayFail()
        {
            IdType dayId = new IdType(1);
            IdType unitId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);


            DietPlanDay day1 = DietPlanDay.AddDayToPlan(
                unitId, "", WeeklyOccuranceValue.TrackOccurance(7), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                MacronutirentWeightValue.MeasureGrams(100));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            Assert.Throws<DietDomainIvariantViolationException>(() =>
            plan.ChangeDietDay(unitId, dayId, MacronutirentWeightValue.MeasureGrams(500), MacronutirentWeightValue.MeasureGrams(100),
                MacronutirentWeightValue.MeasureGrams(100), weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(5)));
        }


        [Fact]
        public void DietPlanChangeDay()
        {
            IdType dayId = new IdType(1);
            IdType unitId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);


            DietPlanDay day1 = DietPlanDay.AddDayToPlan(
                unitId, "", WeeklyOccuranceValue.TrackOccurance(2), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                MacronutirentWeightValue.MeasureGrams(100));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(5));

            DietPlanDay day2 = DietPlanDay.AddDayToPlan(
                unitId, "", WeeklyOccuranceValue.TrackOccurance(5), MacronutirentWeightValue.MeasureGrams(500), MacronutirentWeightValue.MeasureGrams(100),
                MacronutirentWeightValue.MeasureGrams(100), dayType: DietDayTypeEnum.On);

            dayId = dayId + 1;

            // Edit the 2nd element
            plan.ChangeDietDay(unitId, dayId, MacronutirentWeightValue.MeasureGrams(500), MacronutirentWeightValue.MeasureGrams(100),
                MacronutirentWeightValue.MeasureGrams(100), dayType: DietDayTypeEnum.On, weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(5));

            Assert.Equal(2, plan.FindUnitById(unitId).DietDays.Count);
            Assert.NotEqual(day1.Carbs, plan.FindUnitById(unitId).FindDayById(dayId).Carbs);
            Assert.Equal(day2.Carbs, plan.FindUnitById(unitId).FindDayById(dayId).Carbs);
            Assert.NotEqual(day1.DietDayType, plan.FindUnitById(unitId).FindDayById(dayId).DietDayType);
            Assert.Equal(day2.DietDayType, plan.FindUnitById(unitId).FindDayById(dayId).DietDayType);

            // Check 1st eelemnt
            Assert.Equal(day1.DietDayType, plan.FindUnitById(unitId).FindDayById(dayId - 1).DietDayType);
            Assert.NotEqual(day2.DietDayType, plan.FindUnitById(unitId).FindDayById(dayId - 1).DietDayType);
        }


        [Fact]
        public void DietPlanDraftNoWeeklyOccurrancesDayUseDefault()
        {
            IdType unitId = new IdType(1);

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            // Doesn't throw
            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(300), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                    weeklyOccurrances: null, dayType: null, specificWeekday: WeekdayEnum.Tuesday);
        }


        [Fact]
        public void DietPlanDraftExceedingOccurrancesDayFail()
        {
            IdType unitId = new IdType(1);

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            Assert.Throws<ValueObjectInvariantViolationException>(()
                => plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(300), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                    weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(11), dayType: null));
        }


        [Fact]
        public void DietPlanDraftMissingMacrosDayFail()
        {
            IdType unitId = new IdType(1);

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            Assert.Throws<DietDomainIvariantViolationException>(()
                => plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(0), MacronutirentWeightValue.MeasureGrams(0), MacronutirentWeightValue.MeasureGrams(0),
                    weeklyOccurrances: null, dayType: null));
        }


        [Fact]
        public void DietPlanDraftNullMacrosDayFail()
        {
            IdType unitId = new IdType(1);

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            Assert.Throws<DietDomainIvariantViolationException>(()
                => plan.PlanDietDay(unitId, null, null, null, weeklyOccurrances: null, dayType: null));
        }


        [Fact]
        public void DietPlanConsolidateConsolidateOneDay()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: null, dayType: DietDayTypeEnum.NotSet);

            plan.FinalizePlan(new IdType(1));    // Doesn't throw

            Assert.Equal(WeeklyOccuranceValue.MaximumTimes, plan.FindUnitById(unitId).FindDayById(dayId).WeeklyOccurrances.Value);
        }


        [Fact]
        public void DietPlanAddDaysExceedingWeekdaysFail()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                specificWeekday: WeekdayEnum.Friday);  // Days occurance = 1

            Assert.Equal(1, plan.FindUnitById(unitId).FindDayById(dayId).WeeklyOccurrances.Value);

            dayId = dayId + 1;

            Assert.Throws<DietDomainIvariantViolationException>(() =>
            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: null, dayType: DietDayTypeEnum.NotSet));  // Days occurance = 7
        }


        [Fact]
        public void DietPlanAddDayToWrongUnitFail()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            // Add to non-existant Unit
            Assert.Throws<ArgumentException>(() =>
                plan.PlanDietDay(unitId + 1, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                    specificWeekday: WeekdayEnum.Friday));
        }


        [Fact]
        public void ConsolidateWithOneUnitFullCase()
        {
            int dayId, carbs, proteins, fats, salt, water;
            DietDayTypeEnum dayType;
            IdType unitId;

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            unitId = new IdType(1);

            List<DietPlanUnit> units = plan.DietUnits.ToList();
            DietPlanUnit unit = plan.FindUnitById(unitId);

            // Day 1
            dayId = 1;
            carbs = 300;
            fats = 70;
            proteins = 170;
            dayType = DietDayTypeEnum.On;

            DietPlanDay day1 = DietPlanDay.AddDayToPlan(
                new IdType(dayId), "", WeeklyOccuranceValue.TrackOccurance(2), MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats),
                MacronutirentWeightValue.MeasureGrams(proteins), dayType: DietDayTypeEnum.On);

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats), MacronutirentWeightValue.MeasureGrams(proteins), 
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2), dayType: dayType);

            Assert.Equal(day1, unit.FindDayById(new IdType(dayId)));


            // Day 2
            dayId = 2;
            carbs = 300;
            fats = 70;
            proteins = 170;
            dayType = DietDayTypeEnum.Off;

            DietPlanDay day2 = DietPlanDay.AddDayToPlan(
                new IdType(dayId), "", WeeklyOccuranceValue.TrackOccurance(2), MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats),
                MacronutirentWeightValue.MeasureGrams(proteins), dayType: DietDayTypeEnum.On);

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats), MacronutirentWeightValue.MeasureGrams(proteins),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2), dayType: dayType);

            Assert.Equal(day2, unit.FindDayById(new IdType(dayId)));


            // Day 3
            dayId = 3;
            carbs = 100;
            fats = 120;
            salt = 2;
            water = 3;
            proteins = 170;
            dayType = null;

            DietPlanDay day3 = DietPlanDay.AddDayToPlan(
                new IdType(dayId), "", WeeklyOccuranceValue.TrackOccurance(3), MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats),
                MacronutirentWeightValue.MeasureGrams(proteins), dayType: DietDayTypeEnum.On);

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats), MacronutirentWeightValue.MeasureGrams(proteins),
                dayType: dayType, weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(3), salt: MicronutirentWeightValue.MeasureGrams(salt), water: VolumeValue.MeasureLiters(water));

            Assert.Equal(day3, unit.FindDayById(new IdType(dayId)));


            // Schedule Unit and reschedule it
            plan.CloseDietPlanUnitToInfinite(unitId, DateTime.Today);
            Assert.Equal(DateRangeValue.RangeStartingFrom(DateTime.Today), plan.PeriodScheduled);

            int days = 100;
            plan.CloseDietPlanUnit(unitId, DateTime.Today, DateTime.Today.AddDays(days));
            Assert.Equal(DateRangeValue.RangeBetween(DateTime.Today, DateTime.Today.AddDays(days)), plan.PeriodScheduled);
            Assert.Equal(days + 1, plan.PeriodScheduled.GetLength());

            // Change aggregate root
            PersonalNoteValue note = PersonalNoteValue.Write("My note.");
            plan.WriteOwnerNote(note);


            // Other
            CalorieValue targetCal = CalorieValue.MeasureKcal((day1.Calories.Value * day1.WeeklyOccurrances.Value
                + day2.Calories.Value * day2.WeeklyOccurrances.Value + day3.Calories.Value * day3.WeeklyOccurrances.Value) / 7f);

            Assert.Equal(3, unit.DietDays.Count);
            Assert.Equal(string.Empty, plan.Name);
            Assert.Single(units);
            Assert.Equal(note, plan.OwnerNote);
            Assert.Null(plan.PostId);
            Assert.Null(plan.WeeklyFreeMeals);

            Assert.Equal(targetCal, plan.AvgDailyCalories);

            plan.FinalizePlan(new IdType(1));
        }


        [Fact]
        public void DietPlanConsolidateNoIdFail()
        {
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            Assert.Throws<DietDomainIvariantViolationException>(() => plan.FinalizePlan(null));
        }


        [Fact]
        public void DietPlanConsolidateInvalidUnitsFail()
        {
            IdType postId = new IdType(11);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            Assert.Throws<DietDomainIvariantViolationException>(() => plan.FinalizePlan(postId));
        }


        [Fact]
        public void DietPlanConsolidateInvalidUnitsFail2()
        {
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            plan.PlanDietDay(new IdType(1), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2), dayType: null);

            Assert.Throws<DietDomainIvariantViolationException>(() => plan.FinalizePlan(new IdType(1)));
        }


        [Fact]
        public void DietPlanConsolidateInvalidUnitsFail3()
        {
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            plan.PlanDietDay(new IdType(1), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2), dayType: DietDayTypeEnum.NotSet);

            Assert.Throws<DietDomainIvariantViolationException>(() => plan.FinalizePlan(new IdType(1)));
        }


        [Fact]
        public void ConsolidateWithTwoUnitsFullCase()
        {
            int dayId, carbs, proteins, fats, salt, water;
            DietDayTypeEnum dayType;
            IdType unitId;

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            unitId = new IdType(1);


            // Day 1
            dayId = 1;
            carbs = 300;
            fats = 70;
            proteins = 170;
            dayType = DietDayTypeEnum.On;

            DietPlanDay day1 = DietPlanDay.AddDayToPlan(
                new IdType(dayId), "", WeeklyOccuranceValue.TrackOccurance(2), MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats),
                MacronutirentWeightValue.MeasureGrams(proteins), dayType: dayType);

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats), MacronutirentWeightValue.MeasureGrams(proteins),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2), dayType: dayType);


            // Day 2
            dayId = 2;
            carbs = 300;
            fats = 70;
            proteins = 170;
            dayType = DietDayTypeEnum.Off;

            DietPlanDay day2 = DietPlanDay.AddDayToPlan(
                new IdType(dayId), "", WeeklyOccuranceValue.TrackOccurance(2), MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats),
                MacronutirentWeightValue.MeasureGrams(proteins), dayType: dayType);

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats), MacronutirentWeightValue.MeasureGrams(proteins),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2), dayType: dayType);


            // Day 3
            dayId = 3;
            carbs = 100;
            fats = 120;
            salt = 2;
            water = 3;
            proteins = 170;
            dayType = DietDayTypeEnum.On;

            DietPlanDay day3 = DietPlanDay.AddDayToPlan(
                new IdType(dayId), "", WeeklyOccuranceValue.TrackOccurance(3), MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats),
                MacronutirentWeightValue.MeasureGrams(proteins), dayType: dayType);

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats), MacronutirentWeightValue.MeasureGrams(proteins),
                dayType: dayType, weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(3), salt: MicronutirentWeightValue.MeasureGrams(salt), water: VolumeValue.MeasureLiters(water));

            // Schedule Unit
            DateTime left = DateTime.Today;
            DateTime right = left.AddDays(10);
            plan.CloseDietPlanUnit(unitId, left, right);

            CalorieValue calUnit1 = CalorieValue.MeasureKcal
                ((CalorieValue.MeasureKcal(DietAmountsCalculator.ComputeCalories(day1.Carbs, day1.Fats, day1.Proteins).Value * day1.WeeklyOccurrances.Value)
                + CalorieValue.MeasureKcal(DietAmountsCalculator.ComputeCalories(day2.Carbs, day2.Fats, day2.Proteins).Value * day2.WeeklyOccurrances.Value)
                + CalorieValue.MeasureKcal(DietAmountsCalculator.ComputeCalories(day3.Carbs, day3.Fats, day3.Proteins).Value * day3.WeeklyOccurrances.Value))
                .Value / (float)WeekdayEnum.AllTheWeek);

            Assert.Equal(calUnit1, plan.GetLastScheduledDietPlanUnit().AvgDailyCalories);

            // Append new unit
            plan.AppendDietPlanUnitDraft();

            // Day 4
            unitId = unitId + 1;
            dayId = 1;
            carbs = 700;
            fats = 120;
            salt = 2;
            water = 3;
            proteins = 170;
            dayType = DietDayTypeEnum.On;

            DietPlanDay day4 = DietPlanDay.AddDayToPlan(
                new IdType(dayId), "", WeeklyOccuranceValue.TrackOccurance(7), MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats),
                MacronutirentWeightValue.MeasureGrams(proteins), dayType: dayType);

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats), MacronutirentWeightValue.MeasureGrams(proteins),
                dayType: dayType, weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7), salt: MicronutirentWeightValue.MeasureGrams(salt), water: VolumeValue.MeasureLiters(water));

            // Schedule Unit
            DateTime left2 = right.AddDays(1);
            DateTime right2 = left2.AddDays(10);
            plan.CloseDietPlanUnit(unitId, left2, right2);

            CalorieValue calUnit2 = CalorieValue.MeasureKcal
                ((CalorieValue.MeasureKcal(DietAmountsCalculator.ComputeCalories(day4.Carbs, day4.Fats, day4.Proteins).Value * day4.WeeklyOccurrances.Value).Value / (float)WeekdayEnum.AllTheWeek));

            Assert.Equal(calUnit2, plan.GetLastScheduledDietPlanUnit().AvgDailyCalories);

            // Append new unit
            plan.AppendDietPlanUnitDraft();

            // Day 5 -> New unit
            unitId = unitId + 1;
            dayId = 1;
            carbs = 300;
            fats = 100;
            salt = 2;
            water = 3;
            proteins = 170;
            dayType = DietDayTypeEnum.Refeed;

            DietPlanDay day5 = DietPlanDay.AddDayToPlan(
                new IdType(dayId), "", WeeklyOccuranceValue.TrackOccurance(5), MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats),
                MacronutirentWeightValue.MeasureGrams(proteins), dayType: dayType);

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats), MacronutirentWeightValue.MeasureGrams(proteins),
                dayType: dayType, weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(5), salt: MicronutirentWeightValue.MeasureGrams(salt), water: VolumeValue.MeasureLiters(water));

            // Day 6
            dayId = dayId + 1;
            carbs = 700;
            fats = 100;
            salt = 2;
            water = 3;
            proteins = 170;
            dayType = DietDayTypeEnum.Refeed;

            DietPlanDay day6 = DietPlanDay.AddDayToPlan(
                new IdType(dayId), "", WeeklyOccuranceValue.TrackOccurance(2), MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats),
                MacronutirentWeightValue.MeasureGrams(proteins), dayType: dayType);

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats), MacronutirentWeightValue.MeasureGrams(proteins),
                dayType: dayType, weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2), salt: MicronutirentWeightValue.MeasureGrams(salt), water: VolumeValue.MeasureLiters(water));


            // Schedule Unit
            DateTime left3 = right2.AddDays(1);
            DateTime right3 = left3.AddDays(10);
            plan.CloseDietPlanUnit(unitId, left3, right3);

            CalorieValue calUnit3 = CalorieValue.MeasureKcal
                ((CalorieValue.MeasureKcal(DietAmountsCalculator.ComputeCalories(day5.Carbs, day5.Fats, day5.Proteins).Value * day5.WeeklyOccurrances.Value)
                + CalorieValue.MeasureKcal(DietAmountsCalculator.ComputeCalories(day6.Carbs, day6.Fats, day6.Proteins).Value * day6.WeeklyOccurrances.Value))
                .Value / (float)WeekdayEnum.AllTheWeek);

            Assert.Equal(calUnit3, plan.GetLastScheduledDietPlanUnit().AvgDailyCalories);


            // Units check
            List<DietPlanUnit> units = plan.DietUnits.ToList();
            DietPlanUnit unit1 = plan.FindUnitById(unitId - 2);
            DietPlanUnit unit2 = plan.FindUnitById(unitId - 1);
            DietPlanUnit unit3 = plan.FindUnitById(unitId);

            Assert.Equal(3, units.Count);
            Assert.Equal(3, unit1.DietDays.Count);
            Assert.Equal(1, unit2.DietDays.Count);
            Assert.Equal(2, unit3.DietDays.Count);

            Assert.Equal(day1, unit1.FindDayById(new IdType(1)));
            Assert.Equal(day2, unit1.FindDayById(new IdType(2)));
            Assert.Equal(day3, unit1.FindDayById(new IdType(3)));
            Assert.Equal(day4, unit2.FindDayById(new IdType(1)));
            Assert.Equal(day5, unit3.FindDayById(new IdType(1)));
            Assert.Equal(day6, unit3.FindDayById(new IdType(2)));


            // Period check
            DateRangeValue all = unit1.PeriodScheduled.Join(unit2.PeriodScheduled).Join(unit3.PeriodScheduled);
            Assert.Equal(all, plan.PeriodScheduled);

            // Calories check
            CalorieValue targetCal = CalorieValue.MeasureKcal
                ((CalorieValue.MeasureKcal(calUnit1.Value * unit1.PeriodScheduled.GetLength())
                + CalorieValue.MeasureKcal(calUnit2.Value * unit2.PeriodScheduled.GetLength())
                + CalorieValue.MeasureKcal(calUnit3.Value * unit3.PeriodScheduled.GetLength())).Value / plan.PeriodScheduled.GetLength());


            Assert.Equal(targetCal, plan.AvgDailyCalories);
        }


        [Fact]
        public void UnscheduleUnitNotFoundFail()
        {
            IdType unitId = new IdType(1);

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            // Day 1
            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            // Schedule Unit
            DateTime left = DateTime.Today;
            DateTime right = left.AddDays(10);
            plan.CloseDietPlanUnit(unitId, left, right);

            // Unschedule wrong unit
            Assert.Throws<ArgumentException>(() => plan.UnscheduleDietPlanUnit(unitId + 1));
        }


        [Fact]
        public void UnscheduleUnitNotBoundedFail()
        {
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            // Unit1
            IdType unitId = new IdType(1);

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            // Schedule Unit
            DateTime left = DateTime.Today;
            DateTime right = left.AddDays(10);
            plan.CloseDietPlanUnit(unitId, left, right);

            // Unit2
            unitId = unitId + 1;
            plan.AppendDietPlanUnitDraft();

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            // Schedule Unit
            DateTime left2 = right.AddDays(1);
            plan.CloseDietPlanUnit(unitId, left2);

            // Unschedule wrong unit
            Assert.Throws<ArgumentException>(() => plan.UnscheduleDietPlanUnit(unitId + 1));
        }


        //[Fact]
        // Test performed by setting SortUnitsChronologically() as public static instead of private
        //public void SortUnits()
        //{
        //    DateRangeValue range1 = DateRangeValue.RangeBetween(DateTime.Today, DateTime.Today.AddDays(5));        // 26/07 - 31/07
        //    DateRangeValue range2 = DateRangeValue.RangeBetween(range1.End.AddDays(1), range1.End.AddDays(5));     // 01/08 - 06/08
        //    DateRangeValue range3 = DateRangeValue.RangeBetween(range2.End.AddDays(-2), range2.End.AddDays(5));    // 04/08 - 09/08
        //    DateRangeValue range4 = DateRangeValue.RangeBetween(range3.End.AddDays(-2), range3.End.AddDays(5));    // 07/08 - 12/08



        //    List<DietPlanUnit> ret = DietPlan.SortUnitsChronologically(new List<DietPlanUnit>()
        //    {
        //        DietPlanUnit.ScheduleDietUnit(new IdType(1), range1, null),
        //        DietPlanUnit.ScheduleDietUnit(new IdType(2), range2, null),
        //        DietPlanUnit.ScheduleDietUnit(new IdType(3), range4, null),     // Inverted order
        //        DietPlanUnit.ScheduleDietUnit(new IdType(4), range3, null),
        //    }).ToList();

        //    Assert.Equal(range1.GetLength() + range2.GetLength() + range3.GetLength() + range4.GetLength(), ret.Sum(x => x.PeriodScheduled.GetLength()));

        //    List<DietPlanUnit> ret2 = DietPlan.SortUnitsChronologically(new List<DietPlanUnit>()
        //    {
        //        DietPlanUnit.ScheduleDietUnit(new IdType(1), range1, null),
        //        DietPlanUnit.ScheduleDietUnit(new IdType(2), range2, null),
        //        // Missing
        //        DietPlanUnit.ScheduleDietUnit(new IdType(4), range3, null),
        //    }).ToList();

        //    Assert.Equal(range1.GetLength() + range2.GetLength()  + range4.GetLength(), ret2.Sum(x => x.PeriodScheduled.GetLength()));
        //}


        [Fact]
        public void UnscheduleUnitThenSchedule()
        {
            int dayId, carbs, proteins, fats;
            IdType unitId;

            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            unitId = new IdType(1);


            // Day 1
            dayId = 1;
            carbs = 300;
            fats = 70;
            proteins = 170;

            DietPlanDay day1 = DietPlanDay.AddDayToPlan(
                new IdType(dayId), "", WeeklyOccuranceValue.TrackOccurance(5), MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats),
                MacronutirentWeightValue.MeasureGrams(proteins));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats), MacronutirentWeightValue.MeasureGrams(proteins),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(5));


            // Day 2
            dayId = 2;
            carbs = 300;
            fats = 70;
            proteins = 170;

            DietPlanDay day2 = DietPlanDay.AddDayToPlan(
                new IdType(dayId), "", WeeklyOccuranceValue.TrackOccurance(2), MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats),
                MacronutirentWeightValue.MeasureGrams(proteins));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats), MacronutirentWeightValue.MeasureGrams(proteins),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2));

            // Schedule Unit
            DateTime left = DateTime.Today;
            DateTime right = left.AddDays(10);
            plan.CloseDietPlanUnit(unitId, left, right);

            CalorieValue calUnit1 = CalorieValue.MeasureKcal
                ((CalorieValue.MeasureKcal(DietAmountsCalculator.ComputeCalories(day1.Carbs, day1.Fats, day1.Proteins).Value * day1.WeeklyOccurrances.Value)
                + CalorieValue.MeasureKcal(DietAmountsCalculator.ComputeCalories(day2.Carbs, day2.Fats, day2.Proteins).Value * day2.WeeklyOccurrances.Value))
                .Value / (float)WeekdayEnum.AllTheWeek);


            // Append new unit
            plan.AppendDietPlanUnitDraft();

            // Day 3
            unitId = unitId + 1;
            dayId = 1;
            carbs = 700;
            fats = 120;
            proteins = 170;

            DietPlanDay day3 = DietPlanDay.AddDayToPlan(
                new IdType(dayId), "", WeeklyOccuranceValue.TrackOccurance(7), MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats),
                MacronutirentWeightValue.MeasureGrams(proteins));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats), MacronutirentWeightValue.MeasureGrams(proteins),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            // Schedule Unit
            DateTime left2 = right.AddDays(1);
            DateTime right2 = left2.AddDays(10);
            plan.CloseDietPlanUnit(unitId, left2, right2);


            // Append new unit
            plan.AppendDietPlanUnitDraft();

            // Day 4
            unitId = unitId + 1;
            dayId = 1;
            carbs = 500;
            fats = 100;
            proteins = 170;

            DietPlanDay day4 = DietPlanDay.AddDayToPlan(
                new IdType(dayId), "", WeeklyOccuranceValue.TrackOccurance(7), MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats),
                MacronutirentWeightValue.MeasureGrams(proteins));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats), MacronutirentWeightValue.MeasureGrams(proteins),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            // Schedule Unit
            DateTime left3 = right2.AddDays(1);
            DateTime right3 = left3.AddDays(10);
            plan.CloseDietPlanUnit(unitId, left3, right3);

            CalorieValue calUnit3 = CalorieValue.MeasureKcal
                ((CalorieValue.MeasureKcal(DietAmountsCalculator.ComputeCalories(day4.Carbs, day4.Fats, day4.Proteins).Value * day4.WeeklyOccurrances.Value)).Value / (float)WeekdayEnum.AllTheWeek);


            // Unschedule unit 2
            plan.UnscheduleDietPlanUnit(new IdType(2));


            // Units check
            List<DietPlanUnit> units = plan.DietUnits.ToList();
            DietPlanUnit unit1 = plan.FindUnitById(new IdType(1));
            DietPlanUnit unit3 = plan.FindUnitById(new IdType(3));

            Assert.Equal(2, units.Count);
            Assert.Equal(2, unit1.DietDays.Count);
            Assert.Equal(1, unit3.DietDays.Count);

            Assert.Equal(day1, unit1.FindDayById(new IdType(1)));
            Assert.Equal(day2, unit1.FindDayById(new IdType(2)));
            Assert.Equal(day4, unit3.FindDayById(new IdType(1)));


            // Period check
            DateRangeValue all = unit1.PeriodScheduled.Join(unit3.PeriodScheduled);
            Assert.Equal(all, plan.PeriodScheduled);

            // Calories check
            CalorieValue targetCal = CalorieValue.MeasureKcal
                ((CalorieValue.MeasureKcal(calUnit1.Value * unit1.PeriodScheduled.GetLength())
                + CalorieValue.MeasureKcal(calUnit3.Value * unit3.PeriodScheduled.GetLength())).Value / plan.PeriodScheduled.GetLength());


            Assert.Equal(targetCal, plan.AvgDailyCalories);



            // Schedule the unscheduled unit again
            plan.AppendDietPlanUnitDraft();

            // Day 3
            unitId = unitId + 1;
            carbs = 700;
            fats = 120;
            proteins = 170;

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats), MacronutirentWeightValue.MeasureGrams(proteins),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            // Schedule Unit
            DateTime left4 = right3.AddDays(1);
            DateTime right4 = left4.AddDays(10);
            plan.CloseDietPlanUnit(unitId, left3, right3);

            CalorieValue calUnit2 = CalorieValue.MeasureKcal
                ((CalorieValue.MeasureKcal(DietAmountsCalculator.ComputeCalories(day3.Carbs, day3.Fats, day3.Proteins).Value * day3.WeeklyOccurrances.Value).Value / (float)WeekdayEnum.AllTheWeek));

            // Units check
            units = plan.DietUnits.ToList();
            unit1 = plan.FindUnitById(new IdType(1));
            unit3 = plan.FindUnitById(new IdType(3));
            DietPlanUnit unit2 = plan.FindUnitById(new IdType(4));

            Assert.Null(plan.FindUnitById(new IdType(2)));

            Assert.Equal(3, units.Count);
            Assert.Equal(2, unit1.DietDays.Count);
            Assert.Equal(1, unit3.DietDays.Count);
            Assert.Equal(1, unit2.DietDays.Count);

            Assert.Equal(day1, unit1.FindDayById(new IdType(1)));
            Assert.Equal(day2, unit1.FindDayById(new IdType(2)));
            Assert.Equal(day4, unit3.FindDayById(new IdType(1)));
            Assert.Equal(day3, unit2.FindDayById(new IdType(1)));


            // Period check
            all = unit1.PeriodScheduled.Join(unit3.PeriodScheduled).Join(unit2.PeriodScheduled);
            Assert.Equal(all, plan.PeriodScheduled);

            // Calories check
            targetCal = CalorieValue.MeasureKcal
                ((CalorieValue.MeasureKcal(calUnit1.Value * unit1.PeriodScheduled.GetLength())
                + CalorieValue.MeasureKcal(calUnit2.Value * unit2.PeriodScheduled.GetLength())
                + CalorieValue.MeasureKcal(calUnit3.Value * unit3.PeriodScheduled.GetLength())).Value / plan.PeriodScheduled.GetLength());

            Assert.Equal(targetCal, plan.AvgDailyCalories);
        }


        [Fact]
        public void DietPlanAppendUnitBeforeSchedulingPreviousFail()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            // Append new unit before scheduling the previous
            Assert.Throws<InvalidOperationException>(() => plan.AppendDietPlanUnitDraft());
        }


        [Fact]
        public void DietPlanAppendUnitToNotBoundedFail()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            plan.CloseDietPlanUnitToInfinite(unitId, DateTime.Today);

            Assert.Throws<InvalidOperationException>(() => plan.AppendDietPlanUnitDraft());
        }


        [Fact]
        public void DietPlanAppendUnitOverlappingFail()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            DateRangeValue range1 = DateRangeValue.RangeBetween(DateTime.Today, DateTime.Today.AddDays(10));
            DateRangeValue range2 = DateRangeValue.RangeBetween(range1.End.AddDays(1), range1.End.AddDays(11));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            plan.CloseDietPlanUnit(unitId, range1.Start, range1.End);
            plan.AppendDietPlanUnitDraft();

            Assert.Equal(range1, plan.PeriodScheduled);        // Not aligned as unit2 has no units -> draft

            unitId = unitId + 1;

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            Assert.Equal(range1, plan.PeriodScheduled);        // Still not aligned since unit2 not scheduled -> draft

            // Invalid schedule -> overlapping units
            Assert.Throws<DietDomainIvariantViolationException>(() =>
                plan.CloseDietPlanUnit(unitId, DateTime.Today, DateTime.Today.AddDays(20)));

            // Valid schedule
            plan.CloseDietPlanUnit(unitId, range1.End.AddDays(1), range1.End.AddDays(11));
            Assert.Equal(range2.Join(range1), plan.PeriodScheduled);        // This time is equal
        }


        [Fact]
        public void DietPlanCloseAppendUpTo()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            DateRangeValue range1 = DateRangeValue.RangeBetween(DateTime.Today, DateTime.Today.AddDays(10));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            plan.CloseDietPlanUnit(unitId, range1.Start, range1.End);


            unitId = unitId + 1;
            plan.AppendDietPlanUnitDraft();

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
    weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            plan.CloseDietPlanUnit(unitId, range1.End.AddDays(10));

            Assert.Equal(range1.End.AddDays(10), plan.FindUnitById(unitId).PeriodScheduled.End);
            Assert.Equal(range1.End.AddDays(1), plan.FindUnitById(unitId).PeriodScheduled.Start);
        }


        [Fact]
        public void DietPlanCloseInfiniteFail()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            DateRangeValue range1 = DateRangeValue.RangeBetween(DateTime.Today, DateTime.Today.AddDays(10));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            Assert.Throws<DietDomainIvariantViolationException>(() => plan.CloseDietPlanUnit(unitId, range1.End));
            Assert.Throws<ArgumentException>(() => plan.CloseDietPlanUnit(unitId + 10, range1.Start, range1.End));
        }


        [Fact]
        public void DietPlanAppendUnitNonContiguousFail()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            DateRangeValue range1 = DateRangeValue.RangeBetween(DateTime.Today, DateTime.Today.AddDays(10));
            DateRangeValue range2 = DateRangeValue.RangeBetween(range1.End.AddDays(1), range1.End.AddDays(11));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            plan.CloseDietPlanUnit(unitId, range1.Start, range1.End);
            plan.AppendDietPlanUnitDraft();

            Assert.Equal(range1, plan.PeriodScheduled);        // Not aligned as unit2 has no units -> draft

            unitId = unitId + 1;

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            Assert.Equal(range1, plan.PeriodScheduled);        // Still not aligned since unit2 not scheduled -> draft

            // Invalid schedule -> Non contguous units
            Assert.Throws<DietDomainIvariantViolationException>(() =>
                plan.CloseDietPlanUnit(unitId, range1.End.AddDays(+5), range1.End.AddDays(20)));

            // Valid schedule
            plan.CloseDietPlanUnit(unitId, range1.End.AddDays(1), range1.End.AddDays(11));
            Assert.Equal(range2.Join(range1), plan.PeriodScheduled);        // This time is equal
        }


        [Fact]
        public void CloseDietPlan()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);
            DateRangeValue range1 = DateRangeValue.RangeBetween(DateTime.Today, DateTime.Today.AddDays(10));

            // Unit1
            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            plan.CloseDietPlanUnit(unitId, range1.Start, range1.End);

            // Unit2
            plan.AppendDietPlanUnitDraft();
            unitId = unitId + 1;

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            plan.CloseDietPlanUnitToInfinite(unitId, range1.End.AddDays(1));

            Assert.Equal(range1.Start, plan.PeriodScheduled.Start);
            Assert.False(plan.PeriodScheduled.IsRightBounded());

            //Close it
            int days = 10;
            DateTime end = range1.End.AddDays(days);
            plan.CloseDietPlan(end);

            Assert.Equal(range1.Start, plan.PeriodScheduled.Start);
            Assert.Equal(end, plan.PeriodScheduled.End);
            Assert.Equal(plan.FindUnitById(unitId - 1).PeriodScheduled.End.AddDays(1), plan.FindUnitById(unitId).PeriodScheduled.Start);    // Check contiguousness
            Assert.Equal(end, plan.FindUnitById(unitId).PeriodScheduled.End);   // Unit 2 has been closed also
        }


        [Fact]
        public void ShuffleDietPlanUnits()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            int fakeDays2 = 10, days2 = 20, days = 10;
            DateRangeValue range1 = DateRangeValue.RangeBetween(DateTime.Today, DateTime.Today.AddDays(days));
            DateTime fakeEnd2 = range1.End.AddDays(fakeDays2);
            DateTime end2 = range1.End.AddDays(days2);
            DateTime fakeEnd3 = fakeEnd2.AddDays(days);
            DateTime end3 = end2.AddDays(days);

            // Unit1
            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            plan.CloseDietPlanUnit(unitId, range1.Start, range1.End);

            // Unit2
            plan.AppendDietPlanUnitDraft();
            unitId = unitId + 1;

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            plan.CloseDietPlanUnit(unitId, fakeEnd2);

            // Unit2
            plan.AppendDietPlanUnitDraft();
            unitId = unitId + 1;

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            plan.CloseDietPlanUnit(unitId, fakeEnd3);

            // Check the units schedule is as expected
            IdType testUnitId = new IdType(2);
            Assert.Equal(range1.End.AddDays(1), plan.FindUnitById(testUnitId).PeriodScheduled.Start);
            Assert.Equal(fakeEnd2, plan.FindUnitById(testUnitId).PeriodScheduled.End);
            testUnitId = new IdType(3);
            Assert.Equal(fakeEnd2.AddDays(1), plan.FindUnitById(testUnitId).PeriodScheduled.Start);
            Assert.Equal(fakeEnd3, plan.FindUnitById(testUnitId).PeriodScheduled.End);

            // Extend the Unit2 to end2
            unitId = new IdType(2);
            plan.MoveDietPlanUnit(unitId, DateRangeValue.RangeBetween(plan.FindUnitById(unitId).PeriodScheduled.Start, end2));

            // Check the units schedule is as expected
            testUnitId = new IdType(2);
            Assert.Equal(range1.End.AddDays(1), plan.FindUnitById(testUnitId).PeriodScheduled.Start);
            Assert.Equal(end2, plan.FindUnitById(testUnitId).PeriodScheduled.End);
            testUnitId = new IdType(3);
            Assert.Equal(end2.AddDays(1), plan.FindUnitById(testUnitId).PeriodScheduled.Start);
            Assert.Equal(end3, plan.FindUnitById(testUnitId).PeriodScheduled.End);

            // Move Unit2 to a non-contiguous period
            unitId = new IdType(2);
            DateRangeValue newPeriod = DateRangeValue.RangeBetween(end3.AddDays(100), end3.AddDays(120));
            plan.MoveDietPlanUnit(unitId, newPeriod);

            // Check the units schedule is as expected
            //testUnitId = new IdType(1);
            //Assert.Equal(range1.Start, plan.FindUnitById(testUnitId).PeriodScheduled.Start);
            //Assert.Equal(newPeriod.Start.AddDays(-1), plan.FindUnitById(testUnitId).PeriodScheduled.End);
            testUnitId = new IdType(2);
            Assert.Equal(newPeriod, plan.FindUnitById(testUnitId).PeriodScheduled);
            testUnitId = new IdType(3);
            Assert.Equal(range1.End.AddDays(1), plan.FindUnitById(testUnitId).PeriodScheduled.Start);
            Assert.Equal(newPeriod.Start.AddDays(-1), plan.FindUnitById(testUnitId).PeriodScheduled.End);
        }


        [Fact]
        public void RemoveDietPlanDayWeekCoverageFail()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            int days = 10;
            DateRangeValue range1 = DateRangeValue.RangeBetween(DateTime.Today, DateTime.Today.AddDays(days));

            // Unit1
            DietPlanDay day1 = DietPlanDay.AddDayToPlan(
                dayId, "", WeeklyOccuranceValue.TrackOccurance(2), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                     MacronutirentWeightValue.MeasureGrams(100));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2));

            DietPlanDay day2 = DietPlanDay.AddDayToPlan(
                dayId, "", WeeklyOccuranceValue.TrackOccurance(5), MacronutirentWeightValue.MeasureGrams(200), MacronutirentWeightValue.MeasureGrams(100),
                     MacronutirentWeightValue.MeasureGrams(100));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(200), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(5));

            plan.CloseDietPlanUnit(unitId, range1.Start, range1.End);

            // Unit2
            plan.AppendDietPlanUnitDraft();
            unitId = unitId + 1;

            DietPlanDay day3 = DietPlanDay.AddDayToPlan(
                dayId, "", WeeklyOccuranceValue.TrackOccurance(2), MacronutirentWeightValue.MeasureGrams(500), MacronutirentWeightValue.MeasureGrams(100),
                     MacronutirentWeightValue.MeasureGrams(100));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(500), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2));

            DietPlanDay day5 = DietPlanDay.AddDayToPlan(
                dayId, "", WeeklyOccuranceValue.TrackOccurance(5), MacronutirentWeightValue.MeasureGrams(200), MacronutirentWeightValue.MeasureGrams(100),
                     MacronutirentWeightValue.MeasureGrams(100));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(200), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(5));

            plan.CloseDietPlanUnit(unitId, range1.End.AddDays(100));

            IdType removeUnitId = new IdType(1);
            IdType removeDayId = new IdType(1);
            plan.UnplanDietDay(removeUnitId, removeDayId);

            Assert.Throws<DietDomainIvariantViolationException>(() => plan.CloseDietPlanUnit(removeUnitId, range1.Start, range1.End));
        }


        [Fact]
        public void RemoveDietPlanDayWrongDayFail()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            int days = 10;
            DateRangeValue range1 = DateRangeValue.RangeBetween(DateTime.Today, DateTime.Today.AddDays(days));

            // Unit1
            DietPlanDay day1 = DietPlanDay.AddDayToPlan(
                dayId, "", WeeklyOccuranceValue.TrackOccurance(2), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                     MacronutirentWeightValue.MeasureGrams(100));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2));

            DietPlanDay day2 = DietPlanDay.AddDayToPlan(
                dayId, "", WeeklyOccuranceValue.TrackOccurance(5), MacronutirentWeightValue.MeasureGrams(200), MacronutirentWeightValue.MeasureGrams(100),
                     MacronutirentWeightValue.MeasureGrams(100));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(200), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(5));

            plan.CloseDietPlanUnit(unitId, range1.Start, range1.End);

            // Unit2
            plan.AppendDietPlanUnitDraft();
            unitId = unitId + 1;

            DietPlanDay day3 = DietPlanDay.AddDayToPlan(
                dayId, "", WeeklyOccuranceValue.TrackOccurance(2), MacronutirentWeightValue.MeasureGrams(500), MacronutirentWeightValue.MeasureGrams(100),
                     MacronutirentWeightValue.MeasureGrams(100));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(500), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2));

            DietPlanDay day5 = DietPlanDay.AddDayToPlan(
                dayId, "", WeeklyOccuranceValue.TrackOccurance(5), MacronutirentWeightValue.MeasureGrams(200), MacronutirentWeightValue.MeasureGrams(100),
                     MacronutirentWeightValue.MeasureGrams(100));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(200), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(5));

            plan.CloseDietPlanUnit(unitId, range1.End.AddDays(100));

            // Non existing day
            IdType removeUnitId = new IdType(1);
            IdType removeDayId = new IdType(11);
            Assert.Throws<ArgumentException>(() => plan.UnplanDietDay(removeUnitId, removeDayId));

            // Non existing unit
            IdType removeUnitId2 = new IdType(11);
            IdType removeDayId2 = new IdType(1);
            Assert.Throws<ArgumentException>(() => plan.UnplanDietDay(removeUnitId2, removeDayId2));
        }


        [Fact]
        public void RemoveDietPlanDay()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            Trainee dest = Trainee.Register("trynee", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner, dest);

            int days = 10;
            DateRangeValue range1 = DateRangeValue.RangeBetween(DateTime.Today, DateTime.Today.AddDays(days));

            // Unit1
            DietPlanDay day1 = DietPlanDay.AddDayToPlan(
                dayId, "", WeeklyOccuranceValue.TrackOccurance(2), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                     MacronutirentWeightValue.MeasureGrams(100));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2));

            dayId = dayId + 1;
            DietPlanDay day2 = DietPlanDay.AddDayToPlan(
                dayId, "", WeeklyOccuranceValue.TrackOccurance(5), MacronutirentWeightValue.MeasureGrams(200), MacronutirentWeightValue.MeasureGrams(100),
                     MacronutirentWeightValue.MeasureGrams(100));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(200), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(5));

            plan.CloseDietPlanUnit(unitId, range1.Start, range1.End);

            // Unit2
            plan.AppendDietPlanUnitDraft();
            unitId = unitId + 1;

            dayId = new IdType(1);
            DietPlanDay day3 = DietPlanDay.AddDayToPlan(
                dayId, "", WeeklyOccuranceValue.TrackOccurance(2), MacronutirentWeightValue.MeasureGrams(500), MacronutirentWeightValue.MeasureGrams(100),
                     MacronutirentWeightValue.MeasureGrams(100));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(500), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2));

            dayId = dayId + 1;
            DietPlanDay day4 = DietPlanDay.AddDayToPlan(
                dayId, "", WeeklyOccuranceValue.TrackOccurance(5), MacronutirentWeightValue.MeasureGrams(200), MacronutirentWeightValue.MeasureGrams(100),
                     MacronutirentWeightValue.MeasureGrams(100));

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(200), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(5));

            plan.CloseDietPlanUnit(unitId, range1.End.AddDays(100));

            IdType removeUnitId = new IdType(1);
            IdType removeDayId = new IdType(1);
            plan.UnplanDietDay(removeUnitId, removeDayId);

            // Change the other day to avoid the excepetion because of week not fully covered
            plan.ChangeDietDay(removeUnitId, new IdType(2), MacronutirentWeightValue.MeasureGrams(200), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            plan.CloseDietPlanUnit(removeUnitId, range1.Start, range1.End);

            Assert.Single(plan.FindUnitById(removeUnitId).DietDays);
            Assert.Equal(1, plan.FindUnitById(removeUnitId).Id.Id);
            Assert.DoesNotContain(day1, plan.FindUnitById(removeUnitId).DietDays);
            Assert.Contains(day2, plan.FindUnitById(removeUnitId).DietDays);

            Assert.Equal(2, plan.FindUnitById(new IdType(2)).DietDays.Count);
            Assert.Contains(day3, plan.FindUnitById(new IdType(2)).DietDays);
            Assert.Contains(day4, plan.FindUnitById(new IdType(2)).DietDays);
        }


        /// <summary>
        /// UI Vs Application Vs Domain example
        /// assuming the UI can access the Domain Model
        /// </summary>
        //private void Example()
        //{

        //    /////////////////////////////////////////////
        //    // UI Layer
        //    /////////////////////////////////////////////

        //    DietPlan draft = DietPlan.NewDraft();


        //    DietPlanDay day = DietPlanDay.NewDraft();
        //    day.PlanCarbs(MacronutirentWeightValue.MeasureGrams(350));
        //    day.PlanFats(MacronutirentWeightValue.MeasureGrams(40));
        //    day.PlanPros(MacronutirentWeightValue.MeasureGrams(170));
        //    day.SetOccurrance(WeeklyOccuranceValue.TrackOccurance(7));
        //    day.SetDayType(DietDayTypeEnum.NotSet);


        //    DietPlanUnit unit = DietPlanUnit.ScheduleDietUnit(DateRangeValue.RangeStartingFrom(DateTime.Now)
        //        , new List<DietPlanDay>()
        //        {
        //            day,
        //        });

        //    draft.ScheduleDietPlanUnit(unit);
        //    draft.SetName("test");

        //    IdType postId = new IdType(1);
        //    Owner owner = Owner.Register("owner", "pic");
        //    PersonalNoteValue note = PersonalNoteValue.Write("my note.");

        //    draft.AssignAsDietPlan(id, owner, note);        // Changes the status from "Draft" to "Planned"



        //    // -> User clicks on "Schedule"
        //    ApplicationLayer.Schedule();


        //    /////////////////////////////////////////////
        //    // Application Layer
        //    /////////////////////////////////////////////

        //    DietPlan input;

        //    DietPlan plan = input;


        //    if (draft.Status == Draft)
        //        throw new Exception("Draft cannot be ");
        //    else
        //        _repository.Add(plan);

        //    _context.Save();
        //}
    }

    /// <summary>
    /// UI Vs Application Vs Domain example
    /// assumingo only the Application layer can access the Domain Model
    /// </summary>
    //private void Example()
    //{

    //    /////////////////////////////////////////////
    //    // UI Layer
    //    /////////////////////////////////////////////

    //    DietPlanVM vm;
        
    //    vm = ApplicationLayer.NewDraft(); 
    //    vm = ApplicationLayer.AddDay();     // Return with updated Avg Calories
    //    vm = ApplicationLayer.AddDay();

    //    ApplicationLayer.AddUnit();
    //    ApplicationLayer.AddDay();
    //    ApplicationLayer.AddDay();


    //    // -> User clicks on "Schedule"
    //    ApplicationLayer.Schedule();



    //    /////////////////////////////////////////////
    //    // Application Layer
    //    /////////////////////////////////////////////

    //    DietPlan input;

    //    DietPlan plan = input;

    //    _repository.Add(plan);

    //    _context.Save();
    //}
}
