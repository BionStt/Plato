﻿using System.Collections.Generic;
using Plato.Internal.Abstractions;
using Plato.Internal.Theming.Abstractions.Models;

namespace Plato.Internal.Theming.Abstractions
{
    public interface IThemeManager
    {
        string RootPath { get;  }

        IEnumerable<IThemeDescriptor> AvailableThemes { get; }
        
        ICommandResult<IThemeDescriptor> UpdateThemeDescriptor(string themeId, IThemeDescriptor descriptor);

    }

}
