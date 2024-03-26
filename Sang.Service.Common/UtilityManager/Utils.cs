using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
        public static string SerializeData(object result) => 
            JsonConvert.SerializeObject(result);

        public static DataTable DeserializeToTable(string result) => 
            JsonConvert.DeserializeObject<DataTable>(result);
    }
}
