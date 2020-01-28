﻿using System;
using System.Collections.Generic;
using System.Text;
using PlatoCore.Abstractions;
using System.Collections.Concurrent;
using System.Data;

namespace PlatoCore.Abstractions.Extensions
{
    public static class DbModelExtensions
    {

        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, int>> Ordinals =
       new ConcurrentDictionary<Type, ConcurrentDictionary<string, int>>();

        public static int GetOrdinal(this IDbModel model, string columnName, IDataReader dr)
        {
        
            // First attempt to get the ordinal from cache
            int ordinal;
            var type = model.GetType();
            if (Ordinals.ContainsKey(type))
            {
                var ordinals = Ordinals[type];
                if (ordinals != null)
                {
                    // Is our ordinal cached?
                    if (ordinals.ContainsKey(columnName))
                    {
                        return ordinals[columnName];
                    }
                }
            }

            // GetOrdinal() is expensive so we catch the results
            ordinal = dr.GetOrdinal(columnName);

            // Cache our ordinals by IDbModel type in a staic concurrent dictionary            
            Ordinals.AddOrUpdate(type, new ConcurrentDictionary<string, int>()
            {
                [columnName] = ordinal
            }, (k, v) =>
            {
                v.TryAdd(columnName, ordinal);
                return v;
            });

            return ordinal;
            
        }
        
    }

}
