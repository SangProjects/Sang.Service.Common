using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sang.Service.Common.Validators
{
    public static class TokenValidator
    {
        public static ClaimsPrincipal ValidateToken(string token, string TokenKey)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken
                     && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                            StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                throw new SecurityTokenExpiredException("Refresh token has expired");
            }

        }
    }
}
