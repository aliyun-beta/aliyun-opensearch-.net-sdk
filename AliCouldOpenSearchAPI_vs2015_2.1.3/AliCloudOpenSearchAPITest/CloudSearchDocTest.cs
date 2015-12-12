using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using AliCloudOpenSearch.com.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AliCloudAPITest
{
    
    
    /// <summary>
    ///这是 CloudSearchIndexTest 的测试类，旨在
    ///包含所有 CloudSearchIndexTest 单元测试
    ///</summary>
    [TestClass()]
    public class CloudSearchDOcTest:CloudSearchApiAliyunBase
    {
        [TestMethod]
        public void TestDocDetail()
        {
            CloudsearchDoc target = new CloudsearchDoc(ApplicationName, this.api);
            target.Add("[{'id':1,'author':'nathan'}]").Push("main");

            var result = target.Detail("id", "1");
            Assert.AreEqual("OK", result.Status);

            target.Remove("1").Push("main");
            result = target.Detail("id", "1");

            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");

        }


        [TestMethod]
        public void TestDocAdd()
        {

            CloudsearchDoc target = new CloudsearchDoc(ApplicationName, this.api); // TODO: 初始化为适当的值
            String pk = RandomStr(5);
            String data = "[{\"id\":\"" + pk + "\"}]";
            target.Add(data);
            var result = target.Push("main");
            Console.WriteLine(result);
            Console.WriteLine(pk);

            Assert.AreEqual("OK", result.Status);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");

        }

        [TestMethod]
        public void TestDocAdd1()
        {

            CloudsearchDoc target = new CloudsearchDoc(ApplicationName, this.api); // TODO: 初始化为适当的值

            String data = "[{\"a\":\"1\"}]";
            target.Add(data);
            var result = target.Push("main");
            Console.WriteLine(result);

            Assert.AreEqual("OK", result.Status);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");

        }


        [TestMethod]
        public void TestDocDel()
        {

            CloudsearchDoc target = new CloudsearchDoc(ApplicationName, this.api); // TODO: 初始化为适当的值

            var result = target.Remove("999","1").Push("main");
             
            Console.WriteLine(result);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");

            Assert.AreEqual("OK", result.Status);
        }


        [TestMethod]
        public void TestDocBatchSubmit()
        {

            CloudsearchDoc target = new CloudsearchDoc(ApplicationName, this.api); // TODO: 初始化为适当的值

            var docToAdd = new Dictionary<string,object>();
            docToAdd["K1"] = "k1";
            docToAdd["K2"] = "k2";
            docToAdd["id"] = "1";

            var docToUpdate = new Dictionary<string, object>();
            docToUpdate["K1"] = "k1";
            docToUpdate["K2"] = "k2";
            docToUpdate["id"] = "2";

            var result = target.Remove("999", "1").Add(docToAdd).Update(docToUpdate).Push("main");

            Console.WriteLine(result);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");

            Assert.AreEqual("OK", result.Status);

            var d1 = target.Detail("id", "1");

        }

        [TestMethod]
        public void TestDocUpdate()
        {
            CloudsearchDoc target = new CloudsearchDoc(ApplicationName, this.api); // TODO: 初始化为适当的值

            String data = "[{\"fields\":{\"a\":\"1\",\"b\":\"test\"},\"cmd\":\"UPDATE\"}]";
            target.Add(data);
            var result = target.Push("main");
            Console.WriteLine(result);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("验证此测试方法的正确性。");
            
            Assert.AreEqual("OK", result.Status);
        }
    }
}
