using System.Data.SqlClient;

namespace Sang.Service.Common.CommonService
{
    public interface IDefaultDbTransactionService
    {
        Task ExecuteTransactionAsync(Func<SqlConnection, SqlTransaction, Task> transactionAction);
    }
}
