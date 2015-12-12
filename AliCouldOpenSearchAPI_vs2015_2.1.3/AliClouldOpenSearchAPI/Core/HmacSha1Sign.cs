using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AliCloud.com.API.Core
{
    public class HmacSha1Sign : ISignature
    {
        public string SignatureMethod
        {
            get { return "HMAC-SHA1"; }
        }

        public string SignatureVersion
        {
            get { return "1.0"; }
        }

        public string Sign(Dictionary<String, Object> parameters)
        {
            string q = string.Empty;
            String sign_mode = string.Empty;

            if (parameters != null)
            {


                parameters = Utilities.KeySort(parameters);

               
                q = Utilities.http_build_query(parameter);
                q = Regex.Replace(q, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());
                q = percentEncode(q);
                q = HttpUtility.UrlEncode(q);
                q = percentEncode(q);

                q = method.ToUpper() + "&%2F&" + q;

                q = Regex.Replace(q, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());
            }
        }
    }
}
