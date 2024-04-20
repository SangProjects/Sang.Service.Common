using Microsoft.AspNetCore.Http;
using Sang.Service.Common.Extension;
using Sang.Service.Common.Services;
using Sang.Service.Common.Validators;
using System;
using System.Security.Claims;

namespace Sang.Service.Common.CommonService
{
    public class AuthenticationContext : IAuthenticationContext
    {
        private readonly IDefaultDbRepository _defaultDbRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IApiSettings _apiSettings;

        public AuthenticationContext(IDefaultDbRepository defaultDbRepository,
            IHttpContextAccessor contextAccessor, IApiSettings apiSettings)
        {
            _defaultDbRepository = defaultDbRepository;
            _contextAccessor = contextAccessor;
            _apiSettings = apiSettings;
        }

        public async Task<string> GetConnection()
        {
            var token = _contextAccessor?.HttpContext?.Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty);
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    var claimsPrincipal = TokenValidator.ValidateToken(token, _apiSettings.SymmetricSecurityKey);
                    var databseKey = claimsPrincipal?.FindFirst("DatabaseKey")?.Value;
                    return await _defaultDbRepository.GetConnectionString(databseKey);
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> GetUserId()
        {
            var token = _contextAccessor?.HttpContext?.Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty);
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    var claimsPrincipal = TokenValidator.ValidateToken(token, _apiSettings.SymmetricSecurityKey);
                    return int.Parse(claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                }
                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
