﻿using System.Collections.Generic;
using PlatoCore.Models.Notifications;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Security.Abstractions;

namespace Plato.Questions.StopForumSpam.NotificationTypes
{
    
    public class EmailNotifications : INotificationTypeProvider
    {

        public static readonly EmailNotification QuestionSpam =
            new EmailNotification("QuestionSpamEmail", "Questions Spam",
                "Send me an email notification for each question detected as SPAM.");

        public static readonly EmailNotification AnswerSpam =
            new EmailNotification("AnswerSpamEmail", "Question Answer Spam",
                "Send me an email notification for each question answer detected as SPAM.");
        
        public IEnumerable<DefaultNotificationTypes> GetNotificationTypes()
        {
            return new[]
            {
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Administrator,
                    NotificationTypes = new[]
                    {
                        QuestionSpam,
                        AnswerSpam
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        QuestionSpam,
                        AnswerSpam
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
                        QuestionSpam,
                        AnswerSpam
                    }
                },
                new DefaultNotificationTypes
                {
                    RoleName = DefaultRoles.Staff,
                    NotificationTypes = new[]
                    {
                        QuestionSpam,
                        AnswerSpam
                    }
                }

            };
        }

    }

}
