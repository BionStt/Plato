﻿using System.Threading.Tasks;
using Plato.Questions.Models;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using Plato.StopForumSpam.Services;

namespace Plato.Questions.StopForumSpam.ViewProviders
{

    public class QuestionViewProvider : ViewProviderBase<Question>
    {
        private readonly ISpamOperatorManager<Question> _spamOperatorManager;
 
        public QuestionViewProvider(
            ISpamOperatorManager<Question> spamOperatorManager)
        {
            _spamOperatorManager = spamOperatorManager;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Question entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Question entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Question entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override async Task<bool> ValidateModelAsync(Question entity, IUpdateModel updater)
        {
            
            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Question, entity);

            // IF any operators failed ensure we display the operator error message
            var valid = true;
            if (results != null)
            {
                foreach (var result in results)
                {
                    if (!result.Succeeded)
                    {
                        if (result.Operation.CustomMessage)
                        {
                            updater.ModelState.AddModelError(string.Empty,
                                !string.IsNullOrEmpty(result.Operation.Message)
                                    ? result.Operation.Message
                                    : $"Sorry but we've identified your details have been used by known spammers.");
                            valid = false;
                        }
                    }
                }
            }

            return valid;

        }

        public override async Task ComposeModelAsync(Question entity, IUpdateModel updater)
        {
            
            if (!updater.ModelState.IsValid)
            {
                return;
            }

            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Question, entity);
            if (results != null)
            {
                foreach (var result in results)
                {
                    // If any operator failed flag entity as SPAM
                    if (!result.Succeeded)
                    {
                        entity.IsSpam = true;
                    }
                }
            }
            
        }


        public override async Task<IViewProviderResult> BuildUpdateAsync(Question entity, IViewProviderContext context)
        {

            if (!context.Updater.ModelState.IsValid)
            {
                return await BuildIndexAsync(entity, context);
            }

            // Execute UpdateModel within registered spam operators
            await _spamOperatorManager.UpdateModelAsync(SpamOperations.Question, entity);

            // Return view
            return await BuildIndexAsync(entity, context);

        }
        
    }

}
