using ApplicationCore.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Auth
{
    public class JwtTokenValidator : IJwtTokenValidator
    {
        private readonly IJwtTokenHandler _jwtTokenHandler;
        public JwtTokenValidator(IJwtTokenHandler jwtTokenHandler)
        {
            _jwtTokenHandler = jwtTokenHandler;
        }
        public ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey)
        {
            return _jwtTokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ValidateLifetime = false //we check expired tokens here
            });
        }
    }
}
