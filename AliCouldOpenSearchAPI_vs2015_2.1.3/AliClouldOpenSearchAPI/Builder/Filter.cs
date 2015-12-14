using System;
using System.Collections.Generic;
using System.Text;

namespace AliCloudOpenSearch.com.API.Builder
{
    /// <summary>
    ///     Used to generate filter clause
    /// </summary>
    public class Filter : IBuilder
    {
        private readonly IList<IBuilder> _andFilters = new List<IBuilder>();

        private readonly string _filter;
        private readonly IList<IBuilder> _orFilters = new List<IBuilder>();

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="filter">filter</param>
        public Filter(string filter)
        {
            Utilities.Guard(() => !string.IsNullOrEmpty(filter), "filter cannot be null or empty");

            _filter = filter;
        }

        /// <summary>
        ///     Generate filter clause
        /// </summary>
        /// <returns></returns>
        string IBuilder.BuildQuery()
        {
            var qry = new StringBuilder();
            qry.Append(_filter);

            Action<IList<IBuilder>, string> func = (lstQyr, op) =>
            {
                foreach (var q in lstQyr)
                {
                    qry.Append(op).Append("(").Append(q.BuildQuery()).Append(")");
                }
            };

            func(_andFilters, " AND ");
            func(_orFilters, " OR ");

            return qry.ToString();
        }

        /// <summary>
        ///     Add 'and' filter
        /// </summary>
        /// <param name="filter">filter</param>
        /// <returns>Filter instance</returns>
        public Filter And(Filter filter)
        {
            _andFilters.Add(filter);
            return this;
        }

        /// <summary>
        ///     Add 'or' filter
        /// </summary>
        /// <param name="filter">filter</param>
        /// <returns>Filter instance</returns>
        public Filter Or(Filter filter)
        {
            _orFilters.Add(filter);
            return this;
        }
    }
}