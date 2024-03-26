namespace Sang.Service.Common.Models
{
    public class AuthenticationSettings
    {
        public string TokenID { get; set; }
        public string TokenDescription { get; set; }
        public string TokenName { get; set; }
        public string TokenFormat { get; set; }
        public string TokenScheme { get; set; }
        public int TokenExpiryMin { get; set; }
        public int RefreshTokenExpiryMin { get; set; }
        public string TokenKey { get; set; }
        public string TokenExpiryHeader { get; set; }

    }
}
