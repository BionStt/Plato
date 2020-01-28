﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Ideas.Models;
using Plato.Ideas.StopForumSpam.NotificationTypes;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using PlatoCore.Models.Notifications;
using PlatoCore.Models.Users;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Notifications.Extensions;
using PlatoCore.Security.Abstractions;
using PlatoCore.Stores.Abstractions.Users;
using PlatoCore.Stores.Users;
using PlatoCore.Tasks.Abstractions;
using Plato.StopForumSpam.Models;
using Plato.StopForumSpam.Services;

namespace Plato.Ideas.StopForumSpam.SpamOperators
{
    public class CommentOperator : ISpamOperatorProvider<IdeaComment>
    {
        
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly INotificationManager<IdeaComment> _notificationManager;
        private readonly IDeferredTaskManager _deferredTaskManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IEntityReplyStore<IdeaComment> _replyStore;
        private readonly ISpamChecker _spamChecker;

        public CommentOperator(
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            INotificationManager<IdeaComment> notificationManager,
            IDeferredTaskManager deferredTaskManager,
            IPlatoUserStore<User> platoUserStore,
            IEntityReplyStore<IdeaComment> replyStore, 
            ISpamChecker spamChecker)
        {
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _notificationManager = notificationManager;
            _deferredTaskManager = deferredTaskManager;
            _platoUserStore = platoUserStore;
            _spamChecker = spamChecker;
            _replyStore = replyStore;
        }

        public async Task<ISpamOperatorResult<IdeaComment>> ValidateModelAsync(ISpamOperatorContext<IdeaComment> context)
        {

            // Ensure correct operation provider
            if (!context.Operation.Name.Equals(SpamOperations.Comment.Name, StringComparison.Ordinal))
            {
                return null;
            }

            // Get user for reply
            var user = await BuildUserAsync(context.Model);
            if (user == null)
            {
                return null;
            }

            // Create result
            var result = new SpamOperatorResult<IdeaComment>();
            
            // Check if user is already flagged as SPAM within Plato
            if (user.IsSpam)
            {
                return result.Failed(context.Model, context.Operation);
            }

            // Check StopForumSpam service
            var spamResult = await _spamChecker.CheckAsync(user);
            if (spamResult.Succeeded)
            {
                return result.Success(context.Model);
            }

            // Return failed with our updated model and operation
            // This provides the calling code with the operation error message
            return result.Failed(context.Model, context.Operation);
            
        }

        public async Task<ISpamOperatorResult<IdeaComment>> UpdateModelAsync(ISpamOperatorContext<IdeaComment> context)
        {

            // Perform validation
            var validation = await ValidateModelAsync(context);

            // Create result
            var result = new SpamOperatorResult<IdeaComment>();

            // Not an operator of interest
            if (validation == null)
            {
                return result.Success(context.Model);
            }

            // If validation succeeded no need to perform further actions
            if (validation.Succeeded)
            {
                return result.Success(context.Model);
            }

            // Get reply author
            var user = await BuildUserAsync(context.Model);
            if (user == null)
            {
                return null;
            }

            // Flag user as SPAM?
            if (context.Operation.FlagAsSpam)
            {
                var bot = await _platoUserStore.GetPlatoBotAsync();

                // Mark user as SPAM
                if (!user.IsSpam)
                {
                    user.IsSpam = true;
                    user.IsSpamUpdatedUserId = bot?.Id ?? 0;
                    user.IsSpamUpdatedDate = DateTimeOffset.UtcNow;
                    await _platoUserStore.UpdateAsync(user);
                }

                // Mark reply as SPAM
                if (!context.Model.IsSpam)
                {
                    context.Model.IsSpam = true;
                    await _replyStore.UpdateAsync(context.Model);
                }

            }

            // Defer notifications for execution after request completes
            _deferredTaskManager.AddTask(async ctx =>
            {
                await NotifyAsync(context);
            });

            // Return failed with our updated model and operation
            // This provides the calling code with the operation error message
            return result.Failed(context.Model, context.Operation);

        }

        async Task NotifyAsync(ISpamOperatorContext<IdeaComment> context)
        {

            // Get users to notify
            var users = await GetUsersAsync(context.Operation);

            // No users to notify
            if (users == null)
            {
                return;
            }

            // Get bot
            var bot = await _platoUserStore.GetPlatoBotAsync();

            // Send notifications
            foreach (var user in users)
            {

                // Web notification
                if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.CommentSpam))
                {
                    await _notificationManager.SendAsync(new Notification(WebNotifications.CommentSpam)
                    {
                        To = user,
                        From = bot
                    }, context.Model);
                }

                // Email notification
                if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.CommentSpam))
                {
                    await _notificationManager.SendAsync(new Notification(EmailNotifications.CommentSpam)
                    {
                        To = user
                    }, context.Model);
                }

            }

        }

        async Task<IEnumerable<User>> GetUsersAsync(ISpamOperation operation)
        {

            var roleNames = new List<string>(2);
            if (operation.NotifyAdmin)
                roleNames.Add(DefaultRoles.Administrator);
            if (operation.NotifyStaff)
                roleNames.Add(DefaultRoles.Staff);
            if (roleNames.Count == 0)
                return null;
            var users = await _platoUserStore.QueryAsync()
                .Select<UserQueryParams>(q =>
                {
                    q.RoleName.IsIn(roleNames.ToArray());
                })
                .ToList();
            return users?.Data;

        }
        
        async Task<User> BuildUserAsync(IEntityReply reply)
        {

            var user = await _platoUserStore.GetByIdAsync(reply.CreatedUserId);
            if (user == null)
            {
                return null;
            }

            // Ensure we check against the IP address being used at the time of the post
            user.IpV4Address = reply.IpV4Address;
            user.IpV6Address = reply.IpV6Address;
            return user;

        }


    }

}
