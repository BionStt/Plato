﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlatoCore.FileSystem.Abstractions;

namespace PlatoCore.FileSystem
{

    public class PhysicalSitesFolder : ISitesFolder
    {

        private const int ByMaxFileNameLength = 32;

        private readonly IPlatoFileSystem _fileSystem;
        private readonly ILogger<PhysicalSitesFolder> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        private static string InternalRootPath = "Sites";

        public PhysicalSitesFolder(
            IPlatoFileSystem parentFileSystem,
            ILogger<PhysicalSitesFolder> logger,
            IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;

            if (!parentFileSystem.DirectoryExists(InternalRootPath))
            {
                parentFileSystem.CreateDirectory(InternalRootPath);
            }

            var root = parentFileSystem.GetDirectoryInfo(InternalRootPath).FullName;
            _fileSystem = new PlatoFileSystem(root, new PhysicalFileProvider(root), _logger);

        }

        public string RootPath => _fileSystem.RootPath;

        public async Task<string> SaveUniqueFileAsync(
            Stream stream,
            string fileName, 
            string path)
        {
            
            var parts = fileName.Split('.');
            var extension = parts[parts.Length - 1];

            if (string.IsNullOrEmpty(extension))
            {
                throw new Exception("Could not obtain a fie extension!");
            }

            if (extension.Length > 6)
            {
                throw new Exception("The file extension is not valid!");
            }
            
            string fullPath;

            if (!path.EndsWith("\\"))
            {
                path = path + "\\";
            }

            do
            {
                fileName = System.Guid.NewGuid().ToString();
                fileName = fileName.Substring(0, ByMaxFileNameLength - extension.Length - 1);
                fileName = fileName + "." + extension;
                fullPath = path + fileName;
            } while (_fileSystem.FileExists(fullPath));

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Attempting to save unique file to {fullPath}.");
            }

            // write the file 
            await _fileSystem.CreateFileAsync(fullPath, stream);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Successfully saved unique file at {fullPath}.");
            }
            
            return fileName;
            
        }

        public async Task<string> SaveFileAsync(Stream stream, string fileName, string path)
        {

            var parts = fileName.Split('.');
            var extension = parts[parts.Length - 1];

            if (string.IsNullOrEmpty(extension))
            {
                throw new Exception("Could not obtain a fie extension!");
            }

            if (extension.Length > 6)
            {
                throw new Exception("The file extension is not valid!");
            }
            
            if (!path.EndsWith("\\"))
            {
                path = path + "\\";
            }

            var fullPath = path + fileName;

            // write the file 
            await _fileSystem.CreateFileAsync(fullPath, stream);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Successfully saved unique file at {fullPath}.");
            }

            return fileName;


        }

        public async Task CreateFileAsync(string path, Stream steam)
        {
            await _fileSystem.CreateFileAsync(path, steam);
        }

        public bool DeleteFile(string fileName, string path)
        {

            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            
            if (!path.EndsWith("\\"))
            {
                path = path + "\\";
            }


            var fullPath = path + fileName;
            if (_fileSystem.FileExists(fullPath))
            {
                try
                {
                    _fileSystem.DeleteFile(fullPath);
                }
                catch (Exception)
                {
                    return false;
                }

                return true;

            }

            return false;
        }

        public bool DeleteDirectory(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            
            if (!_fileSystem.DirectoryExists(path))
            {
                return false;
            }

            try
            {
                _fileSystem.DeleteDirectory(path);
            }
            catch (Exception)
            {
                return false;
            }

            return true;

        }
        
        public string Combine(params string[] paths)
        {
            return _fileSystem.Combine(paths);
        }

    }

}
