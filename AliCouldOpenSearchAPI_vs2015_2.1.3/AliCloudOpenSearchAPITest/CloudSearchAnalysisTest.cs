using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AliCloudOpenSearch.com.API;

namespace AliCloudAPITest
{
    
    
    /// <summary>
    ///这是 CloudSearchIndexTest 的测试类，旨在
    ///包含所有 CloudSearchIndexTest 单元测试
    ///</summary>
    [TestClass()]
    public class CloudSearchAnalysisTest:CloudSearchApiAliyunBase
    {
        [TestMethod()]
        public void testTopQuery()
        {
            CloudsearchAnalysis css = new CloudsearchAnalysis("hotel", this.api);
            String result = css.GetTopQuery(100, 100);
            Console.WriteLine(result);
        }
    }
}
