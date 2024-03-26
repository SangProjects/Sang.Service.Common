using System.Data;
using System.Threading.Tasks;

namespace Sang.Service.Common.Services
{
    public interface IDefaultDbRepository
    {
        Task<string> GetConnectionString(string databseKey);
        Task<DataTable> GetDatabase();
    }
}
