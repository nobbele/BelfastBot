using System.Collections.Generic;

namespace Common
{
    public static class StringExtensionMethods
    {
        public static string NothingIfCheckNullOrEmpty(this string newText, string check) => string.IsNullOrEmpty(check) ? string.Empty : newText;
        public static string IfTargetIsNullOrEmpty(this string newText, string target) => string.IsNullOrEmpty(target) ? newText : target;

        public static string ShortenText(this string text, int limit = 256)
        {
            if (text.Length > limit)
                text = text?.Substring(0, limit) + "...";

            return text;
        }

        /// <summary>
        /// Assumes no string between 2 delimeters is longer than n
        /// </summary>
        public static string[] NCharLimitToClosestDelimeter(this string str, int n, string delim)
        {
            List<string> list = new List<string>(str.Length / n);

            while (str.Length > n)
            {
                int lastIndexToClosestDelim = 0;
                int indexToClosestDelim = 0;
                while ((indexToClosestDelim = str.IndexOf(delim, indexToClosestDelim + delim.Length)) < n)
                {
                    if (indexToClosestDelim == -1)
                        break;

                    lastIndexToClosestDelim = indexToClosestDelim;
                }

                list.Add(str.Substring(0, lastIndexToClosestDelim));
                str = str.Remove(0, lastIndexToClosestDelim + delim.Length);
            }

            list.Add(str);

            return list.ToArray();
        }
    }
}