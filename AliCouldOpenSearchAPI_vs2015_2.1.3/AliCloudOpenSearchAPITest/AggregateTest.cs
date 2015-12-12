using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AliCloudOpenSearch.com.API.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AliCloudAPITest
{
    [TestClass]
   public class AggregateTest
    {
        [TestMethod]
        public void Test1()
        {
            var agg = new Aggregate("group_id", "count()");

            Assert.AreEqual("group_key:group_id,agg_fun:count()",((IBuilder)agg).BuildQuery());

            agg.AggFilter("abc");
            agg.Range("a");
            Assert.AreEqual("group_key:group_id,agg_fun:count(),range:a,agg_filter:abc", ((IBuilder)agg).BuildQuery());

            agg.MaxGroup(25);
            Assert.AreEqual("group_key:group_id,agg_fun:count(),range:a,agg_filter:abc,max_group:25", ((IBuilder)agg).BuildQuery());

            agg.AggSamplerThreshold("a1");
            agg.AggSamplerStep("a2");
            Assert.AreEqual("group_key:group_id,agg_fun:count(),range:a,agg_filter:abc,agg_sampler_threshold:a1,agg_sampler_step:a2,max_group:25", ((IBuilder)agg).BuildQuery());
        }
    }
}
