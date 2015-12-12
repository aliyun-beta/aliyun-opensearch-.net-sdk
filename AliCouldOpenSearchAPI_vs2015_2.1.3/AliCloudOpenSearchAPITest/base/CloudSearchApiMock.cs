using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using AliCloudOpenSearch.com.API;
using Newtonsoft.Json.Linq;

namespace AliCloudAPITest
{
    class CloudsearchApiMock : CloudsearchApi
    {
        public CloudsearchApiMock(string clientId, string clientSecret, string host = null, int mode = 0, string signatureMethod = "HMAC-SHA1",
            string signatureVersion = "1.0", int requestTimeout = 10000,bool debug=false)
             :base( clientId,  clientSecret,  host ,  mode ,  signatureMethod ,
             signatureVersion ,    requestTimeout , debug)
        {
             
         }

        protected  override JObject requestByWebrequest(string method, string url, Dictionary<String, Object> parameters, NameValueCollection httpOptions = null)
        {

            string args = this.BuildParams(parameters);

            url += "?" + args;
            args = string.Empty;
            
            JObject resultJObject = new JObject();
            resultJObject["rawResponse"] = url;

            return resultJObject;
        }
    }
}
