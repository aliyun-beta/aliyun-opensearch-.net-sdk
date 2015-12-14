using System.Text;

namespace AliCloudOpenSearch.com.API.Builder
{
    /// <summary>
    ///     Used to generate sort clause
    /// </summary>
    public class Sort : IBuilder
    {
        private readonly StringBuilder _q = new StringBuilder();

        string IBuilder.BuildQuery()
        {
            return _q.ToString();
        }

        /// <summary>
        ///     Desc sorted a field
        /// </summary>
        /// <param name="field">field nanme</param>
        /// <returns>Sort instance</returns>
        public Sort Desc(string field)
        {
            addSeperate().Append("-").Append(field);
            return this;
        }

        /// <summary>
        ///     Asc sorted a field
        /// </summary>
        /// <param name="field">field nanme</param>
        /// <returns>Sort instance</returns>
        public Sort Asc(string field)
        {
            addSeperate().Append("+").Append(field);
            return this;
        }

        /// <summary>
        ///     Desc sorted by rank
        /// </summary>
        /// <returns>Sort instance</returns>
        public Sort DescByRank()
        {
            addSeperate().Append("-RANK");
            return this;
        }


        /// <summary>
        ///     Asc sorted by rank
        /// </summary>
        /// <returns>Sort instance</returns>
        public Sort AscByRank()
        {
            addSeperate().Append("+RANK");
            return this;
        }

        private StringBuilder addSeperate()
        {
            if (_q.Length > 0)
            {
                _q.Append(";");
            }

            return _q;
        }
    }
}