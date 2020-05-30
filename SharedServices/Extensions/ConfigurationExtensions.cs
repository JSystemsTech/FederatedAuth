using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace SharedServices.Extensions
{
    public static class ConfigurationExtensions
    {

        public static bool ContainsKey(this NameValueCollection collection, string key)
            => collection.Get(key) == null ? collection.AllKeys.Contains(key) : true;
        public static string GetValue(this NameValueCollection collection, string key, string defaultValue = null)
            => collection.ContainsKey(key) ? collection[key] : defaultValue;

        public static void SetValue(this NameValueCollection collection, string key, object value)
        {
            if (value != null)
            {
                collection.Set(key, value.ToString());
            }
        }

        public static IEnumerable<string> GetEnumerable(this NameValueCollection collection, string key, char delimeter = ',')
        {
            string value = collection.GetValue(key);
            return !string.IsNullOrWhiteSpace(value) ? value.Split(delimeter) : new string[0];
        }
        public static string GetValueHtml(this NameValueCollection collection, string key)
        {
            string value = collection.GetValue(key);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return HttpUtility.HtmlDecode(value);
            }
            return null;
        }
        public static bool GetValueBool(this NameValueCollection collection, string key, bool defaultValue = false)
        {
            string value = collection.GetValue(key);
            bool parsedValue;
            bool canParse = bool.TryParse(value, out parsedValue);
            return canParse ? parsedValue : defaultValue;
        }
        public static int GetValueInt(this NameValueCollection collection, string key, int defaultValue = default)
        {
            string value = collection.GetValue(key);
            int parsedValue;
            bool canParse = int.TryParse(value, out parsedValue);
            return canParse ? parsedValue : defaultValue;
        }
        public static int? GetValueInt(this NameValueCollection collection, string key)
        {
            string value = collection.GetValue(key);
            int parsedValue;
            bool canParse = int.TryParse(value, out parsedValue);
            return canParse ? parsedValue : (int?)null;
        }
        public static double GetValueDouble(this NameValueCollection collection, string key, double defaultValue = default)
        {
            string value = collection.GetValue(key);
            double parsedValue;
            bool canParse = double.TryParse(value, out parsedValue);
            return canParse ? parsedValue : defaultValue;
        }
        public static float GetValueFloat(this NameValueCollection collection, string key, float defaultValue = default)
        {
            string value = collection.GetValue(key);
            float parsedValue;
            bool canParse = float.TryParse(value, out parsedValue);
            return canParse ? parsedValue : defaultValue;
        }
        public static Uri GetValueUri(this NameValueCollection collection, string key)
        {
            string value = collection.GetValue(key);
            if(string.IsNullOrWhiteSpace(value))
            {
                return default;
            }
            try
            {
                return new Uri(value);
            }
            catch
            {
                return default;
            }
        }
    }
}
