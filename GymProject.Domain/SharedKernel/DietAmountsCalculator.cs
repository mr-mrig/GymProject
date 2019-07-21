using System;
using System.Collections.Generic;
using System.Text;

namespace GymProject.Domain.SharedKernel
{
    public static class DietAmountsCalculator
    {


        private const float ProteinsKcal = 4;
        private const float CarbsKcal = 4;
        private const float FatsKcal = 9;
        private const float FibersKcal = 2;




        /// <summary>
        /// Compute the calories from the macronutirents amounts, fibers included
        /// </summary>
        /// <param name="carbs">The carbohydrates amount</param>
        /// <param name="fats">The fats amount</param>
        /// <param name="proteins">The proteins amount</param>
        /// <returns>The CalorieValue or NULL if invalid input</returns>
        public static CalorieValue ComputeCalories(MacronutirentWeightValue carbs, MacronutirentWeightValue fats, MacronutirentWeightValue proteins, MacronutirentWeightValue fibers)
            => ComputeCalories(carbs, fats, proteins) + CalorieValue.Measure(FibersKcal * fibers.Value);


        /// <summary>
        /// Compute the calories from the macronutirents amounts
        /// </summary>
        /// <param name="carbs">The carbohydrates amount</param>
        /// <param name="fats">The fats amount</param>
        /// <param name="proteins">The proteins amount</param>
        /// <returns>The CalorieValue or NULL if invalid input</returns>
        public static CalorieValue ComputeCalories(MacronutirentWeightValue carbs, MacronutirentWeightValue fats, MacronutirentWeightValue proteins)
        {
            if (carbs == null || fats == null || proteins == null)
                return null;

            if (!(carbs.Unit.Equals(fats.Unit) && fats.Unit.Equals(proteins.Unit)))
                return null;

            return CalorieValue.MeasureKcal(CarbsKcal * carbs.Value + ProteinsKcal * proteins.Value + FatsKcal * fats.Value);
        }



        /// <summary>
        /// Compute the macros amount with respect to the distribution ratio and calorie amount
        /// </summary>
        /// <param name="carbsPerc">The carbohydrates percentage/ratio</param>
        /// <param name="fatsPerc">The fats percentage/ratio</param>
        /// <param name="proteinsPerc">The proteins percentage/ratio</param>
        /// <param name="calories">The total calories amount</param>
        /// <param name="macroUnit">The measure unit for the macro - grams defualt</param>
        /// <returns>The macros amount as a list [carbs, fats, pros] or NULL if invalid input</returns>
        public static IEnumerable<MacronutirentWeightValue> MacrosFromDistribution(PercentageValue carbsPerc, PercentageValue fatsPerc, PercentageValue proteinsPerc, CalorieValue calories, WeightUnitMeasureEnum macroUnit = null)
        {
            if (carbsPerc == null || fatsPerc == null || proteinsPerc == null || calories == null)
                return null;

            if (carbsPerc.Unit.Equals(PercentageMeasureUnitEnum.Percentage))
                carbsPerc = carbsPerc.Convert(PercentageMeasureUnitEnum.Ratio);

            if (fatsPerc.Unit.Equals(PercentageMeasureUnitEnum.Percentage))
                fatsPerc = fatsPerc.Convert(PercentageMeasureUnitEnum.Ratio);

            if (proteinsPerc.Unit.Equals(PercentageMeasureUnitEnum.Percentage))
                proteinsPerc = proteinsPerc.Convert(PercentageMeasureUnitEnum.Ratio);

            if (macroUnit == null)
                macroUnit = WeightUnitMeasureEnum.Grams;

            return new List<MacronutirentWeightValue>()
            {
                MacronutirentWeightValue.Measure(calories.Value * carbsPerc.Value / CarbsKcal, macroUnit),
                MacronutirentWeightValue.Measure(calories.Value * fatsPerc.Value / CarbsKcal, macroUnit),
                MacronutirentWeightValue.Measure(calories.Value * proteinsPerc.Value / CarbsKcal, macroUnit),
            };
        }
    }
}
