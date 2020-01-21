using PizzaOrder.Data.Enums;

namespace PizzaOrder.Business.Models
{
    public class EventDataModel
    {
        public int OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public EventDataModel(int orderId, OrderStatus orderStatus = OrderStatus.Created)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
        }
    }
}
