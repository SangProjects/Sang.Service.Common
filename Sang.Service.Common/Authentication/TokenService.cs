using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sang.Service.Common.CommonService;
using Sang.Service.Common.Models;
using Sang.Service.Common.Repositories.DataScripts;
using Sang.Service.Common.Validators;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sang.Service.Common.Authentication
{
    public class TokenService : ITokenService
    {
        //private IUserService _userService;
        private readonly ICommonEntityService _commonEntityService;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly ILogger<TokenService> _logger;

        public TokenService(ILogger<TokenService> logger,
                            IOptions<AuthenticationSettings> authenticationSettings,
                            //IUserService userService,
                            ICommonEntityService commonEntityService)
        {
            _logger = logger;
            _authenticationSettings = authenticationSettings.Value;
            //_userService = userService;
            _commonEntityService = commonEntityService;
        }

        public Tokens CreateToken(string user, string databseKey, int userId)
        {
            return new Tokens()
            {
                AccessToken = GenerateToken(user, databseKey, userId),
                RefreshToken = GenerateToken(user, databseKey, userId, true)
            };
        }
        public async Task<Tokens> RefreshToken(string refershToken)
        {

            var principal = TokenValidator.ValidateToken(refershToken, _authenticationSettings.TokenKey);
            if (principal == null)
            {
                throw new ArgumentNullException("Invalid refresh token");
            }

            var username = principal?.FindFirst(ClaimTypes.Name)?.Value; //principal.Identity?.Name;
            var databaseKey = principal?.FindFirst("DatabaseKey")?.Value;
            int userId = int.Parse(principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@sUserName", username)
            };
            var user = await _commonEntityService.GetDataTable(UserScripts.GetUserByNameSql(), parameters.ToArray());

            //var authUser = await _userService.GetUser(username);

            if (user == null)
            {
                throw new ArgumentNullException("Invalid user");
            }


            return new Tokens()
            {
                AccessToken = GenerateToken(username, databaseKey, userId),
                RefreshToken = GenerateToken(username, databaseKey, userId, true)
            };
        }

        private string GenerateToken(string user, string databseKey, int userId, bool isRefreshToken = false)
        {
            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.TokenKey));

            //Creating Claims. You can add more information in these claims. For example email id.
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, user),
                new Claim(ClaimTypes.Name,user ),
                new Claim("CustomClaimType", "Database"),
                new Claim("DatabaseKey", databseKey) ,
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            //Creating credentials. Specifying which type of Security Algorithm we are using
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Creating Token description
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = !isRefreshToken ? DateTime.UtcNow.AddMinutes(_authenticationSettings.TokenExpiryMin)
                                          : DateTime.UtcNow.AddMinutes(_authenticationSettings.RefreshTokenExpiryMin),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation("Token generated");
            return tokenHandler.WriteToken(token);
        }
    }
}
