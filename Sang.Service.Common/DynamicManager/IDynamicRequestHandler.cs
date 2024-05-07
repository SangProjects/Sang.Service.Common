using Newtonsoft.Json.Linq;
using System.Data;

namespace Sang.Service.Common.CommonService
{
    public interface IDynamicRequestHandler
    {
        dynamic ConvertToExpandoObject(JObject requestJData);
        Task<DataTable?> GetTableTypeFields(string TableTypeName);
    }
}
