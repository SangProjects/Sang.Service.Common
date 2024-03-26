using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;

namespace Sang.Service.Common.CommonService
{
    public class DisableInProductionAttribute : Attribute, IActionFilter
    {
        public const string disableNonProductionControllersKeyName = "DisableNonProductionControllers";
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var config = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            var disableNonProductionControllers = config.GetValue<bool>(disableNonProductionControllersKeyName);
            if (disableNonProductionControllers)
            {
                context.Result = new NotFoundResult();
            }
        }
    }
}
