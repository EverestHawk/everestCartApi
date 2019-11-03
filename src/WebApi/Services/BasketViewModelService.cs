using ApplicationCore.Entities;
using ApplicationCore.Entities.BasketAggregate;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Services.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Interfaces;

namespace WebApi.Services
{
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly IAppLogger<BasketViewModelService> _logger;
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IRepository<CatalogItem> _itemRepository;
        private readonly IUriComposer _uriComposer;
        public BasketViewModelService(IAppLogger<BasketViewModelService> logger,
            IAsyncRepository<Basket> basketRepository,
            IRepository<CatalogItem> itemRepository,
            IUriComposer uriComposer)
        {
            _logger = logger;
            _basketRepository = basketRepository;
            _itemRepository = itemRepository;
            _uriComposer = uriComposer;
        }
        public async Task<BasketViewModel> GetOrCreateBasketForUser(string userName)
        {
            var basketSpec = new BasketWithItemsSpecification(userName);
            var basket = (await _basketRepository.ListAsync(basketSpec)).FirstOrDefault();

            if (basket == null)
            {
                return await CreateBasketForUser(userName);
            }
            return CreateViewModelFromBasket(basket);
        }

        private BasketViewModel CreateViewModelFromBasket(Basket basket)
        {
            var vm = new BasketViewModel() {BuyerId = basket.BuyerId, Id = basket.Id };

            vm.Items = basket.Items.Select(i =>
            {
                var itemViewModel = new BasketItemViewModel
                {
                    Id = i.Id,
                    UnitPrice = i.UnitPrice,
                    CatalogItemId = i.CatalogItemId,
                    Quantity = i.Quantity
                };
                var item = _itemRepository.GetById(i.CatalogItemId);
                itemViewModel.ProductName = item.Name;
                itemViewModel.PictureUri = _uriComposer.ComposePictureUri(item.PictureUri);

                return itemViewModel;
            }).ToList();

            return vm;
        }

        private async Task<BasketViewModel> CreateBasketForUser(string userName)
        {
            var basket = new Basket() { BuyerId = userName };

            await _basketRepository.AddAsync(basket);

            return new BasketViewModel()
            {
                BuyerId = basket.BuyerId,
                Id = basket.Id,
                Items = new List<BasketItemViewModel>()
            };
        }
    }
}
