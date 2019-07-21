using GymProject.Domain.Base;
using GymProject.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Domain.DietDomain
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
            get => _dietDays.ToList().AsReadOnly();
        }




        #region Ctors

        private DietPlanUnit(DateRangeValue unitPeriod, ICollection<DietPlanDay> dietDays = null)
        {
            PeriodScheduled = unitPeriod;

            if(dietDays != null)
1            {
                _dietDays = new List<DietPlanDay>();
                AvgDailyCalories = DietAmountsCalculator
            }

            if (CheckNullState())
                throw new GlobalDomainGenericException($"Cannot create a {GetType().Name} with all NULL fields");

        }
        #endregion



        #region Factories

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="period">The perdiod to be scheduled</param>
        /// <returns>The DietPlanUnitValue instance</returns>
        public static DietPlanUnit ScheduleDietUnit(DateRangeValue period) => new DietPlanUnit(period);


        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="period">The perdiod to be scheduled</param>
        /// <param name="dietDays">The diet days linked to this unit</param>
        /// <returns>The DietPlanUnitValue instance</returns>
        public static DietPlanUnit ScheduleDietUnit(DateRangeValue period, ICollection<DietPlanDay> dietDays) => new DietPlanUnit(period, dietDays);

        #endregion



        #region Business Methods

        /// <summary>
        /// Reschedule the diet unit
        /// </summary>
        /// <param name="newValue">The new period</param>
        public void Reschedule(DateRangeValue newPeriod) => PeriodScheduled = newPeriod;


        /// <summary>
        /// Extend/shorten the period by changing the end date
        /// </summary>
        /// <param name="newEndDate">The new end date</param>
        public void ChangeEndDate(DateTime newEndDate) => PeriodScheduled = PeriodScheduled.RescheduleEnd(newEndDate);


        /// <summary>
        /// Extend/shorten the period by changing the start date
        /// </summary>
        /// <param name="newStartDate">The new start date</param>
        public void ChangeStartDate(DateTime newStartDate) => PeriodScheduled = PeriodScheduled.RescheduleEnd(newStartDate);


        /// <summary>
        /// Assigns the diet days
        /// </summary>
        /// <param name="newDays">The new end date</param>
        public void AssignDietDays(ICollection<DietPlanDay> newDays)
        {
            _dietDays = newDays;
            AvgDailyCalories
        }


        /// <summary>
        /// Schedule a new diet day
        /// </summary>
        /// <param name="newDay">The day to be added</param>
        public void ScheduleNewDay(DietPlanDay newDay)
                {
            _dietDays.Add(newDay);
            AvgDailyCalories
    }


    /// <summary>
    /// Remove the selected diet day
    /// </summary>
    /// <param name="toRemove">The day to be added</param>
    public void UnscheduleDay(DietPlanDay toRemove)
                       {
            _dietDays.Remove(toRemove);
            AvgDailyCalories
    }

    /// <summary>
    /// Checks whether all the properties are null
    /// </summary>
    /// <returns>True if no there are no non-null properties</returns>
    public bool CheckNullState()
            => GetAtomicValues().All(x => x is null);

        #endregion



        protected IEnumerable<object> GetAtomicValues()
        {
            yield return PeriodScheduled;
            yield return DietDays;
        }

    }
}
