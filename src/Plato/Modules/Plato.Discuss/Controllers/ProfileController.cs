﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Plato.Internal.Abstractions.Settings;
using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Entities.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Shell.Abstractions;
using Plato.Internal.Stores.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Discuss.Controllers
{
    public class ProfileController : Controller, IUpdateModel
    {
        private readonly IContextFacade _contextFacade;
        private readonly ISiteSettingsStore _settingsStore;
        private readonly IViewProviderManager<DiscussUser> _viewProvider;

        public ProfileController(
            ISiteSettingsStore settingsStore,
            IContextFacade contextFacade,
            IViewProviderManager<DiscussUser> viewProvider)
        {
            _settingsStore = settingsStore;
            _contextFacade = contextFacade;
            _viewProvider = viewProvider;
        }
        
        public async Task<IActionResult> Index(
            int id,
            EntityIndexOptions opts,
            PagerOptions pager)
        {

            if (id <= 0)
            {
                return NotFound();
            }

            // default options
            if (opts == null)
            {
                opts = new EntityIndexOptions();
            }

            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Get default options
            var defaultViewOptions = new EntityIndexOptions();
            var defaultPagerOptions = new PagerOptions();

            // Add non default route data for pagination purposes
            if (opts.Search != defaultViewOptions.Search)
                this.RouteData.Values.Add("opts.search", opts.Search);
            if (opts.Sort != defaultViewOptions.Sort)
                this.RouteData.Values.Add("opts.sort", opts.Sort);
            if (opts.Order != defaultViewOptions.Order)
                this.RouteData.Values.Add("opts.order", opts.Order);
            if (opts.Filter != defaultViewOptions.Filter)
                this.RouteData.Values.Add("opts.filter", opts.Filter);
            if (pager.Page != defaultPagerOptions.Page)
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.PageSize != defaultPagerOptions.PageSize)
                this.RouteData.Values.Add("pager.size", pager.PageSize);

            var viewModel = new EntityIndexViewModel<Topic>()
            {
                Options = opts,
                Pager = pager
            };

            // Add view options to context for use within view adaptors
            this.HttpContext.Items[typeof(EntityIndexViewModel<Topic>)] = viewModel;
            
            // Build view
            var result = await _viewProvider.ProvideDisplayAsync(new DiscussUser()
            {
                Id = id
            }, this);

            //// Return view
            return View(result);

            //return Task.FromResult((IActionResult)View());
        }

        

    }

}
