﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using PlatoCore.Localization.Abstractions;
using PlatoCore.Localization.Abstractions.Models;

namespace PlatoCore.Layout.Localizers
{

    public class LocaleHtmlLocalizer : IHtmlLocalizer
    {

        private string _cultureCode;
                
        private readonly IOptions<LocaleOptions> _localeOptions;
        private readonly ILocaleStore _localeStore;

        public LocaleHtmlLocalizer(
            IOptions<LocaleOptions> localeOptions,
            ILocaleStore localeStore)
        {            
            _localeOptions = localeOptions;
            _localeStore = localeStore;
        }
        
        public LocalizedString GetString(string name)
        {
            var strings = GetAllStrings(false);
            var value = strings.FirstOrDefault(s => s.Name == name);
            return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
        }

        public LocalizedString GetString(string name, params object[] arguments)
        {
            var strings = GetAllStrings(false);
            var format = strings.FirstOrDefault(s => s.Name == name);
            var value = string.Format(format ?? name, arguments);
            return new LocalizedString(name, value, resourceNotFound: format == null);
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {

            if (String.IsNullOrEmpty(_cultureCode))
            {
                _cultureCode = _localeOptions.Value.Culture;
            }

            return _localeStore.GetAllStringsAsync(_cultureCode)
                .GetAwaiter()
                .GetResult();

        }

        public IHtmlLocalizer WithCulture(CultureInfo culture)
        {
            _cultureCode = culture.Name;
            return this;
        }

        public LocalizedHtmlString this[string name]
        {
            get
            {
                var strings = GetAllStrings(false);
                var value = strings.FirstOrDefault(s => s.Name == name);
                return new LocalizedHtmlString(name, value ?? name);
            }
        }

        public LocalizedHtmlString this[string name, params object[] arguments]
        {
            get
            {
                var strings = GetAllStrings(false);
                var format = strings.FirstOrDefault(s => s.Name == name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedHtmlString(name, value);
            }
        }

    }

}
