using Services.ViewModels;
using System.Threading.Tasks;

namespace WebApi.Interfaces
{
    public interface IBasketViewModelService
    {
        Task<BasketViewModel> GetOrCreateBasketForUser(string userName);
    }
}
