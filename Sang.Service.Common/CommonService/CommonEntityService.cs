using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sang.Service.Common.Extension;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Sang.Service.Common.CommonService
{
    public class CommonEntityService : ICommonEntityService
    {
        private readonly IApiSettings _apiSettings;
        private readonly IAuthenticationContext _authenticationContext;
        private readonly ILogger<CommonEntityService> _logger;

        public CommonEntityService(IApiSettings apiSettings,
                                   IAuthenticationContext authenticationContext,
                                   ILogger<CommonEntityService> logger)
        {
            _apiSettings = apiSettings;
            _logger = logger;
            _authenticationContext = authenticationContext;

            _ = InitializeAsync();
        }
        public async Task InitializeAsync()
        {
            string connection = await _authenticationContext.GetConnection();
            if (!string.IsNullOrEmpty(connection))
                _apiSettings.DBConnection = connection;
        }

        public async Task<IEnumerable<T>> GetAllEntitiesAsync<T>(string sql, SqlParameter[] parameters = null)
        {
            try
            {
                //using (SqlConnection connection = new(_dbConfiguration.DBConnection))
                using (SqlConnection connection = new SqlConnection(_apiSettings.DBConnection))
                {
                    if (connection.State != ConnectionState.Open) await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        if (parameters != null)
                            command.Parameters.AddRange(parameters);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var result = new List<T>();
                            if (!reader.HasRows) return null;

                            while (await reader.ReadAsync())
                            {
                                var obj = Activator.CreateInstance<T>();
                                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                                {

                                    // Check if the property has the JsonProperty attribute
                                    var jsonPropertyAttribute = Attribute.GetCustomAttribute(prop,
                                                                 typeof(JsonPropertyAttribute)) as JsonPropertyAttribute;

                                    // If the JsonProperty attribute is present, use the specified name
                                    string jsonPropertyName = jsonPropertyAttribute?.PropertyName ?? prop.Name;
                                    if (prop.PropertyType == typeof(string))
                                    {
                                        // Handle DBNull.Value for string properties
                                        prop.SetValue(obj, reader[jsonPropertyName] == DBNull.Value ? null : reader[jsonPropertyName], null);
                                    }
                                    else if (prop.PropertyType.IsEnum)
                                    {
                                        // Convert the database value to the corresponding enum value
                                        var enumValue = Enum.Parse(prop.PropertyType, reader[jsonPropertyName].ToString());
                                        prop.SetValue(obj, enumValue, null);
                                    }
                                    else
                                    {
                                        prop.SetValue(obj, reader[jsonPropertyName], null);
                                    }
                                }
                                result.Add(obj);
                            }
                            //await connection.CloseAsync();
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<DataTable> GetDataTable(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_apiSettings.DBConnection))
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

        public async Task<bool> ExecuteStoredProcedure(SqlConnection connection,
                                                    SqlTransaction transaction,
                                                    string procedureName,
                                                    SqlParameter[] parameters)
        {
            try
            {
                using (SqlCommand command = new SqlCommand(procedureName, connection, transaction))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                        command.Parameters.AddRange(parameters);
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public DataSet ExecuteAndFetchDataset(SqlConnection connection,
                                                  SqlTransaction transaction,
                                                  string procedureName,
                                                  SqlParameter[] parameters)
        {
            try
            {
                using (SqlCommand command = new SqlCommand(procedureName, connection, transaction))
                {
                    DataSet dataSet = new DataSet();
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);
                    var dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(dataSet);

                    return dataSet.Tables.Count > 0 ? dataSet : null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public DataTable ExecuteAndFetchDataTable(SqlConnection connection,
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

        public async Task<bool> ExecuteQuery(SqlConnection connection,
                                                     SqlTransaction transaction,
                                                     string query,
                                                     SqlParameter[] parameters)
        {
            try
            {
                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    command.CommandType = CommandType.Text;

                    if (parameters != null)
                        command.Parameters.AddRange(parameters);
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<T> ExecuteScalar<T>(SqlConnection connection,
                                                    SqlTransaction transaction,
                                                    string query,
                                                    SqlParameter[] parameters)
        {
            try
            {
                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    command.CommandType = CommandType.Text;

                    if (parameters != null)
                        command.Parameters.AddRange(parameters);
                    var returnValue = await command.ExecuteScalarAsync();

                    return (T)returnValue;
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
