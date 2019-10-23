﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;

namespace Plato.Internal.Hosting.Web
{
    public class WebHostEnvironment : HostEnvironment
    {

        public WebHostEnvironment(
        IHostingEnvironment hostingEnvironment) : 
            base(hostingEnvironment)
        {
            T = null;
        }

        public IStringLocalizer T { get; set; }

    }

}
