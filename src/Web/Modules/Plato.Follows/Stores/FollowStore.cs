﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Follows.Models;
using Plato.Follows.Repositories;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Stores.Abstractions.Users;
using PlatoCore.Text.Abstractions;

namespace Plato.Follows.Stores
{

    public class FollowStore : IFollowStore<Models.Follow>
    {
        
        private readonly IFollowRepository<Models.Follow> _followRepository;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ILogger<FollowStore> _logger;
        private readonly ICacheManager _cacheManager;
        private readonly IKeyGenerator _keyGenerator;
        
        public FollowStore(
            IFollowRepository<Models.Follow> followRepository,
            IDbQueryConfiguration dbQuery,
            ILogger<FollowStore> logger,
            ICacheManager cacheManager,
            IKeyGenerator keyGenerator)
        {
            _followRepository = followRepository;
            _keyGenerator = keyGenerator;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<Models.Follow> CreateAsync(Models.Follow model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.ThingId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.ThingId));
            }
            
            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }

            if (String.IsNullOrEmpty(model.CancellationToken))
            {
                model.CancellationToken = _keyGenerator.GenerateKey(o =>
                {
                    o.MaxLength = 100;
                });
            }

            var result = await _followRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;
        }

        public async Task<Models.Follow> UpdateAsync(Models.Follow model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.ThingId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.ThingId));
            }

            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }

            if (String.IsNullOrEmpty(model.CancellationToken))
            {
                model.CancellationToken = _keyGenerator.GenerateKey(o =>
                {
                    o.MaxLength = 100;
                });
            }
            
            var result = await _followRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(model);
            }

            return result;
        }

        public async Task<bool> DeleteAsync(Models.Follow model)
        {
            
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }
            
            var success = await _followRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted follow with EntityId {0} UserId {1}",
                        model.ThingId, model.CreatedUserId);
                }

                CancelTokens(model);

            }

            return success;
        }

        public async Task<Models.Follow> GetByIdAsync(int id)
        {
            return await _followRepository.SelectByIdAsync(id);
        }

        public IQuery<Models.Follow> QueryAsync()
        {
            var query = new FollowQuery(this);
            return _dbQuery.ConfigureQuery<Models.Follow>(query); ;
        }

        public async Task<IPagedResults<Models.Follow>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting follows for key '{0}' with parameters: {1}",
                        token.ToString(), dbParams.Select(p => p.Value));
                }

                return await _followRepository.SelectAsync(dbParams);

            });
        }

        public async Task<IEnumerable<Models.Follow>> SelectByNameAndThingId(string name, int thingId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), name, thingId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Adding followers for name {0} with thingId of {1} to cache with key {2}",
                        name, thingId, token.ToString());
                }

                return await _followRepository.SelectByNameAndThingId(name, thingId);

            });
        }

        public async Task<Models.Follow> SelectByNameThingIdAndCreatedUserId(string name, int thingId, int createdUserId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), name, thingId, createdUserId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Adding follow details for name {0} with createdUserId of {1} and thingId of {2} to cache with key {3}",
                      name,  createdUserId, thingId, token.ToString());
                }

                return await _followRepository.SelectByNameThingIdAndCreatedUserId(name, thingId, createdUserId);

            });
        }

        public void CancelTokens(Models.Follow model = null)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

        #endregion

    }

}
