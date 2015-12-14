using System;
using System.Collections.Generic;
using AliCloudOpenSearch.com.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace AliCloudAPITest
{
    /// <summary>
    ///     UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class CloudSearchApiTest : CloudSearchApiAliyunBase
    {
        private int rep;

        /// <summary>
        ///     获取或设置测试上下文，该上下文提供
        ///     有关当前测试运行及其功能的信息。
        /// </summary>
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void TestTopQuery()
        {
            var css = new CloudsearchAnalysis("hotel", api);
            var result = css.GetTopQuery(100, 100);
            var jo = JObject.Parse(result);

            Console.WriteLine(result);
            Assert.AreEqual("OK", jo["status"]);
        }

        [TestMethod]
        public void TestUnixTimeStamp()
        {
            var stamp = Utilities.getUnixTimeStamp();
            Console.WriteLine(stamp);
        }

        [TestMethod]
        public void TestCreateIndex()
        {
            var appName = DateTime.UtcNow.Ticks.ToString();
            var target = new CloudsearchApplication(api);
            var template = "template1";
            string actual;
            actual = target.CreateByTemplate(appName, template);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
            Console.WriteLine(actual);

            var jo = JObject.Parse(actual);

            //Console.WriteLine(result);
            Assert.AreEqual("OK", jo["status"]);


            actual = target.Delete(appName);
            Console.WriteLine(actual);

            //"result":{"index_name":"4N0F6H","description":""},"status":"OK"}
            jo = JObject.Parse(actual);
            Assert.AreEqual("OK", jo["status"]);
            Assert.AreEqual(1, jo["result"]);
        }

        [TestMethod]
        public void TestStatusIndex()
        {
            var target = new CloudsearchApplication(api); // TODO: 初始化为适当的值

            var result = target.Status("hotel");
            Console.WriteLine(result);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
            var jo = JObject.Parse(result);
            Assert.AreEqual("OK", jo["status"]);
        }


        [TestMethod]
        public void TestListIndex()
        {
            var target = new CloudsearchApplication(api);

            var result = target.ListApplications();
            Console.WriteLine(result);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
            var jo = JObject.Parse(result);
            Assert.AreEqual("OK", jo["status"]);

            result = target.ListApplications(2, 2);
            Console.WriteLine(result);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
            jo = JObject.Parse(result);
            Assert.AreEqual("OK", jo["status"]);
        }

        [TestMethod]
        public void TestGetErrorMessage()
        {
            var target = new CloudsearchApplication(api);
            var result = target.GetErrorMessage("hotel");

            var jo = JObject.Parse(result);
            Assert.AreEqual("OK", jo["status"]);
        }

        [TestMethod]
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
    }
}