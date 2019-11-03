using System.Security.Claims;

namespace ApplicationCore.Interfaces
{
    public interface IJwtTokenValidator 
    {
        ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey);
    }
}
