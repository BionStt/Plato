﻿using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Data.Abstractions;

namespace Plato.Media.Repositories
{

    public class MediaRepository : IMediaRepository<Models.Media>
    {
    
        private readonly IDbContext _dbContext;
        private readonly ILogger<MediaRepository> _logger;

        public MediaRepository(
            IDbContext dbContext,
            ILogger<MediaRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        #region "Implementation"

        public async Task<IPagedResults<Models.Media>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<Models.Media> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<Models.Media>>(
                    CommandType.StoredProcedure,
                    "SelectMediaPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<Models.Media>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new Models.Media();
                                entity.PopulateModel(reader);
                                output.Data.Add(entity);
                            }

                            if (await reader.NextResultAsync())
                            {
                                await reader.ReadAsync();
                                output.PopulateTotal(reader);
                            }

                        }

                        return output;

                    },
                    dbParams);
             
            }

            return output;

        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting media with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteMediaById",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;
        }

        public async Task<Models.Media> InsertUpdateAsync(Models.Media media)
        {
            var id = await InsertUpdateInternal(
                media.Id,
                media.Name,
                media.ContentBlob,
                media.ContentType,
                media.ContentLength,
                media.CreatedUserId,
                media.CreatedDate,
                media.ModifiedUserId,
                media.ModifiedDate);

            if (id > 0)
                return await SelectByIdAsync(id);

            return null;
        }

        public async Task<Models.Media> SelectByIdAsync(int id)
        {
            Models.Media media = null;
            using (var context = _dbContext)
            {
                media = await context.ExecuteReaderAsync<Models.Media>(
                    CommandType.StoredProcedure,
                    "SelectMediaById",
                    async reader =>
                    {
                        if ((reader != null) && reader.HasRows)
                        {
                            await reader.ReadAsync();
                            media = new Models.Media(reader);
                        }

                        return media;
                    }, new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });

            }

            return media;
        }

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            string name,
            byte[] contentBlob,
            string contentType,
            long contentLength,
            int createdUserId,
            DateTimeOffset? createdDate,
            int modifiedUserId,
            DateTimeOffset? modifiedDate)
        {

            var test = new IDbDataParameter[]
            {
                new DbParam("Id", DbType.Int32, id),
                new DbParam("Name", DbType.String, 255, name.ToSafeFileName().ToEmptyIfNull()),
                new DbParam("ContentBlob", DbType.Binary, contentBlob ?? new byte[0]),
                new DbParam("ContentType", DbType.String, 75, contentType.ToEmptyIfNull()),
                new DbParam("ContentLength", DbType.Int64, contentLength),
                new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate),
                new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
            };

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateMedia",
                    test);
            }

            return output;

        }

        #endregion
        
    }
}
