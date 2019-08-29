using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.Utils.Extensions
{
    public static class IEnumerableExtensions
    {



        /// <summary>
        /// Provide the value copy of the input list by cloning its elements.
        /// </summary>
        /// <typeparam name="T">Must implement IClonable</typeparam>
        /// <param name="toClone">The input list</param>
        /// <returns>The copy</returns>
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> toClone) where T : ICloneable

            => toClone?.Select(x => (T)x?.Clone());



        /// <summary>
        /// Provide the value copy of the input list by cloning its elements.
        /// </summary>
        /// <typeparam name="T">Must be a class, must implement IClonable</typeparam>
        /// <param name="toClone">The input list</param>
        /// <returns>The copy</returns>
        public static IEnumerable<T> NoDuplicatesClone<T>(this IEnumerable<T> toClone) where T : class, ICloneable
        {
            ICollection<T> src = Clone(toClone).ToList();
            ICollection<T> dest = new List<T>();

            // Add the elements only once
            for (int idIndex = 0; idIndex < src.Count(); idIndex++)
            {
                if (!src.SkipWhile((x, i) => i <= idIndex).Contains(src.ElementAt(idIndex)))
                    dest.Add(src.ElementAt(idIndex));
            }

            return dest;
        }


        /// <summary>
        /// Provide the value copy of the input list by cloning its elements.
        /// </summary>
        /// <typeparam name="T">Must be a class, must implement IClonable</typeparam>
        /// <param name="inputList">The input list</param>
        /// <returns>The copy</returns>
        public static bool ContainsDuplicates<T>(this IEnumerable<T> inputList) where T : class

            => inputList.Count() != inputList.Distinct().Count();


        ///// <summary>
        ///// Provide the value copy of the input list by cloning its elements.
        ///// </summary>
        ///// <typeparam name="T">Must be a class, must implement IClonable</typeparam>
        ///// <param name="inputList">The input list</param>
        ///// <returns>The copy</returns>
        //public static bool ContainsDuplicates<T>(this IEnumerable<T> inputList, Func<T, T> selector) where T : class

        //    => inputList.GroupBy(selector).Any(g => g.Count() > 1);


        /// <summary>
        /// Provide the value copy of the input list by cloning its elements.
        /// Null values are omitted.
        /// </summary>
        /// <typeparam name="T">Must implement IClonable</typeparam>
        /// <param name="toClone">The input list</param>
        /// <returns>The copy</returns>
        public static IEnumerable<T> NullSafeClone<T>(this IEnumerable<T> toClone) where T : ICloneable

            => Clone<T>(toClone?.Where(x => x != null));


    }
}
