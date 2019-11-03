using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;
using Services.ViewModels.Account;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Authorize]
    public class AccountController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IAppLogger<AccountController> _logger;
        public AccountController(IUserService userService,
            IAppLogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _userService.CreateUser(model, model.Password));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            model.RemoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            return Ok(await _userService.SignIn(model));
        }
    }
}
