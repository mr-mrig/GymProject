using System;

namespace GymProject.Domain.Utils
{
    public static class CommonUtilities
    {


        /// <summary>
        /// Round the input number to the nearest 0.5. IE: 3.1 -> 3.0, 3.4 -> 3.5
        /// </summary>
        /// <param name="number">The number to be rounded</param>
        /// <returns>The rounded value</returns>
        public static double RoundToPointFive(double number) => Math.Round(number * 2, MidpointRounding.AwayFromZero) / 2;
    }
}
