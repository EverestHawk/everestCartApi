using ApplicationCore.Entities;
using ApplicationCore.Entities.BasketAggregate;
using ApplicationCore.Entities.OrderAggregate;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Ardalis.GuardClauses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class OrderService : IOrderService
    {
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IAsyncRepository<CatalogItem> _itemRepository;
        private readonly IAppLogger<OrderService> _logger;

        public OrderService(IAsyncRepository<Order> orderRepository,
            IAsyncRepository<Basket> basketRepository,
            IAsyncRepository<CatalogItem> itemRepository,
            IAppLogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _basketRepository = basketRepository;
            _itemRepository = itemRepository;
            _logger = logger;
        }
        public async Task CreateOrderAsync(int basketId, Address shippingAddress)
        {
            _logger.LogInfo($"Creating order of the items in basket {basketId}.");

            var basket = await _basketRepository.GetByIdAsync(basketId);
            Guard.Against.NullBasket(basketId, basket);
            var items = new List<OrderItem>();

            foreach (var item in basket.Items)
            {
                var catalogItem = await _itemRepository.GetByIdAsync(item.CatalogItemId);
                var itemOrdered = new CatalogItemOrdered(catalogItem.Id, catalogItem.Name, catalogItem.PictureUri);
                var orderItem = new OrderItem(itemOrdered, item.UnitPrice, item.Quantity);
                items.Add(orderItem);
            }
            var order = new Order(basket.BuyerId, shippingAddress, items);

            await _orderRepository.AddAsync(order);

            _logger.LogInfo($"Order number {order.Id} has been created with for basket {basketId}.");
        }

        public Order GetByIdWithItems(int orderId)
        {
            var orderSpec = new OrderSpecification(orderId);
            return _orderRepository.GetSingleBySpecAsync(orderSpec).Result;
        }

        public async Task<Order> GetByIdWithItemsAsync(int orderId)
        {
            var orderSpec = new OrderSpecification(orderId);
            return await _orderRepository.GetSingleBySpecAsync(orderSpec);
        }
    }
}
