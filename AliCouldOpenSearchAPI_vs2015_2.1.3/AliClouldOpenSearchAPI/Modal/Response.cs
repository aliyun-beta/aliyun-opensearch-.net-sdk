using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AliCloudOpenSearch.com.API.Modal
{
    /// <summary>
    ///     Response wrapper
    /// </summary>
    public class Response
    {
        public Response()
        {
            Status = "OK";
        }

        public ErrorMessage[] Errors { get; set; }

        /// <summary>
        ///     returned status
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        ///     returned request_id
        /// </summary>
        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        public JObject Result { get; set; }
    }
}