﻿using System.Collections.Generic;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Articles.StopForumSpam.NotificationTypes
{
    
    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification ArticleSpam =
            new EmailNotification("ArticleSpamEmail", "Article Spam",
                "Send me an email notification for each article detected as SPAM.");

        public static readonly EmailNotification CommentSpam =
            new EmailNotification("ArticleCommentSpamEmail", "Article Comment Spam",
                "Send me an email notification for each article comment detected as SPAM.");
        
        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        ArticleSpam,
                        CommentSpam
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        ArticleSpam,
                        CommentSpam
                    }
                }
            };
        }

        public IEnumerable<DefaultNotificationTypes> GetDefaultNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        ArticleSpam,
                        CommentSpam
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        ArticleSpam,
                        CommentSpam
                    }
                }

            };
        }

    }

}
