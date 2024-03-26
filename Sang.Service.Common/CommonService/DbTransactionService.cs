using Sang.Service.Common.Models;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace Sang.Service.Common.CommonService
{
    public class DbTransactionService : IDbTransactionService
    {
        private readonly DatabaseConfiguration _dbConfiguration;
        private ILogger<DbTransactionService> _logger;

        public DbTransactionService(IOptions<DatabaseConfiguration> dbConfiguration,
                                    ILogger<DbTransactionService> logger)            
        {
            _dbConfiguration = dbConfiguration.Value;
            _logger = logger;
        }

        public async Task ExecuteTransactionAsync(Func<SqlConnection, SqlTransaction, Task> transactionAction)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_dbConfiguration.DBConnection))
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
