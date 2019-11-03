using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels.Manage;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Authorize]
    public class ManageController : BaseApiController
    {
        private readonly IManageService _manageService;

        public ManageController(IManageService manageService)
        {
            _manageService = manageService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest tokenRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(await _manageService.GetTokenRefreshed(tokenRequest));
        }
    }
}
