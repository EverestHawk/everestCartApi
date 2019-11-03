using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApplicationCore.Interfaces
{
    public interface IJwtTokenHandler
    {
        string GetToken(JwtSecurityToken jwt);
        ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters);
    }
}
