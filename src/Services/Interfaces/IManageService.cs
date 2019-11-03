using Services.ViewModels;
using Services.ViewModels.Manage;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IManageService
    {
        Task<ResponseMetadata<RefreshTokenResponse>> GetTokenRefreshed(RefreshTokenRequest model);
    }
}
