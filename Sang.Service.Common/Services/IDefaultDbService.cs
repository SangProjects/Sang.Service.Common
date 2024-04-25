using System.Data;

namespace Sang.Service.Common.Services
{
    public interface IDefaultDbService
    {
        Task<string> GetConnectionString(string databseKey);
        Task<DataTable?> GetDatabase();
    }
}
