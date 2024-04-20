namespace Sang.Service.Common.Extension
{
    public interface IApiSettings
    {
        string ApiFullName { get; }
        string ApiName { get; }
        int ApiVersion { get; }
        string ApiVersionString { get; }
        int ApiPort { get; }
        bool IsProduction { get; }
        string DocumentUrl { get; }
        string SymmetricSecurityKey { get; }
        string DefaultDBConnection { get; set; }
        string DBConnection { get; set; }
        int TokenExpiryMin { get; set; }
         int RefreshTokenExpiryMin { get; set; }
         int UserCacheExpirationMinutes { get; set; }
         bool IsRequestResponseLoggingEnabled { get; set; }
         string[] AllowedFileExtensions { get; set; }
    }
}
