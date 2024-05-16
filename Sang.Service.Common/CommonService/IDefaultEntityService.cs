using System.Data;
using System.Data.SqlClient;

namespace Sang.Service.Common.CommonService
{
    public interface IDefaultEntityService
    {
        Task<DataTable> GetDataTable(string query, SqlParameter[] parameters = null);
        DataTable? ExecuteAndFetchDataTable(SqlConnection connection,
                                                  SqlTransaction transaction,
                                                  string procedureName,
                                                  SqlParameter[] parameters);
    }
}
