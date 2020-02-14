﻿using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using PlatoCore.Models.Shell;
using PlatoCore.Navigation.Abstractions;

namespace PlatoCore.Navigation
{
    
    public class BreadCrumbManager : IBreadCrumbManager
    {

        private static readonly string[] Schemes = { "http", "https", "tel", "mailto" };

        private List<MenuItem> _menuItems;

        private INavigationBuilder _builder;

        public INavigationBuilder Builder => _builder;
        
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IShellSettings _shellSettings;
        private IUrlHelper _urlHelper;

        public BreadCrumbManager(
            IUrlHelperFactory urlHelperFactory,
            IShellSettings shellSettings)
        {
            _urlHelperFactory = urlHelperFactory;
            _shellSettings = shellSettings;
        }

        public void Configure(Action<INavigationBuilder> configure)
        {
            _builder = new NavigationBuilder();
            configure(_builder);
            _menuItems = _builder.Build();
        }

        public IEnumerable<MenuItem> BuildMenu(ActionContext actionContext)
        {

            if (_menuItems == null)
            {
                return null;
            }

            return ComputeHref(_menuItems, actionContext);
            
        }
        
        List<MenuItem> ComputeHref(List<MenuItem> menuItems, ActionContext actionContext)
        {
            foreach (var menuItem in menuItems)
            {
                menuItem.Href = GetUrl(menuItem.Url, menuItem.RouteValues, actionContext);
                menuItem.Items = ComputeHref(menuItem.Items, actionContext);
            }

            return menuItems;
        }

        string GetUrl(string menuItemUrl, RouteValueDictionary routeValueDictionary, ActionContext actionContext)
        {
            string url;
            if (routeValueDictionary == null || routeValueDictionary.Count == 0)
            {
                if (String.IsNullOrEmpty(menuItemUrl))
                {
                    return string.Empty;
                }
                else
                {
                    url = menuItemUrl;
                }
            }
            else
            {
                if (_urlHelper == null)
                {
                    _urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);
                }

                url = _urlHelper.RouteUrl(new UrlRouteContext { Values = routeValueDictionary });
            }

            if (!string.IsNullOrEmpty(url) &&
                actionContext?.HttpContext != null &&
                !(url.StartsWith("/") ||
                  Schemes.Any(scheme => url.StartsWith(scheme + ":"))))
            {
                if (url.StartsWith("~/"))
                {
                    if (!String.IsNullOrEmpty(_shellSettings.RequestedUrlPrefix))
                    {
                        url = _shellSettings.RequestedUrlPrefix + "/" + url.Substring(2);
                    }
                    else
                    {
                        url = url.Substring(2);
                    }
                }

                if (!url.StartsWith("#"))
                {
                    var appPath = actionContext.HttpContext.Request.PathBase.ToString();
                    if (appPath == "/")
                        appPath = "";
                    url = appPath + "/" + url;
                }
            }

            return url;

        }

    }

}
