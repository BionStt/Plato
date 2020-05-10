﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PlatoCore.Layout.ModelBinding;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Users;
using PlatoCore.Stores.Abstractions.Users;
using Plato.Users.Services;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{

    public class EditAccountViewProvider : ViewProviderBase<EditAccountViewModel>
    {
        
        private readonly IPlatoUserManager<User> _platoUserManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly UserManager<User> _userManager;

        public EditAccountViewProvider(
            IPlatoUserStore<User> platoUserStore,
            UserManager<User> userManager,
            IPlatoUserManager<User> platoUserManager)
        {
            _platoUserStore = platoUserStore;
            _userManager = userManager;
            _platoUserManager = platoUserManager;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(EditAccountViewModel viewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(EditAccountViewModel viewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(EditAccountViewModel viewModel, IViewProviderContext updater)
        {

            var user = await _platoUserStore.GetByIdAsync(viewModel.Id);
            if (user == null)
            {
                return await BuildIndexAsync(viewModel, updater);
            }
            
            return Views(
                View<User>("Home.Edit.Header", model => user).Zone("header"),
                View<User>("Home.Edit.Sidebar", model => user).Zone("content-left"),
                View<User>("Home.Edit.Tools", model => user).Zone("header-right"),
                View<EditAccountViewModel>("Home.EditAccount.Content", model => viewModel).Zone("content"),
                View<User>("Home.Edit.Footer", model => user).Zone("actions-right")
            );

        }

        public override async Task<bool> ValidateModelAsync(EditAccountViewModel viewModel, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new EditAccountViewModel()
            {
                UserName = viewModel.UserName,
                Email = viewModel.Email
            });
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(EditAccountViewModel userProfile, IViewProviderContext context)
        {
            var user = await _platoUserStore.GetByIdAsync(userProfile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userProfile, context);
            }

            var model = new EditAccountViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(userProfile, context);
            }

            if (context.Updater.ModelState.IsValid)
            {

                //// Has the username changed?
                //if (model.UserName != null && !model.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase))
                //{
                //    // SetUserNameAsync internally sets a new SecurityStamp
                //    // which will invalidate the authentication cookie
                //    // This will force the user to be logged out
                //    await _userManager.SetUserNameAsync(user, model.UserName);
                //}
                
                //// Has the email address changed?
                //if (model.Email != null && !model.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
                //{
                //    // Only call SetEmailAsync if the email address changes
                //    // SetEmailAsync internally sets EmailConfirmed to "false"
                //    await _userManager.SetEmailAsync(user, model.Email);
                //}

                //// Update user
                //var result = await _platoUserManager.UpdateAsync(user);
                //foreach (var error in result.Errors)
                //{
                //    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                //}

            }

            return await BuildEditAsync(userProfile, context);

        }

        #endregion

    }

}
