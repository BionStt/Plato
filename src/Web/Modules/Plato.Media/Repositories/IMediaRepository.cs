﻿using PlatoCore.Repositories;

namespace Plato.Media.Repositories
{

    public interface IMediaRepository<T> : IRepository<T> where T : class
    {

    }

}
