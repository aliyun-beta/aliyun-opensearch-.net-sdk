using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AliCloud.com.API.Core
{
    internal class HttpHandler
    {
        /// <summary>
        /// var string 定义API版本
        /// </summary>
        const string API_VERSION = "v2";
        private const string SDK_VERSION = "2.1.3";
        private const string SignatureMethod = "HMAC-SHA1";
        private const string SignatureVersion = "1.0";


        private string _accessKeyId;
        private string _accessSecret;

        public HttpHandler(string accessKeyId,string accessSecret)
        {
            _accessKeyId = accessKeyId;
            _accessKeyId = accessSecret;
        }

        public Dictionary<String, Object> RebuildParameter(Dictionary<String, Object> parameters)
        {
            parameters = parameters ?? new Dictionary<String, Object>();

            parameters.Add("Version", API_VERSION);
            parameters.Add("AccessKeyId", this._accessKeyId);
            parameters.Add("SignatureMethod", SignatureMethod);
            parameters.Add("SignatureVersion", SignatureVersion);
            parameters.Add("SignatureNonce", GetSignatureNonce());
            //Y-m-d\TH:i:s\Z
            parameters.Add("Timestamp", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            parameters.Add("Signature", this._makeSignAliyun(parameters, method));

            return parameters;
        }

        public string GetSignatureNonce()
        {
            var guid = Guid.NewGuid();
            byte[] guidBytes = guid.ToByteArray();
            
            long timestamp = DateTime.UtcNow.Ticks / 10000L;
            byte[] timestampBytes = BitConverter.GetBytes(timestamp);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestampBytes);
            }

            byte[] nonceGuid = new byte[16];
            Buffer.BlockCopy(timestampBytes, 2, nonceGuid, 0, 6);
            Buffer.BlockCopy(guidBytes, 0, nonceGuid, 6, 10);

            return Convert.ToBase64String(nonceGuid);
        }

        public string DoRequest()
        {

            return "";
        }

    }
}
