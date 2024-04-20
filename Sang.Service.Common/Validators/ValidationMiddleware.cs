using FluentValidation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace Sang.Service.Common.Validators
{
    public class ValidationMiddleware<T> where T : class
    {
        private readonly RequestDelegate _next;
        private readonly IValidator<T> _validator;

        public ValidationMiddleware(RequestDelegate next, IValidator<T> validator)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == "POST"
                || context.Request.Method == "PUT"
                || context.Request.Method == "GET")
            {
                string requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                context.Request.Body.Position = 0; // Reset the stream position for further processing

                var model = JsonConvert.DeserializeObject<T>(requestBody);
                var validationResult = _validator.Validate(model);

                if (!validationResult.IsValid)
                {
                    context.Response.StatusCode = 400;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(validationResult.Errors));
                    return;
                }
            }
            await _next(context);
        }
    }
}
