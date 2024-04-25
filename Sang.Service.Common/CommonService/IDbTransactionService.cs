using System;
using System.Data.SqlClient;

namespace Sang.Service.Common.CommonService
{
    public interface IDbTransactionService
    {
        Task ExecuteTransactionAsync(Func<SqlConnection, SqlTransaction, Task> transactionAction);
    }
}
