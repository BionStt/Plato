﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Extensions;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Layout.Views.Abstractions;

namespace Plato.Entities.ViewComponents
{
    public class EntityTreeViewComponent : ViewComponentBase
    {

        private readonly ISimpleEntityStore<SimpleEntity> _entityStore;
        
        public EntityTreeViewComponent(ISimpleEntityStore<SimpleEntity> entityStore)
        {
            _entityStore = entityStore;
        }

        public Task<IViewComponentResult> InvokeAsync(EntityTreeOptions options)
        {

            if (options == null)
            {
                options = new EntityTreeOptions();
            }
            
            // Build selection including parents 
            var selected = BuildSelectionsAsync(options);
            
            return Task.FromResult((IViewComponentResult) View(new EntityTreeViewModel
            {
                HtmlName = options.HtmlName,
                EnableCheckBoxes = options.EnableCheckBoxes,
                EditMenuViewName = options.EditMenuViewName,
                CssClass = options.CssClass,
                RouteValues = options.RouteValues,
                SelectedEntities = selected,
                SelectedParents = options.Entities?.RecurseParents<Entity>(options.SelectedEntity)
            }));

        }

        IList<Selection<ISimpleEntity>> BuildSelectionsAsync(EntityTreeOptions options)
        {
            
            // Build a model for our tree
            return options.Entities?.Select(e => new Selection<ISimpleEntity>
                {
                    IsSelected = options.SelectedEntity.Equals(e.Id),
                    Value = e
                })
                .ToList();
        
        }
        
    }

}
