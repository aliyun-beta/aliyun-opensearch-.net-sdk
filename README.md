# Aliyun OpenSearch SDK For Dotnet
Welcome to OpenSearch SDK for Dotnet, this project provides all features of Aliyun opensearch api.

## Prepare
1.Before you run the sdk sample, you need to regist an aliyun account and enable your opensearch service
1.You should get your accessKey and accessSecret from yourt aliyun account mangement page, and your opensearch host address from opensearch configuration mangement page

## Usage

```
using System;
using AliCloud.com.API;
using AliCloud.com.API.Builder;

namespace Sample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string client_id = "your access key";
            const string secret_id = "you access secret";

            var apiclient = new CloudsearchApi(client_id, secret_id, "http://opensearch-cn-hangzhou.aliyuncs.com");
            var search = new CloudsearchSearch(apiclient);

            var builder = new QueryBuilder();
            builder.ApplicationNames("indexName").Query(new Query("Keyword1").And(new Query("keyword2"))).Config(new Config().Format(ReponseFormat.Json));

            var result = search.Search(builder);
            Console.WriteLine(result);

            Console.Read();
        }
    }
}
```

## QueryBuilder
Aliyun openapi provides many options for developer, QueryBuilder uses chain grammar to make you easily build your search options, it contains all Query clause: query,config,filter,distinct,sort,kvpair and aggregate,
and it also provides an easy way to set summary option, just enjoy it!

## Response data modal
If you like to use strong data modal to access service response result, you can use Utilities.ConvertResult() to convert it.

## Data update
For the performance reason, you should submmit your data update operation like add/update/delete in one request:
```
CloudsearchDoc target = new CloudsearchDoc(IndexName, this.api);
target.Add("[{'id':1,'author':'nathan'}]").Update(...).Delete(...).Push(tableName);
```

## Suggestion
Suggestion api is also very easy, one thing you should know is that its search query is not totally same as search api query, it do not needs index name in here.
```
var suggest = new CloudsearchSuggest(applicationName, suggestName,api);
string[] suggestions = suggest.GetSuggest(keyword,hit);
```
## Naming Convention
Since aliyun opensearch is a service which updated from another one, so it also keeps some original names which has ambiguity, such as "index".
In this SDK, all 'index' means the index meaning, and use 'applicatin' to describe a index container.

##License
This SDK is distributed under the [Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0.html)