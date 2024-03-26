using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sang.Service.Common.CommonService
{
    public interface IDbTransactionService
    {
        Task ExecuteTransactionAsync(Func<SqlConnection, SqlTransaction, Task> transactionAction);
    }
}
