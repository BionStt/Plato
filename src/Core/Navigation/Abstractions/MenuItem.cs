﻿using System.Collections.Concurrent;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using PlatoCore.Security.Abstractions;

namespace PlatoCore.Navigation.Abstractions
{

    public class MenuItemView
    {
        public string ViewName { get; set; }

        public object Model { get; set; }

    }

    public class MenuItem
    {

        public MenuItem()
        {
            //Permissions = new List<Permission>();
            Classes = new List<string>();
            Attributes = new ConcurrentDictionary<string, object>();
            Permissions = new List<IPermission>();
            Items = new List<MenuItem>();
            LinkToFirstChild = true;
        }

        public string Id { get; set; }

        public LocalizedString Text { get; set; }

        public string Href { get; set; }

        public string Url { get; set; }

        public string Authority { get; set; }

        public int Order { get; set; }

        public bool LinkToFirstChild { get; set; }

        public bool LocalNav { get; set; }
        
        public string Culture { get; set; }

        public object Resource { get; set; }

        public List<MenuItem> Items { get; set; }

        public RouteValueDictionary RouteValues { get; set; }
        
        public MenuItemView View { get; set; }
        
        public List<IPermission> Permissions { get; }
        
        public string DividerCss { get; set; }

        public List<string> Classes { get; }

        public IDictionary<string, object> Attributes { get; set; }

        public string IconCss { get; set; }

        public bool Selected { get; set; }

        public string BadgeText { get; set; }

        public string BadgeCss { get; set; } = "badge badge-primary";

    }

}
