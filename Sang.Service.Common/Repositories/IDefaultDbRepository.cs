using System.Data;

namespace Sang.Service.Common.Repositories
{
    public interface IDefaultDbRepository
    {
        Task<string> GetConnectionString(string databseKey);
        Task<DataTable> GetDatabase();
    }
}
