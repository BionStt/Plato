﻿using System;
using System.Threading.Tasks;
using Plato.Entities.History.Models;
using Plato.Entities.History.Services;
using Plato.Entities.History.Stores;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using PlatoCore.Data.Abstractions;
using PlatoCore.Messaging.Abstractions;

namespace Plato.Entities.History.Subscribers
{

    public class EntitySubscriber<TEntity> : IBrokerSubscriber where TEntity : class, IEntity
    {
        
        private readonly IEntityHistoryManager<EntityHistory> _entityHistoryManager;
        private readonly IEntityHistoryStore<EntityHistory> _entityHistoryStore;
        private readonly IEntityStore<TEntity> _entityStore;
        private readonly IBroker _broker;
      
        public EntitySubscriber(
            IEntityHistoryManager<EntityHistory> entityHistoryManager, 
            IEntityHistoryStore<EntityHistory> entityHistoryStore,
            IEntityStore<TEntity> entityStore,
            IBroker broker)
        {
            _entityHistoryManager = entityHistoryManager;
            _entityHistoryStore = entityHistoryStore;
            _entityStore = entityStore;
            _broker = broker;
        }

        #region "Implementation"

        public void Subscribe()
        {
   
            // Created
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));

            // Updating
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdating"
            }, async message => await EntityUpdating(message.What));
            
            // Updated
            _broker.Sub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdated"
            }, async message => await EntityUpdated(message.What));

        }

        public void Unsubscribe()
        {

            // Created
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityCreated"
            }, async message => await EntityCreated(message.What));
            
            // Updating
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdating"
            }, async message => await EntityUpdating(message.What));

            // Updated
            _broker.Unsub<TEntity>(new MessageOptions()
            {
                Key = "EntityUpdated"
            }, async message => await EntityUpdated(message.What));

        }

        #endregion

        #region "Private Methods"

        async Task<TEntity> EntityCreated(TEntity entity)
        {
            
            // Create entity history point
            await _entityHistoryManager.CreateAsync(new EntityHistory()
            {
                EntityId = entity.Id,
                Message = entity.Message,
                Html = entity.Html,
                CreatedUserId = entity.CreatedUserId,
                CreatedDate = entity.CreatedDate ?? DateTimeOffset.UtcNow
            });
            
            return entity;
            
        }

        async Task<TEntity> EntityUpdating(TEntity entity)
        {

            // Get previous history points
            var previousHistories = await _entityHistoryStore.QueryAsync()
                .Take(1, false)
                .Select<EntityHistoryQueryParams>(q =>
                {
                    q.EntityId.Equals(entity.Id);
                    q.EntityReplyId.Equals(0);
                })
                .OrderBy("CreatedDate", OrderBy.Desc)
                .ToList();

            // Get the most recently added history point
            EntityHistory previousHistory = null;
            if (previousHistories?.Data != null)
            {
                previousHistory = previousHistories.Data[0];
            }

            // If we have previous history we don't need to add a starting point
            if (previousHistory != null)
            {
                return entity;
            }

            // Get existing entity before any changes
            var existingEntity = await _entityStore.GetByIdAsync(entity.Id);

            // We need an existing entity
            if (existingEntity == null)
            {
                return entity;
            }

            // If we don't have any existing history points add our
            // existing entity (before updates) as the starting / original history point
            await _entityHistoryManager.CreateAsync(new EntityHistory()
            {
                EntityId = existingEntity.Id,
                Message = existingEntity.Message,
                Html = existingEntity.Html,
                CreatedUserId = existingEntity.EditedUserId > 0
                    ? existingEntity.EditedUserId
                    : existingEntity.CreatedUserId,
                CreatedDate = existingEntity.EditedDate ?? existingEntity.CreatedDate
            });

            return entity;

        }

        async Task<TEntity> EntityUpdated(TEntity entity)
        {

            // Get previous history points
            var previousHistories = await _entityHistoryStore.QueryAsync()
                .Take(1, false)
                .Select<EntityHistoryQueryParams>(q =>
                {
                    q.EntityId.Equals(entity.Id);
                    q.EntityReplyId.Equals(0);
                })
                .OrderBy("CreatedDate", OrderBy.Desc)
                .ToList();

            // Get the most recently added history point
            EntityHistory previousHistory = null;
            if (previousHistories?.Data != null)
            {
                previousHistory = previousHistories.Data[0];
            }

            // Ensure we actually have changes
            if (previousHistory != null)
            {
                // Don't save a history point if the Html has not changed
                if (entity.Html == previousHistory.Html)
                {
                    return entity;
                }
            }
           
            // Create entity history point
            await _entityHistoryManager.CreateAsync(new EntityHistory()
            {
                EntityId = entity.Id,
                Message = entity.Message,
                Html = entity.Html,
                CreatedUserId = entity.EditedUserId > 0 
                    ? entity.EditedUserId 
                    : entity.ModifiedUserId,
                CreatedDate = entity.EditedDate ?? DateTimeOffset.UtcNow
            });

            return entity;

        }

        #endregion

    }

}
