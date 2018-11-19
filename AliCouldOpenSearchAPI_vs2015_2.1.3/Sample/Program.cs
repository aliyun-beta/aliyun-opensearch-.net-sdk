﻿using System;
using AliCloudOpenSearch.com.API;
using AliCloudOpenSearch.com.API.Builder;
using System.Threading.Tasks;

namespace Sample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string client_id = "TR2QyWfDusb0Tgce";
            const string secret_id = "ZPJZBMEr2pcMP2fsGeHH36PzZeNYHW";

            var apiclient = new CloudsearchApi(client_id, secret_id, "http://opensearch-cn-hangzhou.aliyuncs.com");
            var search = new CloudsearchSearch(apiclient);

            var builder = new QueryBuilder();
            builder.ApplicationNames("datafiddleSearch")
                .Query(new Query("云").And(new Query("搜索")))
                .Config(new Config().Format(ReponseFormat.Json));

            var result = search.Search(builder);
            Console.WriteLine(result);
#if NET45
            string asyncResult = null;
            var t = Task.Run(async () =>
            {
                asyncResult = await search.SearchAsync(builder);
            });
            t.Wait();
            Console.WriteLine(asyncResult);
#endif
            Console.Read();
        }
    }
}