using System.Collections.Generic;
using System.Linq;
using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using GymProject.Domain.SharedKernel.Exceptions;

namespace GymProject.Domain.FitnessJournalDomain.FitnessDayAggregate
{
    public class DailyDietValue : ValueObject
    {

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
        /// Salt quantity
        /// </summary>
        public MicronutirentWeightValue Salt { get; private set; } = null;

        /// <summary>
        /// Water quantity
        /// </summary>
        public VolumeValue Water { get; private set; } = null;

        /// <summary>
        /// Day with free meal
        /// </summary>
        public bool? IsFreeMeal { get; private set; } = null;

        /// <summary>
        /// Diet day type
        /// </summary>
        public DietDayTypeEnum DietDayType { get; private set; } = null;




        #region Ctors

        private DailyDietValue(
            MacronutirentWeightValue carbs = null,
            MacronutirentWeightValue fats = null,
            MacronutirentWeightValue proteins = null,
            MicronutirentWeightValue salt = null,
            VolumeValue water = null,
            bool? isFreeMeal = null,
            DietDayTypeEnum dayType = null)
        {
            Carbs = carbs;
            Fats = fats;
            Proteins = proteins;
            Salt = salt;
            Water = water;
            IsFreeMeal = isFreeMeal;
            DietDayType = dayType;

            if (CheckNullState())
                throw new GlobalDomainGenericException($"Cannot create a {GetType().Name} with all NULL fields");

            //// Here so it is excluded from the CheckNullState
            //DietDayType = dayType;
        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="carbs">Carbohidrates quantity</param>
        /// <param name="fats">Fats quantity</param>
        /// <param name="proteins">Proteins quantity</param>
        /// <param name="salt">Salt quantity</param>
        /// <param name="water">Water quantity</param>
        /// <param name="isFreeMeal">Whether it has a free meal or not</param>
        /// <param name="dayType">The day type</param>
        /// <returns>The DailyDietValue instance</returns>
        public static DailyDietValue TrackDiet(
            MacronutirentWeightValue carbs = null,
            MacronutirentWeightValue fats = null,
            MacronutirentWeightValue proteins = null,
            MicronutirentWeightValue salt = null,
            VolumeValue water = null,
            bool? isFreeMeal = null,
            DietDayTypeEnum dayType = null)
        
            => new DailyDietValue(carbs, fats, proteins, salt, water, isFreeMeal, dayType);
        
        #endregion



        #region Business Methods


        /// <summary>
        /// Checks whether all the properties are null
        /// </summary>
        /// <returns>True if no there are no non-null properties</returns>
        public bool CheckNullState() 
            => GetAtomicValues().All(x => x is null);

        #endregion



        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Carbs;
            yield return Fats;
            yield return Proteins;
            yield return Salt;
            yield return Water;
            yield return IsFreeMeal;
            yield return DietDayType;
        }


    }
}
