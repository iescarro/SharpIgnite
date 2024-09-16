using System;
using System.Configuration;

namespace SharpIgnite
{
    public static class Config
    {
        public static string Get(string key)
        {
            return Get(key, null);
        }

        public static string Get(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (value == null) {
                return defaultValue;
            }
            return value;
        }

        public static T Get<T>(string key)
        {
            return Get<T>(key, default(T));
        }

        public static T Get<T>(string key, T defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (value == null) {
                return defaultValue;
            }

            // If T is Nullable<T>, handle conversion accordingly
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>)) {
                // Convert to underlying type
                Type underlyingType = Nullable.GetUnderlyingType(typeof(T));
                return (T)Convert.ChangeType(value, underlyingType);
            }

            // Convert to non-nullable type
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
