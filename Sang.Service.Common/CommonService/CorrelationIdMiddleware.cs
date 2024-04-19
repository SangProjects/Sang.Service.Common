using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;

namespace Sang.Service.Common.CommonService
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _request;
        private const string _correlationIdHeader = "X-Correlation-Id";

        public CorrelationIdMiddleware(RequestDelegate request)
        {
            _request = request;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            string correlationId = null;
            if (httpContext.Request.Headers.TryGetValue(
                _correlationIdHeader, out StringValues correlationIds))
            {
                correlationId = correlationIds.FirstOrDefault(c => c.Equals(_correlationIdHeader));
            }
            else
            {
                correlationId = Guid.NewGuid().ToString();
                httpContext.Request.Headers.Append(_correlationIdHeader, correlationId);
            }
            httpContext.Response.OnStarting(() =>
            {
                if (!httpContext.Response.Headers.
                TryGetValue(_correlationIdHeader,
                out correlationIds))
                    httpContext.Response.Headers.Append(_correlationIdHeader, correlationId);

                return Task.CompletedTask;
            });

            await _request(httpContext);
        }

    }
}
