using ApplicationCore;
using ApplicationCore.Interfaces;
using Infrastructure.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Infrastructure.Auth
{
    public class JwtFactory : IJwtFactory
    {
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly JwtIssuerOptions _jwtOptions;
        public JwtFactory(IJwtTokenHandler jwtTokenHandler, IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtTokenHandler = jwtTokenHandler;
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

        }
        public async Task<AccessToken> GenerateAccessToken(string id, string userName)
        {
            var identityClaims = GenerateClaimsIdentity(id, userName);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                identityClaims.FindFirst(Constants.JwtClaimIdentifiers.Role),
                identityClaims.FindFirst(Constants.JwtClaimIdentifiers.Id)
            };

            //Create JWT security token
            var jwt = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                _jwtOptions.NotBefore,
                _jwtOptions.Expiration,
                _jwtOptions.SigningCredentials);

            var accessToken = new AccessToken(_jwtTokenHandler.GetToken(jwt), _jwtOptions.ValidFor.TotalSeconds);

            return accessToken;
        }

        private static ClaimsIdentity GenerateClaimsIdentity(string id, string userName)
        {
            return new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
            {
                new Claim(Constants.JwtClaimIdentifiers.Id, id),
                new Claim(Constants.JwtClaimIdentifiers.Role, Constants.JwtClaims.ApiAccess)
            });
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight calendar time).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }
    }
}
