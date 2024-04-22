using Sang.Service.Common.Models;

namespace Sang.Service.Common.Authentication
{
    public interface ITokenService
    {
        Tokens CreateToken(string user, string databseKey, int userId);
        Task<Tokens> RefreshToken(string refershToken);
    }
}
