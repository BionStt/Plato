﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Localization;

namespace PlatoCore.Layout.Alerts
{

    public enum AlertType
    {
        Success,
        Info,
        Warning,
        Danger
    }
    
    public class AlertInfo
    {

        public AlertType Type { get; set; }

        public IHtmlContent Message { get; set; }

        public AlertInfo()
        {

        }

        public AlertInfo(AlertType type, LocalizedHtmlString message)
        {
            Type = type;
            Message = message;
        }

    }
}
