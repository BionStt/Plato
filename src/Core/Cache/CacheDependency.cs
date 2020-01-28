﻿using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;
using PlatoCore.Cache.Abstractions;

namespace PlatoCore.Cache
{

    public class CacheDependency : ICacheDependency
    {
    
        private readonly ConcurrentDictionary<string, CacheDependencyInfo> _dependencies;
        
        public CacheDependency()
        {
            _dependencies = new ConcurrentDictionary<string, CacheDependencyInfo>();
        }
        
        public IChangeToken GetToken(string key)
        {
            return _dependencies.GetOrAdd(key, _ =>
            {
                var cancellationToken = new CancellationTokenSource();
                var changeToken = new CancellationChangeToken(cancellationToken.Token);
                return new CacheDependencyInfo()
                {
                    ChangeToken = changeToken,
                    CancellationToken = cancellationToken
                };
            }).ChangeToken;
        }

        public void CancelToken(string key)
        {
            if (_dependencies.TryRemove(key, out var changeTokenInfo))
            {
                changeTokenInfo.CancellationToken.Cancel();
            }
        }
        
    }
    
}