namespace Sang.Service.Common.Extension
{
    public class SangApiOptions
    {
        public string ApiName { get; set; }
        public int? ApiVersion { get; set; }
        public int? ApiPort { get; set; }
        public string DefaultDBConnection { get; set; }
        public string DBConnection { get; set; }
        public int TokenExpiryMin { get; set; }
        public int RefreshTokenExpiryMin { get; set; }
        public int UserCacheExpirationMinutes { get; set; }
        public bool IsRequestResponseLoggingEnabled { get; set; }
        public string[] AllowedFileExtensions { get; set; }
    }
}
        