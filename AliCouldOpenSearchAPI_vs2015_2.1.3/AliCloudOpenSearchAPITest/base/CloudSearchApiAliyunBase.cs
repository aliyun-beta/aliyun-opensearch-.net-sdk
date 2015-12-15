using System;
using AliCloudOpenSearch.com.API;
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
            //
            //TODO: 在此处添加构造函数逻辑
            //

            const string client_id = "TR2QyWfDusb0Tgce";
            const string secret_id = "ZPJZBMEr2pcMP2fsGeHH36PzZeNYHW ";


            api = new CloudsearchApi(client_id, secret_id, "http://opensearch-cn-hangzhou.aliyuncs.com", 1, "HMAC-SHA1",
                "1.0", 100000);
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

        // public static void MyClassInitialize(TestContext testContext) { }
        // [ClassInitialize()]
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        //
        // 编写测试时，可以使用以下附加特性:

        //
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
    }
}