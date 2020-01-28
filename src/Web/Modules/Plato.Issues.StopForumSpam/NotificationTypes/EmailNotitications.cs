﻿using System.Collections.Generic;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Issues.StopForumSpam.NotificationTypes
{
    
    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification IssueSpam =
            new EmailNotification("IssueSpamEmail", "Issue Spam",
                "Send me an email notification for each issue detected as SPAM.");

        public static readonly EmailNotification CommentSpam =
            new EmailNotification("IssueCommentSpamEmail", "Issue Comment Spam",
                "Send me an email notification for each issue comment detected as SPAM.");
        
        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        IssueSpam,
                        CommentSpam
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        IssueSpam,
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
                        IssueSpam,
                        CommentSpam
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        IssueSpam,
                        CommentSpam
                    }
                }

            };
        }

    }

}
