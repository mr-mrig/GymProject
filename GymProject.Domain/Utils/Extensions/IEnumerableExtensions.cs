using System;
using System.Collections.Generic;
using System.Linq;

namespace GymProject.Domain.Utils.Extensions
{
    public static class IEnumerableExtensions
    {



        public static IEnumerable<T> Clone<T>(this IEnumerable<T> toClone) where T : ICloneable

            => toClone?.Select(x => (T)x?.Clone());
    }
}
