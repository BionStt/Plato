﻿using System.Collections.Generic;

namespace PlatoCore.Security.Abstractions
{

    public interface IPermission
    {
        string Name { get; set; }

        string Description { get; set; }

        string Category { get; set; }

        IEnumerable<IPermission> ImpliedBy { get; set; }

    }

}
