﻿using System;
using System.Collections.Generic;

namespace PlatoCore.Models.Shell
{
    public interface IShellSettings
    {

        string Name { get; set; }

        string Location { get; set; }

        string ConnectionString { get; set; }

        string RequestedUrlHost { get; set; }

        string RequestedUrlPrefix { get; set; }

        string TablePrefix { get; set; }
        
        string DatabaseProvider { get; set; }

        string Theme { get; set; }

        string AuthCookieName { get; }

        TenantState State { get; set; }

        string this[string key] { get; }

        IEnumerable<string> Keys { get; }

        IDictionary<string, string> Configuration { get; }
    }

}
