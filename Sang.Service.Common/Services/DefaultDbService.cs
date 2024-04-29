using Sang.Service.Common.Repositories;
using System.Data;

namespace Sang.Service.Common.Services
{
    public class DefaultDbService : IDefaultDbService
    {
        private IDefaultDbRepository _databaseRepository;

        public DefaultDbService(IDefaultDbRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }

        public async Task<string> GetConnectionString(string databseKey) =>
            await _databaseRepository.GetConnectionString(databseKey);
        public async Task<DataTable?> GetDatabase() =>
            await _databaseRepository.GetDatabase();
    }
}