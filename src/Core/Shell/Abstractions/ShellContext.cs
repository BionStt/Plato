﻿using System;
using Microsoft.Extensions.DependencyInjection;
using PlatoCore.Models.Shell;

namespace PlatoCore.Shell.Abstractions
{

    public class ShellContext : IDisposable
    {

        private bool _disposed = false;

        public bool IsActivated;

        public ShellBlueprint Blueprint { get; set; }

        public IShellSettings Settings { get; set; }

        public IServiceProvider ServiceProvider { get; set; }
        
        public IServiceScope CreateServiceScope()
        {
            return ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        }

        // IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }

                Settings = null;
                ServiceProvider = null;

                // Disposes all the services registered for this shell
                (ServiceProvider as IDisposable)?.Dispose();
                IsActivated = false;

                _disposed = true;
            }
        }

        ~ShellContext()
        {
            Dispose(false);
        }

    }

}
