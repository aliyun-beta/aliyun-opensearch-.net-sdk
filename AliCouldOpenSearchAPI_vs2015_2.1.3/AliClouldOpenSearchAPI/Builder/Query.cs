using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AliCloudOpenSearch.com.API.Builder
{
    /// <summary>
    ///     Used to generate query clause
    /// </summary>
    public class Query : IBuilder
    {
        private readonly IList<IBuilder> _andNotQrys = new List<IBuilder>();
        private readonly IList<IBuilder> _andQrys = new List<IBuilder>();
        private readonly int _boost = -1;
        private readonly string _keywordAndIndex;
        private readonly IList<IBuilder> _orQrys = new List<IBuilder>();
        private readonly IList<IBuilder> _rankQrys = new List<IBuilder>();
        private readonly Regex regex = new Regex("^.+:");

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="keywordAndIndex">search keywrod, it will add 'default:' if you ignore its index</param>
        public Query(string keywordAndIndex)
        {
            Utilities.Guard(() => !string.IsNullOrEmpty(keywordAndIndex), "keywordAndIndex cannot be null or empty");

            if (!regex.IsMatch(keywordAndIndex))
            {
                keywordAndIndex = "default:" + keywordAndIndex;
            }

            _keywordAndIndex = keywordAndIndex;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="keywordAndIndex">search keywrod, it will add 'default:' if you ignore its index</param>
        /// <param name="boost">Search priority</param>
        public Query(string keywordAndIndex, int boost)
        {
            Utilities.Guard(() => !string.IsNullOrEmpty(keywordAndIndex), "keywordAndIndex cannot be null or empty");
            _keywordAndIndex = keywordAndIndex;
            _boost = boost < 0 ? 0 : (boost > 99 ? 99 : boost);
        }

        /// <summary>
        ///     Generate query clause
        /// </summary>
        /// <returns></returns>
        string IBuilder.BuildQuery()
        {
            var qry = new StringBuilder();
            qry.Append(_keywordAndIndex);

            if (_boost > -1)
            {
                qry.Append("^").Append(_boost);
            }

            Action<IList<IBuilder>, string> func = (lstQyr, op) =>
            {
                foreach (var q in lstQyr)
                {
                    qry.Append(op).Append("(").Append(q.BuildQuery()).Append(")");
                }
            };

            func(_andQrys, " AND ");
            func(_orQrys, " OR ");
            func(_andNotQrys, " ANDNOT ");
            func(_rankQrys, " RANK ");

            return qry.ToString();
        }

        /// <summary>
        ///     Add 'and' query
        /// </summary>
        /// <param name="query">query</param>
        /// <returns>Query instance</returns>
        public Query And(Query query)
        {
            _andQrys.Add(query);
            return this;
        }

        /// <summary>
        ///     Add 'or' query
        /// </summary>
        /// <param name="query">query</param>
        /// <returns>Query instance</returns>
        public Query Or(Query query)
        {
            _orQrys.Add(query);
            return this;
        }

        /// <summary>
        ///     Add 'andnot' query
        /// </summary>
        /// <param name="query">query</param>
        /// <returns>Query instance</returns>
        public Query AndNot(Query query)
        {
            _andNotQrys.Add(query);
            return this;
        }

        /// <summary>
        ///     Add 'rank' query
        /// </summary>
        /// <param name="query">query</param>
        /// <returns>Query instance</returns>
        public Query Rank(Query query)
        {
            _rankQrys.Add(query);
            return this;
        }
    }
}