﻿using System.Threading.Tasks;
using Plato.Categories.Roles.Services;
using Plato.Ideas.Categories.Models;
using PlatoCore.Features.Abstractions;

namespace Plato.Ideas.Categories.Roles.Handlers
{
    
    public class FeatureEventHandler : BaseFeatureEventHandler
    {
        
        private readonly IDefaultCategoryRolesManager<Category> _defaultCategoryRolesManager;
        private readonly IFeatureFacade _featureFacade;

        public FeatureEventHandler(
            IDefaultCategoryRolesManager<Category> defaultCategoryRolesManager,
            IFeatureFacade featureFacade)
        {
            _defaultCategoryRolesManager = defaultCategoryRolesManager;
            _featureFacade = featureFacade;
        }
        
        public override async Task InstalledAsync(IFeatureEventContext context)
        {

            // Install default roles for Plato.Docs.Categories
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Ideas.Categories");
            if (feature != null)
            {
                await _defaultCategoryRolesManager.InstallAsync(feature.Id);
            }
       
        }

    }

}
