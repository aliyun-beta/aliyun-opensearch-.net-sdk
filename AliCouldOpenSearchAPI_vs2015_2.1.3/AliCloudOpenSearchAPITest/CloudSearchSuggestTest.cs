using AliCloudOpenSearch.com.API;
using NUnit.Framework;

namespace AliCloudAPITest
{
    [TestFixture]
    public class CloudSearchSuggestTest : CloudSearchApiAliyunBase
    {
#if DEBUG
        [Test]
        public void testGetSuggest()
        {
            var suggest = new CloudsearchSuggest(ApplicationName, "suggest1", api);
            var result = suggest.GetSuggest("云", 5);


            Assert.IsNotNull(result);
        }
#endif
    }
}