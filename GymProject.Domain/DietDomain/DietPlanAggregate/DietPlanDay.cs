using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.DietDomain.DietPlanAggregate
{
    public class DietPlanDay : Entity<IdType>
    {


        /// <summary>
        /// The name
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Carbohidrates quantity
        /// </summary>
        public MacronutirentWeightValue Carbs { get; private set; } = null;

        /// <summary>
        /// Fats quantity
        /// </summary>
        public MacronutirentWeightValue Fats { get; private set; } = null;

        /// <summary>
        /// Proteins quantity
        /// </summary>
        public MacronutirentWeightValue Proteins { get; private set; } = null;


        /// <summary>
        /// Calories amount
        /// </summary>
        public CalorieValue Calories { get; private set; } = null;


        /// <summary>
        /// Salt quantity
        /// </summary>
        public MicronutirentWeightValue Salt { get; private set; } = null;

        /// <summary>
        /// Water quantity
        /// </summary>
        public VolumeValue Water { get; private set; } = null;

        /// <summary>
        /// The specific weekday number, if specififed
        /// </summary>
        public WeekdayEnum SpecificWeekday { get; private set; } = null;

        /// <summary>
        /// The number of times this day is repeated weekly
        /// </summary>
        public int WeeklyOccurrances { get; private set; } = 0;

        /// <summary>
        /// Diet day type
        /// </summary>
        public DietDayTypeEnum DietDayType { get; private set; } = null;



        #region Ctors

        private DietPlanDay(
            string name = null,
            int weeklyOccurrances = 0,
            MacronutirentWeightValue carbs = null,
            MacronutirentWeightValue fats = null,
            MacronutirentWeightValue proteins = null,
            MicronutirentWeightValue salt = null,
            VolumeValue water = null,
            WeekdayEnum specificWeekday = null,
            DietDayTypeEnum dayType = null)
        {
            Name = name ?? string.Empty;
            Carbs = carbs;
            Fats = fats;
            Proteins = proteins;
            Salt = salt;
            Water = water;
            SpecificWeekday = specificWeekday;
            DietDayType = dayType;
            WeeklyOccurrances = weeklyOccurrances;

            Calories = DietAmountsCalculator.ComputeCalories(Carbs, Fats, Proteins);

            if (CheckNullState())
                throw new GlobalDomainGenericException($"Cannot create a {GetType().Name} with all NULL fields");

        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="name">The name which dientifies the day</param>
        /// <param name="carbs">Carbohidrates quantity</param>
        /// <param name="fats">Fats quantity</param>
        /// <param name="proteins">Proteins quantity</param>
        /// <param name="salt">Salt quantity</param>
        /// <param name="water">Water quantity</param>
        /// <param name="specificWeekday">The specific weekday number - or null</param>
        /// <param name="dayType">The day type</param>
        /// <returns>The DietPlanDayValue instance</returns>
        public static DietPlanDay AddDayToPlan(
            string name = null,
            int weeklyOccurrances = 0,
            MacronutirentWeightValue carbs = null,
            MacronutirentWeightValue fats = null,
            MacronutirentWeightValue proteins = null,
            MicronutirentWeightValue salt = null,
            VolumeValue water = null,
            WeekdayEnum specificWeekday = null,
            DietDayTypeEnum dayType = null)

            => new DietPlanDay(name, weeklyOccurrances, carbs, fats, proteins, salt, water, specificWeekday, dayType);

        #endregion



        #region Business Methods

        /// <summary>
        /// Change the name
        /// </summary>
        /// <param name="newName">The new name</param>
        public void Rename(string newName) => Name = newName ?? string.Empty;

        /// <summary>
        /// Set the carbohidrates value
        /// </summary>
        /// <param name="newValue"></param>
        public void PlanCarbs(MacronutirentWeightValue newValue) => Carbs = newValue;


        /// <summary>
        /// Set the proteins value
        /// </summary>
        /// <param name="newValue"></param>
        public void PlanPros(MacronutirentWeightValue newValue) => Proteins = newValue;


        /// <summary>
        /// Set the fats value
        /// </summary>
        /// <param name="newValue"></param>
        public void PlanFats(MacronutirentWeightValue newValue) => Fats = newValue;


        /// <summary>
        /// Set the salt value
        /// </summary>
        /// <param name="newValue"></param>
        public void PlanSalt(MicronutirentWeightValue newValue) => Salt = newValue;


        /// <summary>
        /// Set the water value
        /// </summary>
        /// <param name="newValue"></param>
        public void PlanWater(VolumeValue newValue) => Water = newValue;


        /// <summary>
        /// Schedule this day
        /// </summary>
        /// <param name="newValue"></param>
        public void ScheduleToSpecificDay(WeekdayEnum newValue) => SpecificWeekday = newValue;


        /// <summary>
        /// Set the day type
        /// </summary>
        /// <param name="newValue"></param>
        public void SetDayType(DietDayTypeEnum newValue) => DietDayType = newValue;


        /// <summary>
        /// Checks whether all the properties are null
        /// </summary>
        /// <returns>True if no there are no non-null properties</returns>
        public bool CheckNullState()
            => GetAtomicValues().All(x => Nullable.GetUnderlyingType(x.GetType()) != null && x is null);

        #endregion



        protected IEnumerable<object> GetAtomicValues()
        {
            yield return Name;
            yield return Carbs;
            yield return Fats;
            yield return Proteins;
            yield return Salt;
            yield return Water;
            yield return SpecificWeekday;
            yield return DietDayType;
            yield return WeeklyOccurrances;
        }

    }
}
