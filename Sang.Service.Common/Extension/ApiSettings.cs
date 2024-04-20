namespace Sang.Service.Common.Extension
{
    public class ApiSettings : IApiSettings
    {
        public string ApiFullName { get; internal set; }
        public string ApiName { get; internal set; }
        public int ApiVersion { get; internal set; }
        public string ApiVersionString { get; internal set; }
        public int ApiPort { get; internal set; }
        public bool IsProduction { get; internal set; }
        public string DocumentUrl { get; internal set; }
        public string SymmetricSecurityKey { get; internal set; }
        public string DefaultDBConnection { get; set; }
        public string DBConnection { get; set; }
        public int TokenExpiryMin { get; set; }
        public int RefreshTokenExpiryMin { get; set; }
        public int UserCacheExpirationMinutes { get; set; }
        public bool IsRequestResponseLoggingEnabled { get; set; }
        public string[] AllowedFileExtensions { get; set; }
    }
}
