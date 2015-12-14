// -----------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Aliyun-inc">
// Utilities
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using AliCloudOpenSearch.com.API.Modal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AliCloudOpenSearch.com.API
{
    /// <summary>
    ///     静态Helper类，提供调用阿里云Web API必要的helper函数。
    ///     Helper class, support helper method like calc Md5, get rid url and so on.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        ///     DateTime实例代表1970年1月1日零时。
        /// </summary>
        public static DateTime timeStamp = new DateTime(1970, 1, 1);

        /// <summary>
        ///     Convert result to a modal
        /// </summary>
        /// <param name="rawResult">response raw string</param>
        /// <returns>Converted modal instance</returns>
        public static Response ConvertResult(string rawResult)
        {
            return JsonConvert.DeserializeObject<Response>(rawResult);
        }

        /// <summary>
        ///     计算string的Md5值。
        ///     Calculate the MD5 hash of a specified string
        /// </summary>
        /// <param name="s">
        ///     用来计算Md5的string
        ///     source string to get md5 hash
        /// </param>
        /// <returns>
        ///     计算得到的md5 hash值
        ///     Md5 hash string of the param 's'
        /// </returns>
        public static string CalcMd5(string s)
        {
            var md5 = MD5.Create();
            var byteString = Encoding.UTF8.GetBytes(s);
            var resultByteArray = md5.ComputeHash(byteString);
            var resultBuilder = new StringBuilder();
            foreach (var byt in resultByteArray)
            {
                resultBuilder.Append(byt.ToString("x2"));
            }

            return resultBuilder.ToString();
        }

        /// <summary>
        ///     为NameValueCollection类提供扩展方法。
        ///     将实例的内容转换为JProperty数组。
        /// </summary>
        /// <param name="collection">非空的Name-value对</param>
        /// <returns>将collection中的名称-键值对封装成Jason property数组</returns>
        public static JProperty[] ToJProperty(NameValueCollection collection)
        {
            var jProperties = new List<JProperty>();
            foreach (var key in collection.AllKeys)
            {
                jProperties.Add(new JProperty(key, collection[key]));
            }

            return jProperties.ToArray();
        }


        /// <summary>
        ///     获取unix timestamp
        /// </summary>
        /// <returns></returns>
        public static long getUnixTimeStamp()
        {
            var span = DateTime.UtcNow - timeStamp;
            return Convert.ToInt64(span.TotalMilliseconds);
        }

        public static void Guard(object obj)
        {
            if (obj == null)
            {
                throw new Exception("Parameter cannot be null");
            }
        }

        public static void Guard(Func<bool> verifyFunc, string errMessage)
        {
            if (!verifyFunc())
            {
                throw new Exception(errMessage);
            }
        }

        public static void Guard<T>(IList<T> obj)
        {
            if (obj == null || obj.Count == 0)
            {
                throw new Exception("Parameter cannot be null or empty");
            }
        }

        public static void Guard<T>(T[] obj)
        {
            if (obj == null || obj.Length == 0)
            {
                throw new Exception("Parameter cannot be null or empty");
            }
        }

        /// <summary>
        ///     将日期转换成long型的。
        /// </summary>
        /// <returns></returns>
        public static long time()
        {
            return Convert.ToInt64((double) (DateTime.UtcNow.Ticks - timeStamp.Ticks)/10000000);
        }

        /// <summary>
        ///     PHP函数http_build_query的.NET版本实现。
        /// </summary>
        /// <param name="data">名称-键值对</param>
        /// <returns>拼装的参数-值字符串</returns>
        //public static string http_build_query(NameValueCollection data)
        //{
        //    if (data == null || data.Count == 0)
        //    {
        //        return string.Empty;
        //    }

        //    StringBuilder sb = new StringBuilder();
        //    foreach (string key in data.Keys)
        //    {
        //        sb.Append("&").Append(Utilities.UrlEncode(key)).Append("=").Append(Utilities.UrlEncode(data[key]));
        //    }

        //    return sb.ToString().Substring(1);
        //}
        public static string http_build_query(Dictionary<string, object> data)
        {
            return HttpBuildQueryHelper.FormatDictionary(data);
        }

        /// <summary>
        ///     替换Uri中的转义字符。
        /// </summary>
        /// <param name="value">Uri中的字符串</param>
        /// <returns>转义后的字符串</returns>
        private static string UrlEncode(string value)
        {
            value = HttpUtility.UrlEncode(value);
            value = Regex.Replace(value, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());
            value = value.Replace("(", "%28")
                .Replace(")", "%29")
                .Replace("$", "%24")
                .Replace("*", "%2A")
                .Replace("'", "%26");
            value = value.Replace("%7E", "~");
            return value;
        }

        /// <summary>
        ///     PHP函数Preg_Replace的.NET实现。替换多个正则表达式的匹配项。
        /// </summary>
        /// <param name="pattern">要替换的内容的正则表达式</param>
        /// <param name="replacements">用于替换匹配字符串的内容</param>
        /// <param name="input">原始字符串数据</param>
        /// <returns>替换后的结果</returns>
        public static string PregReplace(string[] pattern, string[] replacements, string input)
        {
            if (replacements.Length != pattern.Length)
            {
                throw new ArgumentException("Replacement and Pattern length should be same.");
            }

            for (var i = 0; i < pattern.Length; i++)
            {
                input = Regex.Replace(input, pattern[i], replacements[i]);
            }

            return input;
        }

        /// <summary>
        ///     将NameValueCollection按key排序
        /// </summary>
        /// <param name="col">原始Namevaluecollection实例</param>
        /// <returns>排序后的NameValueCollection实例</returns>
        public static Dictionary<string, object> KeySort(Dictionary<string, object> col, int case_sensitive = 0)
        {
            var resultCollection = new Dictionary<string, object>();
            var lst = new List<string>(col.Keys);
            lst.Sort(StringComparer.Ordinal);

            foreach (var k in lst)
            {
                resultCollection.Add(k, col[k]);
            }

            return resultCollection;
        }

        /// <summary>
        ///     PHP函数RTrim的.NET实现。去掉字符串末的制定字符串
        /// </summary>
        /// <param name="source">原始字符串</param>
        /// <param name="trimStr">需要去掉的字符串</param>
        /// <returns>结果</returns>
        public static string RTrim(string source, string trimStr)
        {
            if (source.EndsWith(trimStr))
            {
                source = source.Remove(source.LastIndexOf(trimStr));
            }

            return source;
        }

        /// <summary>
        ///     将字符串数组用制定字符串连接
        /// </summary>
        /// <param name="array">原始字符串数组</param>
        /// <param name="connectString">连接字符串</param>
        /// <returns>连接后的结果</returns>
        public static string Implode(string[] array, string connectString)
        {
            if (null != array && array.Length > 0)
            {
                var i = 0;
                var sb = new StringBuilder(array[i++]);
                while (i < array.Length)
                {
                    sb.Append(connectString + array[i++]);
                }

                return sb.ToString();
            }
            return null;
        }

        /// <summary>
        ///     String[] 的扩展方法，判断数组中是否包含某个string
        /// </summary>
        /// <param name="strArray"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool Contains(string[] strArray, string str)
        {
            foreach (var s in strArray)
            {
                if (s.Equals(str))
                {
                    return true;
                }
            }

            return false;
        }
    }
}