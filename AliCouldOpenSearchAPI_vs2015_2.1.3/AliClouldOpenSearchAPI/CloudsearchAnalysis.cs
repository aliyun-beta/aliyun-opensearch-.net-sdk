using System.Collections.Generic;
#if NET45
using System.Threading.Tasks;
#endif

namespace AliCloudOpenSearch.com.API
{
    public class CloudsearchAnalysis
    {
        /// <summary>
        /// CloudsearchClient 实例。
        /// </summary>
        private readonly CloudsearchApi client;

        /// <summary>
        /// 统计的应用名称
        /// </summary>
        private string indexName;

        /// <summary>
        /// 指定API接口的相对路径。
        /// </summary>
        private readonly string path;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="indexName">指定统计信息的应用名称</param>
        /// <param name="client">此对象由CloudsearchClient类实例化</param>
        public CloudsearchAnalysis(string indexName, CloudsearchApi client)
        { 
            this.indexName = indexName;
            this.client = client;
            path = "/top/query/" + indexName;
        }

        /// <summary>
        /// 通过指定的天数和个数，返回top query信息.
        /// </summary>
        /// <param name="num">指定返回的记录个数</param>
        /// <param name="days">指定统计从昨天开始向前多少天的数据</param>
        /// <returns></returns>
        public string GetTopQuery(int num, int days)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("num", num.ToString());
            parameters.Add("days", days.ToString());

            return client.ApiCall(path, parameters);
        }

#if NET45
        /// <summary>
        /// 通过指定的天数和个数，返回top query信息.
        /// </summary>
        /// <param name="num">指定返回的记录个数</param>
        /// <param name="days">指定统计从昨天开始向前多少天的数据</param>
        /// <returns></returns>
        public async Task<string> GetTopQueryAsync(int num, int days)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("num", num.ToString());
            parameters.Add("days", days.ToString());

            return await client.ApiCallAsync(path, parameters).ConfigureAwait(false);
        }
#endif
    }
}