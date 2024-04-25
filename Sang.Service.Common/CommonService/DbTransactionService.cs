using Microsoft.Extensions.Logging;
using Sang.Service.Common.Extension;
using System;
using System.Data.SqlClient;

namespace Sang.Service.Common.CommonService
{
    public class DbTransactionService : IDbTransactionService
    {
        private readonly IApiSettings _apiSettings;
        private ILogger<DbTransactionService> _logger;

        public DbTransactionService(IApiSettings apiSettings,
                                    ILogger<DbTransactionService> logger)
        {
            _apiSettings = apiSettings;
            _logger = logger;
        }

        public async Task ExecuteTransactionAsync(Func<SqlConnection, SqlTransaction, Task> transactionAction)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_apiSettings.DBConnection))
                {
                    await connection.OpenAsync();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            await transactionAction.Invoke(connection, transaction);
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception($"Database connection error: {ex.Message}", ex);
            }
        }
    }
}
