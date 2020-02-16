﻿using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Data.Schemas.Abstractions;
using PlatoCore.Features.Abstractions;

namespace Plato.Entities.Metrics.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {

        public string Version { get; } = "1.0.0";

        // Entity Metrics table
        private readonly SchemaTable _entityMetrics = new SchemaTable()
        {
            Name = "EntityMetrics",
            Columns = new List<SchemaColumn>()
                {
                    new SchemaColumn()
                    {
                        PrimaryKey = true,
                        Name = "Id",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "EntityId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV4Address",
                        DbType = DbType.String,
                        Length = "20"
                    },
                    new SchemaColumn()
                    {
                        Name = "IpV6Address",
                        DbType = DbType.String,
                        Length = "50"
                    },
                    new SchemaColumn()
                    {
                        Name = "UserAgent",
                        DbType = DbType.String,
                        Length = "255"
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedUserId",
                        DbType = DbType.Int32
                    },
                    new SchemaColumn()
                    {
                        Name = "CreatedDate",
                        DbType = DbType.DateTimeOffset
                    }
                }
        };

        private readonly ISchemaBuilder _schemaBuilder;
        private readonly ISchemaManager _schemaManager;

        public FeatureEventHandler(
            ISchemaBuilder schemaBuilder,
            ISchemaManager schemaManager)
        {
            _schemaBuilder = schemaBuilder;
            _schemaManager = schemaManager;
        }
        
        #region "Implementation"

        public override async Task InstallingAsync(IFeatureEventContext context)
        {

            if (context.Logger.IsEnabled(LogLevel.Information))
                context.Logger.LogInformation($"InstallingAsync called within {ModuleId}");
            
            //var schemaBuilder = context.ServiceProvider.GetRequiredService<ISchemaBuilder>();
            using (var builder = _schemaBuilder)
            {

                // configure
                Configure(builder);

                // Metrics schema
                EntityMetrics(builder);
                
                // Log statements to execute
                if (context.Logger.IsEnabled(LogLevel.Information))
                {
                    context.Logger.LogInformation($"The following SQL statements will be executed...");
                    foreach (var statement in builder.Statements)
                    {
                        context.Logger.LogInformation(statement);
                    }
                }

                // Execute statements
                var errors = await _schemaManager.ExecuteAsync(builder.Statements);
                foreach (var error in errors)
                {
                    context.Errors.Add(error, $"InstallingAsync within {this.GetType().FullName}");
                }
          
            }
            
        }

        public override Task InstalledAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override async Task UninstallingAsync(IFeatureEventContext context)
        {

            if (context.Logger.IsEnabled(LogLevel.Information))
                context.Logger.LogInformation($"UninstallingAsync called within {ModuleId}");
            
            using (var builder = _schemaBuilder)
            {
                
                // drop metrics
                builder.TableBuilder.DropTable(_entityMetrics);

                builder.ProcedureBuilder
                    .DropDefaultProcedures(_entityMetrics)
                    .DropProcedure(new SchemaProcedure("SelectEntityMetricsPaged", StoredProcedureType.SelectByKey));
                    
                // Log statements to execute
                if (context.Logger.IsEnabled(LogLevel.Information))
                {
                    context.Logger.LogInformation($"The following SQL statements will be executed...");
                    foreach (var statement in builder.Statements)
                    {
                        context.Logger.LogInformation(statement);
                    }
                }

                // Execute statements
                var errors = await _schemaManager.ExecuteAsync(builder.Statements);
                foreach (var error in errors)
                {
                    context.Logger.LogCritical(error, $"An error occurred within the UninstallingAsync method within {this.GetType().FullName}");
                    context.Errors.Add(error, $"UninstallingAsync within {this.GetType().FullName}");
                }
          
            }
            
        }

        public override Task UninstalledAsync(IFeatureEventContext context)
        {

            return Task.CompletedTask;
        }

        #endregion

        #region "Private Methods"

        void Configure(ISchemaBuilder builder)
        {

            builder
                .Configure(options =>
                {
                    options.ModuleName = ModuleId;
                    options.Version = Version;
                    options.DropTablesBeforeCreate = true;
                    options.DropProceduresBeforeCreate = true;
                });

        }

        void EntityMetrics(ISchemaBuilder builder)
        {

            // Tables
            builder.TableBuilder.CreateTable(_entityMetrics);

            // Procedures
            builder.ProcedureBuilder
                .CreateDefaultProcedures(_entityMetrics)

                // Overwrite our SelectMetricById created via CreateDefaultProcedures
                // above to also return simple user data with the result
                .CreateProcedure(
                    new SchemaProcedure(
                            $"SelectEntityMetricById",
                            @" SELECT em.*, 
                                    e.Title,
                                    e.Alias,
                                    e.FeatureId,
                                    f.ModuleId,
                                    u.UserName,                              
                                    u.DisplayName,                                  
                                    u.Alias,
                                    u.PhotoUrl,
                                    u.PhotoColor
                                FROM {prefix}_EntityMetrics em WITH (nolock) 
                                    INNER JOIN {prefix}_Entities e ON em.EntityId = e.Id
                                    INNER JOIN {prefix}_ShellFeatures f ON e.FeatureId = f.Id
                                    LEFT OUTER JOIN {prefix}_Users u ON em.CreatedUserId = u.Id                                    
                                WHERE (
                                   em.Id = @Id
                                )")
                        .ForTable(_entityMetrics)
                        .WithParameter(_entityMetrics.PrimaryKeyColumn))

                // Create SelectMetricsPaged 
                .CreateProcedure(new SchemaProcedure("SelectEntityMetricsPaged", StoredProcedureType.SelectPaged)
                    .ForTable(_entityMetrics)
                    .WithParameters(new List<SchemaColumn>()
                    {
                        new SchemaColumn()
                        {
                            Name = "IpV4Address",
                            DbType = DbType.String,
                            Length = "255"
                        },
                        new SchemaColumn()
                        {
                            Name = "IpV6Address",
                            DbType = DbType.String,
                            Length = "255"
                        },
                        new SchemaColumn()
                        {
                            Name = "UserAgent",
                            DbType = DbType.String,
                            Length = "255"
                        }
                    }));

            // Indexes
            builder.IndexBuilder.CreateIndex(new SchemaIndex()
            {
                TableName = _entityMetrics.Name,
                Columns = new string[]
                {
                    "EntityId",
                    "CreatedUserId",
                    "CreatedDate"
                }
            });


        }

        #endregion

    }

}
