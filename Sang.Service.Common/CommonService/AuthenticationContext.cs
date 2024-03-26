using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sang.Service.Common.Models;
using Sang.Service.Common.Services;
using Sang.Service.Common.Validators;
using System;
using System.Threading.Tasks;

namespace Sang.Service.Common.CommonService
{
    public class AuthenticationContext : IAuthenticationContext
    {
        private readonly IDefaultDbRepository _defaultDbRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AuthenticationSettings _authenticationSettings;

        public AuthenticationContext(IDefaultDbRepository defaultDbRepository,
            IHttpContextAccessor contextAccessor, IOptions<AuthenticationSettings> authenticationSettings)            
        {
            _defaultDbRepository = defaultDbRepository;
            _contextAccessor = contextAccessor;
            _authenticationSettings = authenticationSettings.Value;
        }

        public async Task<string> GetConnection()
        {
            var token = _contextAccessor.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            //var token = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    var claimsPrincipal = TokenValidator.ValidateToken(token, _authenticationSettings.TokenKey);                    
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
    }
}
