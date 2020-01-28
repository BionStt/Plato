﻿using System;
using Microsoft.Extensions.Localization;
using Plato.Ideas.Models;
using Plato.Entities.Extensions;
using Plato.Entities.Reactions.ViewModels;
using PlatoCore.Navigation.Abstractions;

namespace Plato.Ideas.Reactions.Navigation
{
    public class IdeaCommentMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }

        public IdeaCommentMenu(IStringLocalizer localizer)
        {
            T = localizer;            
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "idea-comment", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get model from navigation builder
            var entity = builder.ActionContext.HttpContext.Items[typeof(Idea)] as Idea;

            if (entity == null)
            {
                return;
            }

            var reply = builder.ActionContext.HttpContext.Items[typeof(IdeaComment)] as IdeaComment;
            
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
                            ModuleId = "Plato.Ideas.Reactions",
                            Entity = entity,
                            Reply = reply,
                            Permission = Permissions.ReactToIdeaComments
                        }
                    })
                );

        }

    }

}
