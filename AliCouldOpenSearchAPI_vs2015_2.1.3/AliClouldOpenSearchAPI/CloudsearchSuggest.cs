using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AliCloudOpenSearch.com.API
{
    public class CloudsearchSuggest
    {
        /// <summary>
        /// Url sub path
        /// </summary>
        private string _path;
        /// <summary>
        /// The working index name
        /// </summary>
        private String _applicationName;

        /// <summary>
        /// Suggest name
        /// </summary>
        private string _suggestNamel;
        /// <summary>
        /// CloudsearchClient instance
        /// </summary>
        private CloudsearchApi _client;

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="applicationName">The index name which working in</param>
        /// <param name="suggestName">The working suggestName name</param>
        /// <param name="client">CloudsearchClient instance</param>
        public CloudsearchSuggest(String applicationName, string suggestName,CloudsearchApi client)
        {
            _applicationName = applicationName;
            _client = client;
            _suggestNamel = suggestName;
            _path = "/suggest";
        }

        /// <summary>
        /// Return the suggestion accoring the query and specified hit
        /// </summary>
        /// <param name="query">Search query without index name</param>
        /// <param name="hit">Return size</param>
        /// <returns></returns>
        public string[] GetSuggest(string query,int hit)
        {
            Dictionary<String, Object> parameters = new Dictionary<String, Object>();

            parameters.Add("query", query);
            parameters.Add("index_name", _applicationName);
            parameters.Add("suggest_name", _suggestNamel);
            parameters.Add("hit", hit);
            JObject json = JObject.Parse(_client.ApiCall(this._path, parameters, "GET"));

            if (json["suggestions"] != null)
            {
                return ((JArray) json["suggestions"]).Select(x => x["suggestion"].ToString()).ToArray();
            }

            return null;
        }
    }
}
