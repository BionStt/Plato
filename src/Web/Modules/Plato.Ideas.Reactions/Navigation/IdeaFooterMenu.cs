﻿using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Localization;
using Plato.Ideas.Models;
using Plato.Entities.Reactions.ViewModels;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Ideas.Reactions.Navigation
{
    public class IdeaFooterMenu : INavigationProvider
    {

      
        public IStringLocalizer T { get; set; }

        public IdeaFooterMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "idea-footer", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Idea)] as Idea;

            if (entity == null)
            {
                return;
            }

            builder
                .Add(T["Reactions"], int.MaxValue, react => react
                    .View("ReactionList", new
                    {
                        model = new ReactionListViewModel()
                        {
                            Entity = entity,
                            Permission = Permissions.ReactToIdeas
                        }
                    })
                    .Permission(Permissions.ViewIdeaReactions)
                );

        }

    }

}
