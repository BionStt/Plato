﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Theming.Abstractions;
using Plato.Internal.Theming.Abstractions.Locator;
using Plato.Internal.Theming.Abstractions.Models;
using Plato.Internal.Yaml;

namespace Plato.Internal.Theming
{
    public class ThemeManager : IThemeManager
    {

        private readonly IThemeLocator _themeLocator;
        private readonly IPlatoFileSystem _platoFileSystem;

        private const string ByThemeFileNameFormat = "Theme.{0}";
        
        private  IEnumerable<IThemeDescriptor> _themeDescriptors;

        public ThemeManager(
            IHostingEnvironment hostingEnvironment,
            IOptions<ThemeOptions> themeOptions,
            IThemeLocator themeLocator,
            IPlatoFileSystem platoFileSystem)
        {
            _themeLocator = themeLocator;
            _platoFileSystem = platoFileSystem;

            var contentRootPath = hostingEnvironment.ContentRootPath;
            var virtualPathToThemesFolder = themeOptions.Value.VirtualPathToThemesFolder;

            RootPath = _platoFileSystem.Combine(contentRootPath, virtualPathToThemesFolder);

            InitializeThemes();
        }


        #region "Implementation"

        public string RootPath { get; private set; }

        public IEnumerable<IThemeDescriptor> AvailableThemes
        {
            get
            {
                InitializeThemes();
                return _themeDescriptors;
            }
        }
        
        public ICommandResult<IThemeDescriptor> UpdateThemeDescriptor(string themeId, IThemeDescriptor descriptor)
        {

            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (string.IsNullOrEmpty(descriptor.Name))
            {
                throw new ArgumentNullException(nameof(descriptor.Name));
            }


            var fileName = string.Format(ByThemeFileNameFormat, "txt");
            var tenantPath = _platoFileSystem.MapPath(
                _platoFileSystem.Combine(RootPath, themeId, fileName));

            var configurationProvider = new YamlConfigurationProvider(new YamlConfigurationSource
            {
                Path = tenantPath,
                Optional = false
            });

            foreach (var key in descriptor.Keys)
            {
                if (!string.IsNullOrEmpty(descriptor[key]))
                {
                    configurationProvider.Set(key, descriptor[key]);
                }
            }

            var result = new CommandResult<ThemeDescriptor>();

            try
            {
                configurationProvider.Commit();
            }
            catch (Exception e)
            {
                return result.Failed(e.Message);
            }
            
            return result.Success(descriptor);

        }

        #endregion
        
        #region "Private Methods"

        IEnumerable<IThemeFile> ListFilesInternal(string path)
        {

            var output = new List<ThemeFile>();

            // Process directories
            var directories = _platoFileSystem.ListDirectories(path);
            foreach (var directory in directories)
            {

                var themeFile = new ThemeFile
                {
                    Name = directory.Name
                };

                foreach (var file in directory.GetFiles())
                {
                    themeFile.Children.Add(new ThemeFile()
                    {
                        Name = file.Name
                    });
                }

                output.Add(themeFile);

            }

            // Process files
            var currentDirectory = _platoFileSystem.GetDirectoryInfo(path);
            foreach (var file in currentDirectory.GetFiles())
            {
                output.Add(new ThemeFile()
                {
                    Name = file.Name
                });
            }

            return output;

        }


        void InitializeThemes()
        {
            if (_themeDescriptors == null)
            {
                _themeDescriptors = new List<IThemeDescriptor>();
                LoadThemeDescriptors();
            }
        }

        void LoadThemeDescriptors()
        {
            _themeDescriptors = _themeLocator.LocateThemes(
                new string[] { RootPath },
                "Themes",
                "theme.txt",
                false);
        }

        #endregion

    }
}
