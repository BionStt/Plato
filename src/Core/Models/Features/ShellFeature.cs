﻿using System;
using System.Data;
using System.Collections.Generic;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Abstractions;
using PlatoCore.Models.Modules;
using PlatoCore.Models.Shell;

namespace PlatoCore.Models.Features
{
    public class ShellFeature : IShellFeature, IDbModel<ShellFeature>
    {

        public int Id { get; set; }

        public string ModuleId { get; set; }

        public IModuleDescriptor Descriptor { get; set; }
        
        public bool IsEnabled { get; set; }

        public bool IsRequired { get; set; }

        public string Version { get; set; }

        public string Settings { get; set; }

        public IFeatureSettings FeatureSettings { get; set; } = new FeatureSettings();

        public IEnumerable<IShellFeature> FeatureDependencies { get; set; } = new List<ShellFeature>();

        public IEnumerable<IShellFeature> DependentFeatures { get; set; } = new List<ShellFeature>();
        
        public IEnumerable<IShellFeature> Dependencies { get; set; } = new List<ShellFeature>();

        public ShellFeature()
        {

        }

        public ShellFeature(ShellModule module)
        {
            Id = module.Id;
            ModuleId = module.ModuleId;
            Version = module.Version;
        }

        public ShellFeature(ModuleDependency dependency)
        {
            // dependency.Id to  this.ModuleId is intended
            // dependency.Id is the value deserialized from module.txt
            ModuleId = dependency.Id;
            Version = dependency.Version;
        }
        
        public ShellFeature(IModuleEntry entry)
        {

            ModuleId = entry.Descriptor.Id;
            Descriptor = entry.Descriptor;
            Version = entry.Descriptor.Version;

            // Add minimal set of dependencies
            var dependencies = new List<ShellFeature>();
            foreach (var dependency in entry.Descriptor.Dependencies)
            {
                dependencies.Add(new ShellFeature(dependency));
            }

            this.Dependencies = dependencies;
            
        }

        public void PopulateModel(IDataReader dr)
        {

            if (dr.ColumnIsNotNull("Id"))
                Id = Convert.ToInt32(dr["Id"]);

            if (dr.ColumnIsNotNull("ModuleId"))
                ModuleId = Convert.ToString(dr["ModuleId"]);

            if (dr.ColumnIsNotNull("Version"))
                Version = Convert.ToString(dr["Version"]);

            if (dr.ColumnIsNotNull("Settings"))
                Settings = Convert.ToString(dr["Settings"]);

        }
    }

}
