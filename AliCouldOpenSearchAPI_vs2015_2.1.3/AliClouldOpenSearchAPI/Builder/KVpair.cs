using System.Collections.Generic;
using System.Text;

namespace AliCloudOpenSearch.com.API.Builder
{
    /// <summary>
    ///     Used to generate kvpair clause
    /// </summary>
    public class KVpair : IBuilder
    {
        private readonly string _key;

        private readonly Dictionary<string, string> _kvs = new Dictionary<string, string>();
        private readonly string _val;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        public KVpair(string key, string val)
        {
            Utilities.Guard(() => !string.IsNullOrEmpty(key), "key cannot be null or empty");
            Utilities.Guard(() => !string.IsNullOrEmpty(val), "val cannot be null or empty");

            _key = key;
            _val = val;
        }

        /// <summary>
        ///     Generate KVpair instance
        /// </summary>
        /// <returns></returns>
        string IBuilder.BuildQuery()
        {
            var q = new StringBuilder();
            q.Append(_key).Append(":").Append(_val);

            foreach (var k in _kvs.Keys)
            {
                q.Append(",").Append(k).Append(":").Append(_kvs[k]);
            }

            return q.ToString();
        }

        /// <summary>
        ///     Add a kvpair
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="val">value</param>
        /// <returns>KVpait instance</returns>
        public KVpair Add(string key, string val)
        {
            _kvs[key] = val;
            return this;
        }
    }
}