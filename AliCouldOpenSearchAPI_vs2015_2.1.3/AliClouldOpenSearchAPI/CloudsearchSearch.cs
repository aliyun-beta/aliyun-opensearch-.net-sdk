using System.Collections.Generic;
using AliCloudOpenSearch.com.API.Builder;

namespace AliCloudOpenSearch.com.API
{
    /// <summary>
    ///     Provide opensearch search features
    /// </summary>
    public class CloudsearchSearch
    {
        private readonly CloudsearchApi _client;
        private readonly string _path = "/search";

        /// <summary>
        ///     Construct
        /// </summary>
        /// <param name="client">apiclient instance</param>
        public CloudsearchSearch(CloudsearchApi client)
        {
            _client = client;
        }

        /// <summary>
        ///     Execute search request
        /// </summary>
        /// <param name="queryBuilder">QueryBuilder instance</param>
        /// <returns>Search result</returns>
        public string Search(QueryBuilder queryBuilder)
        {
            Utilities.Guard(queryBuilder);

            return _client.ApiCall(_path, queryBuilder.BuildQueryParameter(), "GET");
        }

        /// <summary>
        ///     To get the search result ASAP, you can use 'Scan' function, and then call 'ScanThen', more detail
        ///     please refer to http://help.aliyun.com/document_detail/opensearch/api-reference/api-interface/search-related.html
        ///     Note: 1. Scan function only supports query and filter clause currenttly 2. You must specify Config clause otherwise
        ///     you will cannot get scroll_id
        /// </summary>
        /// <param name="queryBuilder">Query build</param>
        /// <param name="scroll">
        ///     The expiration time(default value is '1m'), the default unit is 'ms', you can also use '1M' to specify 1 minute
        ///     , the valid time unit contains w=Week,d=Day,h=Hour,m=Minute,s=Second.
        /// </param>
        /// <returns>scroll_id</returns>
        public string Scan(QueryBuilder queryBuilder, string scroll)
        {
            if (scroll == null)
            {
                scroll = "1m";
            }
            var paras = queryBuilder.BuildQueryParameter();
            paras["scroll"] = scroll;
            paras["search_type"] = "scan";

            return _client.ApiCall("/search", paras, "GET");
        }

        /// <summary>
        ///     The trailing method of Scan()
        /// </summary>
        /// <param name="scrollId">scroll id</param>
        /// <returns>Search result</returns>
        public string ScanThen(string scroll, string scrollId)
        {
            var dics = new Dictionary<string, object>();
            dics["scroll"] = scroll;
            dics["scroll_id"] = scrollId;

            return _client.ApiCall("/search", dics, "GET");
        }
    }
}