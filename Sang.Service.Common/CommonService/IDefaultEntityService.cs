using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sang.Service.Common.CommonService
{
    public interface IDefaultEntityService
    {
        Task<DataTable> GetDataTable(string query, SqlParameter[] parameters = null);
        Task<T> ExecuteScalar<T>(string query, SqlParameter[] parameters = null);
    }
}
