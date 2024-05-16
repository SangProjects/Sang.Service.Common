using Microsoft.Extensions.Logging;
using Sang.Service.Common.CommonService;
using Sang.Service.Common.Repositories.DataScripts;
using System.Data;
using System.Data.SqlClient;

namespace Sang.Service.Common.Repositories
{
    public class DefaultDbRepository : IDefaultDbRepository
    {
        private readonly IDefaultDbTransactionService _defaultDbTransactionService;
        private readonly ILogger<DefaultDbRepository> _logger;
        private readonly IDefaultEntityService _defaultEntityService;

        public DefaultDbRepository(IDefaultEntityService defaultEntityService,
                                   IDefaultDbTransactionService defaultDbTransactionService,
                                   ILogger<DefaultDbRepository> logger)
        {
            _logger = logger;
            _defaultEntityService = defaultEntityService;
            _defaultDbTransactionService = defaultDbTransactionService;
        }
        public async Task<string> GetConnectionString(string databseKey)
        {
            try
            {
                DataTable connection;
                //string connectionString;
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@sDatabase", databseKey)
                };
                connection = await _defaultEntityService.GetDataTable(DefaultDbScripts.GetConnectionStringSql(), parameters.ToArray());
                //connectionString = await _defaultEntityService.ExecuteScalar(DefaultDbScripts.GetConnectionStringSql(), parameters.ToArray());

                if (connection == null) throw new ArgumentException("Invalid database name.");
                var connectionString = Convert.ToString(connection.Rows[0]["sConnection"]);
                _logger.LogInformation("Connection string fetched");

                return connectionString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<DataTable> GetDatabase()
        {
            try
            {
                //database = await _defaultEntityService.GetDataTable(DefaultDbScripts.GetDatabaseSql());
                DataTable? database = new();
                await _defaultDbTransactionService.ExecuteTransactionAsync(async (connection, transaction) =>
                {
                    database = _defaultEntityService.ExecuteAndFetchDataTable(connection, transaction,
                                                                                    DefaultDbScripts.GetDatabaseProcedure(),
                                                                                    null);
                });
                return database;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
