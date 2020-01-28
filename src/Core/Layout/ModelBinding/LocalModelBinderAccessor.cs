﻿using System.Threading;

namespace PlatoCore.Layout.ModelBinding
{
    public class LocalModelBinderAccessor : IUpdateModelAccessor
    {
        private readonly AsyncLocal<IUpdateModel> _storage = new AsyncLocal<IUpdateModel>();

        public IUpdateModel ModelUpdater
        {
            get => _storage.Value;
            set => _storage.Value = value;
        }
    }

}
