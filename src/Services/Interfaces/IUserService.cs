using ApplicationCore.Entities;
using Services.ViewModels;
using Services.ViewModels.Account;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<ResponseMetadata<UserViewModel>> CreateUser(RegisterViewModel user, string password);
        Task<UserViewModel> GetUserByUserName(string userName);
        Task<bool> CheckUserPassword(User user, string password);
        Task<ResponseMetadata<UserViewModel>> SignIn(LoginViewModel model);
    }
}
