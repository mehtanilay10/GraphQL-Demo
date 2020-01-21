using GraphQL.Types;

namespace PizzaOrder.GraphQLModels.Enums
{
    public class OrderStatusEnumType : EnumerationGraphType
    {
        public OrderStatusEnumType()
        {
            Name = "orderStatus";

            AddValue("Created", "Order was created.", 1);
            AddValue("InKitchen", "Order is preparing.", 2);
            AddValue("OnTheWay", "Order is on the way.", 3);
            AddValue("Delivered", "Order was Delivered.", 4);
            AddValue("Canceled", "Order was Canceled.", 5);
        }
    }
}
