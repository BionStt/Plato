﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Stores;
using Plato.Labels.ViewModels;

namespace Plato.Labels.ViewComponents
{

    public class LabelDropDownViewComponent : ViewComponent
    {

        private readonly ILabelStore<LabelBase> _labelStore;
  
        public LabelDropDownViewComponent(ILabelStore<LabelBase> labelStore)
        {
            _labelStore = labelStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(LabelDropDownViewModel model)
        {
            if (model.SelectedLabels == null)
            {
                model.SelectedLabels = new int[0];
            }

            var labels = await BuildSelectionsAsync(model);
            if (labels != null)
            {
                model.LabelsJson = labels.Serialize();
            }
        
            return View(model);

        }

        private async Task<IEnumerable<LabelApiResult>> BuildSelectionsAsync(LabelDropDownViewModel model)
        {

            if (model.SelectedLabels == null)
            {
                return new List<LabelApiResult>();
            }

            if (((int[])model.SelectedLabels).Length == 0)
            {
                return new List<LabelApiResult>();
            }
            
            // Get all labels for selected ids
            var labels = await _labelStore.QueryAsync()
                .Select<LabelQueryParams>(q =>
                {
                    q.Id.IsIn(model.SelectedLabels.ToArray());
                })
                .OrderBy("TotalEntities", OrderBy.Desc)
                .ToList();

            // Build results 
            var results = new List<LabelApiResult>();
            if (labels?.Data != null)
            {
                foreach (var label in labels.Data)
                {
                    results.Add(new LabelApiResult()
                    {
                        Id = label.Id,
                        Name = label.Name,
                        Description = label.Description,
                        ForeColor = label.ForeColor,
                        BackColor = label.BackColor
                    });

                }
            }
            return results;

        }

    }

}

