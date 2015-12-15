// -----------------------------------------------------------------------
// <copyright file="CloudSearchApi.cs" company="Aliyun-inc">
// CloudSearchApi
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json.Linq;

namespace AliCloudOpenSearch.com.API
{
    /// <summary>
    ///     云搜索客户端SDK。
    ///     用于管理索引和获取指定索引的操作接口。
    ///     管理索引：
    ///     1. 列出所有索引
    ///     2. 创建索引
    ///     3. 删除索引
    ///     4. 更改索引名称
    /// </summary>
    public class CloudsearchApi
    {
        /// <summary>
        ///     var string 定义API版本
        /// </summary>
        private const string API_VERSION = "v2";

        private const string SDK_VERSION = "2.1.3";


        // 定义请求方式的常量，GET / POST两种
        private const string METHOD_GET = "GET";
        private const string METHOD_POST = "POST";

        /// <summary>
        ///     API接入点 (Endpoint) URL
        /// </summary>
        private readonly string _apiURL = "http://opensearch.etao.com";


        /// <summary>
        ///     用户唯一编号（client_id）。
        ///     此编号为创建用户时自动创建，请访问以下链接来查看您的用户编号：
        ///     @link http://css.aliyun.com/manager/config/
        /// </summary>
        private readonly string _clientId;

        /// <summary>
        ///     客户的密钥。
        ///     此密钥为创建用户时自动创建，您可以访问以下链接来查看或重置您的密钥：
        ///     @link http://css.aliyun.com/manager/config/
        /// </summary>
        private readonly string _clientSecret;


        /// <summary>
        ///     调试开关。
        ///     开关打开后，会将query信息记录下来
        ///     @link http://css.aliyun.com/manager/config/
        /// </summary>
        private readonly bool debug;

        /// <summary>
        ///     记录调试信息。
        ///     @link http://css.aliyun.com/manager/config/
        /// </summary>
        private readonly List<string> debugInfo;

        /// <summary>
        ///     签名方式。
        ///     0原有 1aliyun签名
        /// </summary>
        private readonly int mode;

        /// <summary>
        ///     请求超时时间,默认10s
        ///     @link http://css.aliyun.com/manager/config/
        /// </summary>
        private readonly int requestTimeout = 50000;


        /// 指定阿里云签名算法方式
        /// @var enum('HMAC-SHA1'）
        private readonly string signatureMethod = "HMAC-SHA1";

        /// 指定阿里云签名算法版本
        /// @var enum('HMAC-SHA1'）
        private readonly string signatureVersion = "1.0";

        /// <summary>
        ///     连接超时时间,默认1s
        ///     @link http://css.aliyun.com/manager/config/
        /// </summary>
        private readonly int timeout = 1000;


        /**                                                          
         * 构造函数。
          
         * @param String clientId 客户唯一ID，注册用户时获得。
         * @param String clientSecret 客户的密钥，用于验证客户的操作合法性。
         * @param String host 指定host替换默认的apiURL
         * @param String mode 客户clientapi类型 0普通,1是aliyun方式
         * @param String signatureMethod 签名方式
         * @param String signatureVersion 签名版本
         * @param String timeout 操作超时时间
         * @param String debug 调试信息开关
         * 
         */

        /// <prototype>
        ///     public CloudsearchApi(string clientId, string clientSecret, string host = null, int mode = 0, string
        ///     signatureMethod = "HMAC-SHA1", string signatureVersion = "1.0", int timeout=20000, bool debug=false)
        /// </prototype>
        public CloudsearchApi(string clientId, string clientSecret, string host = null, int mode = 1,
            string signatureMethod = "HMAC-SHA1",
            string signatureVersion = "1.0", int timeout = 20000, bool debug = false)
        {
            this.mode = mode;

            this.signatureMethod = signatureMethod;
            this.signatureVersion = signatureVersion;
            this.timeout = timeout;
            requestTimeout = requestTimeout;

            this.debug = debug;

            if (this.debug)
            {
                debugInfo = new List<string>();
            }

            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId is empty", "clientId");
            }

            var tmpSecret = clientSecret.Trim();
            if (string.IsNullOrEmpty(tmpSecret))
            {
                throw new ArgumentException("clientSecret is empty", "clientSecret");
            }

            if (!string.IsNullOrEmpty(host))
            {
                host = host.TrimEnd('/');
                _apiURL = host;
            }

//            this._apiURL = this._apiURL + "/" + API_VERSION + "/api";
            _clientId = clientId;
            _clientSecret = tmpSecret;
        }


        /// <summary>
        ///     获取debugInfo
        /// </summary>
        /// <returns>debugInfo</returns>
        public List<string> getDebugInfo()
        {
            return debugInfo;
        }


        /// <summary>
        ///     根据参数创建签名信息。
        ///     @param array 参数数组。
        ///     @return string 签名字符串。
        /// </summary>
        /// <param name="parameter">参数数组</param>
        /// <returns>签名字符串</returns>
        private string _makeSign(Dictionary<string, object> parameter)
        {
            var q = string.Empty;
            var sign_mode = string.Empty;

            if (parameter != null)
            {
                if (parameter.ContainsKey("sign_mode"))
                {
                    sign_mode = parameter["sign_mode"] as string;
                }

                parameter = Utilities.KeySort(parameter);

                if (sign_mode == "1" && parameter.ContainsKey("items"))
                {
                    parameter.Remove("items");
                }
                q = Utilities.http_build_query(parameter);
            }

            return Utilities.CalcMd5(q + _clientSecret);
        }


        private string percentEncode(string str)
        {
            // 使用urlencode编码后，将"+","*","%7E"做替换即满足 API规定的编码规范
            str = str.Replace("+", "%20");
            str = str.Replace("*", "%2A");
            str = str.Replace("%7E", "~");
            str = str.Replace("%7e", "~");

            return str;
        }

        /// <summary>
        ///     根据参数创建签名信息(阿里云的算法)。
        ///     @param array 参数数组。
        ///     @return string 签名字符串。
        /// </summary>
        /// <param name="parameter">参数数组</param>
        /// <returns>签名字符串</returns>
        private string _makeSignAliyun(Dictionary<string, object> parameter, string method)
        {
            var q = string.Empty;
            var sign_mode = string.Empty;

            if (parameter != null)
            {
                if (parameter.ContainsKey("sign_mode"))
                {
                    sign_mode = parameter["sign_mode"] as string;
                }

                parameter = Utilities.KeySort(parameter);

                if (sign_mode == "1" && parameter.ContainsKey("items"))
                {
                    parameter.Remove("items");
                }
                q = Utilities.http_build_query(parameter);
                q = Regex.Replace(q, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());
                q = percentEncode(q);
                q = HttpUtility.UrlEncode(q);
                q = percentEncode(q);

                q = method.ToUpper() + "&%2F&" + q;

                q = Regex.Replace(q, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());
            }

            //Console.WriteLine(q);

            return HmacSha1Sign(q, _clientSecret + "&");
        }


        private string HmacSha1Sign(string text, string key)
        {
            var encode = Encoding.GetEncoding("utf-8");
            var byteData = encode.GetBytes(text);
            var byteKey = encode.GetBytes(key);
            var hmac = new HMACSHA1(byteKey);
            var cs = new CryptoStream(Stream.Null, hmac, CryptoStreamMode.Write);
            cs.Write(byteData, 0, byteData.Length);
            cs.Close();
            return Convert.ToBase64String(hmac.Hash);
        }

        /// <summary>
        ///     创建Nonce信息。
        /// </summary>
        /// <returns>返回Nonce信息。</returns>
        private string _makeNonce()
        {
            var time = Utilities.time();
            var nonce = Utilities.CalcMd5(_clientId + _clientSecret + time) + "." + time;
            return nonce;
        }

        private string _makeNonceAliyun()
        {
            var stamp = Utilities.getUnixTimeStamp();
            var ra = new Random();
            var rand = ra.Next(1000, 9999);
            var sb = new StringBuilder();
            sb.Append(stamp);
            sb.Append(rand);
            return sb.ToString();
        }

        /// <summary>
        ///     将参数数组转换成http query字符串
        ///     @param array $params 参数数组
        ///     @return string query 字符串
        /// </summary>
        /// <param name="parameters">参数数组</param>
        /// <returns>字符串</returns>
        protected string BuildParams(Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return string.Empty;
            }

            var args = Utilities.http_build_query(parameters);
            return args;
            //return Utilities.PregReplace(new string[] { "/%5B(?:[0-9]|[1-9][0-9]+)%5D=/" }, new string[] { "=" }, args);
        }

        /// <summary>
        ///     调用Web API。
        /// </summary>
        /// <param name="method">HTTP请求类型，'GET' 或 'POST'。</param>
        /// <param name="url">WEB API的请求URL</param>
        /// <param name="parameters">参数数组。</param>
        /// <param name="httpOptions">http option参数数组。</param>
        /// <param name="webRequest">默认采用socket方式请求，请根据您的空间或服务器类型进行选择 。</param>
        /// <returns>返回decode后的HTTP response。</returns>
        public string ApiCall(string url, Dictionary<string, object> parameters = null, string method = METHOD_POST,
            NameValueCollection httpOptions = null, bool webRequest = true)
        {
            if (mode == 0)
            {
                url = _apiURL + url;
                parameters = parameters ?? new Dictionary<string, object>();
                httpOptions = httpOptions ?? new NameValueCollection();
                parameters.Add("nonce", _makeNonce());
                parameters.Add("client_id", _clientId);
                parameters.Add("sign", _makeSign(parameters));
            }
            else if (mode == 1)
            {
//阿里云签名方式
                url = _apiURL + url;
                parameters = parameters ?? new Dictionary<string, object>();
                httpOptions = httpOptions ?? new NameValueCollection();
                //parameters.Add("nonce", this._makeNonceAliyun());
                //parameters.Add("accesskey", this._clientId);
                //parameters.Add("timestamp", Utilities.getUnixTimeStamp());
                //parameters.Add("signature", this._makeSignAliyun(parameters));

                parameters.Add("Version", API_VERSION);
                parameters.Add("AccessKeyId", _clientId);
                parameters.Add("SignatureMethod", signatureMethod);
                parameters.Add("SignatureVersion", signatureVersion);
                parameters.Add("SignatureNonce", _makeNonceAliyun());
                //Y-m-d\TH:i:s\Z
                parameters.Add("Timestamp", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
                parameters.Add("Signature", _makeSignAliyun(parameters, method));
            }

            JObject data;
            if (webRequest)
            {
                data = requestByWebrequest(method, url, parameters, httpOptions);
            }
            else
            {
                data = requestBySocket(method, url, parameters, httpOptions);
            }
            var rawResponse = data["rawResponse"].ToString();
            return rawResponse;
        }

        /// <summary>
        ///     HttpWebRequest 版本
        ///     使用方法：
        ///     var post_string = new NameValueCollection();;
        ///     post_string.Add(...);
        ///     requestByWebrequest("POST", "http://alicloud.com/",post_string);
        /// </summary>
        /// <param name="method">发送方式，POST/GET</param>
        /// <param name="url">WEB API的请求URL</param>
        /// <param name="parameters">参数数组。</param>
        /// <param name="httpOptions">http option参数数组。</param>
        /// <returns>返回decode后的HTTP response。</returns>
        protected virtual JObject requestByWebrequest(string method, string url, Dictionary<string, object> parameters,
            NameValueCollection httpOptions = null)
        {
            var args = BuildParams(parameters);

            if (method == "GET")
            {
                url += "?" + args;
                args = string.Empty;
                //Console.WriteLine(url);
            }

            if (debug)
            {
                debugInfo.Add(string.Format("encodedQueryString:   {0}", url));
                debugInfo.Add(string.Format("decodedQueryString:   {0}", HttpUtility.UrlDecode(url)));
            }

            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Headers.Clear();
            request.AutomaticDecompression = DecompressionMethods.GZip;
            //request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            //set Timeout
            request.Timeout = timeout;
            request.ReadWriteTimeout = requestTimeout;

            // set Expect100Continue to false, so it will send only one http package.
            request.ServicePoint.Expect100Continue = false;

            // Set request method: 'GET' / 'POST'
            request.Method = method;
            //request.Headers.Add("Expect:");
            request.ContentType = "application/x-www-form-urlencoded";

            var argsInBytes = Encoding.UTF8.GetBytes(args);
            request.ContentLength = argsInBytes.Length;
            request.UserAgent = string.Format("opensearch/.net sdk {0}", SDK_VERSION);
            //request.Accept = "*/*";
            // Write the post data for the POST request
            if (method == "POST")
            {
                //using (var sw = request.GetRequestStream())
                {
                    var sw = request.GetRequestStream();

                    sw.Write(argsInBytes, 0, argsInBytes.Length);
                    sw.Close();
                }
            }

            var responseString = string.Empty;
            var webResonse = (HttpWebResponse) request.GetResponse();
            using (var sr = new StreamReader(webResonse.GetResponseStream()))
            {
                responseString = sr.ReadToEnd();
            }

            webResonse.Close();
            var resultJObject = new JObject();
            resultJObject["httpCode"] = (int) webResonse.StatusCode;
            //resultJObject["rawResponse"] = JObject.Parse(responseString);
            resultJObject["rawResponse"] = responseString;

            return resultJObject;
        }

        /// <summary>
        ///     Socket版本
        ///     使用方法：
        ///     var post_string = new NameValueCollection();;
        ///     post_string.Add(...);
        ///     requestBySocket("POST", "http://alicloud.com/",post_string);
        /// </summary>
        /// <param name="method">HTTP请求类型，'GET' 或 'POST'。</param>
        /// <param name="url">WEB API的请求URL</param>
        /// <param name="parameters">参数数组。</param>
        /// <param name="httpOptions">http option参数数组。</param>
        /// <returns>返回decode后的HTTP response。</returns>
        private JObject requestBySocket(string method, string url, Dictionary<string, object> parameters,
            NameValueCollection httpOptions)
        {
            var ub = new UriBuilder(url);
            var remote_server = ub.Host;
            var remote_path = ub.Path;
            method = method.ToUpper();

            var parse = parseHost(url);
            var data = Utilities.http_build_query(parameters);

            var content = buildRequestContent(parse, method, data);
            //Console.WriteLine(data);
            var receivceStr = string.Empty;
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SendTimeout = requestTimeout;
            socket.ReceiveTimeout = requestTimeout;

            // Set default receive timeout to 10 seconds
            //socSocket
            try
            {
                //socket.Connect(ub.Host, ub.Port);
                var result = socket.BeginConnect(ub.Host, ub.Port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(timeout, true);
                if (!socket.Connected)
                {
                    // NOTE, MUST CLOSE THE SOCKET
                    socket.Close();
                    throw new ApplicationException("Failed to connect server.");
                }


                var contentInByte = Encoding.UTF8.GetBytes(content);
                socket.Send(contentInByte);

                receivceStr = string.Empty;
                var recvBytes = new byte[1024];
                var bytesList = new List<byte>();
                var bytes = 0;
                while (true)
                {
                    bytes = socket.Receive(recvBytes, recvBytes.Length, SocketFlags.None);
                    if (bytes <= 0)
                    {
                        break;
                    }

                    for (var i = 0; i < bytes; ++i)
                    {
                        bytesList.Add(recvBytes[i]);
                    }
                }
                receivceStr = Encoding.UTF8.GetString(bytesList.ToArray(), 0, bytesList.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            var ret = new JObject();
            var parsedRespon = parseResponse(receivceStr);
            ret["httpCode"] = Convert.ToInt32(parsedRespon["info"]["http_code"].ToString());
            ret["rawResponse"] = parsedRespon["result"];

            return ret;
        }

        /// <summary>
        ///     分析Web API的请求Uri
        /// </summary>
        /// <param name="host">Web API的请求Uri</param>
        /// <returns>封装后的UriBuilder</returns>
        private UriBuilder parseHost(string host)
        {
            var ub = new UriBuilder(host);
            if (ub.Scheme == "http")
            {
                ub.Scheme = "";
            }
            else if (ub.Scheme == "https")
            {
                ub.Scheme = @"ssl://";
            }

            ub.Host = ub.Scheme + ub.Host;
            ub.Path = string.IsNullOrEmpty(ub.Path) ? "/" : ub.Path;

            var query = string.IsNullOrEmpty(ub.Query) ? "" : ub.Query;
            var path = ub.Path.Replace(@"\\", "/").Replace(@"//", "/");
            ub.Path = string.IsNullOrEmpty(query) ? path + "?" + query : path;

            return ub;
        }

        /// <summary>
        ///     构造发送请求的内容
        /// </summary>
        /// <param name="parse">封装后的UriBuilder</param>
        /// <param name="method">HTTP请求类型，'GET' 或 'POST'。</param>
        /// <param name="data">拼接好的Uri参数</param>
        /// <returns>构造的请求内容</returns>
        private static string buildRequestContent(UriBuilder parse, string method, string data)
        {
            var contentLengthStr = string.Empty;
            var postContent = string.Empty;
            var destinationUrl = Utilities.RTrim(parse.Path, "%3F");

            string query = null;
            if (method == METHOD_GET)
            {
                if (data[0] == '&')
                {
                    data = data.Substring(1);
                }

                query = string.IsNullOrEmpty(parse.Query) ? "" : parse.Query;
                destinationUrl += (string.IsNullOrEmpty(query) ? "?" : "&") + data;
            }
            else if (method == METHOD_POST)
            {
                contentLengthStr = "Content-length: " + data.Length + "\r\n";
                postContent = data;
            }

            var write = method + " " + destinationUrl + " HTTP/1.0\r\n";
            write += "Host: " + parse.Host + "\r\n";
            write += "Content-type: application/x-www-form-urlencoded\r\n";
            write += contentLengthStr;
            write += "Connection: close\r\n\r\n";
            write += postContent;

            return write;
        }

        /// <summary>
        ///     分析返回的Response内容并封装到JObject实例中。
        /// </summary>
        /// <param name="responseText">response内容</param>
        /// <returns>用response返回内容封装的JObject 实例</returns>
        private static JObject parseResponse(string responseText)
        {
            var result = new JObject();
            var http_header_str = responseText.Substring(0, responseText.IndexOf("\r\n\r\n"));
            var http_headers = parseHttpSocketHeader(http_header_str);

            responseText = responseText.Substring(responseText.IndexOf("\r\n\r\n") + 4);
            result["result"] = responseText;
            result["info"] = new JObject();
            result["info"]["http_code"] = http_headers["Http_Code"] != null ? http_headers["Http_Code"] : 0;
            result["info"]["headers"] = http_headers;

            return result;
        }

        /// <summary>
        ///     从socket header获取http 返回的status code
        /// </summary>
        /// <param name="str">response内容的header，在'/r/n/r/n'之前。</param>
        /// <returns>将header封装到JObject 实例。</returns>
        private static JObject parseHttpSocketHeader(string str)
        {
            var slice = str.Split(new[] {"\r\n"}, StringSplitOptions.None);
            var headers = new JObject();
            foreach (var sli in slice)
            {
                if (sli.Contains("HTTP"))
                {
                    headers["Http_Code"] = sli.Split(' ')[1];
                    headers["Status"] = sli;
                }
                else
                {
                    var indexOfSplit = sli.IndexOf(':');
                    headers[sli.Substring(0, indexOfSplit)] = sli.Substring(indexOfSplit + 1);
                }
            }

            return headers;
        }

        /// <summary>
        ///     将response返回的错误信息转换成Exception。
        /// </summary>
        /// <param name="errorCode">错误编码</param>
        /// <param name="response">API返回的代码</param>
        /// <returns></returns>
        public Exception ResponseToException(uint errorCode, string response)
        {
            var errorMsg = "unknown error";
            var jobject = JObject.Parse(response);
            JToken errorsToke = null;

            if (jobject.TryGetValue("errors", out errorsToke))
            {
                errorMsg = jobject["errors"][0]["message"].ToString();
            }

            throw new Exception(errorCode + ": " + response);
        }
    }
}