using System.Collections.Generic;

namespace AliCloudOpenSearch.com.API
{
    public class CloudsearchAnalysis
    {
        /**
         * CloudsearchClient 实例。                                  
         * @var CloudsearchClient                                    
         */
        private readonly CloudsearchApi client;
        /**
   * 统计的应用名称。                                          
   * @var string
   */
        private string indexName;

        /**
         * 指定API接口的相对路径。111                                
         * @var string                                               
         */
        private readonly string path;

        /**
         * 构造函数。
         * @param string $indexName 指定统计信息的应用名称。         
         * @param CloudsearchClient $client 此对象由CloudsearchClient类实例化。
         */

        public CloudsearchAnalysis(string indexName, CloudsearchApi client)
        {
            this.indexName = indexName;
            this.client = client;
            path = "/top/query/" + indexName;
        }


        /**
   * 通过指定的天数和个数，返回top query信息。                 
   * @param int $num 指定返回的记录个数。
   * @param int $days 指定统计从昨天开始向前多少天的数据。     
   * @return string                                             
   */

        public string GetTopQuery(int num, int days)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("num", num.ToString());
            parameters.Add("days", days.ToString());

            return client.ApiCall(path, parameters);
        }
    }
}