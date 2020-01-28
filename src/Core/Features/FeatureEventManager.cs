﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Features.Abstractions;

namespace PlatoCore.Features
{
    
    public class FeatureEventManager : IFeatureEventManager
    {

        private readonly IEnumerable<IFeatureEventHandler> _eventProviders;
        private readonly ILogger<FeatureEventManager> _logger;
        
        public FeatureEventManager(
            IEnumerable<IFeatureEventHandler> eventProviders,
            ILogger<FeatureEventManager> logger)
        {
            _eventProviders = eventProviders;
            _logger = logger;
        }
        
        public async Task<IFeatureEventContext> InstallingAsync(IFeatureEventContext context)
        {

            foreach (var eventProvider in _eventProviders)
            {
                try
                {
                    await eventProvider.InstallingAsync(context);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred while raising the 'Installing' event within feature: {context.Feature.ModuleId}");
                    context.Errors.Add($"{context.Feature.ModuleId} threw an exception within the Installing event!", e.Message);
                }
            }

            return context;
        }

        public async Task<IFeatureEventContext> InstalledAsync(IFeatureEventContext context)
        {

            foreach (var eventProvider in _eventProviders)
            {
                try
                {
                    await eventProvider.InstalledAsync(context);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred while raising the 'Installed' event within feature: {context.Feature.ModuleId}");
                    context.Errors.Add($"{context.Feature.ModuleId} threw an exception within the Installed event!", e.Message);
                }
            }

            return context;

        }

        public async Task<IFeatureEventContext> UninstallingAsync(IFeatureEventContext context)
        {

            foreach (var eventProvider in _eventProviders)
            {
                try
                {
                    await eventProvider.InstalledAsync(context);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred while raising the 'Uninstalling' event within feature: {context.Feature.ModuleId}");
                    context.Errors.Add($"{context.Feature.ModuleId} threw an exception within the Uninstalling event!", e.Message);
                }
            }

            return context;

        }

        public async Task<IFeatureEventContext> UninstalledAsync(IFeatureEventContext context)
        {

            foreach (var eventProvider in _eventProviders)
            {
                try
                {
                    await eventProvider.UninstalledAsync(context);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred while raising the 'Uninstalled' event within feature: {context.Feature.ModuleId}");
                    context.Errors.Add($"{context.Feature.ModuleId} threw an exception within the Uninstalled event!", e.Message);
                }
            }

            return context;

        }

        public async Task<IFeatureEventContext> UpdatingAsync(IFeatureEventContext context)
        {
            foreach (var eventProvider in _eventProviders)
            {
                try
                {
                    await eventProvider.UpdatingAsync(context);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred while raising the 'Updating' event within feature: {context.Feature.ModuleId}");
                    context.Errors.Add($"{context.Feature.ModuleId} threw an exception within the Updating event!", e.Message);
                }
            }

            return context;
        }

        public async Task<IFeatureEventContext> UpdatedAsync(IFeatureEventContext context)
        {

            foreach (var eventProvider in _eventProviders)
            {
                try
                {
                    await eventProvider.UpdatedAsync(context);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred while raising the 'Updated' event within feature: {context.Feature.ModuleId}");
                    context.Errors.Add($"{context.Feature.ModuleId} threw an exception within the Updated event!", e.Message);
                }
            }

            return context;

        }

    }

}
