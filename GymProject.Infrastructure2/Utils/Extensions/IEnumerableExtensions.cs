using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GymProject.Infrastructure.Utils.Extensions
{
    public static class IEnumerableExtensions
    {



        public static IEnumerable<T> Clone<T>(this IEnumerable<T> toClone) where T : ICloneable

            => toClone.Select(x => (T)x.Clone());
    }
}
