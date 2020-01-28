﻿using System;
using Microsoft.Extensions.Localization;
using Plato.Articles.Models;
using Plato.Entities.Extensions;
using Plato.Entities.Reactions.ViewModels;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Articles.Reactions.Navigation
{
    public class CommentMenu : INavigationProvider
    {
              
        public IStringLocalizer T { get; set; }

        public CommentMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "article-comment", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Article)] as Article;
            var reply = builder.ActionContext.HttpContext.Items[typeof(Comment)] as Comment;
            
            // We need an entity
            if (entity == null)
            {
                return;
            }

            // We need a reply
            if (reply == null)
            {
                return;
            }

            // No need to show reactions if entity is hidden
            if (entity.IsHidden())
            {
                return;
            }

            // No need to show reactions if reply is hidden
            if (reply.IsHidden())
            {
                return;
            }
            
            // Add reaction menu view to navigation
            builder
                .Add(T["React"], react => react
                    .View("ReactionMenu", new
                    {
                        model = new ReactionMenuViewModel()
                        {
                            ModuleId = "Plato.Articles.Reactions",
                            Entity = entity,
                            Reply = reply,
                            Permission = Permissions.ReactToComments
                        }
                    })
                );
            
        }

    }

}
