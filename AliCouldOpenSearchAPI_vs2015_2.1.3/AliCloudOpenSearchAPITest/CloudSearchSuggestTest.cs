using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AliCloudOpenSearch.com.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AliCloudAPITest
{
    [TestClass]
    public class CloudSearchSuggestTest : CloudSearchApiAliyunBase
    {
        [TestMethod]
        public void testGetSuggest()
        {
            var suggest = new CloudsearchSuggest(CloudSearchApiAliyunBase.ApplicationName, "suggest1",this.api);
            var result = suggest.GetSuggest("云", 5);
            

            Assert.IsNotNull(result);
        }
    }
}
