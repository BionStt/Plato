﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;

namespace PlatoCore.Hosting.Web.Expanders
{
    // TODO: To be deleted
    //public class ThemeViewLocationExpander : IViewLocationExpander
    //{
    //    readonly string _theme;

    //    public ThemeViewLocationExpander(string theme)
    //    {
    //        _theme = theme;
    //    }

    //    /// <inheritdoc />
    //    public void PopulateValues(ViewLocationExpanderContext context)
    //    {
    //    }

    //    /// <inheritdoc />
    //    public virtual IEnumerable<string> ExpandViewLocations(
    //        ViewLocationExpanderContext context,
    //        IEnumerable<string> viewLocations)
    //    {

    //        var result = new List<string>
    //        {
    //            "/Themes/" + _theme + "/{1}/{0}.cshtml",
    //            "/Themes/" + _theme + "/Shared/{0}.cshtml"
    //        };
    //        result.AddRange(viewLocations);
    //        return result;
    //    }

    //}

}
