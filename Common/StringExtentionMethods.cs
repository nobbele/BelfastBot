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
    }
}
