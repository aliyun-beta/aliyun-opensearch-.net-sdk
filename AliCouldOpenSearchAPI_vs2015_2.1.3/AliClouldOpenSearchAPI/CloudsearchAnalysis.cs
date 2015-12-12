namespace AliCloudOpenSearch.com.API
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json.Linq;
    using System.Collections.Specialized;
    using Newtonsoft.Json;

    public class CloudsearchAnalysis
    {
        /**
   * 统计的应用名称。                                          
   * @var string
   */
        private String indexName;

        /**
         * CloudsearchClient 实例。                                  
         * @var CloudsearchClient                                    
         */
        private CloudsearchApi client;

        /**
         * 指定API接口的相对路径。111                                
         * @var string                                               
         */
        private String path;

        /**
         * 构造函数。
         * @param string $indexName 指定统计信息的应用名称。         
         * @param CloudsearchClient $client 此对象由CloudsearchClient类实例化。
         */
        public CloudsearchAnalysis(String indexName, CloudsearchApi client)
        {
            this.indexName = indexName;
            this.client = client;
            this.path = "/top/query/" + indexName;
        }


        /**
   * 通过指定的天数和个数，返回top query信息。                 
   * @param int $num 指定返回的记录个数。
   * @param int $days 指定统计从昨天开始向前多少天的数据。     
   * @return string                                             
   */
        public string GetTopQuery(int num, int days)
        {
            Dictionary<String, Object> parameters = new Dictionary<String, Object>();
            parameters.Add("num", num.ToString());
            parameters.Add("days", days.ToString());

            return this.client.ApiCall(this.path, parameters);
        }

    }
}
