﻿using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace PlatoCore.Data.Abstractions
{
    public interface IDataProvider 
    {
        
        Task<T> ExecuteReaderAsync<T>(CommandType commandType, string commandText,  Func<DbDataReader, Task<T>> populate, IDbDataParameter[] dbParams) where T : class;

        Task<T> ExecuteScalarAsync<T>(CommandType commandType, string commandText, IDbDataParameter[] dbParams); 

        Task<T> ExecuteNonQueryAsync<T>(CommandType commandType, string commandText, IDbDataParameter[] dbParams);
        
        void HandleException(Exception x);
        
    }

}
