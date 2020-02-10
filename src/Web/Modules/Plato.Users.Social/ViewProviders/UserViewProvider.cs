﻿using System.Threading.Tasks;
using PlatoCore.Layout.ViewProviders.Abstractions;
using PlatoCore.Models.Users;
using PlatoCore.Stores.Abstractions.Users;
using Plato.Users.Social.Models;
using Plato.Users.Social.ViewModels;

namespace Plato.Users.Social.ViewProviders
{
    public class UserViewProvider : ViewProviderBase<User>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;

        public UserViewProvider(IPlatoUserStore<User> platoUserStore)
        {
            _platoUserStore = platoUserStore;
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(User user, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(User user, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(User user, IViewProviderContext updater)
        {

            // Don't adapt the view when creating new users
            if (user.Id == 0)
            {
                return Task.FromResult(default(IViewProviderResult));
            }

            var socialLinks = user.GetOrCreate<SocialLinks>();
            return Task.FromResult(Views(
                View<EditSocialViewModel>("Social.Edit.Content", model =>
                {
                    model.FacebookUrl = socialLinks.FacebookUrl;
                    model.TwitterUrl = socialLinks.TwitterUrl;
                    model.YouTubeUrl = socialLinks.YouTubeUrl;
                    return model;
                }).Order(10)
            ));

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(User user, IViewProviderContext context)
        {

            var model = new EditSocialViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(user, context);
            }
           
            if (context.Updater.ModelState.IsValid)
            {

                // Store social links in generic UserData store
                var data = user.GetOrCreate<SocialLinks>();
                data.FacebookUrl = model.FacebookUrl;
                data.TwitterUrl = model.TwitterUrl;
                data.YouTubeUrl = model.YouTubeUrl;
                user.AddOrUpdate<SocialLinks>(data);

                // Update user
                await _platoUserStore.UpdateAsync(user);

            }

            return await BuildEditAsync(user, context);

        }
        
    }

}
