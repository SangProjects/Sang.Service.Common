using Microsoft.Extensions.Logging;
using Sang.Service.Common.Extension;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Sang.Service.Common.CommonService
{
    public class DefaultEntityService : IDefaultEntityService
    {
        private readonly IApiSettings _apiSettings;
        private readonly ILogger<DefaultEntityService> _logger;

        public DefaultEntityService(IApiSettings apiSettings,
                                   ILogger<DefaultEntityService> logger)
        {
            _apiSettings = apiSettings;
            _logger = logger;
        }

        public async Task<DataTable> GetDataTable(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_apiSettings.DefaultDBConnection))
                {
                    if (connection.State != ConnectionState.Open)
                        await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        command.CommandType = CommandType.Text;
                        if (parameters != null)
                            command.Parameters.AddRange(parameters);
                        var dataAdapter = new SqlDataAdapter(command);
                        dataAdapter.Fill(dataTable);

                        return dataTable.Rows.Count > 0 ? dataTable : null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public DataTable? ExecuteAndFetchDataTable(SqlConnection connection,
                                                  SqlTransaction transaction,
                                                  string procedureName,
                                                  SqlParameter[] parameters)
        {
            try
            {
                using (SqlCommand command = new SqlCommand(procedureName, connection, transaction))
                {
                    DataTable dataTable = new DataTable();
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);
                    var dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);

                    return dataTable.Rows.Count > 0 ? dataTable : null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
