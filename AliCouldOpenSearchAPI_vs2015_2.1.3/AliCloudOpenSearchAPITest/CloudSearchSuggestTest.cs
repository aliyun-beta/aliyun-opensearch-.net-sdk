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
            var suggest = new CloudsearchSuggest(ApplicationName, "suggest1", api);
            var result = suggest.GetSuggest("云", 5);


            Assert.IsNotNull(result);
        }
    }
}