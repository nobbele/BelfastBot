using System.Collections.Generic;

namespace Common
{
    public static class StringExtentionMethods
    {
        public static string ShortenText(this string text)
        {
            if (text.Length > 256)
                text = text?.Substring(0, 256) + "...";

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