using System;
using GymProject.Domain.SharedKernel;

namespace GymProject.Domain.Utils
{
    public static class BodyMeasuresCalculator
    {






        /// <summary>
        /// Compute the BF from the FM
        /// </summary>
        /// <param name="weight">The weight</param>
        /// <param name="fatMass">The Fat Mass</param>
        /// <returns>The BF [%], NULL if invalid input</returns>
        public static BodyFatValue ComputeBodyFat(BodyWeightValue weight, BodyWeightValue fatMass)
        {
            // Input not provided
            if (weight == null || fatMass == null)
                return null;

            if (!weight.Unit.Equals(fatMass.Unit))
                return null;

            return BodyFatValue.MeasureBodyFat(fatMass.Value / weight.Value * 100);
        }


        /// <summary>
        /// Compute the BF from the FFM
        /// </summary>
        /// <param name="weight">The weight</param>
        /// <param name="fatMass">The Fat Free Mass</param>
        /// <returns>The BF [%], NULL if invalid input</returns>
        public static BodyFatValue ComputeBodyFatFFM(BodyWeightValue weight, BodyWeightValue fatFreeMass)
        {
            // Input not provided
            if (weight == null || fatFreeMass == null)
                return null;

            if (!weight.Unit.Equals(fatFreeMass.Unit))
                return null;

            return BodyFatValue.MeasureBodyFat((1f - fatFreeMass.Value / weight.Value) * 100f);
        }


        /// <summary>
        /// Compute the BMI
        /// </summary>
        /// <param name="height">The height [cm/inches]</param>
        /// <param name="weight">the weight [Kg/lbs]</param>
        /// <returns>The BMI as a ratio, NULL if invalid input</returns>
        public static PercentageValue ComputeBodyMassIndex(BodyMeasureValue height, BodyWeightValue weight)
        {
            // Input not provided
            if (weight == null || height == null)
                return null;

            MeasurmentSystemEnum measSystem = height.Unit.MeasureSystemType;

            if (!weight.Unit.MeasureSystemType.Equals(measSystem))
                return null;

            if(measSystem.IsMetric())
                return PercentageValue.MeasureRatio(weight.Value / (float)Math.Pow(height.Value, 2) * 10000f);

            if(measSystem.IsImperial())
                return PercentageValue.MeasureRatio(weight.Value / (float)Math.Pow(height.Value, 2) * 703f);

            return null;
        }


        /// <summary>
        /// Compute the FM
        /// </summary>
        /// <param name="weight">The weight</param>
        /// <param name="bodyfat">The bodyfat [%]</param>
        /// <returns>The FM - the measure unit is fixed according to the weight - NULL if invalid input</returns>
        public static BodyWeightValue ComputeFatMass(BodyWeightValue weight, BodyFatValue bodyfat)
        {
            if (weight == null || bodyfat == null)
                return null;

            return BodyWeightValue.Measure(weight.Value * bodyfat.AsRatio().Value, weight.Unit);
        }


        /// <summary>
        /// Compute the FFM
        /// </summary>
        /// <param name="weight">The weight</param>
        /// <param name="bodyfat">The bodyfat [%]</param>
        /// <returns>The FFM - the measure unit is fixed according to the weight - NULL if invalid input</returns>
        public static BodyWeightValue ComputeFatFreeMass(BodyWeightValue weight, BodyFatValue bodyfat)
        {
            if (weight == null || bodyfat == null)
                return null;

            return BodyWeightValue.Measure(weight.Value * (1 - bodyfat.AsRatio().Value), weight.Unit);
        }


        /// <summary>
        /// Compute the FFM
        /// </summary>
        /// <param name="weight">The weight</param>
        /// <param name="fatMass">The FM</param>
        /// <returns>The FFM - the measure unit is fixed according to the other ones - NULL if invalid input</returns>
        public static BodyWeightValue ComputeFatFreeMass(BodyWeightValue weight, BodyWeightValue fatMass)
        {
            if (weight == null || fatMass == null)
                return null;

            if (!weight.Unit.Equals(fatMass.Unit))
                return null;

            return BodyWeightValue.Measure(weight.Value - fatMass.Value, weight.Unit);
        }


    }
}