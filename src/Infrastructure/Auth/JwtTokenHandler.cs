using ApplicationCore.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.Auth
{
    public class JwtTokenHandler : IJwtTokenHandler
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        public JwtTokenHandler()
        {
            if (_jwtSecurityTokenHandler == null)
            {
                _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            }
        }
        public string GetToken(JwtSecurityToken jwt)
        {
            return _jwtSecurityTokenHandler.WriteToken(jwt);
        }

        public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
        {
            try
            {
                var principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token!");
                }
                return principal;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
