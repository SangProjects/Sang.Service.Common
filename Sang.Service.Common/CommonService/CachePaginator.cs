using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Linq;

namespace Sang.Service.Common.CommonService
{
    public class CachePaginator : ICachePaginator
    {
        private ILogger<CachePaginator> _logger;

        public CachePaginator(ILogger<CachePaginator> logger)
        {
            _logger = logger;
        }

        public Task<object> GetPaginator<T>(DataTable cacheData, int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogInformation($"Getting paginator data");

                int startIndex = (pageNumber - 1) * pageSize;
                var paginatedRows = cacheData.AsEnumerable()
                                             .Skip(startIndex)
                                             .Take(pageSize);

                DataTable paginatedDataTable = cacheData.Clone();
                foreach (DataRow row in paginatedRows)
                    paginatedDataTable.ImportRow(row);

                // NOTE: Need to uncomment if required
                //int totalRows = cacheData.Rows.Count;
                //int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                //return Task.FromResult<object>((paginatedDataTable, totalPages));

                return Task.FromResult<object>(paginatedDataTable);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Paginating :{ex.Message}");
                throw;
            }
        }
    }
}