using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sang.Service.Common.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sang.Service.Common.CommonService
{
    public class DefaultEntityService : IDefaultEntityService
    {
        private readonly DatabaseConfiguration _dbConfiguration;
        private readonly ILogger<DefaultEntityService> _logger;       

        public DefaultEntityService(IOptions<DatabaseConfiguration> dbConfiguration,
                                   ILogger<DefaultEntityService> logger)
        {
            _dbConfiguration = dbConfiguration.Value;
            _logger = logger;
        }
 
        public async Task<DataTable> GetDataTable(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_dbConfiguration.DefaultDBConnection))
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
        public async Task<T> ExecuteScalar<T>(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_dbConfiguration.DefaultDBConnection))
                {
                    if (connection.State != ConnectionState.Open)
                        await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;

                        if (parameters != null)
                            command.Parameters.AddRange(parameters);
                        var returnValue = await command.ExecuteScalarAsync();

                        return (T)returnValue;
                    }
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
