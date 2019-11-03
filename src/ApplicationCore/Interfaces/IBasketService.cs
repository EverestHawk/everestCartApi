using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IBasketService
    {
        Task<int> GetBasketItemCountAsync(string username);
        Task TransferBasketAsync(string anonymousId, string username);
        Task AddItemToBasket(int basketId, int catalogItemId, decimal price, int quantity);
        Task SetQuantities(int basketId, Dictionary<string, int> quantities);
        Task DeleteBasketAsync(int basketId);
    }
}
