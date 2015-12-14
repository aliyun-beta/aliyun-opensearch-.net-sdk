using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AliCloudOpenSearch.com.API.Builder
{
    /// <summary>
    ///     Used to generate config clause
    /// </summary>
    public class Config : IBuilder
    {
        private ReponseFormat _format;
        private int _hit = -1;
        private int _rerankSize = -1;
        private int _start = -1;

        /// <summary>
        ///     Generate config clause
        /// </summary>
        /// <returns>config clause</returns>
        string IBuilder.BuildQuery()
        {
            var vals = new Dictionary<string, object>();

            if (_start > -1)
            {
                vals["start"] = _start;
            }

            if (_hit > -1)
            {
                vals["hit"] = _hit;
            }

            if (_rerankSize > -1)
            {
                vals["rerank_size"] = _rerankSize;
            }


            vals["format"] = Enum.GetName(typeof (ReponseFormat), _format).ToLower();


            if (vals.Count() <= 0)
            {
                return string.Empty;
            }

            var strQry = new StringBuilder();

            foreach (var key in vals.Keys)
            {
                strQry.Append(key).Append(":").Append(vals[key]).Append(",");
            }

            strQry.Remove(strQry.Length - 1, 1);
            return strQry.ToString();
        }

        /// <summary>
        ///     Set start
        /// </summary>
        /// <param name="start">start</param>
        /// <returns>Config instance</returns>
        public Config Start(int start)
        {
            _start = start;
            return this;
        }

        /// <summary>
        ///     Set hit
        /// </summary>
        /// <param name="hit">hit</param>
        /// <returns>Config instance</returns>
        public Config Hit(int hit)
        {
            _hit = hit;
            return this;
        }

        /// <summary>
        ///     Set format
        /// </summary>
        /// <param name="format">format</param>
        /// <returns>Config instance</returns>
        public Config Format(ReponseFormat format)
        {
            _format = format;
            return this;
        }

        /// <summary>
        ///     Set rerankSize
        /// </summary>
        /// <param name="rerankSize">rerankSize</param>
        /// <returns>Config instance</returns>
        public Config RerankSize(int rerankSize)
        {
            _rerankSize = rerankSize;
            return this;
        }
    }
}