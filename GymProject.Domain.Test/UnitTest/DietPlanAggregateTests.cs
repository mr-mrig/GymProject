using GymProject.Domain.Base;
using GymProject.Domain.DietDomain;
using GymProject.Domain.DietDomain.DietPlanAggregate;
using GymProject.Domain.DietDomain.Exceptions;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace GymProject.Domain.Test.UnitTest
{
    public class DietPlanAggregateTests
    {



        [Fact]
        public void DietPlanNullUnitsFail()
        {
            IdType id = new IdType(1);
            string name = "My Plan";

            Owner owner = Owner.Register("myUser", "imageUrl");
            PersonalNoteValue note = PersonalNoteValue.Write("my note.");
            WeeklyOccuranceValue freeMeals = WeeklyOccuranceValue.TrackOccurance(2);

            Assert.Throws<DietDomainIvariantViolationException>(() => DietPlan.ScheduleFullDietPlan(id, name, null, owner, note, freeMeals));
        }

        [Fact]
        public void DietPlanEmptyUnitsFail()
        {
            IdType id = new IdType(1);
            string name = "My Plan";

            Owner owner = Owner.Register("myUser", "imageUrl");
            PersonalNoteValue note = PersonalNoteValue.Write("my note.");
            WeeklyOccuranceValue freeMeals = WeeklyOccuranceValue.TrackOccurance(2);

            List<DietPlanUnit> units = new List<DietPlanUnit>();

            Assert.Throws<DietDomainIvariantViolationException>(() => DietPlan.ScheduleFullDietPlan(id, name, units, owner, note, freeMeals));
        }

        [Fact]
        public void DietPlanNewDraft()
        {
            Owner owner = Owner.Register("myUser", "imageUrl");

            DietPlan plan = DietPlan.NewDraft(owner);

            List<DietPlanUnit> units = plan.DietUnits.ToList();

            Assert.NotNull(plan);
            Assert.Equal(owner, plan.Owner);
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

            DietPlan plan = DietPlan.NewDraft(owner);
            plan.WriteOwnerNote(note);
            plan.WriteOwnerNote(note2);
            plan.GiveName(name);
            plan.GiveName(name2);
            plan.GrantFreeMeals(fmeals);
            plan.GrantFreeMeals(fmeals2);
            plan.LinkToPost(postId);

            List<DietPlanUnit> units = plan.DietUnits.ToList();

            Assert.NotNull(plan);
            Assert.Equal(owner, plan.Owner);
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

            DietPlan plan = DietPlan.NewDraft(owner);


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

            DietPlan plan = DietPlan.NewDraft(owner);


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

            DietPlan plan = DietPlan.NewDraft(owner);


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

            DietPlan plan = DietPlan.NewDraft(owner);


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
            DietPlan plan = DietPlan.NewDraft(owner);

            // Doesn't throw
            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(300), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                    weeklyOccurrances: null, dayType: null, specificWeekday: WeekdayEnum.Tuesday);
        }


        [Fact]
        public void DietPlanDraftExceedingOccurrancesDayFail()
        {
            IdType unitId = new IdType(1);

            Owner owner = Owner.Register("myUser", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner);

            Assert.Throws<ValueObjectInvariantViolationException>(()
                => plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(300), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                    weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(11), dayType: null));
        }


        [Fact]
        public void DietPlanDraftMissingMacrosDayFail()
        {
            IdType unitId = new IdType(1);

            Owner owner = Owner.Register("myUser", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner);

            Assert.Throws<DietDomainIvariantViolationException>(()
                => plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(0), MacronutirentWeightValue.MeasureGrams(0), MacronutirentWeightValue.MeasureGrams(0),
                    weeklyOccurrances: null, dayType: null));
        }


        [Fact]
        public void DietPlanDraftNullMacrosDayFail()
        {
            IdType unitId = new IdType(1);

            Owner owner = Owner.Register("myUser", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner);

            Assert.Throws<DietDomainIvariantViolationException>(()
                => plan.PlanDietDay(unitId, null, null, null, weeklyOccurrances: null, dayType: null));
        }


        [Fact]
        public void DietPlanConsolidateConsolidateOneDay()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner);

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: null, dayType: DietDayTypeEnum.NotSet);

            plan.ConsolidatePlan(new IdType(1));    // Doesn't throw

            Assert.Equal(WeeklyOccuranceValue.MaximumTimes, plan.FindUnitById(unitId).FindDayById(dayId).WeeklyOccurrances.Value);
        }


        [Fact]
        public void DietPlanAddDaysExceedingWeekdaysFail()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner);

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
            DietPlan plan = DietPlan.NewDraft(owner);

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
            DietPlan plan = DietPlan.NewDraft(owner);

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
            plan.CloseDietPlanUnit(unitId, DateTime.Today);
            Assert.Equal(DateRangeValue.RangeStartingFrom(DateTime.Today), plan.PeriodScheduled);

            int days = 100;
            plan.CloseDietPlanUnit(unitId, DateTime.Today, DateTime.Today.AddDays(days));
            Assert.Equal(DateRangeValue.RangeBetween(DateTime.Today, DateTime.Today.AddDays(days)), plan.PeriodScheduled);
            Assert.Equal(days, plan.PeriodScheduled.GetLength());

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

            plan.ConsolidatePlan(new IdType(1));
        }


        [Fact]
        public void DietPlanConsolidateNoIdFail()
        {
            Owner owner = Owner.Register("myUser", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner);
            Assert.Throws<DietDomainIvariantViolationException>(() => plan.ConsolidatePlan(null));
        }


        [Fact]
        public void DietPlanConsolidateInvalidUnitsFail()
        {
            IdType postId = new IdType(11);
            Owner owner = Owner.Register("myUser", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner);
            Assert.Throws<DietDomainIvariantViolationException>(() => plan.ConsolidatePlan(postId));
        }


        [Fact]
        public void DietPlanConsolidateInvalidUnitsFail2()
        {
            Owner owner = Owner.Register("myUser", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner);

            plan.PlanDietDay(new IdType(1), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2), dayType: null);

            Assert.Throws<DietDomainIvariantViolationException>(() => plan.ConsolidatePlan(new IdType(1)));
        }


        [Fact]
        public void DietPlanConsolidateInvalidUnitsFail3()
        {
            Owner owner = Owner.Register("myUser", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner);

            plan.PlanDietDay(new IdType(1), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(2), dayType: DietDayTypeEnum.NotSet);

            Assert.Throws<DietDomainIvariantViolationException>(() => plan.ConsolidatePlan(new IdType(1)));
        }



        [Fact]
        public void ConsolidateWithTwoUnitsFullCase()
        {
            int dayId, carbs, proteins, fats, salt, water;
            DietDayTypeEnum dayType;
            IdType unitId;

            Owner owner = Owner.Register("myUser", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner);

            unitId = new IdType(1);


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

            // Schedule Unit
            plan.CloseDietPlanUnit(unitId, DateTime.Today);
            Assert.Equal(DateRangeValue.RangeStartingFrom(DateTime.Today), plan.PeriodScheduled);

            // Append new unit
            plan.AppendDietPlanUnitDraft();

            // Day 4 -> New unit
            unitId = unitId + 1;
            dayId = 1;
            carbs = 700;
            fats = 120;
            salt = 2;
            water = 3;
            proteins = 170;
            dayType = null;

            DietPlanDay day4 = DietPlanDay.AddDayToPlan(
                new IdType(dayId), "", WeeklyOccuranceValue.TrackOccurance(7), MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats),
                MacronutirentWeightValue.MeasureGrams(proteins), dayType: DietDayTypeEnum.On);

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(carbs), MacronutirentWeightValue.MeasureGrams(fats), MacronutirentWeightValue.MeasureGrams(proteins),
                dayType: dayType, weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7), salt: MicronutirentWeightValue.MeasureGrams(salt), water: VolumeValue.MeasureLiters(water));



            List<DietPlanUnit> units = plan.DietUnits.ToList();
            DietPlanUnit unit1 = plan.FindUnitById(unitId - 1);
            DietPlanUnit unit2 = plan.FindUnitById(unitId);

            Assert.Equal(2, units.Count);
            Assert.Equal(3, unit1.DietDays.Count);
            Assert.Equal(1, unit2.DietDays.Count);

            Assert.Equal(day4, unit2.FindDayById(new IdType(dayId)));
            Assert.Equal(day1, unit1.FindDayById(new IdType(1)));
            Assert.Equal(day2, unit1.FindDayById(new IdType(2)));
            Assert.Equal(day3, unit1.FindDayById(new IdType(3)));






            // Change aggregate root
            PersonalNoteValue note = PersonalNoteValue.Write("My note.");
            plan.WriteOwnerNote(note);


            // Other
            CalorieValue targetCal = CalorieValue.MeasureKcal((day1.Calories.Value * day1.WeeklyOccurrances.Value
                + day2.Calories.Value * day2.WeeklyOccurrances.Value + day3.Calories.Value * day3.WeeklyOccurrances.Value) / 7f);


            Assert.Equal(string.Empty, plan.Name);
            Assert.Single(units);
            Assert.Equal(note, plan.OwnerNote);
            Assert.Null(plan.PostId);
            Assert.Null(plan.WeeklyFreeMeals);

            Assert.Equal(targetCal, plan.AvgDailyCalories);

            plan.ConsolidatePlan(new IdType(1));
        }


        [Fact]
        public void DietPlanAppendUnitBeforeSchedulingPreviousFail()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner);
            
            // Append new unit before scheduling the previous
            Assert.Throws<InvalidOperationException>(() => plan.AppendDietPlanUnitDraft());
        }


        [Fact]
        public void DietPlanAppendUnitToNotBoundedFail()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner);

            plan.PlanDietDay(unitId, MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100), MacronutirentWeightValue.MeasureGrams(100),
                weeklyOccurrances: WeeklyOccuranceValue.TrackOccurance(7));

            plan.CloseDietPlanUnit(unitId, DateTime.Today);

            Assert.Throws<InvalidOperationException>(() => plan.AppendDietPlanUnitDraft());
        }


        [Fact]
        public void DietPlanAppendUnitCheckPlanPeriod()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner);

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
        public void CloseDietPlan()
        {
            IdType unitId = new IdType(1);
            IdType dayId = new IdType(1);
            Owner owner = Owner.Register("myUser", "imageUrl");
            DietPlan plan = DietPlan.NewDraft(owner);
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

            plan.CloseDietPlanUnit(unitId, range1.End.AddDays(1));

            Assert.Equal(range1.Start, plan.PeriodScheduled.Start);
            Assert.False(plan.PeriodScheduled.IsRightBounded());

            //Close it
            int days = 10;
            DateTime end = range1.End.AddDays(days);
            plan.CloseDietPlan(end);

            Assert.Equal(range1.Start, plan.PeriodScheduled.Start);
            Assert.Equal(end, plan.PeriodScheduled.End);
            Assert.Equal(plan.FindUnitById(unitId - 1).PeriodScheduled.End.AddDays(1), plan.FindUnitById(unitId).PeriodScheduled.Start);    // Check contiguousness
            Assert.Equal(end, plan.FindUnitById(unitId).PeriodScheduled.End);
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
