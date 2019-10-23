﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Abstractions.SetUp;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Shell.Abstractions;

namespace Plato.SetUp.Services
{
    public class SetUpService :ISetUpService
    {

        private const string TablePrefixSeparator = "_";

        private readonly IShellContextFactory _shellContextFactory;
        private readonly IShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;

        public SetUpService(            
            IShellContextFactory shellContextFactory,
            IShellSettings shellSettings,
            IPlatoHost platoHost)
        {            
            _shellContextFactory = shellContextFactory;
            _shellSettings = shellSettings;
            _platoHost = platoHost;
        }

        public async Task<string> SetUpAsync(SetUpContext context)
        {
            var initialState = _shellSettings.State;
            try
            {
                return await SetUpInternalAsync(context);
            }
            catch (Exception ex)
            {
                context.Errors.Add(ex.Message, ex.Message);
                _shellSettings.State = initialState;
                throw;
            }
        }
        
        async Task<string> SetUpInternalAsync(SetUpContext context)
        {

            // Set shell state to "Initializing" so that subsequent HTTP requests are responded to with "Service Unavailable" while Orchard is setting up.
            _shellSettings.State = TenantState.Initializing;

            var executionId = Guid.NewGuid().ToString("n");

            var shellSettings = new ShellSettings(_shellSettings.Configuration);           
            shellSettings.Name = context.SiteName;
            shellSettings.Location = context.SiteName.ToSafeFileName();

            if (string.IsNullOrEmpty(shellSettings.DatabaseProvider))
            {
                var tablePrefix = context.DatabaseTablePrefix;
                if (!tablePrefix.EndsWith(TablePrefixSeparator))
                    tablePrefix += TablePrefixSeparator;
                shellSettings.DatabaseProvider = context.DatabaseProvider;
                shellSettings.ConnectionString = context.DatabaseConnectionString;
                shellSettings.TablePrefix = tablePrefix;
            }
       
            using (var shellContext = _shellContextFactory.CreateMinimalShellContext(shellSettings))
            {
                using (var scope = shellContext.ServiceProvider.CreateScope())
                {
                    using (var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>())
                    {

                        // update dbContext confirmation
                       dbContext.Configure(options =>
                       {
                           options.ConnectionString = shellSettings.ConnectionString;
                           options.DatabaseProvider = shellSettings.DatabaseProvider;
                           options.TablePrefix = shellSettings.TablePrefix;
                       });

                        var hasErrors = false;
                        void ReportError(string key, string message)
                        {
                            hasErrors = true;
                            context.Errors[key] = message;
                        }

                        // Invoke modules to react to the setup event
                        var setupEventHandlers = scope.ServiceProvider.GetServices<ISetUpEventHandler>();
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SetUpService>>();

                        await setupEventHandlers.InvokeAsync(x => x.SetUp(context, ReportError), logger);

                        if (hasErrors)
                        {
                            return executionId;
                        }

                    }

                }
            }

            if (context.Errors.Count > 0)
            {
                return executionId;
            }

            shellSettings.State = TenantState.Running;
            _platoHost.UpdateShellSettings(shellSettings);

            return executionId;

        }

    }

}
