﻿using System.Data;

namespace Sang.Service.Common.Services
{
    public interface IDefaultDbRepository
    {
        Task<string> GetConnectionString(string databseKey);
        Task<DataTable> GetDatabase();
    }
}
