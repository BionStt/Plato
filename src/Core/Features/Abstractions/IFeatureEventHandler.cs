﻿using System.Threading.Tasks;

namespace PlatoCore.Features.Abstractions
{

    public interface IFeatureEventHandler
    {

        string ModuleId { get; }

        Task InstallingAsync(IFeatureEventContext context);

        Task InstalledAsync(IFeatureEventContext context);

        Task UninstallingAsync(IFeatureEventContext context);

        Task UninstalledAsync(IFeatureEventContext context);

        Task UpdatingAsync(IFeatureEventContext context);

        Task UpdatedAsync(IFeatureEventContext context);

    }

}
