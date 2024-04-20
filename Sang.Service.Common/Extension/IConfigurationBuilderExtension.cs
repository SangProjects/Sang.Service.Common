using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;

namespace Sang.Service.Common.Extension
{
    public static class IConfigurationBuilderExtension
    {
        public static IConfigurationBuilder BuildCommonConfiguration(
            this IConfigurationBuilder builder,
            IApiSettings settings = null)
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            return builder.BuildCommonConfiguration(env, settings);
        }

        public static IConfigurationBuilder BuildCommonConfiguration(
            this IConfigurationBuilder builder,
            string environmentName,
            IApiSettings settings = null)
        {
            var assembly = Assembly.GetEntryAssembly();
            var assemblyPath = Path.GetDirectoryName(assembly.Location);

            Console.WriteLine("Loading application Configuration from {0}", assemblyPath);
            builder.SetBasePath(assemblyPath)
               .AddJsonFile("appsettings.json")
               .AddJsonFile($"appsettings.{environmentName}.json", true, true);

            return builder;
        }
    }
}
