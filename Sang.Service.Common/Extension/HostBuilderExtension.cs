using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace Sang.Service.Common.Extension
{
    public static class HostBuilderExtension
    {
        public const string SymmetricSecurityKey = "E9DB7E89123F52A9F2DB04EF04C7FE88";
        public static IHostBuilder UseSangApi(this IHostBuilder builder)
        {
            var options = LoadSangApiOptions();
            var settings = options.LoadApiSettings();
            ApiExtension.Configure(builder, settings);

            return builder;
        }

        private static SangApiOptions LoadSangApiOptions()
        {
            var configuration = new ConfigurationBuilder()
                .BuildCommonConfiguration()
                .Build();

            var config = configuration
                .GetSection(nameof(SangApiOptions))
                .Get<SangApiOptions>();

            var apiName = config?.ApiName ?? Assembly.GetEntryAssembly()?.GetName().Name;
            var apiVersion = config?.ApiVersion ?? 1;

            var settings = new SangApiOptions
            {
                ApiName = apiName,
                ApiVersion = apiVersion,
                ApiPort = config?.ApiPort ?? 0,
                DefaultDBConnection = config.DefaultDBConnection,
                TokenExpiryMin = config.TokenExpiryMin,
                RefreshTokenExpiryMin = config.RefreshTokenExpiryMin,
                UserCacheExpirationMinutes = config.UserCacheExpirationMinutes,
                IsRequestResponseLoggingEnabled = config.IsRequestResponseLoggingEnabled,
                AllowedFileExtensions = config.AllowedFileExtensions
            };

            return settings;
        }

        private static IApiSettings LoadApiSettings(this SangApiOptions config)
        {
            var apiName = config?.ApiName ?? Assembly.GetEntryAssembly()?.GetName().Name;
            var apiVersion = config?.ApiVersion ?? 1;

            var settings = new ApiSettings
            {
                ApiPort = config?.ApiPort ?? 0,
                ApiName = apiName,
                ApiFullName = $"{apiName}v {apiVersion}",
                ApiVersionString = $"v{apiVersion}",
                SymmetricSecurityKey = SymmetricSecurityKey,
                DefaultDBConnection = config.DefaultDBConnection,
                TokenExpiryMin = config.TokenExpiryMin,
                RefreshTokenExpiryMin = config.RefreshTokenExpiryMin,
                UserCacheExpirationMinutes = config.UserCacheExpirationMinutes,
                IsRequestResponseLoggingEnabled = config.IsRequestResponseLoggingEnabled,
                AllowedFileExtensions = config.AllowedFileExtensions
            };
            return settings;
        }
    }

}
