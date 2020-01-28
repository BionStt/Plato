﻿using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Cache;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;
using Plato.Moderation.Models;
using Plato.Moderation.Repositories;

namespace Plato.Moderation.Stores
{

    public class ModeratorStore : IModeratorStore<Moderator>
    {

        private readonly IModeratorRepository<Moderator> _moderatorRepository;
        private readonly ILogger<ModeratorStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;

        public ModeratorStore(
            IModeratorRepository<Moderator> moderatorRepository,
            ILogger<ModeratorStore> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _moderatorRepository = moderatorRepository;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }
        
        public async Task<Moderator> CreateAsync(Moderator model)
        {
            var result = await _moderatorRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<Moderator> UpdateAsync(Moderator model)
        {
            var result = await _moderatorRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<bool> DeleteAsync(Moderator model)
        {

            var success = await _moderatorRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted moderator for user '{0}' and category '{1}' with id {2}",
                        model.UserId, model.CategoryId, model.Id);
                }

                CancelTokens(model);
                
            }

            return success;

        }

        public async Task<Moderator> GetByIdAsync(int id)
        {
            return await _moderatorRepository.SelectByIdAsync(id);
        }

        public IQuery<Moderator> QueryAsync()
        {
            var query = new ModeratorQuery(this);
            return _dbQuery.ConfigureQuery<Moderator>(query); ;
        }

        public async Task<IPagedResults<Moderator>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting moderators for key '{0}' with the following parameters: {1}",
                        token.ToString(), dbParams.Select(p => p.Value));
                }

                return await _moderatorRepository.SelectAsync(dbParams);

            });
        }
        
        public void CancelTokens(Moderator model = null)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

    }
    
}
