using ApplicationCore.Entities.OrderAggregate;

namespace ApplicationCore.Specifications
{
    public class CustomerOrderWithItemsSpecification : BaseSpecification<Order>
    {
        public CustomerOrderWithItemsSpecification(string buyerId) : base(o => o.BuyerId == buyerId)
        {
            AddInclude(o => o.OrderItems);
            AddInclude($"{nameof(Order.OrderItems)}.{nameof(OrderItem.ItemOrdered)}");
        }
    }
}
