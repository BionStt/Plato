﻿using PlatoCore.Abstractions;

namespace Plato.Ideas.Services
{
    public interface IPostManager<TEntity> : ICommandManager<TEntity> where TEntity : class
    {
    }

}
