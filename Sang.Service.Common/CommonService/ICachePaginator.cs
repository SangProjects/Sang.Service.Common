using System.Data;
using System.Threading.Tasks;

namespace Sang.Service.Common.CommonService
{
    public interface ICachePaginator
    {
        Task<object> GetPaginator<T>(DataTable value, int pageNumber, int pageSize);
    }
}
