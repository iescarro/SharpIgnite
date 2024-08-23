using System;
using System.Configuration;

namespace SharpIgnite
{
    public static class Config
    {
        public static string Item(string key)
        {
            return Item(key, "");
        }
        
        public static string Item(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (value == null) {
                return defaultValue;
            }
            return value;
        }
        
        public static T Item<T>(string key)
        {
            return Item<T>(key, default(T));
        }
        
        public static T Item<T>(string key, T defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (value == null) {
                return defaultValue;
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
