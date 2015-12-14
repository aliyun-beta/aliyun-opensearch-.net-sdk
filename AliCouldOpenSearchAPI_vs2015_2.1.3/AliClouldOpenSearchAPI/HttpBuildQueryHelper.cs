using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AliCloudOpenSearch.com.API
{
    public class HttpBuildQueryHelper
    {
        private static string UrlEncode(string str)
        {
            if (str == null)
            {
                return null;
            }
            var stringToEncode =
                HttpUtility.UrlEncode(str)
                    .Replace("+", "%20")
                    .Replace("*", "%2A")
                    .Replace("(", "%28")
                    .Replace(")", "%29")
                    .Replace("!", "%21")
                    .Replace("~", "%7E");
            return stringToEncode;
        }

        public static string Format(object value, string prefix = "q")
        {
            if (value == null) return string.Format("{0}=", prefix);

            var parts = new List<string>();
            HandleItem(value, parts, prefix);
            return string.Join("&", parts.ToArray<string>());
        }

        public static string FormatValue(object value, string prefix = "q")
        {
            var strValue = UrlEncode(value.ToString());
            if (value is bool)
            {
                strValue = (bool) value ? "1" : "0";
            }
            return string.IsNullOrEmpty(strValue) ? string.Empty : string.Format("{0}={1}", prefix, strValue);
        }

        public static string FormatList(IList obj, string prefix = "q")
        {
            var count = obj.Count;
            var parts = new List<string>();

            for (var i = 0; i < count; i++)
            {
                var newPrefix = string.Format("{0}[{1}]", prefix, i);
                HandleItem(obj[i], parts, newPrefix);
            }
            return string.Join("&", parts.ToArray<string>());
        }

        public static string FormatDictionary(IDictionary<string, object> obj, string prefix = "")
        {
            var parts = new List<string>();
            foreach (var entry in obj)
            {
                var newPrefix = string.IsNullOrEmpty(prefix)
                    ? string.Format("{0}{1}", prefix, entry.Key)
                    : string.Format("{0}[{1}]", prefix, entry.Key);
                HandleItem(entry.Value, parts, newPrefix);
            }
            return string.Join("&", parts.ToArray<string>());
        }

        private static void HandleItem(object value, List<string> parts, string prefix)
        {
            if (value == null) return;

            if (IsStringable(value))
            {
                parts.Add(FormatValue(value, prefix));
            }
            else if (value is IList)
            {
                parts.Add(FormatList((IList) value, prefix));
            }
            else if (value is IDictionary<string, object>)
            {
                parts.Add(FormatDictionary((IDictionary<string, object>) value, prefix));
            }
            else
            {
                parts.Add(FormatValue(value, prefix));
            }
        }

        public static object Convert(object obj, int depthLimit = 0)
        {
            if (depthLimit > 5) return obj; // prevent recursion from not ending
            if (obj == null) return obj;
            var type = obj.GetType();
            if (IsStringable(obj) || IsStringableArray(obj)) return obj;

            if (type.IsArray)
            {
                var parts = new List<object>();
                foreach (var e in (IList) obj)
                {
                    parts.Add(Convert(e));
                }
                return parts.ToArray();
            }

            var dict = new Dictionary<string, object>();
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                dict.Add(prop.Name, Convert(prop.GetValue(obj, null), depthLimit + 1));
            }
            return dict;
        }

        private static bool IsStringable(object o)
        {
            return o is bool || o is byte || o is char || o is decimal ||
                   o is double || o is float || o is int || o is long ||
                   o is sbyte || o is short || o is uint || o is ulong ||
                   o is ushort || o is string;
        }

        private static bool IsStringableArray(object o)
        {
            return o is bool[] || o is byte[] || o is char[] || o is decimal[] ||
                   o is double[] || o is float[] || o is int[] || o is long[] ||
                   o is sbyte[] || o is short[] || o is uint[] || o is ulong[] ||
                   o is ushort[] || o is string[];
        }
    }
}