﻿using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Data.Migrations.Abstractions;

namespace PlatoCore.Data.Migrations.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddDataMigrations(
            this IServiceCollection services)
        {

            services.AddTransient<IDataMigrationManager, DataMigrationManager>();
            services.AddTransient<IDataMigrationBuilder, DataMigrationBuilder>();
            services.AddTransient<AutomaticDataMigrations>();

            return services;
        }


    }
}
