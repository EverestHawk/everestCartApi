using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var defaultUser = new ApplicationUser { UserName = "demouser@everestcart.com", Email = "demouser@everestcart.com" };
            await userManager.CreateAsync(defaultUser, "Hard24Get$");
        }
    }
}
