﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PlatoCore.Layout.Views.Abstractions
{

    public interface IViewInvoker
    {
        ViewContext ViewContext { get; set; }

        IViewInvoker Contextualize(ViewContext viewContext);

        Task<IHtmlContent> InvokeAsync(IView view);

    }

}
