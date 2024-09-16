namespace SharpIgnite
{
    public static class Str
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotNullAndEmpty(this string str)
        {
            return !str.IsNullOrEmpty();
        }

        public static string Capitalize(this string str)
        {
            if (str.Length > 0) {
                var firstCharacter = str[0].ToString().ToUpper();
                return firstCharacter + str.Substring(1, str.Length - 1);
            }
            return "";
        }
    }
}
