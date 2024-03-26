namespace Sang.Service.Common.UtilityManager
{
    public static class PasswordHelper
    {
        public static string HashPasswordWithChannelId(
            string password, int channelId) => BCrypt.Net.BCrypt.HashPassword(password + channelId);

        public static bool VerifyPasswordWithChannelId(string password, int channelId, string hashedPassword)
        {
            string combinedString = password + channelId;            
            bool passwordMatch = BCrypt.Net.BCrypt.Verify(combinedString, hashedPassword);

            return passwordMatch;
        }
    }
}
