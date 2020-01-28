﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Repositories;

namespace Plato.Tags.Stores
{

    public class EntityTagStore : IEntityTagStore<EntityTag>
    {

        private const string ByEntityId = "ByEntityId";
        private const string ByEntityReplyId = "ByEntityReplyId";

        private readonly IEntityTagsRepository<EntityTag> _entityTagsRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<EntityTagStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public EntityTagStore(
            IEntityTagsRepository<EntityTag> entityTagsRepository,
            ICacheManager cacheManager,
            ILogger<EntityTagStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _entityTagsRepository = entityTagsRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        #region "Implementation"

        public async Task<EntityTag> CreateAsync(EntityTag model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.TagId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.TagId));
            }

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }

            var result = await _entityTagsRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;
        }

        public async Task<EntityTag> UpdateAsync(EntityTag model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.TagId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.TagId));
            }

            if (model.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.EntityId));
            }

            var result = await _entityTagsRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;
        }

        public async Task<bool> DeleteAsync(EntityTag model)
        {
            var success = await _entityTagsRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted entity tag for entityId '{0}' and tagId {1}",
                        model.EntityId, model.TagId);
                }

                CancelTokens(model);

            }

            return success;
        }

        public async Task<EntityTag> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _entityTagsRepository.SelectByIdAsync(id));
        }

        public IQuery<EntityTag> QueryAsync()
        {
            var query = new EntityTagQuery(this);
            return _dbQuery.ConfigureQuery<EntityTag>(query); ;
        }

        public async Task<IPagedResults<EntityTag>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityTagsRepository.SelectAsync(dbParams));
        }

        public async Task<IEnumerable<EntityTag>> GetByEntityIdAsync(int entityId)
        {

            if (entityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityId));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByEntityId, entityId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityTagsRepository.SelectByEntityId(entityId));
        }

        public async Task<IEnumerable<EntityTag>> GetByEntityReplyIdAsync(int entityReplyId)
        {

            if (entityReplyId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityReplyId));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByEntityReplyId, entityReplyId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _entityTagsRepository.SelectByEntityReplyId(entityReplyId));
        }
        
        public async Task<bool> DeleteByEntityIdAsync(int entityId)
        {

            if (entityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityId));
            }

            var success = await _entityTagsRepository.DeleteByEntityId(entityId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted all tags for entityId '{0}'",
                        entityId);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        public async Task<bool> DeleteByEntityIdAndTagIdIdAsync(int entityId, int tagId)
        {

            var success = await _entityTagsRepository.DeleteByEntityIdAndTagId(entityId, tagId);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted entity Label for entityId '{0}' and tagId {1}",
                        entityId, tagId);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }

        public void CancelTokens(EntityTag model = null)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

        #endregion

    }

}
