﻿using PlatoCore.Stores.Abstractions;

namespace Plato.Moderation.Stores
{

    public interface IModeratorStore<TModel> : IStore<TModel> where TModel : class
    {

    }

}
