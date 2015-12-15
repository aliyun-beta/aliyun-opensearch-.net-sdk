using System;
using System.Text.RegularExpressions;
using System.Web;
using AliCloudOpenSearch.com.API;
using AliCloudOpenSearch.com.API.Builder;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace AliCloudAPITest
{
    /// <summary>
    ///     UnitTest1 的摘要说明
    /// </summary>
    [TestFixture]
    public class CloudSearchSearchTest : CloudSearchApiAliyunBase
    {
        [Test]
        public void testAggregateWithTwoField()
        {
            var indexName = "index";
            var search = new CloudsearchSearch(mockApi);
            var builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json));

            builder.Aggregate(new Aggregate("price", "count()"), new Aggregate("id", "count()"));

            var result = search.Search(builder);
            var queryDict = HttpUtility.ParseQueryString(JObject.Parse(result)["Query"].ToString());

            var query = queryDict["query"];
            
            var i = query.IndexOf("aggregate=group_key:price,agg_fun:count();group_key:id,agg_fun:count()");
            Assert.IsTrue(i >= 0);
        }

        /// <summary>
        ///     测试搜索相关url
        /// </summary>
        [Test]
        public void TestAllSearch()
        {
            var page = 1;
            var page_size = 20;
            var format = "json";
            var q = "中文";


            var search = new CloudsearchSearch(mockApi);
            var builder = new QueryBuilder();
            builder.ApplicationNames("hotel")
                .Config(new Config().Hit(page_size).Start((page - 1)*page_size).Format(ReponseFormat.Json));
            builder.Query(new Query("default:'" + q + "'"));

            var result = search.Search(builder);
            Console.WriteLine(result);


            /*
             http://opensearch.console.aliyun.com/v2/api/search?
             * query=config=format:json,start:0,hit:20,rerank_size:200&&query=default:'中文'&index_name=hotel&Version=v2
             * &AccessKeyId=uTlPHKQwYjNZMKRE&SignatureMethod=HMAC-SHA1&SignatureVersion=1.0&SignatureNonce=
             * 14129937619908342&Timestamp=2014-10-11T02:16:01Z&Signature=3pxDIMoVHKI6YRKnD6FR34L9qPM=
             */

            var n = HttpUtility.ParseQueryString(JObject.Parse(result)["Query"].ToString());
            var query = n["query"];

            var i = query.IndexOf("start:0");
            Assert.IsTrue(i >= 0);

            i = query.IndexOf("format:json");
            Assert.IsTrue(i >= 0);

            i = query.IndexOf("hit:20");
            Assert.IsTrue(i >= 0);
            i = query.IndexOf("query=default:'中文'");
            Assert.IsTrue(i >= 0);

            var index_name = n["index_name"];
            Assert.AreEqual("hotel", index_name);


            page = 2;
            page_size = 50;
            builder.Config(new Config().Format(ReponseFormat.Xml).Hit(page_size).Start((page - 1)*page_size));
            builder.ApplicationNames("hotel2");
            builder.RemoveApplicationame("hotel");

            result = search.Search(builder);
            Console.WriteLine(result);

            n = HttpUtility.ParseQueryString(JObject.Parse(result)["Query"].ToString());
            query = n["query"];

            i = query.IndexOf("start:50");
            Assert.IsTrue(i >= 0);

            i = query.IndexOf("format:xml");
            Assert.IsTrue(i >= 0);

            i = query.IndexOf("hit:50");
            Assert.IsTrue(i >= 0);
            i = query.IndexOf("query=default:'中文'");
            Assert.IsTrue(i >= 0);


            index_name = n["index_name"];
            Assert.AreEqual("hotel2", index_name);

            var timestamp = n["Timestamp"];
            var sPattern = "\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}Z";

            Assert.IsTrue(Regex.IsMatch(timestamp, sPattern));
        }


        // 按照单字段减序
        [Test]
        public void testDeduceSort()
        {
            var search = new CloudsearchSearch(mockApi);
            var indexName = "index";
            var builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json));
            builder.Sort(new Sort().Desc("price"));

            var result = search.Search(builder);
            var queryDict = HttpUtility.ParseQueryString(JObject.Parse(result)["Query"].ToString());

            var query = queryDict["query"];

            Console.WriteLine(query);

            var i = query.IndexOf("sort=-price");
            Assert.IsTrue(i >= 0);
        }

        //add all params 8
        [Test]
        public void testDistinctWithAllParams()
        {
            var indexName = "index";
            var search = new CloudsearchSearch(mockApi);
            var builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json));

            builder.Distinct(new Distinct("user_id").DistinctCount(1).DistinctTimes(2)
                .Reserved(false).DistinctFilter("price>12").UpdateTotalHit(true).MaxItemCount(25).Grade(3.0));

            var result = search.Search(builder);
            var queryDict = HttpUtility.ParseQueryString(JObject.Parse(result)["Query"].ToString());

            var query = queryDict["query"];

            Console.WriteLine(query);

            var i =
                query.IndexOf(
                    "distinct=dist_key:user_id,dist_count:1,dist_times:2,reserved:false,dist_filter:price>12,update_total_hit:true,max_item_count:25,grade:3");
            Assert.IsTrue(i >= 0);
        }

        [Test]
        public void testFetchFields()
        {
            var indexName = "index";
            var search = new CloudsearchSearch(mockApi);
            var builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json));
            builder.FetchFields("id", "gmt_modified");

            var result = search.Search(builder);
            var queryDict = HttpUtility.ParseQueryString(JObject.Parse(result)["Query"].ToString());

            var query = queryDict["fetch_fields"];

            Console.WriteLine(query);

            var i = query.IndexOf("id;gmt_modified");
            Assert.IsTrue(i >= 0);
        }


        //filter AND
        [Test]
        public void testFilterAnd()
        {
            var indexName = "index";
            var search = new CloudsearchSearch(mockApi);
            var builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json));

            builder.Filter(new Filter("price>12").And(new Filter("id<88")));

            var result = search.Search(builder);
            var queryDict = HttpUtility.ParseQueryString(JObject.Parse(result)["Query"].ToString());

            var query = queryDict["query"];

            Console.WriteLine(query);

            var i = query.IndexOf("filter=price>12 AND (id<88)");
            Assert.IsTrue(i >= 0);
        }

        //多个索引查询
        [Test]
        public void TestMutiIndex()
        {
            var indexName1 = "index1";
            var indexName2 = "index2";

            var search = new CloudsearchSearch(mockApi);
            var builder = new QueryBuilder();
            builder.ApplicationNames(indexName1).ApplicationNames(indexName2);
            builder.Query(new Query("水杯"));
            builder.Config(new Config().Format(ReponseFormat.Json));

            var result = search.Search(builder);
            var queryDict = HttpUtility.ParseQueryString(JObject.Parse(result)["Query"].ToString());

            var query = queryDict["query"];

            Console.WriteLine(query);

            var i = query.IndexOf("format:json");
            Assert.IsTrue(i >= 0);

            i = query.IndexOf("query=default:水杯");
            Assert.IsTrue(i >= 0);

            var index_name = queryDict["index_name"];
            Console.WriteLine(index_name);
            Assert.AreEqual("index1;index2", index_name);

            builder.RemoveApplicationame("index2");

            result = search.Search(builder);
            queryDict = HttpUtility.ParseQueryString(JObject.Parse(result)["Query"].ToString());
            index_name = queryDict["index_name"];
            Console.WriteLine(index_name);
            Assert.AreEqual("index1", index_name);
        }

        [Test]
        public void testPair()
        {
            var indexName = "index";
            var search = new CloudsearchSearch(mockApi);
            var builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json));

            builder.Kvpari(new KVpair("test1", "test2"));

            var result = search.Search(builder);
            var queryDict = HttpUtility.ParseQueryString(JObject.Parse(result)["Query"].ToString());

            var query = queryDict["query"];

            Console.WriteLine(query);

            var i = query.IndexOf("kvpairs=test1:test2");
            Assert.IsTrue(i >= 0);

            // this.assertFalse(strpos(result, "%26%26kvpairs%3Dtest1%3Atest2") === false);
        }


        //单字段搜索
        [Test]
        public void testQueryCq()
        {
            var indexName = "index";
            var search = new CloudsearchSearch(mockApi);
            var builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json));

            var q = "苏宁";
            builder.Query(new Query("security_key:" + q));

            var result = search.Search(builder);
            var queryDict = HttpUtility.ParseQueryString(JObject.Parse(result)["Query"].ToString());

            var query = queryDict["query"];

            var i = query.IndexOf("format:json");
            Assert.IsTrue(i >= 0);

            i = query.IndexOf("query=security_key:" + q);
            Assert.IsTrue(i >= 0);
        }

        [Test]
        public void TestScanSearch()
        {
            var search = new CloudsearchSearch(api);
            var builder = new QueryBuilder();
            builder.ApplicationNames(ApplicationName);
            builder.Query(new Query("云")).Config(new Config().Hit(50));

            var scanRst = Utilities.ConvertResult(search.Scan(builder, "1m"));
            var scrollId = scanRst.Result["scroll_id"].ToString();

            var rst = search.ScanThen("1m", scrollId);
        }


        // 设置正确的飘红字段
        [Test]
        public void testSearchSummary()
        {
            var indexName = "index1";
            var search = new CloudsearchSearch(mockApi);
            var builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json))
                .Summary(new Summary("security_key").SummaryLen(30).SummaryElement("em").SummaryEllipsis("..."));
            builder.Query(new Query("搜索"));


            var result = search.Search(builder);
            var queryDict = HttpUtility.ParseQueryString(JObject.Parse(result)["Query"].ToString());

            var query = queryDict["query"];

            Console.WriteLine(query);

            var i = query.IndexOf("format:json");
            Assert.IsTrue(i >= 0);

            i = query.IndexOf("query=default:搜索");
            Assert.IsTrue(i >= 0);

            var summary = queryDict["summary"];
            Console.WriteLine(summary);
            Assert.AreEqual("summary_field:security_key,summary_len:30,summary_element:em,summary_ellipsis:...;",
                summary);
        }

        //设置 formula_name参数 
        [Test]
        public void TestSetFormulaName()
        {
            var search = new CloudsearchSearch(mockApi);
            var indexName = "index1";
            var builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json))
                .FormulaName("test1").FirstFormulaName("test1");

            var result = search.Search(builder);

            var queryDict = HttpUtility.ParseQueryString(JObject.Parse(result)["Query"].ToString());

            var query = queryDict["query"];

            Console.WriteLine(query);

            var i = query.IndexOf("format:json");
            Assert.IsTrue(i >= 0);

            var formula_name = queryDict["formula_name"];
            Console.WriteLine(formula_name);
            Assert.AreEqual("test1", formula_name);
        }

        [Test]
        public void TestWithRealService()
        {
            var search = new CloudsearchSearch(mockApi);
            var builder = new QueryBuilder();
            builder.ApplicationNames(ApplicationName);
            builder.Query(new Query("云"));

            Assert.AreEqual("OK", Utilities.ConvertResult(search.Search(builder)).Status);

            builder.Config(new Config().Format(ReponseFormat.Xml));
            var rst = search.Search(builder);

            builder.Config(new Config().Format(ReponseFormat.Protobuf));
            rst = search.Search(builder);

            builder.ApplicationNames("hotel");
            builder.Config(new Config().Format(ReponseFormat.Json)).FetchFields("id", "body");
            rst = search.Search(builder);

            builder.Summary(
                new Summary("body").SummaryLen(50).SummaryEllipsis("!!!!").SummaryPrefix("<b>").SummaryPostfix("</b>"));
            rst = search.Search(builder);

            builder.Summary(new Summary("body").SummaryLen(50).SummaryEllipsis("!!!!").SummaryElement("b"));
            rst = search.Search(builder);
        }
    }
}