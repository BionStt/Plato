﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.ViewModels;
using Plato.Discuss.Categories.Models;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Discuss.Categories.ViewComponents
{

    public class DiscussCategoryDropDownViewComponent : ViewComponent
    {
     
        private readonly ICategoryService<Category> _categoryService;

        public DiscussCategoryDropDownViewComponent(ICategoryService<Category> categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(CategoryDropDownViewModel model)
        {

            if (model == null)
            {
                model = new CategoryDropDownViewModel();
            }

            if (model.SelectedCategories == null)
            {
                model.SelectedCategories = new int[0];
            }
            
            model.Categories = await BuildSelectionsAsync(model);

            return View(model);

        }

        private async Task<IList<Selection<CategoryBase>>> BuildSelectionsAsync(CategoryDropDownViewModel model)
        {
           
            // Get categories
            var categories = await _categoryService.GetResultsAsync(
                model.Options, new PagerOptions()
                {
                    Page = 1,
                    Size = int.MaxValue
                });

            // Indicate selections
            return categories?.Data?.Select(c => new Selection<CategoryBase>
                {
                    IsSelected = model.SelectedCategories.Any(v => v == c.Id),
                    Value = c
                })
                .ToList();

        }

    }
    
}

