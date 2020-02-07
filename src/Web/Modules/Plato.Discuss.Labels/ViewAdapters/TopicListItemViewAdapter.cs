﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Discuss.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using PlatoCore.Data.Abstractions;
using PlatoCore.Features.Abstractions;
using PlatoCore.Layout.ViewAdapters;
using PlatoCore.Security.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Stores;
using Label = Plato.Discuss.Labels.Models.Label;

namespace Plato.Discuss.Labels.ViewAdapters
{

    public class TopicListItemViewAdapter : BaseAdapterProvider
    {
        
        private readonly ILabelStore<Label> _labelStore;
        private readonly IFeatureFacade _featureFacade;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEntityLabelStore<EntityLabel> _entityLabelStore;
        private readonly IEntityService<Topic> _entityService;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IAuthorizationService _authorizationService;

        public TopicListItemViewAdapter(
            ILabelStore<Label> labelStore,
            IFeatureFacade featureFacade,
            IEntityService<Topic> entityService, 
            IEntityLabelStore<EntityLabel> entityLabelStore,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor,
            IAuthorizationService authorizationService)
        {
            _labelStore = labelStore;
            _featureFacade = featureFacade;
            _entityService = entityService;
            _entityLabelStore = entityLabelStore;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
            _authorizationService = authorizationService;
            ViewName = "TopicListItem";
        }

        private IDictionary<int, IList<Label>> _lookUp;

        public override async Task<IViewAdapterResult> ConfigureAsync(string viewName)
        {

            if (!viewName.Equals(ViewName, StringComparison.OrdinalIgnoreCase))
            {
                return default(IViewAdapterResult);
            }

            // Get feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");
            if (feature == null)
            {
                // Feature not found
                return default(IViewAdapterResult);
            }


            // Plato.Discuss does not have a dependency on Plato.Discuss.Labels
            // Instead we update the model for the entity list item view component
            // here via our view adapter to include the label data for the entity
            // This way the label data is only ever populated if the labels feature is enabled
            return await Adapt(viewName, v =>
            {
                v.AdaptModel<EntityListItemViewModel<Topic>>(async model =>
                {

                    if (_lookUp == null)
                    {
                        // Get all labels for feature
                        var labels = await _labelStore.GetByFeatureIdAsync(feature.Id);
                        if (labels == null)
                        {
                            // Return an anonymous type, we are adapting a view component
                            return new
                            {
                                model
                            };
                        }

                        // Build a dictionary we can use below within our AdaptModel
                        // method to add the correct labels for each displayed entity
                        _lookUp = await NewMethod(labels);

                    }

                    if (model.Entity == null)
                    {
                        // Return an anonymous type as we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // No need to modify if we don't have a lookup table
                    if (_lookUp == null)
                    {
                        // Return an anonymous type as we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // No need to modify the model if no labels have been found
                    if (!_lookUp.ContainsKey(model.Entity.Id))
                    {
                        // Return an anonymous type as we are adapting a view component
                        return new
                        {
                            model
                        };
                    }

                    // Get labels for entity
                    var entityLabels = _lookUp[model.Entity.Id];

                    // Add labels to the model from our dictionary
                    var modelLabels = new List<Label>();
                    foreach (var label in entityLabels)
                    {
                        modelLabels.Add(label);
                    }

                    model.Labels = modelLabels;

                    // Return an anonymous type as we are adapting a view component
                    return new
                    {
                        model
                    };

                });
            });

        }

        private async Task<IDictionary<int, IList<Label>>> NewMethod(IEnumerable<Label> labels)
        {
            return await BuildLookUpTable(labels.ToList());
        }

        async Task<IDictionary<int, IList<Label>>> BuildLookUpTable(IEnumerable<Label> labels)
        {

            var entities = await GetDisplayedEntitiesAsync();

            // Get all entity label relationships for displayed entities
            IPagedResults<EntityLabel> entityLabels = null;
            if (entities?.Data != null)
            {
                entityLabels = await _entityLabelStore.QueryAsync()
                    .Select<EntityLabelQueryParams>(q =>
                    {
                        q.EntityId.IsIn(entities.Data.Select(e => e.Id).ToArray());
                    })
                    .ToList();
            }

            // Build a dictionary of entity and label relationships
            var output = new ConcurrentDictionary<int, IList<Label>>();
            if (entityLabels?.Data != null)
            {
                var labelList = labels.ToList();
                foreach (var entityLabel in entityLabels.Data)
                {
                    var label = labelList.FirstOrDefault(l => l.Id == entityLabel.LabelId);
                    if (label != null)
                    {
                        output.AddOrUpdate(entityLabel.EntityId, new List<Label>()
                        {
                            label
                        }, (k, v) =>
                        {
                            v.Add(label);
                            return v;
                        });
                    }
                }
            }

            return output;

        }

        private async Task<IPagedResults<Topic>> GetDisplayedEntitiesAsync()
        {

            // Get topic index view model from context
            var viewModel = _actionContextAccessor.ActionContext.HttpContext.Items[typeof(EntityIndexViewModel<Topic>)] as EntityIndexViewModel<Topic>;
            if (viewModel == null)
            {
                return null;
            }

            // Get all entities for our current view
            return await _entityService
                .ConfigureQuery(async q =>
                {

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User,
                        Permissions.ViewPrivateTopics))
                    {
                        q.HidePrivate.True();
                    }

                    // Hide hidden?
                    if (!await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User,
                        Permissions.ViewHiddenTopics))
                    {
                        q.HideHidden.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User,
                        Permissions.ViewSpamTopics))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User,
                        Permissions.ViewDeletedTopics))
                    {
                        q.HideDeleted.True();
                    }

                })
                .GetResultsAsync(viewModel?.Options, viewModel?.Pager);

        }

    }
    
}
