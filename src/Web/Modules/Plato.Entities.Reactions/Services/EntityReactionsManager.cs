﻿using System;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Stores;
using Plato.Entities.Stores;
using PlatoCore.Abstractions;
using PlatoCore.Messaging.Abstractions;

namespace Plato.Entities.Reactions.Services
{

    public class EntityReactionsManager : IEntityReactionsManager<EntityReaction>
    {

        private readonly IEntityReactionsStore<EntityReaction> _entityReactionsStore;
        private readonly IEntityStore<Entity> _entityStore;
        private readonly IBroker _broker;

        public EntityReactionsManager(
            IEntityReactionsStore<EntityReaction> entityReactionsStore,
            IEntityStore<Entity> entityStore,
            IBroker broker)
        {
            _entityReactionsStore = entityReactionsStore;
            _entityStore = entityStore;
            _broker = broker;
        }

        public async Task<ICommandResult<EntityReaction>> CreateAsync(EntityReaction model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }

            // Get entity
            var entity = await _entityStore.GetByIdAsync(model.EntityId);
            if (entity == null)
            {
                throw new Exception("The entity no longer exists!");
            }

            if (String.IsNullOrEmpty(model.ReactionName))
            {
                throw new ArgumentNullException(nameof(model.ReactionName));
            }
            
            model.FeatureId = entity.FeatureId;
            model.CreatedDate = DateTime.UtcNow;

            // Invoke EntityReactionCreating subscriptions
            foreach (var handler in _broker.Pub<EntityReaction>(this, "EntityReactionCreating", model))
            {
                model = await handler.Invoke(new Message<EntityReaction>(model, this));
            }

            // Create result
            var result = new CommandResult<EntityReaction>();

            // Attempt to persist
            var reaction = await _entityReactionsStore.CreateAsync(model);
            if (reaction != null)
            {

                // Invoke EntityReactionCreated subscriptions
                foreach (var handler in _broker.Pub<EntityReaction>(this, "EntityReactionCreated", reaction))
                {
                    reaction = await handler.Invoke(new Message<EntityReaction>(reaction, this));
                }

                return result.Success(reaction);

            }

            return result.Failed($"An unknown error occurred whilst attempting to create a reaction");

        }

        public async Task<ICommandResult<EntityReaction>> UpdateAsync(EntityReaction model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }

            if (String.IsNullOrEmpty(model.ReactionName))
            {
                throw new ArgumentNullException(nameof(model.ReactionName));
            }
            
            // Get entity
            var entity = await _entityStore.GetByIdAsync(model.EntityId);
            if (entity == null)
            {
                throw new Exception("The entity no longer exists!");
            }
            
            model.FeatureId = entity.FeatureId;

            // Invoke EntityReactionUpdating subscriptions
            foreach (var handler in _broker.Pub<EntityReaction>(this, "EntityReactionUpdating", model))
            {
                model = await handler.Invoke(new Message<EntityReaction>(model, this));
            }

            // Create result
            var result = new CommandResult<EntityReaction>();

            // Attempt to persist
            var reaction = await _entityReactionsStore.UpdateAsync(model);
            if (reaction != null)
            {

                // Invoke EntityReactionUpdated subscriptions
                foreach (var handler in _broker.Pub<EntityReaction>(this,"EntityReactionUpdated", reaction))
                {
                    reaction = await handler.Invoke(new Message<EntityReaction>(reaction, this));
                }

                return result.Success(reaction);

            }

            return result.Failed($"An unknown error occurred whilst attempting to update a reaction");

        }

        public async Task<ICommandResult<EntityReaction>> DeleteAsync(EntityReaction model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Invoke EntityReactionDeleting subscriptions
            foreach (var handler in _broker.Pub<EntityReaction>(this, "EntityReactionDeleting", model))
            {
                model = await handler.Invoke(new Message<EntityReaction>(model, this));
            }

            var result = new CommandResult<EntityReaction>();

            var success = await _entityReactionsStore.DeleteAsync(model);
            if (success)
            {

                // Invoke EntityReactionDeleted subscriptions
                foreach (var handler in _broker.Pub<EntityReaction>(this, "EntityReactionDeleted", model))
                {
                    model = await handler.Invoke(new Message<EntityReaction>(model, this));
                }

                return result.Success(model);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the reoaction."));

        }

    }

}
