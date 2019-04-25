﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Metrics.Models;
using Plato.Reporting.ViewModels;

namespace Plato.Reporting.ViewComponents
{
    public class MetricListItemViewComponent : ViewComponent
    {
  
        public MetricListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(MetricListItemViewModel<Metric> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

