using System;
using System.Collections.Generic;
using AliCloudOpenSearch.com.API;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace AliCloudAPITest
{
    /// <summary>
    ///     UnitTest1 的摘要说明
    /// </summary>
    [TestFixture]
    public class CloudSearchApiTest : CloudSearchApiAliyunBase
    {
        private int rep;

        /// <summary>
        ///     获取或设置测试上下文，该上下文提供
        ///     有关当前测试运行及其功能的信息。
        /// </summary>
        public TestContext TestContext { get; set; }

        [Test]
        public void TestAPI()
        {
            var customD2 = new Dictionary<string, object>
            {
                {"param1", new Dictionary<string, object> {{"a", 1}, {"b", 2}}},
                {"param2", new object[] {"c", "d", new Dictionary<string, object> {{"e", 1}}}}
            };


            var res = HttpBuildQueryHelper.FormatDictionary(customD2);
            Console.WriteLine(res);
        }

        [Test]
        public void TestCreateIndex()
        {
            var appName = DateTime.UtcNow.Ticks.ToString();
            var target = new CloudsearchApplication(api);
            var template = "template1";
            string actual;
            actual = target.CreateByTemplate(appName, template);
            Console.WriteLine(actual);

            var jo = JObject.Parse(actual);

            //Console.WriteLine(result);
            Assert.AreEqual("OK", (string)jo["status"]);


            actual = target.Delete(appName);
            Console.WriteLine(actual);

            //"result":{"index_name":"4N0F6H","description":""},"status":"OK"}
            jo = JObject.Parse(actual);
            Assert.AreEqual("OK", (string)jo["status"]);
        }

        [Test]
        public void TestGetErrorMessage()
        {
            var target = new CloudsearchApplication(api);
            var result = target.GetErrorMessage("hotel");

            var jo = JObject.Parse(result);
            Assert.AreEqual("OK", (string)jo["status"]);
        }


        [Test]
        public void TestListIndex()
        {
            var target = new CloudsearchApplication(mockApi);

            var result = target.ListApplications();

            AssertPath("/index", result);
            AssertEqualFromQueryString("page", "1", result);

            result = target.ListApplications(2, 2);
            AssertEqualFromQueryString("page", "2", result);
        }

        [Test]
        public void TestStatusIndex()
        {
            var target = new CloudsearchApplication(api);

            var result = target.Status("hotel");
            Console.WriteLine(result);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
            var jo = JObject.Parse(result);
            Assert.AreEqual("OK", (string)jo["status"]);
        }


        [Test]
        public void TestTopQuery()
        {
            var css = new CloudsearchAnalysis("hotel", api);
            var result = css.GetTopQuery(100, 100);
            var jo = JObject.Parse(result);

            Console.WriteLine(result);
            Assert.AreEqual("OK", (string)jo["status"]);
        }

        [Test]
        public void TestUnixTimeStamp()
        {
            var stamp = Utilities.getUnixTimeStamp();
            Console.WriteLine(stamp);
        }
    }
}