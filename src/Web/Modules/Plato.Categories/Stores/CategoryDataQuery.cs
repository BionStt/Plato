﻿using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Plato.Categories.Models;
using PlatoCore.Data.Abstractions;
using PlatoCore.Stores.Abstractions;

namespace Plato.Categories.Stores
{

    #region "CategoryDataQuery"

    public class CategoryDataQuery : DefaultQuery<CategoryData>
    {

        private readonly IStore<CategoryData> _store;

        public CategoryDataQuery(IStore<CategoryData> store)
        {
            _store = store;
        }

        public CategoryDataQueryParams Params { get; set; }

        public override IQuery<CategoryData> Select<T>(Action<T> configure)
        {
            var defaultParams = new T();
            configure(defaultParams);
            Params = (CategoryDataQueryParams)Convert.ChangeType(defaultParams, typeof(CategoryDataQueryParams));
            return this;
        }

        public override async Task<IPagedResults<CategoryData>> ToList()
        {

            var builder = new CategoryDataQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var key = Params.Key.Value ?? string.Empty;
            
            return await _store.SelectAsync(new[]
            {
                new DbParam("PageIndex", DbType.Int32, PageIndex),
                new DbParam("PageSize", DbType.Int32, PageSize),
                new DbParam("SqlPopulate", DbType.String, populateSql),
                new DbParam("SqlCount", DbType.String, countSql),
                new DbParam("Key", DbType.String, 255, key)
            });

        }


    }

    #endregion

    #region "CategoryDataQueryParams"

    public class CategoryDataQueryParams
    {

        private WhereInt _id;
        private WhereInt _CategoryId;
        private WhereString _key;

        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereInt CategoryId
        {
            get => _CategoryId ?? (_CategoryId = new WhereInt());
            set => _CategoryId = value;
        }

        public WhereString Key
        {
            get => _key ?? (_key = new WhereString());
            set => _key = value;
        }

    }

    #endregion

    #region "CategoryDataQueryBuilder"

    public class CategoryDataQueryBuilder : IQueryBuilder
    {
        #region "Constructor"

        private readonly string _CategoryDataTableName;

        private readonly CategoryDataQuery _query;

        public CategoryDataQueryBuilder(CategoryDataQuery query)
        {
            _query = query;
            _CategoryDataTableName = GetTableNameWithPrefix("CategoryData");
        }

        #endregion

        #region "Implementation"

        public string BuildSqlPopulate()
        {
            var whereClause = BuildWhereClause();
            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();
            sb.Append("SELECT ")
                .Append(BuildPopulateSelect())
                .Append(" FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            sb.Append(" ORDER BY ")
                .Append(!string.IsNullOrEmpty(orderBy)
                    ? orderBy
                    : "Id ASC");
            sb.Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            if (!_query.CountTotal)
                return "SELECT 0";
            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(d.Id) FROM ")
                .Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        #endregion

        #region "Private Methods"

        string BuildPopulateSelect()
        {
            return "*";

        }

        string BuildTables()
        {
            var sb = new StringBuilder();
            sb.Append(_CategoryDataTableName).Append(" d ");
            return sb.ToString();
        }

        private string GetTableNameWithPrefix(string tableName)
        {
            return !string.IsNullOrEmpty(_query.Options.TablePrefix)
                ? _query.Options.TablePrefix + tableName
                : tableName;
        }

        private string BuildWhereClause()
        {
            var sb = new StringBuilder();

            // Id
            if (_query.Params.Id.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("d.Id"));
            }

            // CategoryId
            if (_query.Params.CategoryId.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.CategoryId.Operator);
                sb.Append(_query.Params.CategoryId.ToSqlString("CategoryId"));
            }

            // Keywords
            if (!string.IsNullOrEmpty(_query.Params.Key.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Key.Operator);
                sb.Append(_query.Params.Key.ToSqlString("Key", "Keywords"));
            }


            return sb.ToString();

        }


        string GetQualifiedColumnName(string columnName)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            return columnName.IndexOf('.') >= 0
                ? columnName
                : "d." + columnName;
        }

        private string BuildOrderBy()
        {
            if (_query.SortColumns.Count == 0) return null;
            var sb = new StringBuilder();
            var i = 0;
            foreach (var sortColumn in _query.SortColumns)
            {
                sb.Append(GetQualifiedColumnName(sortColumn.Key));
                if (sortColumn.Value != OrderBy.Asc)
                    sb.Append(" DESC");
                if (i < _query.SortColumns.Count - 1)
                    sb.Append(", ");
                i += 1;
            }
            return sb.ToString();
        }

        #endregion

    }

    #endregion

}
