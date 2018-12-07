﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Tags.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Navigation;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Discuss.Tags.ViewComponents
{

    public class TagListViewComponent : ViewComponent
    {

        private readonly IEnumerable<SortColumn> _defaultSortColumns = new List<SortColumn>()
        {
            new SortColumn()
            {
                Text = "Popular",
                Value = SortBy.Entities
            },
            new SortColumn()
            {
                Text = "Follows",
                Value =  SortBy.Follows
            },
            new SortColumn()
            {
                Text = "Views",
                Value = SortBy.Views
            },
            new SortColumn()
            {
                Text = "First Use",
                Value =  SortBy.Created
            },
            new SortColumn()
            {
                Text = "Last Use",
                Value = SortBy.LastEntity
            },
            new SortColumn()
            {
                Text = "Modified",
                Value = SortBy.Modified
            }
        };

        private readonly IEnumerable<SortOrder> _defaultSortOrder = new List<SortOrder>()
        {
            new SortOrder()
            {
                Text = "Descending",
                Value = OrderBy.Desc
            },
            new SortOrder()
            {
                Text = "Ascending",
                Value = OrderBy.Asc
            },
        };


        private readonly ITagStore<Tag> _tagStore;
        private readonly IFeatureFacade _featureFacade;

        public TagListViewComponent(
            ITagStore<Tag> tagStore, 
            IFeatureFacade featureFacade)
        {
            _tagStore = tagStore;
            _featureFacade = featureFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            TagIndexOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new TagIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }

            var model = await GetViewModel(options, pager);

            return View(model);

        }

        async Task<TagIndexViewModel> GetViewModel(
            TagIndexOptions options,
            PagerOptions pager)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");

            var results = await _tagStore.QueryAsync()
                .Take(pager.Page, pager.PageSize)
                .Select<TagQueryParams>(q =>
                {
                    if (feature != null)
                    {
                        q.FeatureId.Equals(feature.Id);
                    }

                    if (!String.IsNullOrEmpty(options.Search))
                    {
                        q.Keywords.Like(options.Search);
                    }
                })
                .OrderBy(options.Sort.ToString(), options.Order)
                .ToList();
      
            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new TagIndexViewModel
            {
                SortColumns = _defaultSortColumns,
                SortOrder = _defaultSortOrder,
                Results = results,
                Options = options,
                Pager = pager
            };


        }

    }

}
