using Newtonsoft.Json;
using System.Data;

namespace Sang.Service.Common.UtilityManager
{
    public static class Utils
    {
        public static DataTable ConvertToDataTable<T>(this IEnumerable<T> list)
        {
            var dataTable = new DataTable();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                dataTable.Columns.Add(property.Name, property.PropertyType);
            }

            foreach (var item in list)
            {
                var row = dataTable.NewRow();

                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item);
                }

                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        public static IQueryable<T> ConvertFromDataTable<T>(this DataTable dataTable) where T : new()
        {
            var list = new List<T>();
            var properties = typeof(T).GetProperties();

            foreach (DataRow row in dataTable.Rows)
            {
                var item = new T();

                foreach (var property in properties)
                {
                    if (dataTable.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
                    {
                        property.SetValue(item, Convert.ChangeType(row[property.Name], property.PropertyType));
                    }
                }

                list.Add(item);
            }

            return (IQueryable<T>)list;
        }
        public static IQueryable<string> ConvertDataTableToIQ(DataTable dataTable)
        {
            var list = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                foreach (DataColumn col in dataTable.Columns)
                {
                    list.Add(row[col].ToString());
                }
            }

            return list.AsQueryable();
        }

        public static IEnumerable<string> ConvertFromDataTable(DataTable dataTable)
        {
            var list = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                foreach (DataColumn col in dataTable.Columns)
                {
                    list.Add(row[col].ToString());
                }
            }
            return list;
        }

        public static DataTable ToTableValuedParameter<T>(this IEnumerable<T> list)
        {
            var dataTable = new DataTable();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                //dataTable.Columns.Add(property.Name, property.PropertyType);
                dataTable.Columns.Add(property.Name);
            }

            foreach (var item in list)
            {
                var row = dataTable.NewRow();

                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item);
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
        public static DataTable SetColumnsOrder(this DataTable dtbl, DataTable columnNames)
        {
            foreach (DataRow dr in columnNames.Rows)
            {               
                if (!dtbl.Columns.Contains(dr["name"].ToString()))
                {
                    dtbl.Columns.Add(dr["name"].ToString());
                }
                dtbl.Columns[dr["name"].ToString()].SetOrdinal(columnNames.Rows.IndexOf(dr));                
            }
            return dtbl;
        }

        public static DataTable ConvertJObjectToDataTable(dynamic jObject) =>
            JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(jObject));

        public static string SerializeData(object? result) => 
            JsonConvert.SerializeObject(result);

        public static DataTable? DeserializeToTable(string? result) => 
            JsonConvert.DeserializeObject<DataTable>(result);
        public static DataSet? DeserializeToDataSet(string? result) =>
            JsonConvert.DeserializeObject<DataSet>(result);
    }
}
