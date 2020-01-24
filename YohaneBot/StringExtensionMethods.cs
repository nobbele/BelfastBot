namespace YohaneBot
{
    public static class StringExtensionMethods
    {
        public static string NothingIfCheckNullOrEmpty(this string newText, string check) => string.IsNullOrEmpty(check) ? string.Empty : newText;
        public static string IfTargetIsNullOrEmpty(this string newText, string target) => string.IsNullOrEmpty(target) ? newText : target;
    }
}