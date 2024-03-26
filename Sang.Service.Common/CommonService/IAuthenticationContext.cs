using System.Threading.Tasks;

namespace Sang.Service.Common.CommonService
{
    public interface IAuthenticationContext
    {
        Task<string> GetConnection();
    }
}
