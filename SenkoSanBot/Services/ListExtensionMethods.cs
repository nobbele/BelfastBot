using System.Collections.Generic;

namespace SenkoSanBot.Services
{
    public static class ListExtensionMethods
    {
        public static T AddGet<T>(this List<T> me, T item)
        {
            me.Add(item);
            return item;
        }
    }
}