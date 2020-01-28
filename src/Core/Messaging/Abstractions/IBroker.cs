﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlatoCore.Messaging.Abstractions
{
    
    public interface IBroker 
    {

        IEnumerable<Func<Message<T>, Task<T>>> Pub<T>(object sender, MessageOptions opts, T message) where T : class;

        IEnumerable<Func<Message<T>, Task<T>>> Pub<T>(object sender, string key, T message) where T : class;

        IEnumerable<Func<Message<T>, Task<T>>> Pub<T>(object sender, string key) where T : class;

        void Sub<T>(MessageOptions opts, Action<Message<T>> subscription) where T : class;

        void Sub<T>(MessageOptions opts, Func<Message<T>, Task<T>> subscription) where T : class;

        void Unsub<T>(MessageOptions opts, Action<Message<T>> subscription) where T : class;

        void Unsub<T>(MessageOptions opts, Func<Message<T>, Task<T>> subscription) where T : class;

        void Dispose();

    }

}
