using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Sang.Service.Common.UtilityManager;
using System;
using System.Data;

namespace Sang.Service.Common.CommonService
{
    public class MemoryRequestCache : IRequestCache
    {
        private IMemoryCache _cache;
        private MemoryCacheEntryOptions CacheEntryOptions;
        private SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);
        private ILogger<MemoryRequestCache> _logger;
        private readonly ICachePaginator _cachepaginator;

        public MemoryRequestCache(IMemoryCache cache,
                ICachePaginator cachepaginator,
                ILogger<MemoryRequestCache> logger)
        {
            _cache = cache;
            _logger = logger;
            _cachepaginator = cachepaginator;
        }

        public async Task<object> GetOrExecute<T>(string key, Func<Task<T>> action, int pageNumber, int pageSize,
                                                  int cacheEntryLifeTime)
        {
            await _cacheLock.WaitAsync();

            try
            {
                _logger.LogInformation($"Getting cache data {key}.....");
                if (_cache.TryGetValue(key, out var value))
                {
                    _logger.LogInformation($"Cache hit:{key} successfully retrieved");
                    return await GetPaginatedData(value, pageNumber, pageSize, key);
                }

                _logger.LogInformation($"Cache Getting {key} from database");
                var result = await action();
                CacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(cacheEntryLifeTime));
                _cache.Set(key, result, CacheEntryOptions);
                return await GetPaginatedData(result, pageNumber, pageSize, key);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Getting {key} from database {ex.Message}");
                throw;
            }
            finally
            {
                _cacheLock.Release();
            }
        }

        public void ClearCache(string userCacheKey)
        {
            _cache.Remove(userCacheKey);
        }

        private async Task<object> GetPaginatedData(object data, int pageNumber, int pageSize, string key)
        {
            if (pageNumber <= 0 || data == null)
                return data;

            var deserializeData = Utils.DeserializeToTable((string)data);

            if (deserializeData == null)
            {
                _logger.LogInformation($"Unable to deserialize cache data {key}.....");
                throw new NullReferenceException("Unable to deserialize cache data");
            }

            return await _cachepaginator.GetPaginator<DataTable>(deserializeData, pageNumber, pageSize);
        }

    }
}