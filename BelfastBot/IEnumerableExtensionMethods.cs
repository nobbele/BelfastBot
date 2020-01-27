using System.Collections.Generic;

namespace BelfastBot
{
    public static class IEnumerableExtensionMethods
    {
        public static string DelimeterSeperatedString<T>(this IEnumerable<T> list, string delimeter) => string.Join(delimeter, list);
        public static string CommaSeperatedString<T>(this IEnumerable<T> list) => list.DelimeterSeperatedString(", ");
        public static string NewLineSeperatedString<T>(this IEnumerable<T> list) => list.DelimeterSeperatedString("\n");
    }
}