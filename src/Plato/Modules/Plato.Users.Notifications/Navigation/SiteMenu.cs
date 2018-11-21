﻿using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation;

namespace Plato.Users.Notifications.Navigation
{
    public class SiteMenu : INavigationProvider
    {
   
        public SiteMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, NavigationBuilder builder)
        {

            if (!String.Equals(name, "site", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            // We need an empty anonymous type to invoke as as ViewComponent
            builder
                .Add(T["Notifications"], notifications => notifications
                    .View("NotificationMenu", new {})
                );

        }

    }

}