﻿using System;
using System.Collections.Generic;
using PlatoCore.Data.Abstractions;
using PlatoCore.Data.Schemas.Abstractions;
using PlatoCore.Data.Schemas.Abstractions.Builders;
using PlatoCore.Text.Abstractions;

namespace PlatoCore.Data.Schemas.Builders
{

    /// <summary>
    /// A common base class to assist various schema builders.
    /// </summary>
    public class SchemaBuilderBase : ISchemaBuilderBase
    {

        public string NewLine => Environment.NewLine;
        
        public ICollection<string> Statements { get; }

        public SchemaBuilderOptions Options { get; private set; }

        private readonly string _tablePrefix;

        private readonly IPluralize _pluralize;

        public SchemaBuilderBase(
            IDbContext dbContext,
            IPluralize pluralize)
        {
            _pluralize = pluralize;
            _tablePrefix = dbContext.Configuration.TablePrefix;
            Statements = new List<string>();
        }

        public ISchemaBuilderBase Configure(Action<SchemaBuilderOptions> configure)
        {
            Options = new SchemaBuilderOptions();
            configure(Options);
            return this;
        }
        

        public string GetSingularTableName(string tableName)
        {
            return _pluralize.Singular(tableName);
        }
        
        public string PrependTablePrefix(string input)
        {
            return !string.IsNullOrEmpty(_tablePrefix)
                ? _tablePrefix + input
                : input;
        }

        public ISchemaBuilderBase AddStatement(string statement)
        {
            if (!string.IsNullOrEmpty(statement))
                Statements.Add(statement);
            return this;
        }

        public string ParseExplicitSql(string input)
        {
            return input
                .Replace("{prefix}_", _tablePrefix)
                .Replace("  ", "")
                .Replace("      ", "");
        }

        public void Dispose()
        {
            this.Statements.Clear();
        }

    }
    
}
