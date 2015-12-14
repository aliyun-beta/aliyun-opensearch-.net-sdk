using System.Collections.Generic;
using System.Text;

namespace AliCloudOpenSearch.com.API.Builder
{
    /// <summary>
    ///     Used to generate summary parameter
    /// </summary>
    public class Summary
    {
        private string _summary_element;
        private string _summary_ellipsis;
        private string _summary_field;
        private string _summary_len;
        private string _summary_postfix;
        private string _summary_prefix;
        private int _summary_snipped = -1;
        private readonly Dictionary<string, string> dics = new Dictionary<string, string>();

        /// <summary>
        ///     Construct
        /// </summary>
        /// <param name="summary_field"></param>
        public Summary(string summary_field)
        {
            dics["summary_field"] = summary_field;
        }

        /// <summary>
        ///     Set summary parameter:summary_element
        /// </summary>
        /// <param name="summary_element">summary_element</param>
        /// <returns>Summary instance</returns>
        public Summary SummaryElement(string summary_element)
        {
            dics["summary_element"] = summary_element;
            return this;
        }

        /// <summary>
        ///     Set summary_ellipsis
        /// </summary>
        /// <param name="summary_ellipsis">summary_ellipsis</param>
        /// <returns>Summary instance</returns>
        public Summary SummaryEllipsis(string summary_ellipsis)
        {
            dics["summary_ellipsis"] = summary_ellipsis;
            return this;
        }

        /// <summary>
        ///     Set summary_snipped
        /// </summary>
        /// <param name="summary_snipped">summary_snipped</param>
        /// <returns>Summary instance</returns>
        public Summary SummarySnipped(int summary_snipped)
        {
            dics["summary_snipped"] = "" + summary_snipped;
            return this;
        }

        /// <summary>
        ///     Set summary_len
        /// </summary>
        /// <param name="summary_len">summary_len</param>
        /// <returns>Summary instance</returns>
        public Summary SummaryLen(int summary_len)
        {
            dics["summary_len"] = summary_len.ToString();
            return this;
        }

        /// <summary>
        ///     Set summary_prefix
        /// </summary>
        /// <param name="summary_prefix">summary_prefix</param>
        /// <returns>Summary instance</returns>
        public Summary SummaryPrefix(string summary_prefix)
        {
            dics["summary_prefix"] = summary_prefix;
            return this;
        }

        /// <summary>
        ///     Set summary_postfix
        /// </summary>
        /// <param name="summary_postfix">summary_postfix</param>
        /// <returns>Summary instance</returns>
        public Summary SummaryPostfix(string summary_postfix)
        {
            dics["summary_postfix"] = summary_postfix;
            return this;
        }

        /// <summary>
        ///     Generate summary query
        /// </summary>
        /// <returns>summary query</returns>
        internal string BuilderQuery()
        {
            var q = new StringBuilder();

            foreach (var k in dics.Keys)
            {
                q.Append(k).Append(":").Append(dics[k]).Append(",");
            }

            q.Remove(q.Length - 1, 1).Append(";");
            return q.ToString();
        }
    }
}