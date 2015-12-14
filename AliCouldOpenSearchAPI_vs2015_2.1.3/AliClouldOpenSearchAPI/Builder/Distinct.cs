using System.Text;

namespace AliCloudOpenSearch.com.API.Builder
{
    /// <summary>
    ///     Used to generate distinct clause
    /// </summary>
    public class Distinct : IBuilder
    {
        private string _dist_filter;
        private int _distcnts = -1;
        private readonly string _distinctKey;
        private int _disttimes = -1;
        private double _grade = -1;
        private int _maxItemCount = -1;
        private bool _reserved = true;
        private bool _update_total_hit;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="distinctKey">distinctKey</param>
        public Distinct(string distinctKey)
        {
            Utilities.Guard(() => !string.IsNullOrEmpty(distinctKey), "distinctKey cannot be null or empty");
            _distinctKey = distinctKey;
        }

        /// <summary>
        ///     Generate distinct clause
        /// </summary>
        /// <returns></returns>
        string IBuilder.BuildQuery()
        {
            var q = new StringBuilder();
            q.Append("dist_key:").Append(_distinctKey);

            if (_distcnts > -1)
            {
                q.Append(",dist_count:").Append(_distcnts);
            }

            if (_disttimes > -1)
            {
                q.Append(",dist_times:").Append(_disttimes);
            }

            q.Append(",reserved:").Append(_reserved.ToString().ToLower());

            if (!string.IsNullOrEmpty(_dist_filter))
            {
                q.Append(",dist_filter:").Append(_dist_filter);
            }

            q.Append(",update_total_hit:").Append(_update_total_hit.ToString().ToLower());


            if (_maxItemCount > -1)
            {
                q.Append(",max_item_count:").Append(_maxItemCount);
            }

            if (_grade > -1)
            {
                q.Append(",grade:").Append(_grade);
            }

            return q.ToString();
        }

        /// <summary>
        ///     Set dist_times
        /// </summary>
        /// <param name="dist_times">dist_times</param>
        /// <returns>Distinct instance</returns>
        public Distinct DistinctTimes(int dist_times)
        {
            _disttimes = dist_times;
            return this;
        }

        /// <summary>
        ///     Set dist_count
        /// </summary>
        /// <param name="dist_count">dist_count</param>
        /// <returns>Distinct instance</returns>
        public Distinct DistinctCount(int dist_count)
        {
            _distcnts = dist_count;
            return this;
        }

        /// <summary>
        ///     Set reserved
        /// </summary>
        /// <param name="reserved">reserved</param>
        /// <returns>Distinct instance</returns>
        public Distinct Reserved(bool reserved)
        {
            _reserved = reserved;
            return this;
        }

        /// <summary>
        ///     Set update_total_hit
        /// </summary>
        /// <param name="update_total_hit">update_total_hit</param>
        /// <returns>Distinct instance</returns>
        public Distinct UpdateTotalHit(bool update_total_hit)
        {
            _update_total_hit = update_total_hit;
            return this;
        }

        /// <summary>
        ///     Set dist_filter
        /// </summary>
        /// <param name="dist_filter">dist_filter</param>
        /// <returns>Distinct instance</returns>
        public Distinct DistinctFilter(string dist_filter)
        {
            _dist_filter = dist_filter;
            return this;
        }

        /// <summary>
        ///     Set maxItemCount
        /// </summary>
        /// <param name="maxItemCount">maxItemCount</param>
        /// <returns>Distinct instance</returns>
        public Distinct MaxItemCount(int maxItemCount)
        {
            _maxItemCount = maxItemCount;
            return this;
        }

        /// <summary>
        ///     Set grade
        /// </summary>
        /// <param name="grade">grade</param>
        /// <returns>Distinct instance</returns>
        public Distinct Grade(double grade)
        {
            _grade = grade;
            return this;
        }
    }
}