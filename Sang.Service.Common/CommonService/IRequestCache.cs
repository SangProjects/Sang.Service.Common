using System;

namespace Sang.Service.Common.CommonService
{
    public interface IRequestCache
    {
        Task<object> GetOrExecute<T>(string key,
                                     Func<Task<T>> action,
                                     int pageNumber,
                                     int pageSize,
                                     int cacheEntryLifeTime,
                                     string searchString = "");
        void ClearCache(string cacheKey);
    }
}
