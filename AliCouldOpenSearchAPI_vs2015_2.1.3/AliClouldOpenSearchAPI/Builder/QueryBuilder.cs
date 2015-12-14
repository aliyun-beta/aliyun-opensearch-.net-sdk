using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AliCloudOpenSearch.com.API.Builder
{
    /// <summary>
    ///     A helper class for build opensearch query
    /// </summary>
    public class QueryBuilder
    {
        private IBuilder[] _aggregates;
        private Config _config;

        private bool _disable;
        private Distinct _distinct;
        private List<string> _fetchFields;
        private Filter _filter;
        private string _first_formula_name;
        private ReponseFormat _format;
        private string _formula_name;
        private List<string> _indexNameList;
        private KVpair _kvpair;

        private string _qp;
        private Query _query;
        private Sort _sort;
        private Summary _summary;


        /// <summary>
        ///     Constructor
        /// </summary>
        public QueryBuilder()
        {
            _indexNameList = new List<string>();
            _fetchFields = new List<string>();
        }

        /// <summary>
        ///     Application names (The official document also name it index name)
        /// </summary>
        /// <param name="indexNames"></param>
        /// <returns></returns>
        public QueryBuilder ApplicationNames(params string[] indexNames)
        {
            Utilities.Guard(indexNames);
            _indexNameList.AddRange(indexNames);
            return this;
        }

        public QueryBuilder RemoveApplicationame(params string[] indexNames)
        {
            if (indexNames != null)
            {
                _indexNameList = _indexNameList.FindAll(x => !indexNames.Contains(x));
            }
            return this;
        }

        public QueryBuilder FetchFields(params string[] fields)
        {
            _fetchFields.AddRange(fields);
            return this;
        }

        public QueryBuilder RemoveFetchFields(params string[] fields)
        {
            if (fields != null)
            {
                _fetchFields = _fetchFields.FindAll(x => !fields.Contains(x));
            }

            return this;
        }

        public QueryBuilder Query(Query query)
        {
            _query = query;
            return this;
        }

        public QueryBuilder QP(string qp)
        {
            _qp = qp;
            return this;
        }

        public QueryBuilder Disable(bool disable)
        {
            _disable = disable;
            return this;
        }

        public QueryBuilder FirstFormulaName(string first_formula_name)
        {
            _first_formula_name = first_formula_name;
            return this;
        }

        public QueryBuilder FormulaName(string formula_name)
        {
            _formula_name = formula_name;
            return this;
        }

        public QueryBuilder Summary(Summary summary)
        {
            _summary = summary;
            return this;
        }

        public QueryBuilder Config(Config config)
        {
            _config = config;
            return this;
        }

        public QueryBuilder Aggregate(params Aggregate[] aggregate)
        {
            _aggregates = aggregate;
            return this;
        }

        public QueryBuilder Distinct(Distinct distinct)
        {
            _distinct = distinct;
            return this;
        }

        public QueryBuilder Kvpari(KVpair kvpair)
        {
            _kvpair = kvpair;
            return this;
        }

        public QueryBuilder Sort(Sort sort)
        {
            _sort = sort;
            return this;
        }

        public QueryBuilder Filter(Filter filter)
        {
            _filter = filter;
            return this;
        }

        private StringBuilder addClauseSeperate(StringBuilder str)
        {
            if (str.Length > 0)
            {
                str.Append("&&");
            }

            return str;
        }

        private void addClause(StringBuilder strBuilder, string clauseKey, IBuilder clauseBuilder)
        {
            if (clauseBuilder != null)
            {
                addClauseSeperate(strBuilder).Append(clauseKey).Append("=").Append(clauseBuilder.BuildQuery());
            }
        }

        private void addOption(string key, string val, Dictionary<string, object> parameters)
        {
            if (!string.IsNullOrEmpty(val))
            {
                parameters[key] = val;
            }
        }

        internal Dictionary<string, object> BuildQueryParameter()
        {
            var q = new StringBuilder();

            addClause(q, "config", _config);
            addClause(q, "query", _query);
            addClause(q, "sort", _sort);
            addClause(q, "filter", _filter);
            addClause(q, "distinct", _distinct);
            addClause(q, "kvpairs", _kvpair);

            if (_aggregates != null && _aggregates.Length > 0)
            {
                addClauseSeperate(q)
                    .Append("aggregate=")
                    .Append(string.Join(";", _aggregates.Select(x => x.BuildQuery()).ToArray()));
            }

            var indexs = string.Join(";", _indexNameList);

            var parameters = new Dictionary<string, object>();
            parameters.Add("query", q.ToString());
            parameters.Add("index_name", indexs);

            if (_summary != null)
            {
                parameters.Add("summary", _summary.BuilderQuery());
            }

            addOption("fetch_fields", string.Join(";", _fetchFields.ToArray()), parameters);
            addOption("qp", _qp, parameters);
            addOption("disable", _disable.ToString().ToLower(), parameters);
            addOption("first_formula_name", _first_formula_name, parameters);
            addOption("formula_name", _formula_name, parameters);

            return parameters;
        }
    }
}