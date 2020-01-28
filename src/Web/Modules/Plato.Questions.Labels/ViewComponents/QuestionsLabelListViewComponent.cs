﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlatoCore.Data.Abstractions;
using PlatoCore.Navigation.Abstractions;
using Plato.Labels.Services;
using Plato.Labels.ViewModels;
using Plato.Questions.Labels.Models;

namespace Plato.Questions.Labels.ViewComponents
{

    public class QuestionsLabelListViewComponent : ViewComponent
    {

        private readonly IEnumerable<SortColumn> _defaultSortColumns = new List<SortColumn>()
        {
            new SortColumn()
            {
                Text = "Questions",
                Value = LabelSortBy.Entities
            },
            new SortColumn()
            {
                Text = "Follows",
                Value =  LabelSortBy.Follows
            },
            new SortColumn()
            {
                Text = "Views",
                Value = LabelSortBy.Views
            },
            new SortColumn()
            {
                Text = "Created",
                Value =  LabelSortBy.Created
            },
            new SortColumn()
            {
                Text = "Last Seen",
                Value = LabelSortBy.LastEntity
            },
            new SortColumn()
            {
                Text = "Modified",
                Value = LabelSortBy.Modified
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
        
        private readonly ILabelService<Label> _labelService;

        public QuestionsLabelListViewComponent(
            ILabelService<Label> labelService)
        {
            _labelService = labelService;
        }

        public async Task<IViewComponentResult> InvokeAsync(LabelIndexOptions options, PagerOptions pager)
        {

            if (options == null)
            {
                options = new LabelIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            return View(await GetViewModel(options, pager));

        }

        async Task<LabelIndexViewModel<Label>> GetViewModel(LabelIndexOptions options, PagerOptions pager)
        {

            var results = await _labelService
                .GetResultsAsync(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new LabelIndexViewModel<Label>
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
