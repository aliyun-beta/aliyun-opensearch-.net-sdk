using AliCloudOpenSearch.com.API.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AliCloudAPITest
{
    [TestClass]
    public class FilterTest
    {
        [TestMethod]
        public void Test1()
        {
            var filter = new Filter("filed1=a");
            Assert.AreEqual("filed1=a", ((IBuilder) filter).BuildQuery());

            filter.And(new Filter("field2>3"));
            Assert.AreEqual("filed1=a AND (field2>3)", ((IBuilder) filter).BuildQuery());

            filter.Or(new Filter("field2<-1"));
            Assert.AreEqual("filed1=a AND (field2>3) OR (field2<-1)", ((IBuilder) filter).BuildQuery());
        }
    }
}