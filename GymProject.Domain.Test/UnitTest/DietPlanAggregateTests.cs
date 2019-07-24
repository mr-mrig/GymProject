using GymProject.Domain.Base;
using GymProject.Domain.DietDomain.DietPlanAggregate;
using GymProject.Domain.DietDomain.Exceptions;
using GymProject.Domain.SharedKernel;
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
        public void DietPlanlMissingIdFail()
        {
            Assert.Throws<DietDomainIvariantViolationException>(() => DietPlan.ScheduleDietPlan(null, string.Empty));
        }

        [Fact]
        public void DietPlanNullUnitsFail()
        {
            IdType id = new IdType(1);
            string name = "My Plan";

            Owner owner = Owner.Register("myUser", "imageUrl");
            PersonalNoteValue note = PersonalNoteValue.Write("my note.");
            WeeklyOccuranceValue freeMeals = WeeklyOccuranceValue.TrackOccurance(2);

            Assert.Throws<DietDomainIvariantViolationException>(() => DietPlan.ScheduleDietPlan(id, name, null, owner, note, freeMeals));

        }

        [Fact]
        public void DietPlanEmptyUnits()
        {
            IdType id = new IdType(1);
            string name = "My Plan";

            Owner owner = Owner.Register("myUser", "imageUrl");
            PersonalNoteValue note = PersonalNoteValue.Write("my note.");
            WeeklyOccuranceValue freeMeals = WeeklyOccuranceValue.TrackOccurance(2);

            List<DietPlanUnit> units = new List<DietPlanUnit>();

            DietPlan plan = DietPlan.ScheduleDietPlan(id, name, units, owner, note, freeMeals);

            Assert.NotNull(plan);
            Assert.Equal(id, plan.PostId);
            Assert.Equal(name, plan.Name);
            Assert.Equal(owner, plan.Owner);
            Assert.Equal(note, plan.OwnerNote);
            Assert.Equal(freeMeals, plan.WeeklyFreeMeals);
            Assert.Empty(plan.DietUnits);
            Assert.Null(plan.DomainEvents);
        }

        //[Fact]
        //public void DietPlanEmptyUnits()
        //{
        //    IdType id = new IdType(1);
        //    string name = "My Plan";

        //    Owner owner = Owner.Register("myUser", "imageUrl");
        //    PersonalNoteValue note = PersonalNoteValue.Write("my note.");
        //    WeeklyOccuranceValue freeMeals = WeeklyOccuranceValue.TrackOccurance(2);

        //    List<DietPlanUnit> units = new List<DietPlanUnit>();

        //    DietPlan plan = DietPlan.ScheduleDietPlan(id, name, units, owner, note, freeMeals);

        //    Assert.NotNull(plan);
        //    Assert.Equal(id, plan.PostId);
        //    Assert.Equal(name, plan.Name);
        //    Assert.Equal(owner, plan.Owner);
        //    Assert.Equal(note, plan.OwnerNote);
        //    Assert.Equal(freeMeals, plan.WeeklyFreeMeals);
        //    Assert.Empty(plan.DietUnits);
        //    Assert.Null(plan.DomainEvents);
        //}




        /// <summary>
        /// UI Vs Application Vs Domain example
        /// assuming the UI can access the Domain Model
        /// </summary>
        private void Example()
        {

            /////////////////////////////////////////////
            // UI Layer
            /////////////////////////////////////////////

            DietPlan draft = DietPlan.NewDraft();


            DietPlanDay day = DietPlanDay.NewDraft();
            day.PlanCarbs(MacronutirentWeightValue.MeasureGrams(350));
            day.PlanFats(MacronutirentWeightValue.MeasureGrams(40));
            day.PlanPros(MacronutirentWeightValue.MeasureGrams(170));
            day.SetOccurrance(WeeklyOccuranceValue.TrackOccurance(7));
            day.SetDayType(DietDayTypeEnum.NotSet);


            DietPlanUnit unit = DietPlanUnit.ScheduleDietUnit(DateRangeValue.RangeStartingFrom(DateTime.Now)
                , new List<DietPlanDay>()
                {
                    day,
                });

            draft.ScheduleDietPlanUnit(unit);
            draft.SetName("test");

            IdType postId = new IdType(1);
            Owner owner = Owner.Register("owner", "pic");
            PersonalNoteValue note = PersonalNoteValue.Write("my note.");

            draft.AssignAsDietPlan(id, owner, note);        // Changes the status from "Draft" to "Planned"



            // -> User clicks on "Schedule"
            ApplicationLayer.Schedule();


            /////////////////////////////////////////////
            // Application Layer
            /////////////////////////////////////////////

            DietPlan input;

            DietPlan plan = input;


            if (draft.Status == Draft)
                throw new Exception("Draft cannot be ");
            else
                _repository.Add(plan);

            _context.Save();
        }
    }

    /// <summary>
    /// UI Vs Application Vs Domain example
    /// assumingo only the Application layer can access the Domain Model
    /// </summary>
    private void Example()
    {

        /////////////////////////////////////////////
        // UI Layer
        /////////////////////////////////////////////

        DietPlanVM vm;
        
        vm = ApplicationLayer.NewDraft(); 
        vm = ApplicationLayer.AddDay();     // Return with updated Avg Calories
        vm = ApplicationLayer.AddDay();

        ApplicationLayer.AddUnit();
        ApplicationLayer.AddDay();
        ApplicationLayer.AddDay();


        // -> User clicks on "Schedule"
        ApplicationLayer.Schedule();



        /////////////////////////////////////////////
        // Application Layer
        /////////////////////////////////////////////

        DietPlan input;

        DietPlan plan = input;

        _repository.Add(plan);

        _context.Save();
    }
}
