﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Messaging.Abstractions;

namespace PlatoCore.Messaging.Extensions
{
    public static class BrokerExtensions
    {

        public static IEnumerable<Func<Message<T>, Task<T>>> Pub<T>(
            this IBroker broker,
            object sender, 
            string key,
            T value) where T : class
        {
            return broker.Pub<T>(sender, new MessageOptions()
            {
                Key = key
            }, value);
        }

    }
}
