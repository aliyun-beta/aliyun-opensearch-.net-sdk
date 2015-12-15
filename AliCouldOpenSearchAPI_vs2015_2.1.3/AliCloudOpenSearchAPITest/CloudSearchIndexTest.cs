using System;
using AliCloudOpenSearch.com.API;
using NUnit.Framework;

namespace AliCloudAPITest
{
    /// <summary>
    ///     这是 CloudSearchIndexTest 的测试类，旨在
    ///     包含所有 CloudSearchIndexTest 单元测试
    /// </summary>
    [TestFixture]
    public class CloudSearchIndexTest : CloudSearchApiAliyunBase
    {
        /// <summary>
        ///     createIndex 的测试
        /// </summary>
        [Test]
        public void testCreateIndex()
        {
            var indexName = "index";
            var target = new CloudsearchApplication(mockApi);
            var template = "news";
            string result;
            result = target.CreateByTemplate(indexName, template);
            Console.WriteLine(result);
            Assert.IsTrue(result.IndexOf("action=create&template=news") >= 0);
        }

        [Test]
        public void TestRebuildIndex()
        {
            var target = new CloudsearchApplication(mockApi);

            var result = target.RebuildIndex("hotel", null, null);
            var j = Utilities.ConvertResult(result);

            Assert.AreEqual("OK",j.Status);
        }
    }
}