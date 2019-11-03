using ApplicationCore.Entities.OrderAggregate;

namespace ApplicationCore.Specifications
{
    public class OrderSpecification : BaseSpecification<Order>
    {
        public OrderSpecification(int orderId) : base(o => o.Id == orderId)
        {
            AddInclude(o => o.OrderItems);
            AddInclude($"{nameof(Order.OrderItems)}.{nameof(OrderItem.ItemOrdered)}");
        }
    }
}
