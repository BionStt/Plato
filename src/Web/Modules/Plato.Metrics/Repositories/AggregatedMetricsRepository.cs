﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Metrics;
using PlatoCore.Security.Abstractions;

namespace Plato.Metrics.Repositories
{

    public class AggregatedMetricsRepository : IAggregatedMetricsRepository
    {

        public IDbHelper DbHelper { get; }

        public AggregatedMetricsRepository(IDbHelper dbHelper)
        {
            DbHelper = dbHelper;
        }

        public async Task<AggregatedResult<DateTimeOffset>> SelectGroupedByDateAsync(string groupBy, DateTimeOffset start, DateTimeOffset end)
        {
            // Sql query
            const string sql = @"
                SELECT 
                    COUNT(Id) AS [Count], 
                    MAX({groupBy}) AS [Aggregate] 
                FROM 
                    {prefix}_Metrics
                WHERE 
                    {groupBy} >= '{start}' AND {groupBy} <= '{end}'
                GROUP BY 
                    YEAR({groupBy}),
                    MONTH({groupBy}), 
                    DAY({groupBy})
            ";

            // Sql replacements
            var replacements = new Dictionary<string, string>()
            {
                ["{groupBy}"] = groupBy,
                ["{start}"] = start.ToSortableDateTimePattern(),
                ["{end}"] = end.ToSortableDateTimePattern()
            };

            // Execute and return results
            return await DbHelper.ExecuteReaderAsync(sql, replacements, async reader =>
            {
                var output = new AggregatedResult<DateTimeOffset>();
                while (await reader.ReadAsync())
                {
                    var aggregatedCount = new AggregatedCount<DateTimeOffset>();
                    aggregatedCount.PopulateModel(reader);
                    output.Data.Add(aggregatedCount);
                }
                return output;
            });


        }
        
        // ----------------
        // Grouped by feature
        // ----------------

        public async Task<AggregatedResult<string>> SelectGroupedByFeatureAsync(DateTimeOffset start, DateTimeOffset end)
        {

            // Sql query
            const string sql = @"
                SELECT 
                    f.ModuleId AS [Aggregate] ,
                    COUNT(m.Id) AS Count
                FROM 
                    {prefix}_Metrics m INNER JOIN {prefix}_ShellFeatures f ON f.Id = m.FeatureId
                WHERE 
                    m.CreatedDate >= '{start}' AND m.CreatedDate <= '{end}'
                GROUP BY 
                    f.ModuleId
            ";
            
            // Sql replacements
            var replacements = new Dictionary<string, string>()
            {
                ["{start}"] = start.ToSortableDateTimePattern(),
                ["{end}"] = end.ToSortableDateTimePattern()
            };

            // Execute and return results
            return await DbHelper.ExecuteReaderAsync(sql, replacements, async reader =>
            {
                var output = new AggregatedResult<string>();
                while (await reader.ReadAsync())
                {
                    var aggregatedCount = new AggregatedCount<string>();
                    aggregatedCount.PopulateModel(reader);
                    output.Data.Add(aggregatedCount);
                }
                return output;
            });
            
        }

        // ----------------
        // Grouped by string Title, Url etc
        // ----------------

        public async Task<AggregatedResult<string>> SelectGroupedByStringAsync(
            string groupBy,
            DateTimeOffset start, 
            DateTimeOffset end,
            int limit = 10)
        {

            // Sql query
            const string sql = @"
                SELECT TOP {limit}
                    m.{groupBy} AS [Aggregate] ,
                    COUNT(m.Id) AS [Count]
                FROM 
                    {prefix}_Metrics m 
                WHERE 
                    m.CreatedDate >= '{start}' AND m.CreatedDate <= '{end}'
                GROUP BY 
                    m.{groupBy}
                ORDER BY [Count] DESC
            ";

            // Sql replacements
            var replacements = new Dictionary<string, string>()
            {
                ["{groupBy}"] = groupBy,
                ["{limit}"] = limit.ToString(),
                ["{start}"] = start.ToSortableDateTimePattern(),
                ["{end}"] = end.ToSortableDateTimePattern()
            };

            // Execute and return results
            return await DbHelper.ExecuteReaderAsync(sql, replacements, async reader =>
            {
                var output = new AggregatedResult<string>();
                while (await reader.ReadAsync())
                {
                    var aggregatedCount = new AggregatedCount<string>();
                    aggregatedCount.PopulateModel(reader);
                    output.Data.Add(aggregatedCount);
                }
                return output;
            });

        }

        // ----------------
        // Grouped by role
        // ----------------

        public async Task<AggregatedResult<string>> SelectGroupedByRoleAsync(DateTimeOffset start, DateTimeOffset end)
        {

            // Sql query
            const string sql = @"                                
                DECLARE @temp TABLE
                (
	                [Aggregate] nvarchar(255) NOT NULL,
	                [Count] int NOT NULL
                );

                INSERT INTO @temp
	                SELECT 
		                r.[Name] AS [Aggregate],
		                COUNT(m.Id) AS Count
	                FROM 
		                {prefix}_Metrics m 
		                RIGHT OUTER JOIN {prefix}_UserRoles ur ON ur.UserId = m.CreatedUserId
		                RIGHT OUTER JOIN {prefix}_Roles r ON r.Id = ur.RoleId
                    WHERE 
                        m.CreatedDate >= '{start}' AND m.CreatedDate <= '{end}'
	                GROUP BY 
		                r.[Name]
		                
                -- Get anonymous count
                DECLARE @anonymousCount int;
                SET @anonymousCount = (
	                SELECT 
		                COUNT(m.Id) AS Count
	                FROM 
		                {prefix}_Metrics m 
	                WHERE                     
                        m.CreatedDate >= '{start}' AND m.CreatedDate <= '{end}' AND
                        m.CreatedUserId = 0
                );

                UPDATE @temp SET 
                    [Count] = (@anonymousCount) 
                WHERE [Aggregate] = '{anonymousName}'

                SELECT [Aggregate] AS Aggregate, [Count] AS Count FROM @temp

            ";

            // Sql replacements
            var replacements = new Dictionary<string, string>()
            {
                ["{start}"] = start.ToSortableDateTimePattern(),
                ["{end}"] = end.ToSortableDateTimePattern(),
                ["{anonymousName}"] = DefaultRoles.Anonymous
            };

            // Execute and return results
            return await DbHelper.ExecuteReaderAsync(sql, replacements, async reader =>
            {
                var output = new AggregatedResult<string>();
                while (await reader.ReadAsync())
                {
                    var aggregatedCount = new AggregatedCount<string>();
                    aggregatedCount.PopulateModel(reader);
                    output.Data.Add(aggregatedCount);
                }
                return output;
            });
            

        }
        
    }

}
