using System;

namespace SharpIgnite
{
    public static class StringHelper
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
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
