using Microsoft.Extensions.Logging;
using Sang.Service.Common.Repositories;
using System.Data;

namespace Sang.Service.Common.Services
{
    public class DefaultDbService : IDefaultDbService
    {
        private IDefaultDbRepository _databaseRepository;
        private ILogger<DefaultDbService> _logger;

        public DefaultDbService(IDefaultDbRepository databaseRepository,
                           ILogger<DefaultDbService> logger)
        {
            _databaseRepository = databaseRepository;
            _logger = logger;
        }

        public async Task<string> GetConnectionString(string databseKey) =>
            await _databaseRepository.GetConnectionString(databseKey);
        public async Task<DataTable?> GetDatabase() =>
            await _databaseRepository.GetDatabase();
    }
}