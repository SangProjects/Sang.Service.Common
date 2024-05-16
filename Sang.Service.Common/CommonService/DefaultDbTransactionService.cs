using Microsoft.Extensions.Logging;
using Sang.Service.Common.Extension;
using System.Data.SqlClient;

namespace Sang.Service.Common.CommonService
{
    public class DefaultDbTransactionService : IDefaultDbTransactionService
    {
        private readonly IApiSettings _apiSettings;
        private ILogger<DefaultDbTransactionService> _logger;

        public DefaultDbTransactionService(IApiSettings apiSettings,
                                    ILogger<DefaultDbTransactionService> logger)            
        {
            _apiSettings = apiSettings;
            _logger = logger;
        }

        public async Task ExecuteTransactionAsync(Func<SqlConnection, SqlTransaction, Task> transactionAction)
        {
            try
            {
                using (SqlConnection connection = new(_apiSettings.DefaultDBConnection))
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
                throw new Exception($"Default Database connection error: {ex.Message}", ex);
            }
        }
    }
}
