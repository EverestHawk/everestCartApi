using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        public string UserNameOrEmail { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
        public string RemoteIpAddress { get; set; }
    }
}
