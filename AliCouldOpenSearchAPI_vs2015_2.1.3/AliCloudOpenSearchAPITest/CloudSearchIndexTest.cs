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
    public class CloudSearchIndexTest:CloudSearchApiAliyunBase
    {
        /// <summary>
        ///createIndex 的测试
        ///</summary>
        [TestMethod()]
        public void testCreateIndex()
        {
            string indexName ="index";
            CloudsearchApplication target = new CloudsearchApplication( this.mockApi); 
            string template = "news"; 
            string result;
            result = target.CreateByTemplate(indexName,template);
            Console.WriteLine(result);
            Assert.IsTrue(result.IndexOf("action=create&template=news")>=0);
        }

        [TestMethod()]
        public void TestRebuildIndex()
        {
            CloudsearchApplication target = new CloudsearchApplication(this.api);
            
            var result = target.RebuildIndex("hotel", null, null);
            var j = Utilities.ConvertResult(result);

            //You may get 'Unfinished task is exists' error
            //Assert.AreEqual("OK",j.Status);
        }
    }
}
