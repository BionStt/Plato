﻿using PlatoCore.Stores.Abstractions;

namespace Plato.Email.Stores
{

    public interface IEmailStore<TModel> : IStore<TModel> where TModel : class
    {

    }

}
