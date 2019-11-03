using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Services.Interfaces;
using Services.ViewModels;
using Services.ViewModels.Manage;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class ManageService : IManageService
    {
        private readonly IAsyncRepository<User> _userRepository;
        private readonly IJwtFactory _jwtFactory;
        private readonly ITokenFactory _tokeFactory;
        private readonly IJwtTokenValidator _jwtTokenValidator;
        private readonly IAppLogger<ManageService> _logger;
        public ManageService(IAsyncRepository<User> userRepository,
            IJwtFactory jwtFactory,
            ITokenFactory tokenFactory,
            IJwtTokenValidator jwtTokenValidator,
            IAppLogger<ManageService> logger)
        {
            _userRepository = userRepository;
            _jwtFactory = jwtFactory;
            _tokeFactory = tokenFactory;
            _jwtTokenValidator = jwtTokenValidator;
            _logger = logger;
        }
        public async Task<ResponseMetadata<RefreshTokenResponse>> GetTokenRefreshed(RefreshTokenRequest model)
        {
            _logger.LogInfo($"GetTokenRefreshed :: for token: {model.RefreshToken}.");
            var cp = _jwtTokenValidator.GetPrincipalFromToken(model.AccessToken, model.SigningKey);
            // invalid token/signing key was passed and we can't extract user claims
            if (cp != null)
            {
                var id = cp.Claims.First(c => c.Type == "id");
                var user = await _userRepository.GetSingleBySpecAsync(new UserSpecification(id.Value));
                if (user.HasValidRefreshToken(model.RefreshToken))
                {
                    var accessToken = await _jwtFactory.GenerateAccessToken(user.IdentityId, user.UserName);
                    var refreshToken = _tokeFactory.GenerateToken();
                    user.ExpireRefreshToken(model.RefreshToken);
                    user.AddRefreshToken(refreshToken, user.Id, model.RemoteIpAddress);
                    await _userRepository.UpdateAsync(user);

                    _logger.LogInfo($"GetTokenRefreshed :: for token: {model.RefreshToken} has been completed successfully.");

                    return new ResponseMetadata<RefreshTokenResponse>((int)HttpStatusCode.OK, new RefreshTokenResponse[] { new RefreshTokenResponse
                    {
                        AccessToken = accessToken.Token,
                        RefreshToken = refreshToken,
                        ExpiresIn = accessToken.ExpiresIn
                    } }, null);
                }
            }
            _logger.LogError($"GetTokenRefreshed :: for token: {model.RefreshToken} completed with error.");
            return new ResponseMetadata<RefreshTokenResponse>((int)HttpStatusCode.BadRequest, null, new string[] { "Invalid token." });
        }
    }
}
