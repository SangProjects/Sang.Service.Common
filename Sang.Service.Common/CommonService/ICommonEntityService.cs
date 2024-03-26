using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sang.Service.Common.CommonService
{
    public interface ICommonEntityService
    {
        Task<IEnumerable<T>> GetAllEntitiesAsync<T>(string sql, SqlParameter[] parameters = null);
        Task<DataTable> GetDataTable(string query, SqlParameter[] parameters = null);
        Task<bool> ExecuteStoredProcedure(SqlConnection connection,
                                              SqlTransaction transaction,
                                              string procedureName,
                                              SqlParameter[] parameters);
        DataSet ExecuteAndFetchDataset(SqlConnection connection,
                                                  SqlTransaction transaction,
                                                  string procedureName,
                                                  SqlParameter[] parameters);
        DataTable ExecuteAndFetchDataTable(SqlConnection connection,
                                                  SqlTransaction transaction,
                                                  string procedureName,
                                                  SqlParameter[] parameters);
        Task<bool> ExecuteQuery(SqlConnection connection,
                                                     SqlTransaction transaction,
                                                     string query,
                                                     SqlParameter[] parameters);
        Task<T> ExecuteScalar<T>(SqlConnection connection,
                                                    SqlTransaction transaction,
                                                    string query,
                                                    SqlParameter[] parameters);        
    }
}
