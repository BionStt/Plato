﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Layout.ModelBinding
{
    public interface IUpdateModel
    {
        Task<bool> TryUpdateModelAsync(object model, Type modelType, string prefix);
        //Task<bool> TryUpdateModelAsync<TModel>(TModel model, string prefix, params Expression<Func<TModel, object>>[] includeExpressions) where TModel : class;
        bool TryValidateModel(object model);
        bool TryValidateModel(object model, string prefix);
        //ModelStateDictionary ModelState { get; }
    }
}
