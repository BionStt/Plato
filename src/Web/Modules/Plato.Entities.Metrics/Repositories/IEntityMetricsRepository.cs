﻿using System.Collections.Generic;
using System.Threading.Tasks;
using PlatoCore.Repositories;

namespace Plato.Entities.Metrics.Repositories
{
    public interface IEntityMetricsRepository<TModel> : IRepository<TModel> where TModel : class
    {

    }

}