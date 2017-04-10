using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using AliCloudOpenSearch.com.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AliCloudAPITest
{
    internal class CloudsearchApiMock : CloudsearchApi
    {
        public CloudsearchApiMock(string clientId, string clientSecret, string host = null, int mode = 0,
            string signatureMethod = "HMAC-SHA1",
            string signatureVersion = "1.0", int requestTimeout = 10000, bool debug = false)
            : base(clientId, clientSecret, host, mode, signatureMethod,
                signatureVersion, requestTimeout, debug)
        {
        }

        protected override JObject requestByWebrequest(string method, string url, Dictionary<string, object> parameters,
            NameValueCollection httpOptions = null)
        {
            var args = BuildParams(parameters);

            var resultJObject = new JObject();
            resultJObject["rawResponse"] = JsonConvert.SerializeObject(new  {Url=url,Query=args,status="OK"});

            return resultJObject;
        }
#if NET45
        protected override async Task<JObject> requestByWebrequestAsync(string method, string url, Dictionary<string, object> parameters, NameValueCollection httpOptions = null)
        {
            return await Task.Run(() =>
            {
                return requestByWebrequest(method, url, parameters, httpOptions);
            }).ConfigureAwait(false);
        }
#endif
    }
}