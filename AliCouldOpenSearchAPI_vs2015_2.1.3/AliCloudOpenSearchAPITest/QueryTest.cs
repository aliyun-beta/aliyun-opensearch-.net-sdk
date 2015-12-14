using AliCloudOpenSearch.com.API.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AliCloudAPITest
{
    [TestClass]
    public class QueryTest
    {
        [TestMethod]
        public void Test1()
        {
            IBuilder qry = new Query("id1:a");
            Assert.AreEqual("id1:a", qry.BuildQuery());

            qry = new Query("a");
            Assert.AreEqual("default:a", qry.BuildQuery());
        }

        [TestMethod]
        public void Test2()
        {
            IBuilder qry = new Query("default:a", 100);
            Assert.AreEqual("default:a^99", qry.BuildQuery());

            qry = new Query("default:a", -2);
            Assert.AreEqual("default:a^0", qry.BuildQuery());

            qry = new Query("default:a", 2);
            Assert.AreEqual("default:a^2", qry.BuildQuery());
        }

        [TestMethod]
        public void Test3()
        {
            var qry = new Query("default:a", 2);
            qry.And(new Query("index1:B"));
            Assert.AreEqual("default:a^2 AND (index1:B)", ((IBuilder) qry).BuildQuery());

            qry.Or(new Query("index2:B"));
            Assert.AreEqual("default:a^2 AND (index1:B) OR (index2:B)", ((IBuilder) qry).BuildQuery());

            qry.AndNot(new Query("index2:B"));
            Assert.AreEqual("default:a^2 AND (index1:B) OR (index2:B) ANDNOT (index2:B)", ((IBuilder) qry).BuildQuery());

            qry.Rank(new Query("index2:B"));
            Assert.AreEqual("default:a^2 AND (index1:B) OR (index2:B) ANDNOT (index2:B) RANK (index2:B)",
                ((IBuilder) qry).BuildQuery());
        }
    }
}