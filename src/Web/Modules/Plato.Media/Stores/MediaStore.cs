﻿using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Media.Repositories;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;

namespace Plato.Media.Stores
{

    public class MediaStore : IMediaStore<Models.Media>
    {
        private const string ById = "ById";

        private readonly IMediaRepository<Models.Media> _mediaRepository;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<MediaStore> _logger;
       
        public MediaStore(
            IMediaRepository<Models.Media> mediaRepository, 
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager,
            ILogger<MediaStore> logger)
        {
            _mediaRepository = mediaRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }

        #region "Implementation"

        public async Task<Models.Media> CreateAsync(Models.Media model)
        {
            var result = await _mediaRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<Models.Media> UpdateAsync(Models.Media model)
        {
            var result = await _mediaRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<bool> DeleteAsync(Models.Media model)
        {

            var success = await _mediaRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted media '{0}' with id {1}",
                        model.Name, model.Id);
                }

                CancelTokens(model);

            }

            return success;

        }

        public async Task<Models.Media> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ById, id);
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _mediaRepository.SelectByIdAsync(id));
        }

        public IQuery<Models.Media> QueryAsync()
        {
            var query = new MediaQuery(this);
            return _dbQuery.ConfigureQuery<Models.Media>(query); ;
        }

        public async Task<IPagedResults<Models.Media>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting media for key '{0}' with the following parameters: {1}",
                        token.ToString(), dbParams.Select(p => p.Value));
                }

                return await _mediaRepository.SelectAsync(dbParams);
              
            });
        }

        public void CancelTokens(Models.Media model = null)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

        #endregion

    }

}
