﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace PlatoCore.Layout.ActionFilters
{
    
    public class ModularFilter : IActionFilter, IAsyncResultFilter
    {

        private readonly IEnumerable<IModularActionFilter> _providers;
        private readonly ILogger<ModularFilter> _logger;

        public ModularFilter(
            IEnumerable<IModularActionFilter> providers,
            ILogger<ModularFilter> logger)
        {
            _providers = providers;
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

            foreach (var provider in _providers)
            {
                try
                {
                    provider.OnActionExecuting(context);
                }
                catch (Exception e)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError(e, e.Message);
                    }
                }
            }

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            foreach (var provider in _providers)
            {
                try
                {
                    provider.OnActionExecuted(context);
                }
                catch (Exception e)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError(e, e.Message);
                    }
                }
            }

        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {

            // do something before the action executes
            foreach (var provider in _providers)
            {
                try
                {
                   await provider.OnActionExecutingAsync(context);
                }
                catch (Exception e)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError(e, e.Message);
                    }
                }
            }

            await next();
            
            // do something after the action executes; resultContext.Result will be set
            foreach (var provider in _providers)
            {
                try
                {
                    await provider.OnActionExecutedAsync(context);
                }
                catch (Exception e)
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError(e, e.Message);
                    }
                }
            }


        }
    }
}
