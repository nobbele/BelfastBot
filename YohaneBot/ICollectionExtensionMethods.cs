using System.Collections.Generic;

namespace YohaneBot
{
    public static class ICollectionExtensionMethods
    {
        public static T AddGet<T>(this ICollection<T> me, T item)
        {
            me.Add(item);
            return item;
        }
    }
}