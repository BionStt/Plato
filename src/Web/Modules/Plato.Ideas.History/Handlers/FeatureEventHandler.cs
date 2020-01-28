﻿using System.Threading.Tasks;
using PlatoCore.Features.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Ideas.History.Handlers
{

    public class FeatureEventHandler : BaseFeatureEventHandler
    {
  
        private readonly IDefaultRolesManager _defaultRolesManager;

        public FeatureEventHandler(IDefaultRolesManager defaultRolesManager)
        {
            _defaultRolesManager = defaultRolesManager;
        }

        #region "Implementation"

        public override async Task InstalledAsync(IFeatureEventContext context)
        {
            // Apply default permissions to default roles for new feature
            await _defaultRolesManager.UpdateDefaultRolesAsync(new Permissions());
        }

        #endregion

    }

}
