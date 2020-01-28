﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Services;
using Plato.Categories.ViewModels;
using Plato.Questions.Categories.Models;
using PlatoCore.Features.Abstractions;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Discuss.Categories.ViewComponents
{

    public class QuestionCategoryListSidebarViewComponent : ViewComponent
    {

        private readonly ICategoryService<Category> _categoryService;
        
        public QuestionCategoryListSidebarViewComponent(ICategoryService<Category> categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(CategoryIndexOptions options)
        {

            if (options == null)
            {
                options = new CategoryIndexOptions();
            }
            
            return View(await GetIndexModel(options));

        }
        
        async Task<CategoryListViewModel<Category>> GetIndexModel(CategoryIndexOptions options)
        {

            // Get categories
            var categories = await _categoryService
                .GetResultsAsync(new CategoryIndexOptions()
                {
                    FeatureId = options.FeatureId,
                    CategoryId = 0
                }, new PagerOptions()
                {
                    Page = 1,
                    Size = int.MaxValue
                });
            
            return new CategoryListViewModel<Category>()
            {
                Options = options,
                Categories = categories?.Data?.Where(c => c.ParentId == 0)
            };
        }


    }


}
