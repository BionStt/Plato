﻿using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Features.Abstractions;
using PlatoCore.Models.Features;
using PlatoCore.Models.Shell;
using PlatoCore.Modules.Abstractions;
using PlatoCore.Shell.Abstractions;
using PlatoCore.Stores.Abstractions.Shell;

namespace PlatoCore.Features
{
    
    public class ShellDescriptorManager : IShellDescriptorManager
    {

        // Build described features
        private ConcurrentDictionary<string, IShellFeature> _features;

        private readonly ConcurrentDictionary<string, IEnumerable<IShellFeature>> _featureDependencies
            = new ConcurrentDictionary<string, IEnumerable<IShellFeature>>();

        private readonly ConcurrentDictionary<string, IEnumerable<IShellFeature>> _dependentFeatures
            = new ConcurrentDictionary<string, IEnumerable<IShellFeature>>();

        private readonly IShellDescriptorStore _shellDescriptorStore;
        private readonly IShellContextFactory _shellContextFactory;
        private readonly IModuleManager _moduleManager;

        public ShellDescriptorManager(
            IShellDescriptorStore shellDescriptorStore,
            IShellContextFactory shellContextFactory, 
            IModuleManager moduleManager)
        {     
            _shellDescriptorStore = shellDescriptorStore;
            _shellContextFactory = shellContextFactory;
            _moduleManager = moduleManager;
        }

        public async Task<IShellDescriptor> GetEnabledDescriptorAsync()
        {
            return await _shellDescriptorStore.GetAsync() ??
                _shellContextFactory.MinimumShellDescriptor();
        }

        public async Task<IEnumerable<IShellFeature>> GetEnabledFeaturesAsync()
        {

            // Get all enabled features
            var descriptor = await GetEnabledDescriptorAsync();
            
            var features = new List<ShellFeature>();
            if (descriptor != null)
            {
                foreach (var module in descriptor.Modules)
                {
                    features.Add(new ShellFeature(module));
                }
            }
            return features;

        }

        public async Task<IEnumerable<IShellFeature>> GetFeaturesAsync()
        {

            // Load all features
            await EnsureFeaturesLoadedAsync();

            // Update dependencies
            await EnsureDependenciesAreEstablishedAsync();
            
            return _features.Values;

        }

        public async Task<IShellFeature> GetFeatureAsync(string featureId)
        {
            var features = await GetFeaturesAsync();
            return features.FirstOrDefault(f => f.ModuleId == featureId);
        }

        public async Task<IEnumerable<IShellFeature>> GetFeaturesAsync(string[] featureIds)
        {
            var features = await GetFeaturesAsync();
            return features
                .Where(f => featureIds.Any(v => v.Equals(f.ModuleId, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();
        }

        public async Task<IEnumerable<IShellFeature>> GetFeatureDependenciesAsync(string featureId)
        {
            // Load minimal features
            await EnsureFeaturesLoadedAsync();
            return _featureDependencies.GetOrAdd(featureId, key =>
            {

                if (!_features.ContainsKey(key))
                {
                    return Enumerable.Empty<ShellFeature>();
                }

                var feature = _features[key];
                return QueryDependencies(
                    feature,
                    _features.Values.ToArray(),
                    (currentFeature, fs) => fs
                        .Where(f => currentFeature.Dependencies.Any(dep => dep.ModuleId == f.ModuleId))
                        .ToArray());

            });
        }

        public async Task<IEnumerable<IShellFeature>> GetDependentFeaturesAsync(string featureId)
        {
            // Load minimal features
            await EnsureFeaturesLoadedAsync();
            return _dependentFeatures.GetOrAdd(featureId, key =>
            {

                if (!_features.ContainsKey(key))
                {
                    return Enumerable.Empty<ShellFeature>();
                }

                var feature = _features[key];
                return QueryDependencies(
                    feature,
                    _features.Values.ToArray(),
                    (currentFeature, fs) => fs
                        .Where(f => f.Dependencies.Any(dep => dep.ModuleId == currentFeature.ModuleId))
                        .ToArray());

            });
        }

        // --------------  

        async Task EnsureDependenciesAreEstablishedAsync()
        {
            foreach (var feature in _features.Values)
            {
                var f = feature;
                f.FeatureDependencies = await GetFeatureDependenciesAsync(f.ModuleId);
                f.DependentFeatures = await GetDependentFeaturesAsync(f.ModuleId);
                _features.TryUpdate(f.ModuleId, f, feature);
            }
        }

        /* General Idea...
         
            public static IEnumerable<T> Traverse<T>(T item, Func<T, IEnumerable<T>> childSelector)
            {
                var stack = new Stack<T>();
                stack.Push(item);
                while (stack.Any())
                {
                    var next = stack.Pop();
                    yield return next;
                    foreach (var child in childSelector(next))
                        stack.Push(child);
                }
            } 

        */

        IEnumerable<IShellFeature> QueryDependencies(
            IShellFeature feature,
            IShellFeature[] features,
            Func<IShellFeature, IShellFeature[], IShellFeature[]> query)
        {

            var dependencies = new HashSet<IShellFeature>() { feature };

            var stack = new Stack<IShellFeature[]>();
            stack.Push(query(feature, features));

            while (stack.Count > 0)
            {
                var next = stack.Pop();
                foreach (var dependency in next.Where(dependency => !dependencies.Contains(dependency)))
                {
                    dependencies.Add(dependency);
                    stack.Push(query(dependency, features));
                }
            }

            return _features
                .Where(f => dependencies.Any(d => d.ModuleId == f.Value.ModuleId))
                .Select(f => f.Value);

        }

        async Task EnsureFeaturesLoadedAsync()
        {
            
            if (_features == null)
            {
                
                _features = new ConcurrentDictionary<string, IShellFeature>();
                var modules = await _moduleManager.LoadModulesAsync();
                if (modules != null)
                {

                    // Get explicitly enabled features
                    var enabledFeatures = await GetEnabledFeaturesAsync();
                    var enabledFeatureNames = enabledFeatures
                        .Select(f => f.ModuleId).ToArray();
                    
                    // Build features and dependencies 
                    foreach (var module in modules)
                    {
                        var feature = new ShellFeature(module);
                        feature.IsRequired = IsFeatureRequired(feature);

                        if (enabledFeatureNames.Contains(module.Descriptor.Id))
                        {
                            feature.IsEnabled = true;
                        }

                        foreach (var dependency in module.Descriptor.Dependencies)
                        {
                            if (enabledFeatureNames.Contains(dependency.Id))
                            {
                                dependency.IsEnabled = true;
                            }
                        }

                        _features.TryAdd(module.Descriptor.Id, feature);

                    }
                }
            }
        }

        bool IsFeatureRequired(ShellFeature feature)
        {
            // Mark SetUp as required
            if (feature.ModuleId.Equals("Plato.SetUp", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            // Returns true if the supplied feature exists within our minimal shell descriptor
            var minimalDescriptor = _shellContextFactory.MinimumShellDescriptor();
            return minimalDescriptor.Modules.FirstOrDefault(m => m.ModuleId.Equals(feature.ModuleId, StringComparison.OrdinalIgnoreCase)) != null;
        }

    }

}
