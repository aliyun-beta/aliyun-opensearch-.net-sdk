using System;
using AliCloudOpenSearch.com.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AliCloudAPITest
{
    /// <summary>
    ///     这是 CloudSearchIndexTest 的测试类，旨在
    ///     包含所有 CloudSearchIndexTest 单元测试
    /// </summary>
    [TestClass]
    public class CloudSearchAnalysisTest : CloudSearchApiAliyunBase
    {
        [TestMethod]
        public void testTopQuery()
        {
            var css = new CloudsearchAnalysis("hotel", api);
            var result = css.GetTopQuery(100, 100);
            Console.WriteLine(result);
        }
    }
}