﻿using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Internal;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Plato.Internal.Modules.Models;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Internal.Modules
{
    public class ModuleViewFileProvider : IFileProvider
    {

        private readonly IOptions<ModuleOptions> _moduleOptions;
        private readonly IModuleManager _moduleManager;

        private string _moduleRoot;
        private string _root;

        public ModuleViewFileProvider(IServiceProvider services)
        {
            var env = services.GetRequiredService<IHostingEnvironment>();
            _moduleOptions = services.GetRequiredService<IOptions<ModuleOptions>>();
            _moduleManager = services.GetRequiredService<IModuleManager>();            
            _moduleRoot = _moduleOptions.Value.VirtualPathToModulesFolder + "/";
            _root = env.ContentRootPath + "\\";
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
          
            if (subpath == null)
            {
                return NotFoundDirectoryContents.Singleton;
            }

            var folder = NormalizePath(subpath);

                // Under "Modules/**".
             if (folder.StartsWith(_moduleRoot, StringComparison.Ordinal))
            {
                // Check for a "Pages" or a "Views" segment.
                var tokenizer = new StringTokenizer(folder, new char[] { '/' });
                if (tokenizer.Any(s => s == "Pages" || s == "Views"))
                {
                    // Resolve the subpath relative to the application's module root.
                    var folderSubPath = folder.Substring(_moduleRoot.Length);

                    // And serve the contents from the physical application root folder.
                    return new PhysicalDirectoryContents(_root + folder.Replace("/", "\\"));
                }
            }

            return NotFoundDirectoryContents.Singleton;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (subpath == null)
            {
                return new NotFoundFileInfo(subpath);
            }

            var path = NormalizePath(subpath);

            // "Modules/**/*.*".
            if (path.StartsWith(_moduleRoot, StringComparison.Ordinal))
            {
                // Resolve the subpath relative to the application's module.
                var fileSubPath = path.Substring(_moduleRoot.Length);

                // And serve the file from the physical application root folder.
                return new PhysicalFileInfo(new FileInfo(_root + fileSubPath));
            }

            return new NotFoundFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            if (filter == null)
            {
                return NullChangeToken.Singleton;
            }

            var path = NormalizePath(filter);

            // "Areas/{ApplicationName}/**/*.*".
            if (path.StartsWith(_moduleRoot, StringComparison.Ordinal))
            {
                // Resolve the subpath relative to the application's module.
                var fileSubPath = path.Substring(_moduleRoot.Length);

                // And watch the application file from the physical application root folder.
                return new PollingFileChangeToken(new FileInfo(_root + fileSubPath));
            }

            return NullChangeToken.Singleton;
        }

        private string NormalizePath(string path)
        {
            return path.Replace('\\', '/').Trim('/');
        }

    }

}
