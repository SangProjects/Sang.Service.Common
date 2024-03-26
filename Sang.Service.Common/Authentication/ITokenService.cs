using Sang.Service.Common.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sang.Service.Common.Authentication
{
    public interface ITokenService
    {
        Tokens CreateToken(string user, string databseKey, int userId);
        Task<Tokens> RefreshToken(string refershToken);
    }
}
