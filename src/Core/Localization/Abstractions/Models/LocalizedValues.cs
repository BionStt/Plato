﻿using System.Collections.Generic;

namespace PlatoCore.Localization.Abstractions.Models
{

    public class LocalizedValues<TModel> where TModel : class, ILocalizedValue
    {
        public LocaleResource Resource { get; set; }

        public IEnumerable<TModel> Values { get; set; } = new List<TModel>();

        public LocalizedValues()
        {
        }

        public LocalizedValues(LocaleResource resource, IEnumerable<TModel> values)
        {
            this.Resource = resource;
            this.Values = values;
        }
    }

}
