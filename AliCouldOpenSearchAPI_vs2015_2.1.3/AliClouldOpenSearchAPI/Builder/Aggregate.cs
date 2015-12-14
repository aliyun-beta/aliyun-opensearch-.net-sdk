using System;
using System.Text;

namespace AliCloudOpenSearch.com.API.Builder
{
    /// <summary>
    ///     Used to generate aggrefate clause
    /// </summary>
    public class Aggregate : IBuilder
    {
        private string _agg_sampler_step;
        private string _agg_sampler_threshold;
        private string _aggfilter;
        private readonly string _aggfun;
        private readonly string _grpkey;
        private int _maxgroup = -1;
        private string _range;

        /// <summary>
        ///     Construct
        /// </summary>
        /// <param name="group_key">aggregate parameter:group_key</param>
        /// <param name="agg_fun">aggregate parameter:agg_fun</param>
        public Aggregate(string group_key, string agg_fun)
        {
            Utilities.Guard(() => !string.IsNullOrEmpty(group_key), "group_key cannot be null or empty");
            Utilities.Guard(() => !string.IsNullOrEmpty(agg_fun), "agg_fun cannot be null or empty");

            _grpkey = group_key;
            _aggfun = agg_fun;
        }

        /// <summary>
        ///     Generate aggregate clause
        /// </summary>
        /// <returns>aggregate clause</returns>
        string IBuilder.BuildQuery()
        {
            var q = new StringBuilder();
            q.Append("group_key:").Append(_grpkey).Append(",agg_fun:").Append(_aggfun);

            Action<string, string> s = (v, k) =>
            {
                if (!string.IsNullOrEmpty(v))
                {
                    q.Append(",").Append(k).Append(":").Append(v);
                }
            };

            s(_range, "range");
            s(_aggfilter, "agg_filter");
            s(_agg_sampler_threshold, "agg_sampler_threshold");
            s(_agg_sampler_step, "agg_sampler_step");

            if (_maxgroup > -1)
            {
                q.Append(",").Append("max_group").Append(":").Append(_maxgroup);
            }

            return q.ToString();
        }

        /// <summary>
        ///     set aggregate parameter:range
        /// </summary>
        /// <param name="range">aggregate parameter:range</param>
        /// <returns>Aggregate instance</returns>
        public Aggregate Range(string range)
        {
            _range = range;
            return this;
        }

        /// <summary>
        ///     set aggrefate parameter:aggfilter
        /// </summary>
        /// <param name="aggfilter">aggrefate parameter:aggfilter</param>
        /// <returns>Aggregate instance</returns>
        public Aggregate AggFilter(string aggfilter)
        {
            _aggfilter = aggfilter;
            return this;
        }

        /// <summary>
        ///     Set aggrefate parameter:agg_sampler_threshold
        /// </summary>
        /// <param name="agg_sampler_threshold">aggrefate parameter:agg_sampler_threshold</param>
        /// <returns>Aggregate instance</returns>
        public Aggregate AggSamplerThreshold(string agg_sampler_threshold)
        {
            _agg_sampler_threshold = agg_sampler_threshold;
            return this;
        }

        /// <summary>
        ///     Set aggrefate parameter:agg_sampler_step
        /// </summary>
        /// <param name="agg_sampler_step">aggrefate parameter:agg_sampler_step</param>
        /// <returns>Aggregate instance</returns>
        public Aggregate AggSamplerStep(string agg_sampler_step)
        {
            _agg_sampler_step = agg_sampler_step;
            return this;
        }

        /// <summary>
        ///     Set aggrefate parameter:max_group
        /// </summary>
        /// <param name="max_group">aggrefate parameter:max_group</param>
        /// <returns>Aggregate instance</returns>
        public Aggregate MaxGroup(int max_group)
        {
            _maxgroup = max_group;
            return this;
        }
    }
}