﻿using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Repositories;

namespace Plato.Categories.Repositories
{

    public interface ICategoryRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> SelectByFeatureIdAsync(int featureId);

    }

}
