﻿using System.Linq;
using System.Threading.Tasks;
using PlatoCore.Data.Schemas.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Features;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.Shell;

namespace Plato.Issues.Handlers
{
    
    public class FeatureEventHandler : BaseFeatureEventHandler
    {
  
        public string Version { get; } = "1.0.0";

        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IShellFeatureStore<ShellFeature> _shellFeatureStore;
        private readonly IDefaultRolesManager _defaultRolesManager;

        public FeatureEventHandler(
            ISchemaBuilder schemaBuilder,
            IShellFeatureStore<ShellFeature> shellFeatureStore,
            IDefaultRolesManager defaultRolesManager)
        {
            _schemaBuilder = schemaBuilder;
            _shellFeatureStore = shellFeatureStore;
            _defaultRolesManager = defaultRolesManager;
        }
        
        #region "Implementation"

        public override Task InstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;

        }

        public override async Task InstalledAsync(IFeatureEventContext context)
        {

            // Update default feature settings
            var features =  await _shellFeatureStore.SelectFeatures();
            var feature = features.FirstOrDefault(f => f.ModuleId == base.ModuleId);
            if (feature != null)
            {
                feature.FeatureSettings = new FeatureSettings()
                {
                    Title = "Articles",
                    Description = ""
                };

                // Persist changes
                await _shellFeatureStore.UpdateAsync(feature);

            }

            // Apply default permissions to default roles for new feature
            await _defaultRolesManager.UpdateDefaultRolesAsync(new Permissions());

        }

        public override Task UninstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override Task UninstalledAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region "Private Methods"

        #endregion
        
    }
}
