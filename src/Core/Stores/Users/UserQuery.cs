﻿using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using PlatoCore.Models.Users;
using PlatoCore.Data.Abstractions;
using PlatoCore.Stores.Abstractions;

namespace PlatoCore.Stores.Users
{

    #region "UserQuery"

    public class UserQuery : DefaultQuery<User>
    {

        private readonly IQueryableStore<User> _store;

        public UserQuery(IQueryableStore<User> store)
        {
            _store = store;
        }

        public UserQueryParams Params { get; set; } = new UserQueryParams();

        public override IQuery<User> Select<TParams>(Action<TParams> configure)
        {
            var defaultParams = new TParams();
            configure(defaultParams);
            Params = (UserQueryParams)Convert.ChangeType(defaultParams, typeof(UserQueryParams));
            return this;
        }

        public override async Task<IPagedResults<User>> ToList()
        {

            var builder = new UserQueryBuilder(this);
            var populateSql = builder.BuildSqlPopulate();
            var countSql = builder.BuildSqlCount();
            var keywords = Params.Keywords.Value ?? string.Empty;
            var roleName = Params.RoleName.Value ?? string.Empty;

            return await _store.SelectAsync(new IDbDataParameter[]
            {
                new DbParam("PageIndex", DbType.Int32, PageIndex),
                new DbParam("PageSize", DbType.Int32, PageSize),
                new DbParam("SqlPopulate", DbType.String, populateSql),
                new DbParam("SqlCount", DbType.String, countSql),
                new DbParam("Keywords", DbType.String, 255, keywords),
                new DbParam("RoleName", DbType.String, 255, roleName),
            });

        }

    }

    #endregion

    #region "UserQueryParams"

    public class UserQueryParams
    {

        private WhereInt _id;
        private WhereString _keywords;
        private WhereInt _roleId;
        private WhereString _roleName;
        private WhereBool _showSpam;
        private WhereBool _hideSpam;
        private WhereBool _showStaff;
        private WhereBool _hideStaff;
        private WhereBool _showVerified;
        private WhereBool _hideVerified;
        private WhereBool _showBanned;
        private WhereBool _hideBanned;
        private WhereBool _showConfirmed;
        private WhereBool _hideConfirmed;
        private WhereBool _showUnconfirmed;
        private WhereBool _hideUnconfirmed;
        private WhereBool _showLocked;
        private WhereBool _hideLocked;
        private WhereEnum<UserType> _userType;
        
        public WhereInt Id
        {
            get => _id ?? (_id = new WhereInt());
            set => _id = value;
        }

        public WhereString Keywords
        {
            get => _keywords ?? (_keywords = new WhereString());
            set => _keywords = value;
        }

        public WhereInt RoleId
        {
            get => _roleId ?? (_roleId = new WhereInt());
            set => _roleId = value;
        }

        public WhereString RoleName
        {
            get => _roleName ?? (_roleName = new WhereString());
            set => _roleName = value;
        }
        
        public WhereBool HideSpam
        {
            get => _hideSpam ?? (_hideSpam = new WhereBool());
            set => _hideSpam = value;
        }

        public WhereBool ShowSpam
        {
            get => _showSpam ?? (_showSpam = new WhereBool());
            set => _showSpam = value;
        }

        public WhereBool HideStaff
        {
            get => _hideStaff ?? (_hideStaff = new WhereBool());
            set => _hideStaff = value;
        }

        public WhereBool ShowStaff
        {
            get => _showStaff ?? (_showStaff = new WhereBool());
            set => _showStaff = value;
        }
        
        public WhereBool HideVerified
        {
            get => _hideVerified ?? (_hideVerified = new WhereBool());
            set => _hideVerified = value;
        }

        public WhereBool ShowVerified
        {
            get => _showVerified ?? (_showVerified = new WhereBool());
            set => _showVerified = value;
        }
        
        public WhereBool HideBanned
        {
            get => _hideBanned ?? (_hideBanned = new WhereBool());
            set => _hideBanned = value;
        }

        public WhereBool ShowBanned
        {
            get => _showBanned ?? (_showBanned = new WhereBool());
            set => _showBanned = value;
        }

        public WhereBool HideConfirmed
        {
            get => _hideConfirmed ?? (_hideConfirmed = new WhereBool());
            set => _hideConfirmed = value;
        }

        public WhereBool ShowConfirmed
        {
            get => _showConfirmed ?? (_showConfirmed = new WhereBool());
            set => _showConfirmed = value;
        }

        public WhereBool HideUnconfirmed
        {
            get => _hideUnconfirmed ?? (_hideUnconfirmed = new WhereBool());
            set => _hideUnconfirmed = value;
        }

        public WhereBool ShowUnconfirmed
        {
            get => _showUnconfirmed ?? (_showUnconfirmed = new WhereBool());
            set => _showUnconfirmed = value;
        }


        public WhereBool HideLocked
        {
            get => _hideLocked ?? (_hideLocked = new WhereBool());
            set => _hideLocked = value;
        }

        public WhereBool ShowLocked
        {
            get => _showLocked ?? (_showLocked = new WhereBool());
            set => _showLocked = value;
        }

        public WhereEnum<UserType> UserType
        {
            get => _userType ?? (_userType = new WhereEnum<UserType>());
            set => _userType = value;
        }
    }

    #endregion

    #region "UserQueryBuilder"

    public class UserQueryBuilder : IQueryBuilder
    {

        #region "Constructor"

        private readonly string _usersTableName;
        private readonly string _userRolesTableName;
        private readonly string _rolesTableName;
        private readonly UserQuery _query;

        public UserQueryBuilder(UserQuery query)
        {
            _query = query;
            _usersTableName = GetTableNameWithPrefix("Users");
            _userRolesTableName = GetTableNameWithPrefix("UserRoles");
            _rolesTableName = GetTableNameWithPrefix("Roles");
        }

        #endregion

        #region "Implementation"

        public string BuildSqlPopulate()
        {
            var whereClause = BuildWhereClause();
            var orderBy = BuildOrderBy();
            var sb = new StringBuilder();
            sb.Append("SELECT * FROM ").Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            // Order only if we have something to order by
            sb.Append(" ORDER BY ").Append(!string.IsNullOrEmpty(orderBy)
                ? orderBy
                : "(SELECT NULL)");
            // Limit results only if we have a specific page size
            if (!_query.IsDefaultPageSize)
                sb.Append(" OFFSET @RowIndex ROWS FETCH NEXT @PageSize ROWS ONLY;");
            return sb.ToString();
        }

        public string BuildSqlCount()
        {
            if (!_query.CountTotal)
                return string.Empty;
            var whereClause = BuildWhereClause();
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(Id) FROM ").Append(BuildTables());
            if (!string.IsNullOrEmpty(whereClause))
                sb.Append(" WHERE (").Append(whereClause).Append(")");
            return sb.ToString();
        }

        #endregion

        #region "Private Methods"

        private string BuildTables()
        {
            var sb=  new StringBuilder();
            sb.Append(_usersTableName).Append(" u");
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

            if (_query.Params.Id.Value > -1)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Id.Operator);
                sb.Append(_query.Params.Id.ToSqlString("Id"));
            }

            // -----------------
            // Keywords 
            // -----------------

            if (!String.IsNullOrEmpty(_query.Params.Keywords.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.Keywords.Operator);
                sb
                    .Append("(")
                    .Append(_query.Params.Keywords.ToSqlString("UserName", "Keywords"))
                    .Append(" OR ")
                    .Append(_query.Params.Keywords.ToSqlString("DisplayName", "Keywords"))
                    .Append(" OR ")
                    .Append(_query.Params.Keywords.ToSqlString("Email", "Keywords"))
                    .Append(" OR ")
                    .Append(_query.Params.Keywords.ToSqlString("FirstName", "Keywords"))
                    .Append(" OR ")
                    .Append(_query.Params.Keywords.ToSqlString("LastName", "Keywords"))
                    .Append(")");
            }

            // -----------------
            // RoleName 
            // -----------------

            if (!String.IsNullOrEmpty(_query.Params.RoleName.Value))
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.RoleName.Operator);
                sb.Append("u.Id IN (")
                    .Append("SELECT ur.UserId FROM ")
                    .Append(_userRolesTableName)
                    .Append(" ur WHERE ur.RoleId IN (SELECT r.Id FROM ")
                    .Append(_rolesTableName).Append(" r WHERE ")
                    .Append(_query.Params.RoleName.ToSqlString("r.[Name]", "RoleName"))
                    .Append("))");
            }

            // -----------------
            // UserType 
            // -----------------

            if (_query.Params.UserType.Value != UserType.None)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.UserType.Operator);
                sb.Append(_query.Params.UserType.ToSqlString("UserType"));

            }

            // -----------------
            // IsSpam 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideSpam.Value && !_query.Params.ShowSpam.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideSpam.Operator);
                sb.Append("IsSpam = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowSpam.Value && !_query.Params.HideSpam.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowSpam.Operator);
                sb.Append("IsSpam = 1");
            }

            // -----------------
            // IsStaff
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideStaff.Value && !_query.Params.ShowStaff.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideStaff.Operator);
                sb.Append("IsStaff = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowStaff.Value && !_query.Params.HideStaff.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowStaff.Operator);
                sb.Append("IsStaff = 1");
            }
            
            // -----------------
            // IsVerified
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideVerified.Value && !_query.Params.ShowVerified.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideVerified.Operator);
                sb.Append("IsVerified = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowVerified.Value && !_query.Params.HideVerified.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowVerified.Operator);
                sb.Append("IsVerified = 1");
            }

            // -----------------
            // IsBanned 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideBanned.Value && !_query.Params.ShowBanned.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideBanned.Operator);
                sb.Append("IsBanned = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowBanned.Value && !_query.Params.HideBanned.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowBanned.Operator);
                sb.Append("IsBanned = 1");
            }

            // -----------------
            // EmailConfirmed 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideConfirmed.Value && !_query.Params.ShowConfirmed.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideConfirmed.Operator);
                sb.Append("EmailConfirmed = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowConfirmed.Value && !_query.Params.HideConfirmed.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowConfirmed.Operator);
                sb.Append("EmailConfirmed = 1");
            }

            // hide = true, show = false
            if (_query.Params.HideUnconfirmed.Value && !_query.Params.ShowUnconfirmed.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideConfirmed.Operator);
                sb.Append("EmailConfirmed = 1");
            }

            // show = true, hide = false
            if (_query.Params.ShowUnconfirmed.Value && !_query.Params.HideUnconfirmed.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowConfirmed.Operator);
                sb.Append("EmailConfirmed = 0");
            }
            
            // -----------------
            // LockoutEnabled 
            // -----------------

            // hide = true, show = false
            if (_query.Params.HideLocked.Value && !_query.Params.ShowLocked.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.HideLocked.Operator);
                sb.Append("LockoutEnabled = 0");
            }

            // show = true, hide = false
            if (_query.Params.ShowLocked.Value && !_query.Params.HideLocked.Value)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                    sb.Append(_query.Params.ShowLocked.Operator);
                sb.Append("LockoutEnabled = 1");
            }
                       
            return sb.ToString();

        }

        private string BuildOrderBy()
        {
            if (_query.SortColumns.Count == 0) return null;
            var sb = new StringBuilder();
            var i = 0;
            foreach (var sortColumn in GetSafeSortColumns())
            {
                sb.Append(sortColumn.Key);
                if (sortColumn.Value != OrderBy.Asc)
                    sb.Append(" DESC");
                if (i < _query.SortColumns.Count - 1)
                    sb.Append(", ");
                i += 1;
            }

            return sb.ToString();
        }

        private IDictionary<string, OrderBy> GetSafeSortColumns()
        {
            var output = new Dictionary<string, OrderBy>();
            foreach (var sortColumn in _query.SortColumns)
            {
                var columnName = GetSortColumn(sortColumn.Key);
                if (!String.IsNullOrEmpty(columnName))
                {
                    output.Add(columnName, sortColumn.Value);
                }
            }
            return output;
        }

        private string GetSortColumn(string columnName)
        {

            if (String.IsNullOrEmpty(columnName))
            {
                return string.Empty;
            }

            switch (columnName.ToLower())
            {
                case "id":
                    return "Id";
                case "username":
                    return "UserName";
                case "email":
                    return "Email";
                case "firstname":
                    return "FirstName";
                case "lastname":
                    return "LastName";
                case "visits":
                    return "Visits";
                case "totalvisits":
                    return "Visits";
                case "minutes":
                    return "MinutesActive";
                case "minutesactive":
                    return "MinutesActive";
                case "reputation":
                    return "Reputation";
                case "totalreputation":
                    return "Reputation";
                case "rank":
                    return "[Rank]";
                case "createddate":
                    return "CreatedDate";
                case "modified":
                    return "ModifiedDate";
                case "modifieddate":
                    return "ModifiedDate";
                case "lastlogindate":
                    return "LastLoginDate";
            }

            return string.Empty;
            
        }

        #endregion

    }

    #endregion

}