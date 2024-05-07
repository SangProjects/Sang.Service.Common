using System.Xml.Linq;

namespace Sang.Service.Common.Repositories.DataScripts
{
    public static class SysObjectScript
    {
        public static string GetTableTypeField(string typeName) => $"SELECT name FROM sys.columns WHERE object_id IN(SELECT type_table_object_id FROM sys.table_types WHERE name = '{typeName}')";
    }
}
