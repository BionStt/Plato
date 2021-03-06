﻿using PlatoCore.Models.Users;

namespace PlatoCore.Models.Notifications
{
    
    public class Notification : INotification
    {
   
        public IUser To { get; set; }

        public IUser From { get; set; }

        public INotificationType Type { get; }

        public Notification(INotificationType type)
        {
            this.Type = type;
        }

    }

}
