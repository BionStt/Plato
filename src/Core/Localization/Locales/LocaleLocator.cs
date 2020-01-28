﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.FileSystem.Abstractions;
using PlatoCore.Localization.Abstractions;
using PlatoCore.Localization.Abstractions.Models;

namespace PlatoCore.Localization.Locales
{

    public class LocaleLocator : ILocaleLocator
    {
        
        private readonly IPlatoFileSystem _fileSystem;
        private readonly ILogger<LocaleLocator> _logger;
   
        public LocaleLocator(
            IPlatoFileSystem fileSystem,
            ILogger<LocaleLocator> logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;
        }
        
        #region "Implementation"
        
        public async Task<IEnumerable<LocaleDescriptor>> LocateLocalesAsync(IEnumerable<string> paths)
        {

            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

            var descriptors = new List<LocaleDescriptor>();
            foreach (var path in paths)
            {
                descriptors.AddRange(await AvailableLocales(path));
            }

            return descriptors;

        }
        
        #endregion

        #region "Private Methods"

        async Task<IEnumerable<LocaleDescriptor>> AvailableLocales(string path)
        {
            var locales = await AvailableLocalesInFolder(path);
            return locales.ToReadOnlyCollection();
        }

        async Task<IList<LocaleDescriptor>> AvailableLocalesInFolder(string path)
        {
            var localList = new List<LocaleDescriptor>();

            if (string.IsNullOrEmpty(path))
            {
                return localList;
            }

            var subfolders = _fileSystem.ListDirectories(path);
            foreach (var subfolder in subfolders)
            {

                var localeId = subfolder.Name;
                var localePath = _fileSystem.Combine(path, localeId);
                try
                {
                    var descriptor = await GetLocaleDescriptorAsync(
                        localeId,
                        localePath);
                    if (descriptor == null)
                        continue;
                    descriptor.DirectoryInfo = subfolder;
                    localList.Add(descriptor);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred whilst reading a locale file for locale '{localeId}' at '{localePath}'.");
                }

            }

            return localList;

        }

        async Task<LocaleDescriptor> GetLocaleDescriptorAsync(
            string localeId,
            string localePath)
        {

            var resources = new List<LocaleResource>();
            foreach (var file in _fileSystem.ListFiles(localePath))
            {
                var filePath = _fileSystem.Combine(localePath, file.Name);
                resources.Add(new LocaleResource()
                {
                    Name = file.Name,
                    Path = filePath,
                    Location = localePath,
                    FileInfo = file,
                    Contents = await _fileSystem.ReadFileAsync(filePath)
                });
            }

            return new LocaleDescriptor()
            {
                Name = localeId,
                Path = localePath,
                Resources = resources
            };

        }

        #endregion
        
    }

}
