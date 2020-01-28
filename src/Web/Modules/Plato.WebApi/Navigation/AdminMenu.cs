﻿using System;
using Microsoft.Extensions.Localization;
using PlatoCore.Navigation.Abstractions;

namespace Plato.WebApi.Navigation
{

    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Settings"], int.MaxValue, configuration => configuration
                    .IconCss("fal fa-cog")
                    .Add(T["Web Api"], int.MaxValue, webApiSettings => webApiSettings
                        .Action("Index", "Admin", "Plato.WebApi")
                        .Permission(Permissions.ManageWebApiSettings)
                        .LocalNav())
                );

        }

    }

}
