using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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


        /// <summary>
        /// Creates a new collection which is the same as the input one but does not contain duplicates
        /// </summary>
        /// <typeparam name="T">An object</typeparam>
        /// <param name="src">The source list</param>
        /// <returns>The new list with no duplicates</returns>
        public static ICollection<T> RemoveDuplicatesFrom<T>(ICollection<T> src)
        {
            if (src == null)
                return null;

            ICollection<T> dest = new List<T>();

            // Add the elements only once
            for (int idIndex = 0; idIndex < src.Count(); idIndex++)
            {
                if (!src.SkipWhile((x, i) => i <= idIndex).Contains(src.ElementAt(idIndex)))
                    dest.Add(src.ElementAt(idIndex));
            }
            return dest;
        }
    }
}
