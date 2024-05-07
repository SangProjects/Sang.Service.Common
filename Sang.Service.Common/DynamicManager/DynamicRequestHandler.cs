using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Sang.Service.Common.CommonService;
using Sang.Service.Common.Repositories.DataScripts;
using System.Data;
using System.Dynamic;

namespace Sang.Service.Common.DynamicManager
{
    public class DynamicRequestHandler : IDynamicRequestHandler
    {
        private readonly ICommonEntityService _commonEntityService;
        private ILogger<DbTransactionService> _logger;

        public DynamicRequestHandler(ICommonEntityService commonEntityService,
            ILogger<DbTransactionService> logger)
        {
            _commonEntityService = commonEntityService;
            _logger = logger;
        }

        public dynamic ConvertToExpandoObject(JObject requestJData)
        {
            try
            {
                dynamic resultEntity = new ExpandoObject();
                IDictionary<string, object> resultDictionary = resultEntity;

                foreach (var property in requestJData.Properties())
                {
                    resultDictionary[property.Name] = JTokenToObject(property.Value);
                }

                return resultEntity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<DataTable> GetTableTypeFields(string TableTypeName) =>
            await _commonEntityService.GetDataTable(SysObjectScript.GetTableTypeField(TableTypeName));

        private object JTokenToObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    IDictionary<string, object> obj = new ExpandoObject();
                    foreach (var child in token.Children<JProperty>())
                    {
                        obj[child.Name] = JTokenToObject(child.Value);
                    }
                    return obj;

                case JTokenType.Array:
                    return token.Select(JTokenToObject).ToList();

                default:
                    return ((JValue)token).Value;
            }
        }


    }
}
