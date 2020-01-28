﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Metrics.Repositories;
using Plato.Reports.ViewModels;

namespace Plato.Reports.ViewComponents
{
    public class ViewsByFeatureChartViewComponent : ViewComponent
    {

        private readonly IAggregatedMetricsRepository _aggregatedMetricsRepository;

        public ViewsByFeatureChartViewComponent(IAggregatedMetricsRepository aggregatedMetricsRepository)
        {
            _aggregatedMetricsRepository = aggregatedMetricsRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(ReportOptions options)
        {
            
            if (options == null)
            {
                options = new ReportOptions();
            }
            
            var viewByFeature = await _aggregatedMetricsRepository.SelectGroupedByFeatureAsync(options.Start, options.End);
            return View(viewByFeature);

        }

    }

}
