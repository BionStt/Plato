﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Questions.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using PlatoCore.Navigation.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Questions.ViewComponents
{

    public class QuestionAnswerListViewComponent : ViewComponent
    {

        private readonly IEntityStore<Question> _entityStore;
        private readonly IEntityReplyStore<Answer> _entityReplyStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityReplyService<Answer> _replyService;

        public QuestionAnswerListViewComponent(
            IEntityReplyStore<Answer> entityReplyStore,
            IEntityStore<Question> entityStore,
            IEntityReplyService<Answer> replyService,
            IAuthorizationService authorizationService)
        {
            _entityReplyStore = entityReplyStore;
            _entityStore = entityStore;
            _replyService = replyService;
            _authorizationService = authorizationService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            EntityOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new EntityOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            return View(await GetViewModel(options, pager));

        }

        async Task<EntityViewModel<Question, Answer>> GetViewModel(
            EntityOptions options,
            PagerOptions pager)
        {

            var entity = await _entityStore.GetByIdAsync(options.Id);
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            var results = await _replyService
                .ConfigureQuery(async q =>
                {

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewHiddenAnswers))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamAnswers))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedAnswers))
                    {
                        q.HideDeleted.True();
                    }

                })
                .GetResultsAsync(options, pager);
            
            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new EntityViewModel<Question, Answer>
            {
                Options = options,
                Pager = pager,
                Entity = entity,
                Replies = results
        };

        }

    }

}