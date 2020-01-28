﻿namespace PlatoCore.Localization.Abstractions.Models
{

    public class LocaleEmail : ILocalizedValue
    {

        public string Name { get; set; }

        public string To { get; set; }

        public string From { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

    }

}
