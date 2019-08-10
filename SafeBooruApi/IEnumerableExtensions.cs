using System;
using System.Collections.Generic;
using System.Linq;

namespace SafeBooruApi
{
    public static class IEnumerableExtensions
    {
        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            var r = new Random();
            var list = enumerable as IList<T> ?? enumerable.ToList();
            return list.ElementAt(r.Next(0, list.Count()));
        }
    }
}
