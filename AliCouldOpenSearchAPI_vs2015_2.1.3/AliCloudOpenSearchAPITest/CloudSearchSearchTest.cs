using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AliCloudOpenSearch.com.API;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Collections.Specialized;
using AliCloudOpenSearch.com.API.Builder;

namespace AliCloudAPITest
{
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class CloudSearchSearchTest : CloudSearchApiAliyunBase
    {

        //多个索引查询
        [TestMethod]
        public void TestMutiIndex()
        {
            String indexName1 = "index1";
            String indexName2 = "index2";

            var search = new CloudsearchSearch(this.mockApi);
            QueryBuilder builder = new QueryBuilder();
            builder.ApplicationNames(indexName1).ApplicationNames(indexName2);
            builder.Query(new Query("水杯"));
            builder.Config(new Config().Format(ReponseFormat.Json));

            String result = search.Search(builder);
            Uri uri = new Uri(result);
            var queryDict = HttpUtility.ParseQueryString(uri.Query);

            String query = queryDict["query"];

            Console.WriteLine(query);

            int i = query.IndexOf("format:json");
            Assert.IsTrue(i >= 0);

            i = query.IndexOf("query=default:水杯");
            Assert.IsTrue(i >= 0);

            var index_name = queryDict["index_name"];
            Console.WriteLine(index_name);
            Assert.AreEqual("index1;index2", index_name);

            builder.RemoveApplicationame("index2");

            result = search.Search(builder);
            uri = new Uri(result);
            queryDict = HttpUtility.ParseQueryString(uri.Query);
            index_name = queryDict["index_name"];
            Console.WriteLine(index_name);
            Assert.AreEqual("index1", index_name);

        }

        //设置 formula_name参数 
        [TestMethod]
        public void TestSetFormulaName()
        {
            var search = new CloudsearchSearch(this.mockApi);
            String indexName = "index1";
            QueryBuilder builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json))
                .FormulaName("test1").FirstFormulaName("test1");

            String result = search.Search(builder);

            Uri uri = new Uri(result);
            var queryDict = HttpUtility.ParseQueryString(uri.Query);

            String query = queryDict["query"];

            Console.WriteLine(query);

            int i = query.IndexOf("format:json");
            Assert.IsTrue(i >= 0);

            var formula_name = queryDict["formula_name"];
            Console.WriteLine(formula_name);
            Assert.AreEqual("test1", formula_name);
        }


        // 设置正确的飘红字段
        [TestMethod]
        public void testSearchSummary()
        {
            String indexName = "index1";
            var search = new CloudsearchSearch(this.mockApi);
            QueryBuilder builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json))
                .Summary(new Summary("security_key").SummaryLen(30).SummaryElement("em").SummaryEllipsis("..."));
            builder.Query(new Query("搜索"));


            String result = search.Search(builder);
            Uri uri = new Uri(result);
            var queryDict = HttpUtility.ParseQueryString(uri.Query);

            String query = queryDict["query"];

            Console.WriteLine(query);

            int i = query.IndexOf("format:json");
            Assert.IsTrue(i >= 0);

            i = query.IndexOf("query=default:搜索");
            Assert.IsTrue(i >= 0);

            var summary = queryDict["summary"];
            Console.WriteLine(summary);
            Assert.AreEqual("summary_field:security_key,summary_len:30,summary_element:em,summary_ellipsis:...;", summary);


        }


        //单字段搜索
        [TestMethod]
        public void testQueryCq()
        {
            var indexName = "index";
            var search = new CloudsearchSearch(this.mockApi);
            QueryBuilder builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json));

            var q = "苏宁";
            builder.Query(new Query("security_key:" + q));

            String result = search.Search(builder);
            Uri uri = new Uri(result);
            var queryDict = HttpUtility.ParseQueryString(uri.Query);

            String query = queryDict["query"];

            int i = query.IndexOf("format:json");
            Assert.IsTrue(i >= 0);

            i = query.IndexOf("query=security_key:" + q);
            Assert.IsTrue(i >= 0);

        }


        // 按照单字段减序
        [TestMethod]
        public void testDeduceSort()
        {
            var search = new CloudsearchSearch(this.mockApi);
            string indexName = "index";
            QueryBuilder builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json));
            builder.Sort(new Sort().Desc("price"));

            String result = search.Search(builder);
            Uri uri = new Uri(result);
            var queryDict = HttpUtility.ParseQueryString(uri.Query);

            String query = queryDict["query"];

            Console.WriteLine(query);

            int i = query.IndexOf("sort=-price");
            Assert.IsTrue(i >= 0);
        }



        //filter AND
        [TestMethod]
        public void testFilterAnd()
        {
            String indexName = "index";
            var search = new CloudsearchSearch(this.mockApi);
            QueryBuilder builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json));

            builder.Filter(new Filter("price>12").And(new Filter("id<88")));

            String result = search.Search(builder);
            Uri uri = new Uri(result);
            var queryDict = HttpUtility.ParseQueryString(uri.Query);

            String query = queryDict["query"];

            Console.WriteLine(query);

            int i = query.IndexOf("filter=price>12 AND (id<88)");
            Assert.IsTrue(i >= 0);


        }

        [TestMethod]
        public void testAggregateWithTwoField()
        {
            String indexName = "index";
            var search = new CloudsearchSearch(this.mockApi);
            QueryBuilder builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json));

            builder.Aggregate(new Aggregate("price", "count()"), new Aggregate("id", "count()"));

            String result = search.Search(builder);
            Uri uri = new Uri(result);
            var queryDict = HttpUtility.ParseQueryString(uri.Query);

            String query = queryDict["query"];

            Console.WriteLine(query);

            int i = query.IndexOf("aggregate=group_key:price,agg_fun:count();group_key:id,agg_fun:count()");
            Assert.IsTrue(i >= 0);
        }

        [TestMethod]
        public void testFetchFields()
        {
            String indexName = "index";
            var search = new CloudsearchSearch(this.mockApi);
            QueryBuilder builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json));
            builder.FetchFields("id", "gmt_modified");

            String result = search.Search(builder);
            Uri uri = new Uri(result);
            var queryDict = HttpUtility.ParseQueryString(uri.Query);

            String query = queryDict["fetch_fields"];

            Console.WriteLine(query);

            int i = query.IndexOf("id;gmt_modified");
            Assert.IsTrue(i >= 0);


        }

        //add all params 8
        [TestMethod]
        public void testDistinctWithAllParams()
        {
            String indexName = "index";
            var search = new CloudsearchSearch(this.mockApi);
            QueryBuilder builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json));

            builder.Distinct(new Distinct("user_id").DistinctCount(1).DistinctTimes(2)
                .Reserved(false).DistinctFilter("price>12").UpdateTotalHit(true).MaxItemCount(25).Grade(3.0));

            String result = search.Search(builder);
            Uri uri = new Uri(result);
            var queryDict = HttpUtility.ParseQueryString(uri.Query);

            String query = queryDict["query"];

            Console.WriteLine(query);

            int i = query.IndexOf("distinct=dist_key:user_id,dist_count:1,dist_times:2,reserved:false,dist_filter:price>12,update_total_hit:true,max_item_count:25,grade:3");
            Assert.IsTrue(i >= 0);


        }

        [TestMethod]
        public void testPair()
        {
            String indexName = "index";
            var search = new CloudsearchSearch(this.mockApi);
            QueryBuilder builder = new QueryBuilder();
            builder.ApplicationNames(indexName).Config(new Config().Format(ReponseFormat.Json));

            builder.Kvpari(new KVpair("test1", "test2"));

            String result = search.Search(builder);
            Uri uri = new Uri(result);
            var queryDict = HttpUtility.ParseQueryString(uri.Query);

            String query = queryDict["query"];

            Console.WriteLine(query);

            int i = query.IndexOf("kvpairs=test1:test2");
            Assert.IsTrue(i >= 0);

            // this.assertFalse(strpos(result, "%26%26kvpairs%3Dtest1%3Atest2") === false);
        }

        /// <summary>
        /// 
        /// 测试搜索相关url
        /// 
        /// </summary>

        [TestMethod]
        public void TestAllSearch()
        {

            Int32 page = 1;
            Int32 page_size = 20;
            String format = "json";
            String q = "中文";


            CloudsearchSearch search = new CloudsearchSearch(this.mockApi);
            QueryBuilder builder = new QueryBuilder();
            builder.ApplicationNames("hotel").Config(new Config().Hit(page_size).Start((page - 1) * page_size).Format(ReponseFormat.Json));
            builder.Query(new Query("default:'" + q + "'"));

            String result = search.Search(builder);
            Console.WriteLine(result);


            /*
             http://opensearch.console.aliyun.com/v2/api/search?
             * query=config=format:json,start:0,hit:20,rerank_size:200&&query=default:'中文'&index_name=hotel&Version=v2
             * &AccessKeyId=uTlPHKQwYjNZMKRE&SignatureMethod=HMAC-SHA1&SignatureVersion=1.0&SignatureNonce=
             * 14129937619908342&Timestamp=2014-10-11T02:16:01Z&Signature=3pxDIMoVHKI6YRKnD6FR34L9qPM=
             */

            Uri uri = new Uri(result);
            var n = HttpUtility.ParseQueryString(uri.Query);
            var query = n["query"];

            int i = query.IndexOf("start:0");
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
            builder.Config(new Config().Format(ReponseFormat.Xml).Hit(page_size).Start((page - 1) * page_size));
            builder.ApplicationNames("hotel2");
            builder.RemoveApplicationame("hotel");

            result = search.Search(builder);
            Console.WriteLine(result);

            uri = new Uri(result);
            n = HttpUtility.ParseQueryString(uri.Query);
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
            string sPattern = "\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}Z";

            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(timestamp, sPattern));
        }

        [TestMethod]
        public void TestWithRealService()
        {
            var search = new CloudsearchSearch(this.api);
            QueryBuilder builder = new QueryBuilder();
            builder.ApplicationNames(CloudSearchApiAliyunBase.ApplicationName);
            builder.Query(new Query("云"));

            Assert.AreEqual("OK",Utilities.ConvertResult(search.Search(builder)).Status);

            builder.Config(new Config().Format(ReponseFormat.Xml));
            var rst = search.Search(builder);

            builder.Config(new Config().Format(ReponseFormat.Protobuf));
             rst = search.Search(builder);

            builder.ApplicationNames("hotel");
            builder.Config(new Config().Format(ReponseFormat.Json)).FetchFields("id","body");
            rst = search.Search(builder);

            builder.Summary(new Summary("body").SummaryLen(50).SummaryEllipsis("!!!!").SummaryPrefix("<b>").SummaryPostfix("</b>"));
            rst = search.Search(builder);

            builder.Summary(new Summary("body").SummaryLen(50).SummaryEllipsis("!!!!").SummaryElement("b"));
            rst = search.Search(builder);
        }

        [TestMethod]
        public void TestScanSearch()
        {
            var search = new CloudsearchSearch(this.api);
            QueryBuilder builder = new QueryBuilder();
            builder.ApplicationNames(CloudSearchApiAliyunBase.ApplicationName);
            builder.Query(new Query("云")).Config(new Config().Hit(50));

            var scanRst = Utilities.ConvertResult(search.Scan(builder, "1m"));
            var scrollId = scanRst.Result["scroll_id"].ToString();

            var rst = search.ScanThen("1m", scrollId);
        }
    }
}
