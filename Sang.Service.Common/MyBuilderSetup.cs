using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sang.Service.Common.Authentication;

namespace Sang.Service.Common
{
    internal class MyBuilderSetup
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Add services to the container
                    services.AddSingleton<ITokenService, TokenService>();
                });

            return builder;
        }
    }
}
