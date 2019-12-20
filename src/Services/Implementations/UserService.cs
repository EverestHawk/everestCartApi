using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using ApplicationCore.TypeMapping;
using Infrastructure.Enums;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.ViewModels;
using Services.ViewModels.Account;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAsyncRepository<User> _userRepository;
        private readonly ITokenFactory _tokenFactory;
        private readonly IJwtFactory _jwtFactory;
        private readonly IAutoMapper _mapper;
        private readonly ILogger<UserService> _logger;
        public UserService(UserManager<ApplicationUser> userManager,
            IAsyncRepository<User> userRepository,
            ITokenFactory tokenFactory,
            IJwtFactory jwtFactory,
            IAutoMapper mapper,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _tokenFactory = tokenFactory;
            _jwtFactory = jwtFactory;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<bool> CheckUserPassword(User user, string password)
        {
            var appUser = _mapper.Map<ApplicationUser>(user);
            return await _userManager.CheckPasswordAsync(appUser, password);
        }

        public async Task<ResponseMetadata<UserViewModel>> CreateUser(RegisterViewModel rvm, string password)
        {
            var appUser = new ApplicationUser { Email = rvm.Email, UserName = rvm.UserName };
            IdentityResult identityUser = await _userManager.CreateAsync(appUser, password);
            if (!identityUser.Succeeded)
            {
                return new ResponseMetadata<UserViewModel>((int)HttpStatusCode.InternalServerError, null, identityUser.Errors.Select(x => x.Description).ToArray());
            }

            var userEntity = new User(rvm.FirstName, rvm.LastName, appUser.Id, appUser.UserName);
            userEntity.Address = rvm.Address;
            userEntity.DateOfBirth = rvm.DateOfBirth;
            userEntity.Gender = Convert.ToByte((int)Enum.Parse(typeof(Genders), rvm.Gender, true));

            await _userRepository.AddAsync(userEntity);
            var user = _mapper.Map<UserViewModel>(userEntity);
            user.Email = rvm.Email;

            return new ResponseMetadata<UserViewModel>((int)HttpStatusCode.OK, new UserViewModel[] { user }, new string[] { });
        }

        public async Task<UserViewModel> GetUserByUserName(string userName)
        {
            var userEntity = await GetUserEntity(userName);

            UserViewModel userModel = null;
            if (userEntity != null)
            {
                userModel = new UserViewModel
                {
                    Id = userEntity.Id,
                    IdentityId = userEntity.IdentityId,
                    Email = userEntity.Email,
                    UserName = userName,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    AccessToken = string.Empty,
                    Token = string.Empty
                };
            }
            return userModel;
        }        

        public async Task<ResponseMetadata<UserViewModel>> SignIn(LoginViewModel model)
        {
            var user = await GetUserEntity(model.UserNameOrEmail);
            user = user ?? await GetUserEntityByEmail(model.UserNameOrEmail);

            if (user != null)
            {
                var uvm = _mapper.Map<UserViewModel>(user);
                //validate password
                if (await CheckUserPassword(user, model.Password))
                {
                    //generate refresh token
                    var refreshToken = _tokenFactory.GenerateToken();

                    user.ExpireAllActiveTokenForUser(uvm.Id);
                    user.AddRefreshToken(refreshToken, uvm.Id, model.RemoteIpAddress);
                    await _userRepository.UpdateAsync(user);


                    //access token
                    var accessToken = await _jwtFactory.GenerateAccessToken(uvm.IdentityId, uvm.UserName);

                    uvm.Token = refreshToken;
                    uvm.AccessToken = accessToken.Token;
                    uvm.ExpiresIn = accessToken.ExpiresIn;

                    return new ResponseMetadata<UserViewModel>((int)HttpStatusCode.OK, new UserViewModel[] { uvm }, new string[] { });
                }
            }
            return new ResponseMetadata<UserViewModel>((int)HttpStatusCode.BadRequest, null, new string[] { "Invalid username or password." });
        }

        private async Task<User> GetUserEntity(string userName)
        {
            var appUser = await _userManager.FindByNameAsync(userName);

            if (appUser != null)
            {
                var user = await _userRepository.GetSingleBySpecAsync(new UserSpecification(appUser.Id));
                user = _mapper.Map(appUser, user);

                return user;
            }              
            return null;
        }

        private async Task<User> GetUserEntityByEmail(string email)
        {
            var appUser = await _userManager.FindByEmailAsync(email);

            if (appUser != null)
            {
                var user = await _userRepository.GetSingleBySpecAsync(new UserSpecification(appUser.Id));
                user = _mapper.Map(appUser, user);
                return user;
            }

            return null;
        }
    }
}
