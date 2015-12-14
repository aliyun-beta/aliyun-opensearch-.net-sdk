using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AliCloudOpenSearch.com.API
{
    public class CloudsearchSuggest
    {
        /// <summary>
        ///     The working index name
        /// </summary>
        private readonly string _applicationName;

        /// <summary>
        ///     CloudsearchClient instance
        /// </summary>
        private readonly CloudsearchApi _client;

        /// <summary>
        ///     Url sub path
        /// </summary>
        private readonly string _path;

        /// <summary>
        ///     Suggest name
        /// </summary>
        private readonly string _suggestNamel;

        /// <summary>
        ///     Constructor function
        /// </summary>
        /// <param name="applicationName">The index name which working in</param>
        /// <param name="suggestName">The working suggestName name</param>
        /// <param name="client">CloudsearchClient instance</param>
        public CloudsearchSuggest(string applicationName, string suggestName, CloudsearchApi client)
        {
            _applicationName = applicationName;
            _client = client;
            _suggestNamel = suggestName;
            _path = "/suggest";
        }

        /// <summary>
        ///     Return the suggestion accoring the query and specified hit
        /// </summary>
        /// <param name="query">Search query without index name</param>
        /// <param name="hit">Return size</param>
        /// <returns></returns>
        public string[] GetSuggest(string query, int hit)
        {
            var parameters = new Dictionary<string, object>();

            parameters.Add("query", query);
            parameters.Add("index_name", _applicationName);
            parameters.Add("suggest_name", _suggestNamel);
            parameters.Add("hit", hit);
            var json = JObject.Parse(_client.ApiCall(_path, parameters, "GET"));

            if (json["suggestions"] != null)
            {
                return ((JArray) json["suggestions"]).Select(x => x["suggestion"].ToString()).ToArray();
            }

            return null;
        }
    }
}