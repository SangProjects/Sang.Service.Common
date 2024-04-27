using System.Data;

namespace Sang.Service.Common.CommonService
{
    public interface ICachePaginator
    {
        Task<DataSet> GetPaginator<T>(DataTable value, int pageNumber, int pageSize,string searchString="");
    }
}
