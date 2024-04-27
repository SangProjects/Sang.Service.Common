using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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

        public Task<DataSet> GetPaginator<T>(DataTable cacheData, int pageNumber, int pageSize, string searchString="")
        {
            try
            {
                int startIndex;
                DataTable paginatedDataTable = new DataTable();
                DataTable filteredDataTable = new DataTable();
                IEnumerable<DataRow> paginatedRows;
                int totalRows;
                _logger.LogInformation($"Getting paginator data");
                
                if (pageNumber <= 0)               
                    startIndex = 0;              

                if (pageSize <= 0)                
                    pageSize = cacheData.Rows.Count;              
              
               if(!searchString.IsNullOrEmpty())
               {
                    filteredDataTable = cacheData.Select("Name Like '%"
                                                         + searchString
                                                         + "%' OR Address Like '%"
                                                         + searchString
                                                         + "'").CopyToDataTable();
                    if(pageSize<=0)
                    {
                        pageSize = filteredDataTable.Rows.Count;
                    }
                    startIndex = (pageNumber - 1) * pageSize;
                    paginatedRows = filteredDataTable.AsEnumerable()
                                                 .Skip(startIndex)
                                                 .Take(pageSize);

                    paginatedDataTable = filteredDataTable.Clone();

                }
                else
                {
                    startIndex = (pageNumber - 1) * pageSize;

                    paginatedRows = cacheData.AsEnumerable()

                                             .Skip(startIndex)
                                             .Take(pageSize);

                    paginatedDataTable = cacheData.Clone();

                }  
                
                foreach (DataRow row in paginatedRows)
                    paginatedDataTable.ImportRow(row);
               
                totalRows = searchString.IsNullOrEmpty() ? cacheData.Rows.Count : filteredDataTable.Rows.Count;

                int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(paginatedDataTable);
                dataSet.Tables[0].TableName = "Data";                

                dataSet.Tables.Add(new DataTable("TotalRows")
                {
                    Columns = { { "TotalRows", typeof(int) } },
                    Rows = { { totalRows } }
                });

                dataSet.Tables.Add(new DataTable("TotalPages")
                {
                    Columns = { { "TotalPages", typeof(int) } },
                    Rows = { { totalPages } }
                });

                return Task.FromResult(dataSet);              
                //return Task.FromResult<object>(paginatedDataTable);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Paginating :{ex.Message}");
                throw;
            }
        }
    }
}