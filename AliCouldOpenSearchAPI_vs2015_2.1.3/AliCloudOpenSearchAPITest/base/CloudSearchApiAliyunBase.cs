using System;
using AliCloudOpenSearch.com.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace AliCloudAPITest
{
    /// <summary>
    ///     UnitTest1 的摘要说明
    /// </summary>
    [TestFixture]
    public class CloudSearchApiAliyunBase
    {
        public const string ApplicationName = "datafiddleSearch";
        protected CloudsearchApi api;
        protected CloudsearchApi mockApi;


        public CloudSearchApiAliyunBase()
        {
            const string client_id = "TR2QyWfDusb0Tgce";
            const string secret_id = "ZPJZBMEr2pcMP2fsGeHH36PzZeNYHW ";

#if DEBUG
            api = new CloudsearchApi(client_id, secret_id, "http://opensearch-cn-hangzhou.aliyuncs.com", 1, "HMAC-SHA1",
               "1.0", 100000);       
#else
            api = new CloudsearchApiMock(client_id, secret_id, "http://opensearch-cn-hangzhou.aliyuncs.com", 1, "HMAC-SHA1",
                "1.0", 100000);
#endif
            mockApi = new CloudsearchApiMock(client_id, secret_id, "http://opensearch.console.aliyun.com/", 1,
                "HMAC-SHA1",
                "1.0", 50000, true);
        }

        /// <summary>
        ///     获取或设置测试上下文，该上下文提供
        ///     有关当前测试运行及其功能的信息。
        /// </summary>
        public TestContext TestContext { get; set; }

        public static string RandomStr(int codeCount)
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public void AssertEqualFromQueryString(string key, string expectedVal, string mockResult)
        {
            var json = JObject.Parse(mockResult);
            
            var querys = System.Web.HttpUtility.ParseQueryString(json["Query"].ToString());
            Assert.AreEqual(expectedVal, querys[key]);
        }

        public void AssertPath(string expectedVal, string mockResult)
        {
            var json = JObject.Parse(mockResult);
            var uri = new Uri(json["Url"].ToString());
            Assert.AreEqual(expectedVal, uri.LocalPath);
        }
    }
}