using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Sang.Service.Common.Extension
{
    public static class LoggingExtension
    {
        public static IHostBuilder UseSangLogging(this IHostBuilder builder)
        {
            builder.ConfigureLogging((hostingContext, logging) =>
            {
                // Configure Serilog for logging
                var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithCorrelationIdHeader("X-Correlation-Id")
                    .CreateLogger();

                logging.ClearProviders();
                logging.AddSerilog(logger, dispose: true);
            });

            return builder;
        }
    }
}
