using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AliCloudOpenSearch.com.API;
using AliCloudOpenSearch.com.API.Modal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AliCloudAPITest
{
    [TestClass]
    public class DeserializeToModalTest
    {
        [TestMethod]
        public void TestResponseDeserialize()
        {
            var json = "{'status':'OK','request_id':'1422348642065805100373587'}";

            var deserializedObj = Utilities.ConvertResult(json);

            Assert.AreEqual("OK", deserializedObj.Status);
            Assert.AreEqual("1422348642065805100373587", deserializedObj.RequestId);
        }

        [TestMethod]
        public void TestResponseDeserializeWithErrors()
        {
            var json = "{'status':'FAIL','errors':[{'code':4012,'message':'Table dose not exist'}],'request_id':'1422348739084222300234072'}";

            var deserializedObj = Utilities.ConvertResult(json);

            Assert.AreEqual("FAIL", deserializedObj.Status);
            Assert.AreEqual("1422348739084222300234072", deserializedObj.RequestId);
            Assert.IsNotNull(deserializedObj.Errors);
            Assert.AreEqual(1, deserializedObj.Errors.Length);
            Assert.AreEqual("4012", deserializedObj.Errors[0].Code);
            Assert.AreEqual("Table dose not exist", deserializedObj.Errors[0].Message);
        }
    }
}
