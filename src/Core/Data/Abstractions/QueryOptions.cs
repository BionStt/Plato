﻿namespace PlatoCore.Data.Abstractions
{

    public enum SearchTypes
    {
        Tsql = 0,
        ContainsTable = 1,
        FreeTextTable = 2
    }

    public interface IQueryOptions
    {

        string TablePrefix { get; set; }

        int MaxResults { get; set; }

        SearchTypes SearchType { get; set; }
        

    }

    public class QueryOptions : IQueryOptions
    {

        public string TablePrefix { get; set; }

        public int MaxResults { get; set; }

        public SearchTypes SearchType { get; set; } = SearchTypes.Tsql;

    }

}
