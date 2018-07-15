﻿using System;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.ViewModels;
using Plato.Discuss.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Shell;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class ChannelViewProvider : BaseViewProvider<Category>
    {


        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<Category> _categoryStore;

        public ChannelViewProvider(
            IContextFacade contextFacade,
            ICategoryStore<Category> categoryStore)
        {
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(Category viewModel, IUpdateModel updater)
        {
            var indexViewModel = await GetIndexModel();
         
            return Views(
                View<ChannelIndexViewModel>("Admin.Index.Header", model => indexViewModel).Zone("header").Order(1),
                View<ChannelIndexViewModel>("Admin.Index.Tools", model => indexViewModel).Zone("tools").Order(1),
                View<ChannelIndexViewModel>("Admin.Index.Content", model => indexViewModel).Zone("content").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Category viewModel, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));

        }
        
        public override async Task<IViewProviderResult> BuildEditAsync(Category viewModel, IUpdateModel updater)
        {

            var defaultIcons = new DefaultIcons();

            EditChannelViewModel editChannelViewModel = null;
            if (viewModel.Id == 0)
            {
                editChannelViewModel = new EditChannelViewModel()
                {
                    IconPrefix = defaultIcons.Prefix,
                    ChannelIcons = defaultIcons
                };
            }
            else
            {
                editChannelViewModel = new EditChannelViewModel()
                {
                    Id = viewModel.Id,
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    ForeColor = viewModel.ForeColor,
                    BackColor = viewModel.BackColor,
                    IconCss = viewModel.IconCss,
                    IconPrefix = defaultIcons.Prefix,
                    ChannelIcons = defaultIcons
                };
            }
            
            return Views(
                View<EditChannelViewModel>("Admin.Edit.Header", model => editChannelViewModel).Zone("header").Order(1),
                View<EditChannelViewModel>("Admin.Edit.Content", model => editChannelViewModel).Zone("content").Order(1),
                View<EditChannelViewModel>("Admin.Edit.Actions", model => editChannelViewModel).Zone("actions").Order(1),
                View<EditChannelViewModel>("Admin.Edit.Footer", model => editChannelViewModel).Zone("footer").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Category viewModel, IUpdateModel updater)
        {

            var model = new EditChannelViewModel();

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(viewModel, updater);
            }

            model.Name = model.Name?.Trim();
            model.Description = model.Description?.Trim();
            
            Category category = null;

            if (updater.ModelState.IsValid)
            {

                var featureId = 0;
                var feature = await GetcurrentFeature();
                if (feature != null)
                {
                    featureId = feature.Id;
                }

                category = new Category()
                {
                    Id = model.Id,
                    FeatureId = featureId,
                    Name = model.Name,
                    Description = model.Description,
                    ForeColor = model.ForeColor,
                    BackColor = model.BackColor,
                    IconCss = model.IconPrefix + model.IconCss
                };

                category = await _categoryStore.CreateAsync(category);
            
            }

            return await BuildEditAsync(category, updater);


        }

        #endregion

        #region "Private Methods"

        async Task<ChannelIndexViewModel> GetIndexModel()
        {
            var feature = await GetcurrentFeature();
            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);
            return new ChannelIndexViewModel()
            {
                Channels = categories
            };
        }
        
        async Task<ShellModule> GetcurrentFeature()
        {
            var featureId = "Plato.Discuss.Channels";
            var feature = await _contextFacade.GetFeatureByModuleIdAsync(featureId);
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id '{featureId}'");
            }
            return feature;
        }

        #endregion

    }
}